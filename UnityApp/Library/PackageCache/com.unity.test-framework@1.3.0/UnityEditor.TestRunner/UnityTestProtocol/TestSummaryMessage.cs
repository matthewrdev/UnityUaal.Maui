using System.Collections.Generic;
using UnityEditor.TestRunner.UnityTestProtocol;

namespace UnityEditor.TestTools.TestRunner.UnityTestProtocol
{
    internal class TestSummaryMessage: Message
    {
        public TestRunData[] TestRunDataList;
        public TestSummaryMessage()
        {
            type = "TestSummary";
        }
    }
}