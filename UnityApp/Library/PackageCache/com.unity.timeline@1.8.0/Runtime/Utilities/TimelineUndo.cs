using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Timeline
{
    static class TimelineUndo
    {
        internal static bool undoEnabled
        {
            get
            {
#if UNITY_EDITOR
                return DisableUndoGuard.enableUndo && DisableUndoScope.enableUndo;
#else
                return false;
#endif
            }
        }

        public static void PushDestroyUndo(TimelineAsset timeline, Object thingToDirty, Object objectToDestroy)
        {
#if UNITY_EDITOR
            if (objectToDestroy == null || !undoEnabled)
                return;

            if (thingToDirty != null)
                EditorUtility.SetDirty(thingToDirty);

            if (timeline != null)
                EditorUtility.SetDirty(timeline);

            Undo.DestroyObjectImmediate(objectToDestroy);
#else
            if (objectToDestroy != null)
                Object.Destroy(objectToDestroy);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void PushUndo(Object[] thingsToDirty, string operation)
        {
#if UNITY_EDITOR
            if (thingsToDirty == null || !undoEnabled)
                return;

            for (var i = 0; i < thingsToDirty.Length; i++)
            {
                if (thingsToDirty[i] is TrackAsset track)
                    track.MarkDirty();
                EditorUtility.SetDirty(thingsToDirty[i]);
            }
            Undo.RegisterCompleteObjectUndo(thingsToDirty, UndoName(operation));
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void PushUndo(Object thingToDirty, string operation)
        {
#if UNITY_EDITOR
            if (thingToDirty != null && undoEnabled)
            {
                var track = thingToDirty as TrackAsset;
                if (track != null)
                    track.MarkDirty();

                EditorUtility.SetDirty(thingToDirty);
                Undo.RegisterCompleteObjectUndo(thingToDirty, UndoName(operation));
            }
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void RegisterCreatedObjectUndo(Object thingCreated, string operation)
        {
#if UNITY_EDITOR
            if (undoEnabled)
            {
                Undo.RegisterCreatedObjectUndo(thingCreated, UndoName(operation));
            }
#endif
        }

        internal static string UndoName(string name) => "Timeline " + name;

#if UNITY_EDITOR
        /// <summary>
        /// Provides stack management of the undo state.
        /// </summary>
        internal struct DisableUndoGuard : IDisposable
        {
            internal static bool enableUndo = true;
            static readonly Stack<bool> m_UndoStateStack = new Stack<bool>();
            bool m_MustDispose;
            public DisableUndoGuard(bool disable)
            {
                m_MustDispose = true;
                m_UndoStateStack.Push(enableUndo);
                enableUndo = !disable;
            }

            public void Dispose()
            {
                if (m_MustDispose)
                {
                    if (m_UndoStateStack.Count == 0)
                    {
                        Debug.LogError("UnMatched DisableUndoGuard calls");
                        enableUndo = true;
                        return;
                    }
                    enableUndo = m_UndoStateStack.Pop();
                    m_MustDispose = false;
                }
            }
        }

        /// <summary>
        /// Provides an undo state switch.
        /// </summary>
        internal class DisableUndoScope : IDisposable
        {
            internal static bool enableUndo => m_Depth == 0;
            static int m_Depth;
            bool m_MustDispose;
            public DisableUndoScope()
            {
                m_MustDispose = true;
                m_Depth++;
            }

            public void Dispose()
            {
                if (m_MustDispose)
                {
                    if (m_Depth == 0)
                    {
                        Debug.LogError("UnMatched DisableUndoScope calls");
                        return;
                    }
                    m_Depth--;
                    m_MustDispose = false;
                }
            }
        }
#endif
    }
}
