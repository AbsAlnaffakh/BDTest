using BDTest.Test;

namespace BDTest.ReportGenerator.RazorServer.Models
{
    public class ScenarioViewModel
    {
        public Scenario Scenario { get; set; }
        public ReferenceInt ScenarioIndex { get; set; }
        public ReferenceInt GroupedScenarioIndex { get; set; }
        public bool IsPartOfGroupedScenarios { get; set; }
    }
}