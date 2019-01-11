﻿using System;

namespace BDTest.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StoryAttribute : Attribute
    {
        public string AsA { get; set; }
        public string IWant { get; set; }
        public string SoThat { get; set; }

        public string GetStoryText()
        {
            if (!AsA.ToLower().StartsWith("as a")) AsA = $"As a {AsA}";
            if (!IWant.ToLower().StartsWith("i want")) IWant = $"I want {IWant}";
            if (!SoThat.ToLower().StartsWith("so that")) SoThat = $"So that {SoThat}";

            return $"{AsA}" +
                   $"{Environment.NewLine}{IWant}" +
                   $"{Environment.NewLine}{SoThat}{Environment.NewLine}";
        }
    }
}
