using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iGP11.Library;
using iGP11.Library.DDD;

namespace iGP11.Tool.Domain.Model.Directory
{
    public class Directory : AggregateRoot<string>
    {
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly ICollection<File> _files;
        private readonly ICollection<File> _newFiles = new List<File>();

        public Directory(string id, IEnumerable<File> files)
            : base(id)
        {
            _files = files?.ToList() ?? new List<File>();
        }

        public void AddFile(string name, string content)
        {
            AddFile(name, _encoding.GetBytes(content));
        }

        public void AddFile(string name, byte[] content)
        {
            _files.Remove(file => string.Equals(file.Name, name, StringComparison.OrdinalIgnoreCase));
            _files.Add(new File(name, content));
            _newFiles.Add(new File(name, content));
        }

        public void Commit()
        {
            _newFiles.Clear();
        }

        public IEnumerable<File> GetNewFiles()
        {
            return _newFiles.ToArray();
        }
    }
}