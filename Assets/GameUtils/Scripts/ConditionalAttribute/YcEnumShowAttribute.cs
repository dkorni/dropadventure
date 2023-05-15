using UnityEngine;
using System;
using System.Collections.Generic;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class YcEnumShowAttribute : PropertyAttribute {

    public string ConditionalSourceField = "";
    public string[] HideWhenEqualToValues;

    public YcEnumShowAttribute(string conditionalSourceField, object enumValue1, object enumValue2 = null, object enumValue3 = null, object enumValue4 = null,
                                                              object enumValue5 = null, object enumValue6 = null, object enumValue7 = null) {
        this.Setup(conditionalSourceField, new string[] { enumValue1.ToString(),
                                                          enumValue2 != null ? enumValue2.ToString() : "",
                                                          enumValue3 != null ? enumValue3.ToString() : "",
                                                          enumValue4 != null ? enumValue4.ToString() : "",
                                                          enumValue5 != null ? enumValue5.ToString() : "",
                                                          enumValue6 != null ? enumValue6.ToString() : "",
                                                          enumValue7 != null ? enumValue7.ToString() : ""});
    }

    private void Setup(string conditionalSourceField, string[] hideWhenEqualToValues) {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideWhenEqualToValues = hideWhenEqualToValues;
    }
}
