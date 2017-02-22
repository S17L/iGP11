#include "stdafx.h"
#include "listener.h"

#include "Ws2tcpip.h"

const int InitialChunkLength = 10;
const unsigned long SocketNonBlockingMode = 1;
const long SocketNewConnectionTimeout = 5;
const long SocketResponseConnectionTimeout = 1;

std::string getString(std::string string) {
    return core::stringFormat(ENCRYPT_STRING("core::communication::TcpServer: %s"), string.c_str());
}

fd_set getSocketSet(SOCKET socket) {
    fd_set set;
    FD_ZERO(&set);
    FD_SET(socket, &set);

    return set;
}

timeval getTimeval(long seconds) {
    timeval timeval;
    timeval.tv_sec = seconds;
    timeval.tv_usec = 0;

    return timeval;
}

core::communication::TcpServer::~TcpServer() {
    stop();
    ::WSACleanup();
}

bool core::communication::TcpServer::start() {
    if (_isListening || _thread != nullptr) {
        return false;
    }

    WSADATA data;
    int result = ::WSAStartup(MAKEWORD(2, 2), &data);
    if (result != NO_ERROR) {
        return false;
    }

    _socket = ::socket(AF_INET, SOCK_STREAM, IPPROTO_IP);
    if (_socket == INVALID_SOCKET) {
        return false;
    }

    sockaddr_in address;
    ::memset(&address, 0, sizeof(address));
    address.sin_family = AF_INET;
    address.sin_port = htons(_port);
    ::inet_pton(address.sin_family, _address.c_str(), &(address.sin_addr));

    if (::bind(_socket, (sockaddr *)&address, sizeof(address)) == SOCKET_ERROR) {
        ::closesocket(_socket);
        return false;
    }

    if (::listen(_socket, 1) == SOCKET_ERROR) {
        ::closesocket(_socket);
        return false;
    }

    auto nonBlockingMode = SocketNonBlockingMode;
    if (::ioctlsocket(_socket, FIONBIO, &nonBlockingMode) != NO_ERROR) {
        ::closesocket(_socket);
        return false;
    };

    _isListening = true;
    _thread.reset(new std::thread([this]() { listen(); }));

    return true;
}

bool core::communication::TcpServer::stop() {
    if (_isListening || _thread) {
        ::closesocket(_socket);
        _isListening = false;
        _listenerSockets.clear();

        if (_thread != nullptr) {
            _thread->detach();
            _thread.reset();
        }
    }

    return true;
}

void core::communication::TcpServer::detach(core::communication::TcpListenerSocket *listenerSocket) {
    auto i = _listenerSockets.begin();
    while (i != _listenerSockets.end()) {
        if ((*i).get() == listenerSocket) {
            i = _listenerSockets.erase(i);
            break;
        }
        else {
            ++i;
        }
    }
}

void core::communication::TcpServer::listen() {
    while (_isListening) {
        auto readableSet = ::getSocketSet(_socket);
        auto readableTimeout = ::getTimeval(SocketNewConnectionTimeout);
        if (select(0, &readableSet, NULL, NULL, &readableTimeout) == 0) {
            log(::getString(ENCRYPT_STRING("no pending connections...")));
            continue;
        }

        auto socket = ::accept(_socket, NULL, NULL);
        if (socket != INVALID_SOCKET) {
            log(::getString(core::stringFormat(ENCRYPT_STRING("connection accepted [ socket: %llu ]"), socket)));
            auto nonBlockingMode = SocketNonBlockingMode;
            if (::ioctlsocket(_socket, FIONBIO, &nonBlockingMode) != NO_ERROR) {
                log(error, ::getString(core::stringFormat(ENCRYPT_STRING("socket could not be opened in non blocking mode: [ socket: %llu ]"), socket)));
                ::closesocket(_socket);
            }
            else {
                _listenerSockets.push_back(std::shared_ptr<TcpListenerSocket>(new TcpListenerSocket(shared_from_this(), socket)));
            }
        }
        else {
            log(::getString(ENCRYPT_STRING("socket could not be obtained")));
        }
    }
}

core::communication::TcpListener::TcpListener(const std::string &address, const unsigned short &port, IRequestHandler *requestHandler) {
    _tcpServer.reset(new core::communication::TcpServer(address, port, requestHandler));
}

bool core::communication::TcpListener::start() {
    return _tcpServer->start();
}

bool core::communication::TcpListener::stop() {
    return _tcpServer->stop();
}

core::communication::TcpListenerSocket::TcpListenerSocket(std::shared_ptr<core::communication::TcpServer> listener, SOCKET socket)
    : _listener(listener), _socket(socket) {
    _isWorking = true;
    _thread.reset(new std::thread([this]() { doWork(); }));
}

core::communication::TcpListenerSocket::~TcpListenerSocket() {
    _thread->detach();
    _thread.reset();
    ::closesocket(_socket);
    log(::getString(ENCRYPT_STRING("socket released")));
}

void core::communication::TcpListenerSocket::doWork() {
    while (_isWorking) {
        auto readableSet = ::getSocketSet(_socket);
        auto readableTimeout = ::getTimeval(SocketResponseConnectionTimeout);
        if (select(0, &readableSet, NULL, NULL, &readableTimeout) == 0) {
            log(debug, ::getString(core::stringFormat(ENCRYPT_STRING("socket disconnected [ code: %d ]"), ::WSAGetLastError())));
            _isWorking = false;
        }
        else {
            std::string request;
            if (receive(InitialChunkLength, request)) {
                log(debug, ::getString(core::stringFormat(ENCRYPT_STRING("initial request received [ size: %s ]"), request.c_str())));
                int requestLength = core::toInt(request);
                log(debug, ::getString(core::stringFormat(ENCRYPT_STRING("initial request size estimated [ size: %d ]"), requestLength)));
                if (receive(requestLength, request)) {
                    log(debug, ::getString(core::stringFormat(ENCRYPT_STRING("final request received [ request: %s ]"), request.c_str())));
                    auto response = _listener->_requestHandler->handle(request);
                    log(debug, ::getString(core::stringFormat(ENCRYPT_STRING("response estimated [ response: %s ]"), response.c_str())));
                    auto responseLength = static_cast<int>(response.length());
                    auto responseByteLength = core::toString(responseLength, InitialChunkLength);
                    if (::send(_socket, responseByteLength.c_str(), InitialChunkLength, 0) == SOCKET_ERROR) {
                        log(debug, ::getString(core::stringFormat(ENCRYPT_STRING("initial sending failed, socket disconnected [ code: %d ]"), ::WSAGetLastError())));
                        _isWorking = false;
                    }
                    else {
                        log(debug, ::getString(core::stringFormat(ENCRYPT_STRING("initial response sent [ size: %s ]"), responseByteLength.c_str())));
                        if (::send(_socket, response.c_str(), responseLength, 0) == SOCKET_ERROR) {
                            log(debug, ::getString(core::stringFormat(ENCRYPT_STRING("sending failed, socket disconnected [ code: %d ]"), ::WSAGetLastError())));
                            _isWorking = false;
                        }
                        else {
                            log(debug, ::getString(core::stringFormat(ENCRYPT_STRING("response sent [ message: %s ]"), response.c_str())));
                        }
                    }
                }
                else {
                    _isWorking = false;
                }
            }
            else {
                _isWorking = false;
            }
        }
    }

    _listener->detach(this);
}

bool core::communication::TcpListenerSocket::receive(int maxLength, std::string &response) {
    std::unique_ptr<char> chunk(new char[maxLength + 1]);
    int length = ::recv(_socket, chunk.get(), maxLength, 0);
    if (length == SOCKET_ERROR) {
        log(debug, ::getString(core::stringFormat(ENCRYPT_STRING("reading failed, socket disconnected [ code: %d ]"), ::WSAGetLastError())));
    }
    else if (length == 0) {
        log(debug, ::getString(core::stringFormat(ENCRYPT_STRING("reading no data, socket disconnected [ code: %d ]"), ::WSAGetLastError())));
    }
    else {
        chunk.get()[length] = '\0';
        response = std::string(chunk.get());

        return true;
    }

    return false;
}