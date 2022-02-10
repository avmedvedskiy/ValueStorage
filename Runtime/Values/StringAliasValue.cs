using System;
namespace ValueStorage
{
    [Serializable]
    public class StringAliasValue : StringValue
    {
        /// <summary>
        /// alias
        /// </summary>
        public string a;

        public StringAliasValue()
        {
            k = string.Empty;
            v = string.Empty;
            a = string.Empty;
        }
    }
}
