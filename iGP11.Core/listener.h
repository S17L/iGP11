#pragma once

#include "stdafx.h"
#include "igp11core.h"

using namespace core::logging;

namespace core {
	namespace communication {
		class TcpServer : public std::enable_shared_from_this<TcpServer> {
			friend class TcpListenerSocket;
		private:
			std::string _address;
			unsigned short _port;
			IRequestHandler *_requestHandler;
			std::list<std::shared_ptr<TcpListenerSocket>> _listenerSockets;
			bool _isListening;
			SOCKET _socket;
			std::unique_ptr<std::thread> _thread;
		public:
			TcpServer(const std::string &address, const unsigned short &port, IRequestHandler *requestHandler)
				: _address(address), _port(port), _requestHandler(requestHandler) {}
			virtual ~TcpServer();
			bool start();
			bool stop();
		private:
			void detach(TcpListenerSocket *listenerSocket);
			void listen();
		};

        class TcpListener : public IListener {
        private:
            std::shared_ptr<TcpServer> _tcpServer;
        public:
            TcpListener(const std::string &address, const unsigned short &port, IRequestHandler *requestHandler);
            virtual ~TcpListener() {};
            virtual bool start() override;
            virtual bool stop() override;
        };

		class TcpListenerSocket {
		private:
			std::shared_ptr<TcpServer> _listener;
			SOCKET _socket;
			bool _isWorking;
			std::unique_ptr<std::thread> _thread;
		public:
			TcpListenerSocket(std::shared_ptr<TcpServer> listener, SOCKET socket);
			~TcpListenerSocket();
		private:
			void doWork();
			bool receive(int maxLength, std::string &response);
		};
	}
}