using System;

namespace ValueStorage
{
    //Other types like float, string can be added later
    [Serializable]
    public class IntValue : GenericValue<int>
    {
        public IntValue()
        {
            k = string.Empty;
            v = 0;
        }

    }
}
