using Webinex.Asky;
using static Webinex.Activity.Server.Controllers.ActivityRowAskyFieldMap;

namespace Webinex.Activity.Server.Controllers
{
    internal class ActivityValueFilterRuleReplaceVisitor : FilterRuleVisitor
    {
        public override FilterRule Visit(ValueFilterRule valueFilterRule)
        {
            if (!valueFilterRule.FieldId.StartsWith(VALUE_FIELD_ID_PREFIX))
                return null;

            var fieldId = valueFilterRule.FieldId.Substring(VALUE_FIELD_ID_PREFIX.Length);

            return FilterRule.Any(
                VALUE_FIELD_ID,
                FilterRule.And(
                    new ValueFilterRule(VALUE_VALUE_FIELD_ID, valueFilterRule.Operator, valueFilterRule.Value),
                    FilterRule.Eq(VALUE_SEARCH_PATH_FIELD_ID, fieldId)));
        }

        public override FilterRule Visit(CollectionFilterRule collectionFilterRule)
        {
            if (!collectionFilterRule.FieldId.StartsWith(VALUE_FIELD_ID_PREFIX))
                return null;

            var fieldId = collectionFilterRule.FieldId.Substring(VALUE_FIELD_ID_PREFIX.Length);

            return FilterRule.Any(VALUE_FIELD_ID, FilterRule.And(
                new CollectionFilterRule(VALUE_VALUE_FIELD_ID, collectionFilterRule.Operator,
                    collectionFilterRule.Values),
                FilterRule.Eq(VALUE_SEARCH_PATH_FIELD_ID, fieldId)));
        }
    }
}