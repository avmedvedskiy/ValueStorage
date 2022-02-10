using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ValueStorage
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class ValueAttribute : PropertyAttribute
    {
        public string storageName;

        public ValueAttribute(string name)
        {
            this.storageName = name;
        }
    }
}
