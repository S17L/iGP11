namespace iGP11.Tool.Model
{
    public class ValueDescription
    {
        public ValueDescription()
        {
        }

        public ValueDescription(string description, object value)
        {
            Description = description;
            Value = value;
        }

        public string Description { get; set; }

        public object Value { get; set; }
    }
}