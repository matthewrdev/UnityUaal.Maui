using System.Collections.Generic;
using UnityEditor.TestRunner.UnityTestProtocol;
using UnityEditor.TestTools.TestRunner.Api;

namespace UnityEditor.TestTools.TestRunner.UnityTestProtocol
{
    interface ITestRunnerApiMapper
    {
        string GetRunStateFromResultNunitXml(ITestResultAdaptor result);
        TestState GetTestStateFromResult(ITestResultAdaptor result);
        List<string> FlattenTestNames(ITestAdaptor testsToRun);
        TestPlanMessage MapTestToTestPlanMessage(ITestAdaptor testsToRun);
        TestStartedMessage MapTestToTestStartedMessage(ITestAdaptor test);
        TestFinishedMessage TestResultToTestFinishedMessage(ITestResultAdaptor result);
        TestSummaryMessage TestRunDataHolderToTestSummaryMessage(ITestRunDataHolder testRunDataHolder);
    }
}
