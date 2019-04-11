using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace socket
{
    public partial class MainForm : Form
    {
        private string ip = "";
        private string port = "";
        private string serverStatus = "close";
        private int clientNumber = 0;
        private Dictionary<string, Socket> socketList = new Dictionary<string, Socket>();
        private Dictionary<string, SocketInfo> socketInfoList = new Dictionary<string, SocketInfo>();
        private BackgroundWorker socketConnectProcess;

        public class Data
        {
            public string from { get; set; }
            public string to { get; set; }
            public string data { get; set; }
            public string time { get; set; }
        }

        public class SocketInfo
        {
            public string name { get; set; }
            public string ip { get; set; }
            public string port { get; set; }
            public string character { get; set; }
        }

        public class AsyncState
        {
            public Socket socket { get; set; }
            public byte[] byteData { get; set; }
            public BackgroundWorker worker { get; set; }
        }

        private class ClientForm : Form
        {
            private MainForm ownerForm;
            private TextBox chatTextBox;
            private TextBox inputTextBox;
            private Button sendButton;

            private string clientName;
            private string clientStatus = "close";
            private Socket clientSocket;
            //private BackgroundWorker socketRecvProcess;



            public ClientForm()
            {
                InitializeComponent();
            }
            private void InitializeComponent()
            {

                this.chatTextBox = new TextBox();
                this.inputTextBox = new TextBox();
                this.sendButton = new Button();
                // 
                // chatTextBox
                // 
                this.chatTextBox.Location = new System.Drawing.Point(12, 12);
                this.chatTextBox.Multiline = true;
                this.chatTextBox.ReadOnly = true;
                this.chatTextBox.Name = "chatTextBox";
                this.chatTextBox.Size = new System.Drawing.Size(450, 300);
                this.chatTextBox.TabIndex = 1;
                // 
                // inputTextBox
                // 
                this.inputTextBox.Location = new System.Drawing.Point(12, 333);
                this.inputTextBox.Name = "inputTextBox";
                this.inputTextBox.Size = new System.Drawing.Size(355, 25);
                this.inputTextBox.TabIndex = 0;
                // 
                // sendButton
                // 
                this.sendButton.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                this.sendButton.Location = new System.Drawing.Point(370, 324);
                this.sendButton.Name = "sendButton";
                this.sendButton.Size = new System.Drawing.Size(86, 38);
                this.sendButton.TabIndex = 2;
                this.sendButton.Text = "发送";
                this.sendButton.UseVisualStyleBackColor = true;
                this.sendButton.Click += new EventHandler(sendButtonClick);
                //
                // ClientForm
                //
                this.Name = "ClientForm";
                this.Controls.Add(this.chatTextBox);
                this.Controls.Add(this.inputTextBox);
                this.Controls.Add(this.sendButton);
                this.ClientSize = new System.Drawing.Size(475, 380);
                this.Load += new EventHandler(this.ClientForm_FormLoad);
                this.FormClosed += new FormClosedEventHandler(this.ClientForm_FormClosed);
            }
            private void ClientForm_FormLoad(object sender, EventArgs e)
            {
                MainForm owner = (MainForm)this.Owner;
                this.ownerForm = owner;

                this.clientName = String.Format("客户端{0}", this.ownerForm.clientNumber);

                this.Text = this.clientName;

                startClient();
            }

            private void ClientForm_FormClosed(object sender, FormClosedEventArgs e)
            {
                // 关闭客户端窗口时，若存在连接的 socket，则关闭此 socket
                if (this.clientSocket != null)
                {
                    // 向服务器发送关闭信息
                    this.clientSocket.Shutdown(SocketShutdown.Send);
                }
            }

            private void startClient()
            {
                // socket 绑定 ip 和端口
                IPAddress ip = IPAddress.Parse(this.ownerForm.ip);
                int port = int.Parse(this.ownerForm.port);
                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // 尝试连接
                try
                {
                    clientSocket.Connect(new IPEndPoint(ip, port));
                    this.chatTextBox.Text += String.Format("{0} 系统 > 开始连接至 {1}:{2}\r\n", DateTime.Now.ToString(), Convert.ToString(ip), port);
                }
                catch
                {
                    this.chatTextBox.Text += String.Format("{0} 系统 > 连接失败\r\n", DateTime.Now.ToString());
                    this.sendButton.Enabled = false;
                    return;
                }
                this.chatTextBox.Text += String.Format("{0} 系统 > 连接建立，客户端 Socket 地址为 {1}\r\n", DateTime.Now.ToString(), clientSocket.LocalEndPoint.ToString());
                // 发送身份信息
                string reportMessage = JsonConvert.SerializeObject(new Data() { from = this.clientName, data = "hello！", time = DateTime.Now.ToString() });
                //string reportMessage = String.Format("ClientName:{0}", this.clientName);
                clientSocket.Send(Encoding.UTF8.GetBytes(reportMessage));

                this.clientSocket = clientSocket;
                this.clientStatus = "active";

                //AsyncReceive();
                // 开启后台工作接收服务端转发的数据并更新 chatLabel
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += new DoWorkEventHandler((sender, e) =>
                {
                    while (this.clientStatus != "close")
                    {  
                        Thread.Sleep(50);
                    }
                    //MessageBox.Show("DoWorker quit");
                });
                worker.ProgressChanged += new ProgressChangedEventHandler((sender, e) =>
                {
                    Data recvData = (Data)e.UserState;
                    newMessageDisplay(recvData);
                });
                worker.RunWorkerAsync();

                AsyncState asyncState = new AsyncState() { socket = clientSocket, byteData = new byte[1024], worker = worker };
                // offset=0, size=1024
                this.clientSocket.BeginReceive(asyncState.byteData, 0, asyncState.byteData.Length, SocketFlags.None, new AsyncCallback(AsyncReceiveCallBack), asyncState);
            }

            private void AsyncReceiveCallBack(IAsyncResult iar)
            {
                AsyncState asyncState = (AsyncState)iar.AsyncState;

                Data recvData;
                int recvLength;
                //socket 结束只能靠捕获异常吗？
                try
                {
                    recvLength = this.clientSocket.EndReceive(iar);
                }
                catch
                {
                    if (this.clientStatus == "close")
                    {
                        //MessageBox.Show(this.clientName + "残余线程已关闭");
                        MessageBox.Show("end 异常");
                    }
                    return;
                }

                
                if (recvLength > 0)
                {
                    recvData = JsonConvert.DeserializeObject<Data>(Encoding.UTF8.GetString(asyncState.byteData));
                    // 创建新的接收进程
                    AsyncState asyncStateNew = new AsyncState() { socket = asyncState.socket, byteData = new byte[1024], worker = asyncState.worker };
                    asyncState.socket.BeginReceive(asyncStateNew.byteData, 0, asyncState.byteData.Length, SocketFlags.None, new AsyncCallback(AsyncReceiveCallBack), asyncStateNew);
                }
                else
                {
                    recvData = new Data() { from = "系统", data = "与服务器连接已断开", time = DateTime.Now.ToString() };
                    asyncState.socket.Shutdown(SocketShutdown.Both);
                    asyncState.socket.Close();
                    this.clientSocket = null;
                    this.clientStatus = "close";
                }
                asyncState.worker.ReportProgress(1, (object)recvData);
            }

            private void newMessageDisplay(Data recvData)
            {
                this.chatTextBox.Text += String.Format("{0} {1} > {2}\r\n", recvData.time, recvData.from, recvData.data);
            }

            private void sendButtonClick(object sender, EventArgs e)
            {
                if(this.clientSocket != null)
                {
                    Data sendMessage = new Data() { from = clientName, to = "server", data = inputTextBox.Text, time = DateTime.Now.ToString() };
                    string sendMessage_json = JsonConvert.SerializeObject(sendMessage);
                    //MessageBox.Show(sendMessage_json);  
                    this.clientSocket.Send(Encoding.UTF8.GetBytes(sendMessage_json));
                }
            }

        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void serverStartButtonClick(object sender, EventArgs e)
        {
            if(this.ip == ipTextBox.Text && this.port == portTextBox.Text)
            {
                MessageBox.Show("同一地址不能重复使用！");
                return;
            }
            this.ip = ipTextBox.Text;
            this.port = portTextBox.Text;
            startServer();
        }

        private void clientStartButton_Click(object sender, EventArgs e)
        {
            this.ip = ipTextBox.Text;
            this.port = portTextBox.Text;
            this.clientNumber += 1;
            ClientForm NewClinetForm = new ClientForm();
            NewClinetForm.Owner = this;
            NewClinetForm.Show();
        }

        private void startServer()
        {
            IPAddress ip = IPAddress.Parse(this.ip);
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                serverSocket.Bind(new IPEndPoint(ip, int.Parse(this.port)));
            }
            catch(Exception e)
            {
                MessageBox.Show("绑定服务器失败，原因：\r\n" + e.ToString());
                return;
            }
            serverSocket.Listen(15);

            // 在 socketList 中加入服务器信息
            SocketInfo serverInfo = new SocketInfo();
            serverInfo.name = "server";
            serverInfo.ip = this.ip;
            serverInfo.port = this.port;
            serverInfo.character = "server";
            this.socketInfoList.Add(serverSocket.LocalEndPoint.ToString(), serverInfo);
            this.socketList.Add(serverSocket.LocalEndPoint.ToString(), serverSocket);
            // 设置服务器状态为激活
            this.serverStatus = "active";

            // 更新 socket 连接信息表
            SocketContInfoUpdate();
            // 开启 backgroundworker 运行异步监听连接请求并报告进展
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            // 让 DoWork 空转，使用它的 reportProgress 功能，之后可以把异步的函数换成同步让 DoWork 真正起到作用
            worker.DoWork += new DoWorkEventHandler((sender, e) =>
            {
                while (this.serverStatus != "close")
                {
                    Thread.Sleep(50);
                }
                MessageBox.Show("server quit");
            });
            worker.ProgressChanged += new ProgressChangedEventHandler((sender, e) => 
            {
                SocketContInfoUpdate();
            });
            worker.RunWorkerAsync();

            AsyncState asyncState = new AsyncState() { socket = serverSocket, worker = worker };
            serverSocket.BeginAccept(new AsyncCallback(AsyncAcceptCallback), asyncState);

            MessageBox.Show("服务端建立成功,开始监听...");
        }
        private void AsyncAcceptCallback(IAsyncResult iar)
        {
            // 结束异步监听进程并取得建立连接的 socket
            AsyncState asyncstate = (AsyncState)iar.AsyncState;
            Socket service = asyncstate.socket.EndAccept(iar);
            // 建立一个新的监听进程
            AsyncState asyncstateNew = new AsyncState() { socket = asyncstate.socket, worker = asyncstate.worker };
            asyncstate.socket.BeginAccept(new AsyncCallback(AsyncAcceptCallback), asyncstateNew);
            // 接受客户端发来的第一个表明身份的数据包
            byte[] recvByte = new byte[1024];
            service.Receive(recvByte);
            Data recvData = new Data();
            try
            {
                recvData = JsonConvert.DeserializeObject<Data>(Encoding.UTF8.GetString(recvByte));
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
                return;
            }
            MessageBox.Show(JsonConvert.SerializeObject(recvData));
            if (recvData.from.Length == 0)
            {
                MessageBox.Show("数据包格式不正确，连接已阻止");
                service.Shutdown(SocketShutdown.Both);
                service.Close();
                return;
            }
            else
            {
                // 在 socketList 中加入客户端信息
                SocketInfo client = new SocketInfo();
                string IPEndPoint = service.RemoteEndPoint.ToString();
                client.name = recvData.from;
                client.ip = IPEndPoint.Split(':')[0];
                client.port = IPEndPoint.Split(':')[1];
                client.character = "client";
                this.socketList.Add(IPEndPoint, service);
                this.socketInfoList.Add(IPEndPoint, client);
                MessageBox.Show(String.Format("与{0}建立连接", client.name));
                // 更新 socket 连接信息表
                asyncstate.worker.ReportProgress(1);
            }

            // 开始异步接收进程
            AsyncState recv_asyncState = new AsyncState() { socket = service, byteData = new byte[1024] , worker = asyncstate.worker };
            service.BeginReceive(recv_asyncState.byteData, 0, recv_asyncState.byteData.Length, SocketFlags.None, new AsyncCallback(AsyncReceiveCallback), recv_asyncState);
        }

        private void AsyncReceiveCallback(IAsyncResult iar)
        {
            // 结束接受并获取接受信息的长度
            AsyncState asyncState = (AsyncState)iar.AsyncState;
            int recvLenth = asyncState.socket.EndReceive(iar);
            // 若长度为 0，则代表连接中止
            if (recvLenth == 0)
            {
                
                string IPEndPoint = asyncState.socket.RemoteEndPoint.ToString();
                // 显示通知
                MessageBox.Show(String.Format("{0}已断开连接", socketInfoList[IPEndPoint].name));
                // 从列表中移除此 socket 信息
                this.socketList.Remove(IPEndPoint);
                this.socketInfoList.Remove(IPEndPoint);
                // 更新 socket 列表
                asyncState.worker.ReportProgress(1);
                // 关闭 socket
                //asyncState.socket.Shutdown(SocketShutdown.Both);
                asyncState.socket.Close();
                return;
            }
            // 开始另一个异步接受信息线程
            AsyncState asyncStateNew = new AsyncState() { socket = asyncState.socket, byteData = new byte[1024] , worker = asyncState.worker};
            asyncState.socket.BeginReceive(asyncStateNew.byteData, 0, asyncStateNew.byteData.Length, SocketFlags.None, new AsyncCallback(AsyncReceiveCallback), asyncStateNew);
            // 解码信息
            Data recvData = JsonConvert.DeserializeObject<Data>(Encoding.UTF8.GetString(asyncState.byteData));
            // 向所有客户端广播此信息
            Data boardcastData = new Data();
            boardcastData.from = recvData.from;
            boardcastData.to = recvData.to;
            boardcastData.data = recvData.data;
            boardcastData.time = recvData.time;
            Boardcast(boardcastData);

            MessageBox.Show(String.Format("{0} {1}输入：{2}", DateTime.Now.ToString(), recvData.from, recvData.data));
        }
        /// <summary>
        /// 广播接受到的消息给列表中的所有客户端（目前同一程序会向其所有服务器连接的所有客户端广播）
        /// </summary>
        /// <param name="boardcastData"></param>
        private void Boardcast(Data boardcastData)
        {
            string boardcastData_json = JsonConvert.SerializeObject(boardcastData);
            foreach (var item in this.socketList)
            {
                if (this.socketInfoList[item.Key].character != "server")
                {
                    item.Value.Send(Encoding.UTF8.GetBytes(boardcastData_json));
                }
            }
            //MessageBox.Show("消息已转发");
        }
        /// <summary>
        /// 更新右侧的 socket 连接信息
        /// </summary>
        private void SocketContInfoUpdate()
        {
            // 清空原信息
            infoLabel.Text = "";  
            // 打印每个服务器端 socket 的信息
            foreach (var item in socketInfoList)
            {
                //MessageBox.Show(item.Key + item.Value.character);
                if (item.Value.character == "server")
                {
                    infoLabel.Text += String.Format("服务端:{0}:{1}\r\n", item.Value.ip, item.Value.port);
                }
            }
            // 打印每个客户端 socket 的信息
            foreach (var item in socketInfoList)
            {
                if (item.Value.character == "client")
                {
                    infoLabel.Text += String.Format("客户端:{0}:{1}\r\n", item.Value.ip, item.Value.port);
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void InfoTitleLabel_Click(object sender, EventArgs e)
        {

        }

        private void IpTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }
    }
}
