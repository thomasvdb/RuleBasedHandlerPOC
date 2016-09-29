using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace RuleBasedHandlerPOC
{
    /// Author: Cole Francis, Architect
    /// The pre-compiled rules type
    ///
    public class PrecompiledRules
    {
        ///
        /// A method used to precompile rules for a provided type
        ///
        public static List<Func<T, bool>> CompileRule<T>(List<T> targetEntity, List<Rule> rules)
        {
            var compiledRules = new List<Func<T, bool>>();

            // Loop through the rules and compile them against the properties of the supplied shallow object
            rules.ForEach(rule =>
            {
                var genericType = Expression.Parameter(typeof(T));
                var key = MemberExpression.Property(genericType, rule.ComparisonPredicate);
                Type propertyType = null; // = typeof(T).GetProperty(rule.ComparisonPredicate).PropertyType;

                TypeInfo typeInfo = typeof(T).GetTypeInfo();
                foreach (PropertyInfo propInfo in typeInfo.DeclaredProperties){
                    if (propInfo.Name == rule.ComparisonPredicate)
                    {
                        propertyType = propInfo.PropertyType;
                        break;
                    }
                }

                var value = Expression.Constant(Convert.ChangeType(rule.ComparisonValue, propertyType));
                var binaryExpression = Expression.MakeBinary(rule.ComparisonOperator, key, value);

                compiledRules.Add(Expression.Lambda<Func<T, bool>>(binaryExpression, genericType).Compile());
            });

            // Return the compiled rules to the caller
            return compiledRules;
        }
    }

    ///
    /// The Rule type
    ///
    public class Rule
    {
        ///
        /// Denotes the rules predictate (e.g. Name); comparison operator(e.g. ExpressionType.GreaterThan); value (e.g. "Cole")
        ///
        public string ComparisonPredicate { get; set; }
        public ExpressionType ComparisonOperator { get; set; }
        public string ComparisonValue { get; set; }

        ///
        /// The rule method that
        ///
        public Rule(string comparisonPredicate, ExpressionType comparisonOperator, string comparisonValue)
        {
            ComparisonPredicate = comparisonPredicate;
            ComparisonOperator = comparisonOperator;
            ComparisonValue = comparisonValue;
        }
    }
}