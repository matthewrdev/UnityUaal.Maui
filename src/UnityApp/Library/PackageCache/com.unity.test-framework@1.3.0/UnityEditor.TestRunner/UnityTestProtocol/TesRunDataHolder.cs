using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.TestRunner.UnityTestProtocol
{
    public interface ITestRunDataHolder
    {
        IList<TestRunData> TestRunDataList { get; }
    }

    public class TestRunDataHolder: ScriptableSingleton<TestRunDataHolder>, ISerializationCallbackReceiver, ITestRunDataHolder
    {
        [SerializeField]
        private TestRunData[] TestRunData;
        public IList<TestRunData> TestRunDataList { get; private set; } = new List<TestRunData>();

        public void OnBeforeSerialize()
        {
            TestRunData = TestRunDataList.ToArray(); 
        }

        public void OnAfterDeserialize()
        {
            TestRunDataList = TestRunData.ToList();
        }
    }
}