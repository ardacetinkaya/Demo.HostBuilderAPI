namespace SomeKindOfProcessor
{

    public class CustomProcessorData : ICustomProcessorData
    {
        private string _name = string.Empty;
        public CustomProcessorData(string name)
        {
            _name = name;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }
    }
}
