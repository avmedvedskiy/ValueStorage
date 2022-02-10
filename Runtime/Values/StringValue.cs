using System;
namespace ValueStorage
{
    [Serializable]
    public class StringValue : GenericValue<string>
    {
        public StringValue()
        {
            k = string.Empty;
            v = string.Empty;
        }
    }
}
