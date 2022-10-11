using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientSocket.Business
{
    public interface ISocketDelegate
    {
        void onConnectSuccess();
        void onConnectFail();
        void onDisconnect();
        void onReceiveData(short cmd, string response);
    }
}
