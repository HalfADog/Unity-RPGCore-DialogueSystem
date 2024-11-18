using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class DialoguePropertyAttribute : Attribute
{
    public string PropertyName;
    public DialoguePropertyAttribute(string PropertyName)
    {
        this.PropertyName = PropertyName;
    }
}
