using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace iGP11.Library.File
{
    public abstract class DatabaseContext<TDatabase>
    {
        private readonly DirectoryContext _directoryContext;
        private readonly string _fileName;
        private readonly object _lock = new object();
        private TDatabase _database;

        private IDatabaseEncryption _databaseEncryption;
        private IDatabaseSerializer<TDatabase> _databaseSerializer;
        private bool _initialized;

        protected DatabaseContext(string filePath)
        {
            _fileName = Path.GetFileName(filePath);
            _directoryContext = new DirectoryContext(Path.GetDirectoryName(filePath));
            _directoryContext.DemandAccess();
        }

        protected TDatabase Database
        {
            get
            {
                Initialize();
                return _database;
            }
        }

        public void Commit()
        {
            lock (_lock)
            {
                _directoryContext.CreateFile(
                    _fileName,
                    _databaseEncryption.Encrypt(_databaseSerializer.Serialize(_database)));
            }
        }

        public void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            var configurator = new DatabaseConfigurationBuilder<TDatabase>();
            OnDatabaseInitializing(configurator);

            if (configurator.DatabaseFactory == null)
            {
                throw new InvalidOperationException("database factory is undefined");
            }

            _databaseEncryption = configurator.DatabaseEncryption ?? new NoDatabaseEncryption();
            _databaseSerializer = configurator.DatabaseSerializer ?? new DataContractDatabaseSerializer<TDatabase>();

            var file = _directoryContext.GetFile(_fileName);
            if (file != null)
            {
                try
                {
                    _database = _databaseSerializer.Deserialize(_databaseEncryption.Decrypt(file.Content));
                }
                catch (CryptographicException)
                {
                    _database = configurator.DatabaseFactory.Create();
                }
                catch (FormatException)
                {
                    _database = configurator.DatabaseFactory.Create();
                }
                catch (SerializationException)
                {
                    _database = configurator.DatabaseFactory.Create();
                }
                catch (InvalidCastException)
                {
                    _database = configurator.DatabaseFactory.Create();
                }
            }
            else
            {
                _database = _database = configurator.DatabaseFactory.Create();
            }

            _initialized = true;
        }

        protected abstract void OnDatabaseInitializing(DatabaseConfigurationBuilder<TDatabase> configurator);
    }
}