using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Transactions;

namespace iGP11.Library.File
{
    public class DirectoryContext
    {
        private const FileIOPermissionAccess PermissionLevel = FileIOPermissionAccess.AllAccess;
        private readonly string _directoryPath;
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly IDictionary<string, Journal> _journals = new Dictionary<string, Journal>();
        private readonly object _lock = new object();

        public DirectoryContext(string directoryPath)
        {
            _directoryPath = directoryPath;
        }

        public void CreateFile(string name, string content)
        {
            CreateFile(name, _encoding.GetBytes(content));
        }

        public void CreateFile(string name, byte[] content)
        {
            var filePath = GetFilePath(name);
            VerifyPermissions(filePath);

            AddOperation(new CreateFileOperation(filePath, content));
        }

        public void DeleteFile(string name)
        {
            var filePath = GetFilePath(name);
            VerifyPermissions(filePath);

            AddOperation(new DeleteFileOperation(filePath));
        }

        public void DemandAccess()
        {
            Directory.CreateDirectory(_directoryPath);
            new FileIOPermission(PermissionLevel, _directoryPath).Demand();
        }

        public File GetFile(string name)
        {
            var filePath = GetFilePath(name);
            VerifyPermissions(filePath);

            return System.IO.File.Exists(filePath)
                       ? new File(name, filePath)
                       : null;
        }

        public IEnumerable<File> GetFiles(string pattern = null)
        {
            return Directory.EnumerateFiles(_directoryPath, pattern ?? "*.*", SearchOption.TopDirectoryOnly)
                .Select(filePath => new File(Path.GetFileName(filePath), filePath))
                .ToArray();
        }

        internal void Remove(Journal journal)
        {
            lock (_lock)
            {
                _journals.Remove(pair => ReferenceEquals(pair.Value, journal));
            }
        }

        private static void VerifyPermissions(string filePath)
        {
            if (!FileAccessUtility.HasAccess(filePath, PermissionLevel))
            {
                throw new UnauthorizedAccessException($"insufficient permissions to access: {filePath}");
            }
        }

        private void AddOperation(IOperation operation)
        {
            var transation = Transaction.Current;
            if ((transation != null) && (transation.TransactionInformation.Status == TransactionStatus.Active))
            {
                Journal journal;
                lock (_lock)
                {
                    var id = transation.TransactionInformation.LocalIdentifier;
                    if (!_journals.ContainsKey(id))
                    {
                        journal = _journals.AddAndReturn(new KeyValuePair<string, Journal>(id, new Journal(this)))
                            .Value;
                        transation.EnlistVolatile(journal, EnlistmentOptions.None);
                    }
                    else
                    {
                        journal = _journals[id];
                    }
                }

                journal.Add(operation);
                return;
            }

            operation.Commit();
        }

        private string GetFilePath(string name)
        {
            return Path.Combine(_directoryPath, name);
        }
    }
}