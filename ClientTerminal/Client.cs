using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientTerminal
{
    public delegate void ConnectHandler(Socket connectSocket);
    public partial class Client : Form
    {
        private readonly string _ip = "127.0.0.1";
        private readonly int _port = 20001;
        private Socket connectSocket = null;

        private event ConnectHandler PassiveDisConnect;
        private event ConnectHandler InitiativeDisConnect;
        public Client()
        {
            InitializeComponent();
            PassiveDisConnect += PassiveDisConnectDeal;
            InitiativeDisConnect += InitiativeDisConnectDeal;
            //启动客户端连接
            Start();
        }

        private void InitiativeDisConnectDeal(Socket connectSocket)
        {
            if (MsgtextBox.InvokeRequired)
                this.Invoke(new Action(()=> {
                    MsgtextBox.Text += "服务器主动断开连接\r\n"; 
                }));
        }

        private void PassiveDisConnectDeal(Socket connectSocket)
        {
            throw new NotImplementedException();
        }

        private void Start()
        {
            IPAddress iPAddress = IPAddress.Parse(_ip);
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, _port);
            connectSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            connectSocket.BeginConnect(iPEndPoint, ConnectCallback, connectSocket);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            Socket connSocket = ar.AsyncState as Socket;
            connSocket.EndConnect(ar);
            Thread tSendHBPacket = new Thread(SendHBPacket)
            {
                IsBackground = true
            };
            tSendHBPacket.Start(connSocket);
            Thread tRecvMsg = new Thread(RecvMsg)
            {
                IsBackground = true
            };
            tRecvMsg.Start();
        }

        private void RecvMsg()
        {
            //byte[] netPointBuffer = Encoding.UTF8.GetBytes("\0\0\0HB");
            while (true)
            {
                byte[] buffer = new byte[1024 * 1024];
                int recCount = 0;
                try
                {
                    recCount = connectSocket.Receive(buffer);
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode == 10054)
                    {
                        this.Invoke(new Action(()=> {
                            MsgtextBox.Text += "服务器主动关闭\r\n";
                        }));
                        break;
                    }
                }
                if (recCount > 0)
                {
                    byte[] tempBuffer = new byte[recCount];
                    Array.Copy(buffer, tempBuffer, recCount);
                    string recvNetPoint = Encoding.UTF8.GetString(tempBuffer);
                    if (recvNetPoint.Contains("\0\0\0sendNetPoint"))
                    {
                        this.Invoke(new Action(() => {
                            string finalTxt = recvNetPoint.Replace("\0\0\0sendNetPoint", "");
                            object[] obj = new object []{ finalTxt };
                            ClientcomboBox.Items.AddRange(obj);
                            //ClientcomboBox.DisplayMember = "Value";
                            ////ClientcomboBox.ValueMember = "Text";
                            ClientcomboBox.SelectedItem = obj[0];
                        }));
                    }
                    if (recvNetPoint.Contains("\0\0\0sendOfflineClient"))
                    {
                        string finalTxt = recvNetPoint.Replace("\0\0\0sendOfflineClient", "");
                        //string pattern = 
                        //recvNetPoint.Replace();
                        this.Invoke(new Action(() => {
                            object[] obj = new object[]{ finalTxt};
                            ClientcomboBox.Items.Remove(obj[0]);
                            //ClientcomboBox.SelectedItem = 0;
                            ClientcomboBox.SelectedIndex = ClientcomboBox.SelectionStart;
                            
                        }));
                    }
                }        
            }
        }

        private void SendHBPacket(object obj)
        {
            Socket connSocket = obj as Socket;
            byte[] HBBuffer = Encoding.UTF8.GetBytes("\0\0\0HB");
            while (true)
            {
                try
                {
                    connSocket.Send(HBBuffer);
                }
                catch(SocketException ex)
                {
                    if (ex.ErrorCode == 10053)
                    {
                        PassiveDisConnect?.Invoke(connSocket);
                    }
                    if (ex.ErrorCode == 10054)
                    {
                        InitiativeDisConnect?.Invoke(connSocket);
                    }

                    connSocket.Close();
                    break;
                }
                Thread.Sleep(3000);
            }
        }

        public byte[] ParseMsg(string text)
        {
            byte[] buffer = new byte[1024 * 1024];
            buffer = Encoding.UTF8.GetBytes(text);
            return buffer;
        }
        private void Client_Load(object sender, EventArgs e)
        {

        }

        private void Sendbutton_Click(object sender, EventArgs e)
        {
            try
            {
                connectSocket.Send(ParseMsg(SendMsgtextBox.Text));
                MsgtextBox.Text += SendMsgtextBox.Text + "\r\n";
            }
            catch (Exception)
            {
                MsgtextBox.Text += "!!!!!!!!"+SendMsgtextBox.Text + "\r\n";
            }
          
        }
    }
}
