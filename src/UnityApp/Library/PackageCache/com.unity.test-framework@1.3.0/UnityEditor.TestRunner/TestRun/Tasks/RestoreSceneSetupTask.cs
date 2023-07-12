using System;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace UnityEditor.TestTools.TestRunner.TestRun.Tasks
{
    internal class RestoreSceneSetupTask : TestTaskBase
    {
        internal Action<SceneSetup[]> RestoreSceneManagerSetup = EditorSceneManager.RestoreSceneManagerSetup;
        internal Func<NewSceneSetup, NewSceneMode, Scene> NewScene = EditorSceneManager.NewScene; // NewSceneSetup.DefaultGameObjects, NewSceneMode.Single
        public override IEnumerator Execute(TestJobData testJobData)
        {
            var sceneSetup = testJobData.SceneSetup;
            if (sceneSetup != null && sceneSetup.Length > 0)
            {
                RestoreSceneManagerSetup(sceneSetup);
            }
            else
            {
                NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            }
            
            yield break;
        }
    }
}