using System;
using System.Collections.Generic;
using UnityEditor.Networking.PlayerConnection;
using UnityEditor.TestTools.TestRunner;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEditor.TestTools.TestRunner.UnityTestProtocol;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using UnityEngine.TestRunner.TestLaunchers;

namespace UnityEditor.TestRunner.TestLaunchers
{
    [Serializable]
    internal class RemoteTestRunController : ScriptableSingleton<RemoteTestRunController>
    {
        internal const int k_HeartbeatTimeout = 60 * 10;
        
        [SerializeField]
        private RemoteTestResultReciever m_RemoteTestResultReciever;

        [SerializeField]
        private PlatformSpecificSetup m_PlatformSpecificSetup;

        [SerializeField]
        private bool m_RegisteredConnectionCallbacks;

        [SerializeField] 
        private int m_HearbeatTimeOut;
        
        enum MessageType
        {
            TestStarted,
            TestFinished,
        }

        [Serializable]
        struct Message
        {
            public byte[] Bytes;
            public MessageType Type;
        }

        [SerializeField]
        private List<Message> m_IncomingMessages = new List<Message>();

        [SerializeField]
        private bool m_RegisteredMessageCallback;

        private UnityEditor.TestTools.TestRunner.DelayedCallback m_TimeoutCallback;
        
        public void Init(BuildTarget buildTarget, int heartbeatTimeout)
        {
            m_HearbeatTimeOut = heartbeatTimeout;
            m_PlatformSpecificSetup = new PlatformSpecificSetup(buildTarget);
            m_PlatformSpecificSetup.Setup();
            m_RemoteTestResultReciever = new RemoteTestResultReciever();
            EditorConnection.instance.Initialize();
            if (!m_RegisteredConnectionCallbacks)
            {
                EditorConnection.instance.Initialize();
                DelegateEditorConnectionEvents();
            }
        }

        private void DelegateEditorConnectionEvents()
        {
            m_RegisteredConnectionCallbacks = true;
            //This is needed because RemoteTestResultReceiver is not a ScriptableObject
            EditorConnection.instance.Register(PlayerConnectionMessageIds.runStartedMessageId, RunStarted);
            EditorConnection.instance.Register(PlayerConnectionMessageIds.runFinishedMessageId, RunFinished);
            EditorConnection.instance.Register(PlayerConnectionMessageIds.playerAliveHeartbeat, PlayerAliveHeartbeat);

            // When a message comes in, we should not immediately process it but instead enqueue it for processing later
            // in the frame. The problem this solves is that Unity only reserves about 1ms worth of time every frame to
            // process message from the player connection. When some tests run in a player, it can take the editor
            // minutes to react to all messages we receive because we only do 1ms of processing, then render all of the
            // editor etc. -- Instead, we use that 1ms time-window to enqueue messages and then react to them later
            // during the frame. This reduces the waiting time from minutes to seconds.
            EditorConnection.instance.Register(PlayerConnectionMessageIds.testStartedMessageId, args => EnqueueMessage(args, MessageType.TestStarted));
            EditorConnection.instance.Register(PlayerConnectionMessageIds.testFinishedMessageId, args => EnqueueMessage(args, MessageType.TestFinished));
        }

        private void FlushMessageQueue()
        {
            EditorApplication.update -= FlushMessageQueue;
            m_RegisteredMessageCallback = false;
            foreach (var msg in m_IncomingMessages)
            {
                switch (msg.Type)
                {
                    case MessageType.TestFinished:
                    {
                        CallbacksDelegator.instance.TestFinishedRemotely(msg.Bytes);
                        break;
                    }
                    case MessageType.TestStarted:
                    {
                        CallbacksDelegator.instance.TestStartedRemotely(msg.Bytes);
                        break;
                    }
                }
            }
            m_IncomingMessages.Clear();
        }

        private void EnqueueMessage(MessageEventArgs args, MessageType type)
        {
            m_TimeoutCallback?.Reset();
            if (!m_RegisteredMessageCallback)
            {
                EditorApplication.update += FlushMessageQueue;
                m_RegisteredMessageCallback = true;
            }
            m_IncomingMessages.Add(new Message
            {
                Bytes = args.data,
                Type = type
            });
        }

        private void RunStarted(MessageEventArgs messageEventArgs)
        {
            m_TimeoutCallback?.Reset();
            m_RemoteTestResultReciever.RunStarted(messageEventArgs);
            CallbacksDelegator.instance.RunStartedRemotely(messageEventArgs.data);
        }

        private void RunFinished(MessageEventArgs messageEventArgs)
        {
            m_TimeoutCallback?.Clear();
            m_RemoteTestResultReciever.RunFinished(messageEventArgs);
            m_PlatformSpecificSetup.CleanUp();

            CallbacksDelegator.instance.RunFinishedRemotely(messageEventArgs.data);
        }
        
        private void PlayerAliveHeartbeat(MessageEventArgs messageEventArgs)
        {
            m_TimeoutCallback?.Reset();
        }

        private void TimeoutCallback()
        {
            CallbacksDelegator.instance.RunFailed($"Test execution timed out. No activity received from the player in {m_HearbeatTimeOut} seconds.");
        }

        public void PostBuildAction()
        {
            m_PlatformSpecificSetup.PostBuildAction();
        }

        public void PostSuccessfulBuildAction()
        {
            m_PlatformSpecificSetup.PostSuccessfulBuildAction();
            m_TimeoutCallback = new UnityEditor.TestTools.TestRunner.DelayedCallback(TimeoutCallback, m_HearbeatTimeOut);
        }

        public void PostSuccessfulLaunchAction()
        {
            m_PlatformSpecificSetup.PostSuccessfulLaunchAction();
        }

        public void CleanUp()
        {
            m_PlatformSpecificSetup.CleanUp();
        }
    }
}
