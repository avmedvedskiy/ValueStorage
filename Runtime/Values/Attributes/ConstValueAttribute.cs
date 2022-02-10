using System;

namespace ValueStorage
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class ConstValueAttribute : ValueAttribute
    {
        public ConstValueAttribute(string name) : base(name)
        {

        }
    }
}
