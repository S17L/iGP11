using System;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using iGP11.Library;
using iGP11.Tool.Application.Model;

namespace iGP11.Tool.Infrastructure.Communication
{
    internal class CommunicationProxy : ICommunicationProxy
    {
        private const int InitialChunkLength = 10;
        private readonly string _address;
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly ushort _port;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly ILogger _logger;

        private bool _hasAccessDenied;
        private TcpClient _tcpClient;

        public CommunicationProxy(string address, ushort port, ILogger logger)
        {
            _address = address;
            _port = port;
            _logger = logger;
        }

        public void Dispose()
        {
            Disconnect();
        }

        public async Task<CommunicationResult<string>> PublishAsync(string request)
        {
            await _semaphore.WaitAsync();
            try
            {
                return await Task.Run(() => Publish(request));
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void Disconnect()
        {
            if (_tcpClient == null)
            {
                return;
            }

            try
            {
                if (_tcpClient.Connected)
                {
                    _tcpClient.GetStream()
                        .Close();
                }

                _tcpClient.Close();
            }
            catch (InvalidOperationException)
            {
            }
            finally
            {
                _tcpClient = null;
            }
        }

        private CommunicationResult<string> Publish(string request)
        {
            if (request.IsNullOrEmpty() || _hasAccessDenied)
            {
                _logger.Log(LogLevel.Error, "request is empty or access is denied");
                return new CommunicationResult<string>(false, null);
            }

            try
            {
                if (_tcpClient == null)
                {
                    _tcpClient = new TcpClient();
                    _tcpClient.Connect(_address, _port);
                }

                _tcpClient.Client.Send(_encoding.GetBytes(request.Length.ToString($"D{InitialChunkLength}")), SocketFlags.None);
                _tcpClient.Client.Send(_encoding.GetBytes(request), SocketFlags.None);
                var chunk = new byte[InitialChunkLength];
                _tcpClient.Client.Receive(chunk, SocketFlags.None);
                chunk = new byte[int.Parse(_encoding.GetString(chunk))];
                if (chunk.Length > 0)
                {
                    _tcpClient.Client.Receive(chunk, SocketFlags.None);
                }

                return new CommunicationResult<string>(true, _encoding.GetString(chunk));
            }
            catch (ObjectDisposedException)
            {
                _logger.Log(LogLevel.Error, "communication error due to client being disposed");
                _tcpClient = null;
            }
            catch (SecurityException)
            {
                _logger.Log(LogLevel.Error, "communication error due security exception");
                _hasAccessDenied = true;
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, $"communication error due to unknown exception; exception: {exception}");
                Disconnect();
            }

            return new CommunicationResult<string>(false, null);
        }
    }
}