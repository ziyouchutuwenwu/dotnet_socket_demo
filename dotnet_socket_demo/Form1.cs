using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClientSocket.Business;

namespace dotnet_socket_demo
{
    public partial class Form1 : Form, ISocketDelegate
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AsyncSocket.shareInstance().setDelegate(this);
            AsyncSocket.shareInstance().setTimeout(100000);
            AsyncSocket.shareInstance().setPackageMaxSize(4096);
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            AsyncSocket.shareInstance().connect("192.168.0.179", 9999);
        }

        public void onConnectSuccess()
        {
            MethodInvoker invoker = new MethodInvoker(() =>
            {
                string info = "连接成功";
                List<string> infoList = infoBox.Lines.ToList<string>();
                infoList.Add(info);
                infoBox.Lines = infoList.ToArray();
            });
            this.BeginInvoke(invoker);
        }

        public void onConnectFail()
        {
            MethodInvoker invoker = new MethodInvoker(() =>{
                string info = "连接失败";
                List<string> infoList = infoBox.Lines.ToList<string>();
                infoList.Add(info);
                infoBox.Lines = infoList.ToArray();
            });
            this.BeginInvoke(invoker);
        }

        public void onDisconnect()
        {
            MethodInvoker invoker = new MethodInvoker(() =>{
                string info = "连接断开";
                List<string> infoList = infoBox.Lines.ToList<string>();
                infoList.Add(info);
                infoBox.Lines = infoList.ToArray();
            });
            this.BeginInvoke(invoker);
        }

        public void onReceiveData(short cmd, string response)
        {
            MethodInvoker invoker = new MethodInvoker(() =>
            {
                string info = response;
                List<string> infoList = infoBox.Lines.ToList<string>();
                infoList.Add(info);
                infoBox.Lines = infoList.ToArray();
            });
            this.BeginInvoke(invoker);
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            AsyncSocket.shareInstance().send(1112, "i am android");
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            infoBox.Clear();
        }
    }
}
