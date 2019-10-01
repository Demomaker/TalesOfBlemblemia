﻿using System;
using UnityEngine;

namespace Harmony
{
    /// <summary>
    /// Mark a [SerializedField] as a flag list and draws it accordingly in the inspector.
    ///
    /// A Flag list is a list that can have many items checked.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class FlagsFieldAttribute : PropertyAttribute
    {
        public string Label { get; }

        public FlagsFieldAttribute()
        {
            Label = null;
        }

        public FlagsFieldAttribute(string label)
        {
            Label = label;
        }
    }
}