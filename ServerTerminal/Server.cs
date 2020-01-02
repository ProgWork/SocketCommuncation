using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerTerminal
{
    public delegate void ConnectEventHandler(Socket acceptSocket);
    public partial class Server : Form
    {
        public event ConnectEventHandler DisConnect;
        public event ConnectEventHandler OffLine;
        private readonly string _ip = "127.0.0.1";
        private readonly int _port = 20001;
        private Dictionary<string, ClientPacket> _clientDic;
        //private List<ClientPacket> _clientList;
        private Thread tHeartBeat;
        private object locker = new object();
        public Server()
        {
            InitializeComponent();
            this.DisConnect += DisConnectDeal;
            this.OffLine += OffLineDeal;
            Start();//启动服务器
        }

        private void OffLineDeal(Socket acceptSocket)
        {
            this.Invoke(new Action(() => {
                MsgtextBox.Text += "客户端：" + acceptSocket.RemoteEndPoint.ToString() + " 下线\r\n";
            }));
           
            _clientDic.Remove(acceptSocket.RemoteEndPoint.ToString());
            string head = "\0\0\0sendOfflineClient";
            InformClientOfflineInfo(acceptSocket.RemoteEndPoint.ToString(),head);
            

            acceptSocket.Close();
        }

        private void DisConnectDeal(Socket acceptSocket)
        {
            this.Invoke(new Action(() => {
                MsgtextBox.Text += "客户端：" + acceptSocket.RemoteEndPoint.ToString() + " 已经掉线\r\n";
            }));
            _clientDic.Remove(acceptSocket.RemoteEndPoint.ToString());
            //_clientList.re
            acceptSocket.Close();
        }

        public void Start()
        {
            _clientDic = new Dictionary<string, ClientPacket>();
            //开启心跳检测线程
            tHeartBeat = new Thread(HeartBeatCheck)
            {
                IsBackground = true
            };
            tHeartBeat.Start();
            //监听端口
            ConfigListen();
        }

        private void HeartBeatCheck()
        {
            while (true)
            {
                lock (locker)
                {
                    for(int i = _clientDic.Count-1;i>=0;i--)
                    {
                        if (_clientDic.ElementAtOrDefault(i).Value.countHB == 5)
                        {
                            DisConnect?.Invoke(_clientDic.ElementAtOrDefault(i).Value.acceptSocket);
                        }
                        else if (_clientDic.ElementAtOrDefault(i).Value.countHB < 5)
                        {
                            _clientDic.ElementAtOrDefault(i).Value.countHB += 1;
                        }
                    }
                    Thread.Sleep(1000);
                }
            }          
        }

        public void ConfigListen()
        {
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress iPAddress = IPAddress.Parse(_ip);
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, _port);
            listenSocket.Bind(iPEndPoint);
            listenSocket.Listen(20);
            //创建监听线程
            Thread tListen = new Thread(ListenConnect)
            {
                IsBackground = true
            };
            tListen.Start(listenSocket);
        }

        private void ListenConnect(object obj)
        {
            while (true)
            {
                Socket acceptSocket = (obj as Socket).Accept();
                if (acceptSocket != null)
                {
                   
                    //ClientInfo clientInfo = new ClientInfo(System.DateTime.Now,acceptSocket,true);

                    string netPointStr = acceptSocket.RemoteEndPoint.ToString();
                    ClientPacket clientPacket = new ClientPacket(acceptSocket, 0);
                    this.Invoke(new Action(() => {
                        MsgtextBox.Text += "客户端：" + netPointStr + " 上线\r\n";
                    }));
                    string sendMsgHead = "\0\0\0sendNetPoint";
                    InformClientOnlineInfo(netPointStr,acceptSocket, sendMsgHead);
                    lock (locker)
                    {
                        _clientDic.Add(netPointStr, clientPacket);

                    }                   
                    Thread tRecvMsg = new Thread(RecvMsg)
                    {
                        IsBackground = true
                    };
                    tRecvMsg.Start(acceptSocket);
                }
                Thread.Sleep(200);
            }    
        }
        public byte[] ParseMsg(string text)
        {
            byte[] buffer = new byte[1024 * 1024];
            buffer = Encoding.UTF8.GetBytes(text);
            return buffer;
        }
        private void InformClientOnlineInfo(string netPointStr,Socket acceptSocket,string head)
        {
            lock (locker)
            {
                for (int i= 0; i < _clientDic.Count; i++)
                {   
                    _clientDic.ElementAt(i).Value.acceptSocket.Send(ParseMsg(head+netPointStr));   
                    acceptSocket.Send(ParseMsg(head + _clientDic.ElementAt(i).Key));           
                }
                
            }
        }
        private void InformClientOfflineInfo(string netPointStr, string head)
        {
            lock (locker)
            {
                for (int i = 0; i < _clientDic.Count; i++)
                {
                    _clientDic.ElementAt(i).Value.acceptSocket.Send(ParseMsg(head + netPointStr));
                }
            }
        }

        private void RecvMsg(object obj)
        {
            Socket acceptSocket = obj as Socket;
            string netPointStr = acceptSocket.RemoteEndPoint.ToString();
            string msg = null;           
            byte[] HBBuffer = Encoding.UTF8.GetBytes("\0\0\0HB");
            //int readCount = 0; 
            while (true)
            {

                int readCount = 0;
                byte[] buffer = new byte[1024 * 1024];
                try
                {
                    readCount = acceptSocket.Receive(buffer);
                }
                catch (SocketException ex)
                {
                    //MessageBox.Show(string.Format("错误代码{0}",ex.ErrorCode));
                    if (ex.ErrorCode == 10054)
                    {
                        OffLine?.Invoke(acceptSocket);
                    }
                    lock (locker)
                    {
                        _clientDic.Remove(netPointStr);
                    }
                    break;
                }

                if (readCount != 0)
                {
                    lock (_clientDic)
                    {
                        _clientDic[netPointStr].countHB = 0;
                    }
                    byte[] tempBuffer = new byte[readCount];
                    Array.Copy(buffer, tempBuffer, readCount);
                    if (!tempBuffer.SequenceEqual(HBBuffer))
                    {
                        msg = Encoding.UTF8.GetString(buffer);
                        this.Invoke(new Action(() => {
                            string msgTxt = "[接收] " + netPointStr + ":" + msg;
                            MsgtextBox.Text = string.Join("\r\n",new string[] { MsgtextBox.Text,msgTxt});
                        }));
                    }  
                }
                //Thread.Sleep(200);
            }
        }

        public class ClientPacket
        {
            public Socket acceptSocket;
            public int countHB;

            public ClientPacket(Socket acceptSocket, int count)
            {
                this.acceptSocket = acceptSocket;
                this.countHB = count;
            }
        }
        //public class ClientInfo
        //{
        //    public DateTime lastComTime;
        //    public Socket clientSocket;
        //    public bool state;

        //    public ClientInfo(DateTime lastComTime, Socket clientSocket, bool state)
        //    {
        //        this.lastComTime = lastComTime;
        //        this.clientSocket = clientSocket;
        //        this.state = state;
        //    }
        //}
    }
   
}
