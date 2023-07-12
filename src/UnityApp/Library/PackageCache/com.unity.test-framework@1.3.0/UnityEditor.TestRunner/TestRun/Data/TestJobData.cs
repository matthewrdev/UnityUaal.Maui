using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using UnityEditor.SceneManagement;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEditor.TestTools.TestRunner.TestRun.Tasks;
using UnityEngine;

namespace UnityEditor.TestTools.TestRunner.TestRun
{
    [Serializable]
    internal class TestJobData: ISerializationCallbackReceiver
    {
        [SerializeField] 
        public string guid;
        
        [SerializeField]
        public string startTime;

        [NonSerialized]
        public Stack<TaskInfo> taskInfoStack = new Stack<TaskInfo>();

        [SerializeField] 
        public int taskPC;

        [SerializeField] 
        public bool isRunning;
        
        [SerializeField]
        public ExecutionSettings executionSettings;

        [SerializeField]
        public RunProgress runProgress = new RunProgress();

        [SerializeField]
        public string[] existingFiles;

        [SerializeField] 
        public int undoGroup = -1;

        [SerializeField] 
        public EditModeRunner editModeRunner;

        [SerializeField]
        private TaskInfo[] savedTaskInfoStack;

        [NonSerialized] 
        public bool isHandledByRunner;

        [SerializeField]
        public SceneSetup[] SceneSetup;
        [NonSerialized]
        public TestTaskBase[] Tasks;
        
        public ITest testTree;

        public TestJobData(ExecutionSettings settings)
        {
            guid = Guid.NewGuid().ToString();
            executionSettings = settings;
            isRunning = false;
            startTime = DateTime.Now.ToString("o");
        }
        
        public void OnBeforeSerialize()
        {
            savedTaskInfoStack = taskInfoStack.ToArray();
        }

        public void OnAfterDeserialize()
        {
            taskInfoStack = new Stack<TaskInfo>(savedTaskInfoStack);
        }

    }
}
