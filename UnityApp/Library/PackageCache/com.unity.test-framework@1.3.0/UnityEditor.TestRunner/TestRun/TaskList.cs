using System.Collections.Generic;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEditor.TestTools.TestRunner.TestRun.Tasks;
using UnityEngine.TestTools;


namespace UnityEditor.TestTools.TestRunner.TestRun
{
    internal static class TaskList
    {
        public static IEnumerable<TestTaskBase> GetTaskList(ExecutionSettings settings)
        {
            if (settings == null)
            {
                yield break;
            }

            if (settings.EditModeIncluded() || (PlayerSettings.runPlayModeTestAsEditModeTest && settings.PlayModeInEditorIncluded()))
            {
                yield return new SaveModiedSceneTask();
                yield return new RegisterFilesForCleanupVerificationTask();
                yield return new SaveUndoIndexTask();
                yield return new BuildTestTreeTask(TestPlatform.EditMode);
                yield return new PrebuildSetupTask();
                yield return new SaveSceneSetupTask();
                yield return new LegacyEditModeRunTask();
                yield return new RestoreSceneSetupTask();
                yield return new PerformUndoTask();
                yield return new CleanupVerificationTask();
                yield return new UnlockReloadAssembliesTask();
                yield break;
            }

            if (settings.PlayModeInEditorIncluded() && !PlayerSettings.runPlayModeTestAsEditModeTest)
            {
                yield return new SaveModiedSceneTask();
                yield return new LegacyPlayModeRunTask();
                yield return new UnlockReloadAssembliesTask();
                yield break;
            }

            if (settings.PlayerIncluded())
            {
                yield return new LegacyPlayerRunTask();
                yield return new UnlockReloadAssembliesTask();
                yield break;
            }
        }
    }
}
