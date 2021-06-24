namespace HighlyDeveloped.Core.Extensions
{
    using Examine.Search;
    using Examine;
    using System.Collections.Generic;

    public static class StringArrayExtension
    {
        public static IExamineValue[] Fuzzy(this string[] terms)
        {
            if (terms == null) return null;

            var values = new List<IExamineValue>();
            foreach (var item in terms)
            {
                values.Add(item.Fuzzy());
            }

            return values.ToArray();
        }
        public static IExamineValue[] Boost(this string[] terms, float boost)
        {
            if (terms == null) return null;

            var values = new List<IExamineValue>();
            foreach (var term in terms)
            {
                values.Add(term.Boost(boost));
            }
            
            return values.ToArray();
        }
    }
}