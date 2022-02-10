using System;
using System.Collections.Generic;
using UnityEngine;
namespace ValueStorage
{
    //Other types like float, string can be added later
    // Used only for storage and alias for editors
    [Serializable]
    public class IntAliasValue : IntValue
    {
        /// <summary>
        /// alias
        /// </summary>
        public string a;

        public IntAliasValue()
        {
            k = string.Empty;
            v = 0;
            a = string.Empty;
        }

    }
}