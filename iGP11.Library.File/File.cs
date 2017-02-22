namespace iGP11.Library.File
{
    public class File
    {
        private string _content;

        public File(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public string Content
        {
            get { return _content = _content ?? Load(); }
        }

        public string Name { get; }

        public string Path { get; }

        private string Load()
        {
            return System.IO.File.ReadAllText(Path);
        }
    }
}