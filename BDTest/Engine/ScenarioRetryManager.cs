using System;
using System.Reflection;
using System.Threading.Tasks;
using BDTest.Attributes;
using BDTest.Exceptions;
using BDTest.Interfaces.Internal;
using BDTest.Reporters;
using BDTest.Test;

namespace BDTest.Engine
{
    public class ScenarioRetryManager : IScenarioRetryManager
    {
        private readonly ITypeMatcher _typeMatcher;

        public ScenarioRetryManager(ITypeMatcher typeMatcher)
        {
            _typeMatcher = typeMatcher;
        }
        
        public async Task CheckIfAlreadyExecuted(Scenario scenario)
        {
            if (scenario.ShouldRetry)
            {
                await ResetData(scenario).ConfigureAwait(false);
                return;
            }
            
            if (scenario.AlreadyExecuted)
            {
                throw new AlreadyExecutedException("This scenario has already been executed");
            }

            scenario.AlreadyExecuted = true;
        }
        
        private async Task ResetData(Scenario scenario)
        {
            SetRetryData(scenario);

            ResetStepData(scenario);

            scenario.Status = Status.Inconclusive;

            await RunRetryTestHooks(scenario).ConfigureAwait(false);

            ConsoleReporter.WriteLine($"{Environment.NewLine}Retrying test...{Environment.NewLine}");
        }

        private async Task RunRetryTestHooks(Scenario scenario)
        {
            try
            {
                var bdTestBase = scenario.BuildableTest.BdTestBase;

                // Run TearDown Attributed Method
                await bdTestBase.RunMethodWithAttribute<BDTestRetryTearDownAttribute>();

                // Run Custom Test Hook In Base Class
                await bdTestBase.OnBeforeRetry();

                ResetContext(scenario);

                // Run SetUp Attributed Method
                await bdTestBase.RunMethodWithAttribute<BDTestRetrySetUpAttribute>();
            }
            catch (Exception e)
            {
                throw new ErrorOccurredDuringRetryActionException(e);
            }
        }

        private static void ResetStepData(Scenario scenario)
        {
            foreach (var step in scenario.Steps)
            {
                step.ResetData();
            }
        }

        private static void SetRetryData(Scenario scenario)
        {
            scenario.ShouldRetry = false;

            scenario.RetryCount++;
        }

        private void ResetContext(Scenario scenario)
        {
            if (_typeMatcher.IsSuperClassOfAbstractContextBDTestBase(scenario.BuildableTest.BdTestBase))
            {
                scenario.BuildableTest.BdTestBase.GetType().GetMethod("RecreateContextOnRetry", BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.Invoke(scenario.BuildableTest.BdTestBase, Array.Empty<object>());
            }
        }
    }
}