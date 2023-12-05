namespace ValueStorage
{
    public class EditableValueAttribute : ValueAttribute
    {
        public bool ReadOnly { get; }

        public EditableValueAttribute(string name, bool readOnly = false) : base(name)
        {
            ReadOnly = readOnly;
        }
    }
}
