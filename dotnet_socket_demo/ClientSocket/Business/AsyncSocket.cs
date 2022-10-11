﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ClientSocket.Business
{
    class AsyncSocket : Foundation.IClientSocket
    {
        Foundation.CClientSocket _socket = null;
        private static AsyncSocket _instance = null;

        private Thread _sendThread = null;
        private bool _shouldSendExit = false;
        private bool _isConnected = false;

        private int _timeout = 10;

        private ISocketDelegate _socketDelegate = null;

        public static AsyncSocket shareInstance()
        {
            if (null == _instance)
            {
                _instance = new AsyncSocket();
            }
            return _instance;
        }

        private AsyncSocket()
        {
            _socket = new Foundation.CClientSocket();
            _socket.init();
            _socket.setDelegate(this);
        }

        public void setPackageMaxSize(int packageMaxSize)
        {
            _socket.setPackageMaxSize(packageMaxSize);
        }

        public void setTimeout(int timeout)
        {
            _timeout = timeout;
        }

        public void connect(string ip,int port)
        {
            new Thread(delegate() {
                _socket.connectWithTimeout(ip, port, _timeout);
                }).Start();
        }

        public void disConnect()
        {
            _socket.disConnect();
        }

        public bool isConnected()
        {
            return _isConnected;
        }

        public void startSendLoop()
        {
            _sendThread = new Thread(new ThreadStart(loopSend));
            _sendThread.Start();
        }

        private void loopSend()
        {
            while (true)
            {
                if (_shouldSendExit) break;

                int queueSize = SendDataQueue.shareInstance().getSize();
                if (queueSize == 0) continue;

                byte[] dataBytes = SendDataQueue.shareInstance().getDataBytes();

                if ( dataBytes.Length > 0 )
                {
                    _socket.sendData(dataBytes);
                }
            }
        }

        public void send(short cmd, string dataInfo)
        {
            if (!_shouldSendExit)
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(dataInfo);
                byte[] sendBytes = ClientSocket.Foundation.Codec.GenCodec.encode(cmd, data);

                SendDataQueue.shareInstance().addDataBytes(sendBytes);
            }
        }

        public void setDelegate(ISocketDelegate socketDelegate)
        {
            _socketDelegate = socketDelegate;
        }

        public void onConnectSuccess()
        {
            _isConnected = true;

            _socket.startReadLoop();
            _shouldSendExit = false;
            this.startSendLoop();

            if (null != _socketDelegate) _socketDelegate.onConnectSuccess();
        }

        public void onConnectFail()
        {
            _isConnected = false;

            _shouldSendExit = true;
            if (null != _socketDelegate) _socketDelegate.onConnectFail();
        }

        public void onDisconnect()
        {
            _socket.init();
            _isConnected = false;

            _shouldSendExit = true;
            if (null != _socketDelegate) _socketDelegate.onDisconnect();

            SendDataQueue.shareInstance().clear();
        }

        public void onReceiveData(byte[] data, int length)
        {
            short cmd;
            byte[] dataBytes;
            ClientSocket.Foundation.Codec.GenCodec.decode(data, length, out cmd, out dataBytes);

            string response = System.Text.Encoding.UTF8.GetString(dataBytes);
            if (null != _socketDelegate)
            {
                _socketDelegate.onReceiveData(cmd, response);
            }
        }

        public void onSendSuccess(byte[] data)
        {
            SendDataQueue.shareInstance().removeDataBytes();
        }

        public void onSendFail(byte[] data)
        {
            SendDataQueue.shareInstance().removeDataBytes();
            this.disConnect();
        }
    }
}
