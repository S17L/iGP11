using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace iGP11.Library.Network
{
    public sealed class NetworkListener : IListener
    {
        private readonly ICollection<INetworkCommandHandler> _handlers = new List<INetworkCommandHandler>();
        private readonly HttpListener _httpListener = new HttpListener();
        private readonly ILogger _logger;
        private readonly string _serverUri;
        private readonly int _timeout;

        private bool _isDisposed;
        private bool _isListening;
        private Thread _thread;

        public NetworkListener(string serverUri, ILogger logger, int timeout = 5000)
        {
            _serverUri = serverUri;
            _timeout = timeout;
            _logger = logger;
            _httpListener.Prefixes.Add(serverUri);
            _httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            try
            {
                Stop();
                _httpListener.Close();
            }
            finally
            {
                _isDisposed = true;
            }
        }

        public void Register(INetworkCommandHandler commandHandler)
        {
            _handlers.Add(commandHandler);
        }

        public void Start()
        {
            try
            {
                _logger.Log($"attempting to start server at: {_serverUri}...");
                _httpListener.Start();
            }
            catch (HttpListenerException exception)
            {
                _logger.Log(LogLevel.Error, $"exception occured while starting server; exception: {exception}");
                return;
            }

            _logger.Log("server has been started.");
            _isListening = true;
            _thread = new Thread(KeepListening);
            _thread.Start();
            _logger.Log("server is listening...");
        }

        public void Stop()
        {
            if (!_isListening)
            {
                return;
            }

            try
            {
                _logger.Log($"attempting to stop server at: {_serverUri}...");
                _httpListener.Stop();
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, $"exception occured while stopping server; exception: {exception}");
                throw;
            }
            finally
            {
                _isListening = false;
            }

            _logger.Log("server has been stoped.");
        }

        private static void InternalServerError(HttpListenerResponse response)
        {
            response.StatusCode = 500;
            response.StatusDescription = "internal server error";
            response.Close();
        }

        private void KeepListening()
        {
            while (_isListening)
            {
                ProcessRequest();
            }
        }

        private void ListenerCallback(IAsyncResult result)
        {
            if (!_isListening || _isDisposed)
            {
                return;
            }

            try
            {
                var context = _httpListener.EndGetContext(result);
                var response = context.Response;

                using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    var command = reader.ReadToEnd().Deserialize<Command>();
                    var handled = false;
                    CommandOutput commandOutput = null;

                    try
                    {
                        if (_handlers.Any(handler => handler.Handle(command, ref commandOutput)))
                        {
                            handled = true;
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.Log(LogLevel.Error, $"exception occured while handling request input stream: {exception}");
                        InternalServerError(response);

                        return;
                    }

                    if (handled && (commandOutput != null))
                    {
                        var buffer = Encoding.UTF8.GetBytes(commandOutput.Serialize());
                        response.ContentLength64 = buffer.Length;
                        var output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        output.Close();
                    }

                    response.StatusCode = 200;
                    response.StatusDescription = "OK";
                    response.Close();
                }
            }
            catch (HttpListenerException exception)
            {
                _logger.Log($"connection dropped; exception: {exception}");
            }
        }

        private void ProcessRequest()
        {
            try
            {
                if (_httpListener.IsListening)
                {
                    _httpListener.BeginGetContext(ListenerCallback, _httpListener)
                        .AsyncWaitHandle
                        .WaitOne(_timeout, true);
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (AbandonedMutexException)
            {
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, $"exception occured while processing request; exception: {exception}");
            }
        }
    }
}