using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ValueStorage
{
    [CreateAssetMenu(menuName = STORAGE_CREATE_MENU)]
    public class ValueStorage : ScriptableObject
    {
        public const string STORAGE_CREATE_MENU = "Scriptable Objects/ValueStorage";

        public List<IntAliasValue> intValues;

        public List<StringAliasValue> stringValues;
    }
}
