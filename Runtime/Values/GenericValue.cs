using System;
namespace ValueStorage
{
    //short names used for editor and save data
    /// <summary>
    /// Generic value for nodes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericValue<T>
    {
        /// <summary>
        /// Key
        /// </summary>
        public string k;

        /// <summary>
        /// Value
        /// </summary>
        public T v;
    }
}
