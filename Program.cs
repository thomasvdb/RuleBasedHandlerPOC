using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace RuleBasedHandlerPOC
{
    class Program
    {
        static void Main(string[] args)
        {
            RuleHandler handler = new RuleHandler();
            handler.RunRules(new [] { RuleCategory.Feedback });
        }
    }

    public enum RuleCategory
    {
        Startup,
        Feedback
    }

    public class UserPropertyRule
    {
        public string Name { get; set; }
        public List<Func<UserProperties, bool>> Rule { get; set; }
        public Action ActionToExecuteOnMatch { get; set; }
        public bool IsHandled { get; set; }
        public RuleCategory Category { get; set; }
    }

    public class RuleHandler
    {
        private UserPropertyRule[] _rules;

        public RuleHandler()
        {
            List<Rule> rules = new List<Rule>
            {
                // Create some rules using LINQ.ExpressionTypes for the comparison operators
                new Rule ( "AppStartups", ExpressionType.Equal, "2")
            };

            var compiledRules = PrecompiledRules.CompileRule(new List<UserProperties>(), rules);

            _rules = new UserPropertyRule[2];
            _rules[0] = new UserPropertyRule
            {
                ActionToExecuteOnMatch = () => Console.WriteLine("The app started 2 times so this rules matches!"),
                Category = RuleCategory.Startup,
                IsHandled = false,
                Name = "Startup geolocation",
                Rule = compiledRules
            };

            _rules[1] = new UserPropertyRule
            {
                ActionToExecuteOnMatch = () => Console.WriteLine("The app is open for 3 minutes so this rules matches!"),
                Category = RuleCategory.Feedback,
                IsHandled = false,
                Name = "Show popup",
                Rule = compiledRules
            };
        }

        public void RunRules(RuleCategory[] ruleCategories)
        {
            var matchingRulesByCategory = _rules.Where(rule => ruleCategories.Contains(rule.Category));
            var userProperties = new UserProperties();
            userProperties.AppStartups = 2;

            foreach (var rule in matchingRulesByCategory)
            {
                if (rule.Rule.TakeWhile(testRule => testRule(userProperties)).Any())
                {
                    rule.ActionToExecuteOnMatch();
                    rule.IsHandled = true;
                    break;
                }
            }
        }
    }
}


