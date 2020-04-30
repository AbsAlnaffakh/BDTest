﻿using System;
using BDTest.Attributes;
using Newtonsoft.Json;

namespace BDTest.Test
{
    public class BuildableTest
    {
        [JsonIgnore]
        protected Guid Guid { get; set; }

        [JsonProperty] public StoryText StoryText { get; protected set; }

        [JsonProperty]
        public ScenarioText ScenarioText { get; protected set; }

        [JsonProperty]
        public TestDetails TestDetails { get; protected set; }
        
        [JsonProperty]
        public TestInformationAttribute[] CustomTestInformation { get; set; } = Array.Empty<TestInformationAttribute>();

        public string GetScenarioText()
        {
            return ScenarioText?.Scenario ?? "Scenario Text Not Defined";
        }

        public string GetStoryText()
        {
            return StoryText?.Story ?? "Story Text Not Defined";
        }
    }
}
