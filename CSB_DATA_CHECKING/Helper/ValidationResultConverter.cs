using CSB_DATA_CHECKING.Models;

namespace CSB_DATA_CHECKING.Helpers
{
    public static class ValidationResultConverter
    {
        public static object Convert(CsbValidationResults oldResult, string? downloadUrl)
        {
            var successfulRules = new Dictionary<string, string>();
            var unsuccessfulRules = new Dictionary<string, string>();

            // ✅ Passed rules
            foreach (var rule in oldResult.PassedRules)
            {
                string ruleKey = GetRuleKey(rule);
                successfulRules[ruleKey] = "Passed";
            }

            // ❌ Failed rules
            foreach (var rule in oldResult.FailedRules)
            {
                string ruleKey = GetRuleKey(rule);
                unsuccessfulRules[ruleKey] = "Failed";
            }

            // Summary
            int totalRules = oldResult.PassedRules.Count + oldResult.FailedRules.Count;
            int passed = oldResult.PassedRules.Count;

            return new
            {
                summary = $"{passed} / {totalRules} Rules Validated Successfully",
                isSuccess = oldResult.Success,
                successfulRules,
                unsuccessfulRules,
                downloadUrl
            };
        }

        // ✅ Extracts only "Rule X" regardless of text after it
        private static string GetRuleKey(string rule)
        {
            // Handles: "Rule 1 - Valid Filename" OR "Rule 2: Column Data Types"
            var parts = rule.Split(new[] { '-', ':' }, 2);
            return parts[0].Trim();  // → always "Rule X"
        }
    }
}
