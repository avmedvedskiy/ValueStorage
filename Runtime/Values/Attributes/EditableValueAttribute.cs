namespace ValueStorage
{
    public class EditableValueAttribute : ValueAttribute
    {
        public bool OnlyKey { get; }

        public EditableValueAttribute(string name, bool onlyKey = false) : base(name)
        {
            OnlyKey = onlyKey;
        }
    }
}
