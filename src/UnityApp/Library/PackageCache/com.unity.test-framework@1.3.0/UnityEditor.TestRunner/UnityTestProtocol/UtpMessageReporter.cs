using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.TestRunner.UnityTestProtocol;
using UnityEditor.TestTools.TestRunner.Api;
using CompositeWorkItem = UnityEngine.TestRunner.NUnitExtensions.Runner.CompositeWorkItem;
namespace UnityEditor.TestTools.TestRunner.UnityTestProtocol
{
    internal class UtpMessageReporter : IUtpMessageReporter
    {
        public ITestRunnerApiMapper TestRunnerApiMapper;
        public IUtpLogger Logger;
        public ITestRunDataHolder TestRunDataHolder;

        public UtpMessageReporter(IUtpLogger utpLogger, string projectRepoPath)
        {
            TestRunnerApiMapper = new TestRunnerApiMapper(projectRepoPath);
            Logger = utpLogger;
            TestRunDataHolder = UnityEditor.TestRunner.UnityTestProtocol.TestRunDataHolder.instance;
        }

        public void ReportTestRunStarted(ITestAdaptor testsToRun)
        {
            var msg = TestRunnerApiMapper.MapTestToTestPlanMessage(testsToRun);

            Logger.Log(msg);
        }

        public void ReportTestStarted(ITestAdaptor test)
        {
            if (test.IsSuite)
                return;

            var msg = TestRunnerApiMapper.MapTestToTestStartedMessage(test);

            Logger.Log(msg);
        }

        public void ReportTestFinished(ITestResultAdaptor result)
        {
            if (result.Test.IsSuite)
            {
                SaveOneTimeSetupTearDownDurations(result);
                return;
            }

            var msg = TestRunnerApiMapper.TestResultToTestFinishedMessage(result);
            Logger.Log(msg);
        }

        public void ReportRunFinished()
        {
            var msg = TestRunnerApiMapper.TestRunDataHolderToTestSummaryMessage(TestRunDataHolder);
            Logger.Log(msg);
        }

        private void SaveOneTimeSetupTearDownDurations(ITestResultAdaptor result)
        {
            var oneTimeSetUpDuration = GetDurationFromString(result.Output, DurationType.OneTimeSetUpDuration);
            var oneTimeTearDownDuration = GetDurationFromString(result.Output, DurationType.OneTimeTearDownDuration);
            if (oneTimeSetUpDuration <= -1 || oneTimeTearDownDuration <= -1) return;
            var t = new TestRunData
            {
                SuiteName = result.Test.Name,
                TestsInFixture = GetAllTestsInAFixture(result.Test, new List<string>()),
                OneTimeSetUpDuration = oneTimeSetUpDuration,
                OneTimeTearDownDuration = oneTimeTearDownDuration
            };
            TestRunDataHolder.TestRunDataList.Add(t);
        }

        private long GetDurationFromString(string output, DurationType type)
        {
            if (output == null)
            {
                return -1;
            }

            if (!output.Contains(type.ToString()))
            {
                return -1;
            }
            
            var pattern = type.Equals(DurationType.OneTimeSetUpDuration)
                ? string.Format(@"({0}).(\d+)ms;", CompositeWorkItem.OneTimeSetUpDuration)
                : string.Format(@"({0}).(\d+)ms;", CompositeWorkItem.OneTimeTearDownDuration);

            var match = Regex.Match(output, pattern);

            long duration = 0;
            if (!match.Success)
            {
                return duration;
            }

            try
            {
                duration = long.Parse(match.Groups[2].Value);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
                return duration;
            }

            return duration;
        }

        private string[] GetAllTestsInAFixture(ITestAdaptor testAdaptor, ICollection<string> allChildren)
        {
            if (!testAdaptor.HasChildren) return allChildren.ToArray();
            foreach (var child in testAdaptor.Children)
            {
                allChildren.Add(child.FullName);
                GetAllTestsInAFixture(child, allChildren);
            }

            return allChildren.ToArray();
        }

        private enum DurationType
        {
            OneTimeSetUpDuration,
            OneTimeTearDownDuration
        }
    }
}
