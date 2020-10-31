using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;


namespace FTPServer
{
    public partial class Form1 : Form
    {
        static class CommandDefine
        {
            public const string ctransferStart = "501";
            public const string ctransfering = "502";
            public const string ctransferEnd = "503";

            public const string stransferStart = "601";
            public const string stransfering = "602";
            public const string stransferEnd = "603";

            public const byte packetHead = 0x000;
            public const byte dataHead = 0x001;
        }

        static string selectFolder;
        TCPServer tcpServer;
        Thread serverThread;
        FolderBrowserDialog folderBrowserDialog;
        Boolean bStart;

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    
        public Form1()
        {
            InitializeComponent();

            textBoxIP.Text = GetLocalIPAddress();
            textBoxPort.Text = "100";
         
            tcpServer = new TCPServer(this);
            folderBrowserDialog = new FolderBrowserDialog();
            bStart = false;

            // default
            selectFolder = @"D:\Me\Code\C#\FTP Server Code\upload folder";
            textBoxFolder.Text = selectFolder;
        }

        private void buttonListen_Click(object sender, EventArgs e)
        {
            try
            {
                if (buttonListen.Text == "Listen")
                {
                    if (!bStart)
                    {
                        serverThread = new Thread(tcpServer.StartServer);
                        serverThread.IsBackground = true;
                        serverThread.Start();
                        bStart = true;
                    }
                    else
                    {
                        serverThread.Resume();
                    }
                    buttonListen.Text = "Close";
                }
                else
                {
                    buttonListen.Text = "Listen";
                    serverThread.Suspend();
                }
            }
            catch (SocketException error)
            {
                String message;
                message = String.Format("SocketException: {0}", error);
                MessageBox.Show(message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxFolder.Text = folderBrowserDialog.SelectedPath;
                selectFolder = folderBrowserDialog.SelectedPath;
            }
        }

        // TCP Server 
        class TCPServer
        {
            Form1 mainWindow;
            TcpListener tcpListener;
          
            public TCPServer(Form1 f)
            {
                mainWindow = f;
                tcpListener = new TcpListener(IPAddress.Any, System.Convert.ToInt32(mainWindow.textBoxPort.Text));
            }

            public void StartServer()
            {
                tcpListener.Start();
                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    // Add connect client IP to list
                    IPEndPoint ipEndpoint = tcpClient.Client.RemoteEndPoint as IPEndPoint;
                    string connectMessage = "Client ip = " + ipEndpoint.Address.ToString() + " connected to server.";
                    if (mainWindow.listBoxClient.InvokeRequired)
                    {
                        mainWindow.Invoke(new MethodInvoker(delegate()
                        { mainWindow.listBoxClient.Items.Add(connectMessage); }));
                    }
                    else
                    {                       
                        mainWindow.listBoxClient.Items.Add(connectMessage);                  
                    }  
                    // Create socket handler for dealing client request
                    SocketHandler socketHandler = new SocketHandler(tcpClient);
                    Thread t1 = new Thread(socketHandler.ProcessSocketRequest);
                    t1.IsBackground = true;
                    t1.Start();
                }
            }

            public void CloseServer()
            {
                tcpListener.Stop();
            }

            class SocketHandler
            {
                TcpClient client;

                public SocketHandler(TcpClient tc)
                {
                    client = tc;
                }

                public void ProcessSocketRequest()
                {
                    try
                    {
                        NetworkStream networkStream = client.GetStream();
                        FileStream fstream = null;
                        long curFilePointer = 0;
                        int maxBufferLen = 500000;
                        while (true)
                        {
                            if (networkStream.ReadByte() == CommandDefine.packetHead)
                            {
                                byte[] command = new byte[3];
                                networkStream.Read(command, 0, command.Length);
                                byte[] receivedData = ReadDataPacket(networkStream);
                                switch (System.Text.Encoding.UTF8.GetString(command))
                                {
                                    // Client upload
                                    case CommandDefine.ctransferStart:
                                        {
                                            string fileName = System.Text.Encoding.UTF8.GetString(receivedData);
                                            fstream = new FileStream(selectFolder + '\\' + fileName, FileMode.Create);
                                            byte[] sendData = CreateDataPacket(System.Text.Encoding.UTF8.GetBytes(CommandDefine.ctransfering), System.Text.Encoding.UTF8.GetBytes(System.Convert.ToString(curFilePointer)));
                                            networkStream.Write(sendData, 0, sendData.Length);
                                            networkStream.Flush();
                                        }
                                        break;
                                    case CommandDefine.ctransfering:
                                        {
                                            fstream.Seek(curFilePointer, SeekOrigin.Begin);
                                            fstream.Write(receivedData, 0, receivedData.Length);
                                            curFilePointer = fstream.Position; // update file pointer
                                            byte[] sendData = CreateDataPacket(System.Text.Encoding.UTF8.GetBytes(CommandDefine.ctransfering), System.Text.Encoding.UTF8.GetBytes(System.Convert.ToString(curFilePointer))); // Send it to client 
                                            networkStream.Write(sendData, 0, sendData.Length);
                                            networkStream.Flush();
                                        }
                                        break;
                                    case CommandDefine.ctransferEnd:
                                        {
                                            curFilePointer = 0;
                                            networkStream.Flush();
                                            fstream.Close();
                                        }
                                        break;

                                    // Client download
                                    case CommandDefine.stransferStart:  // Server先收到Client傳來的要下載的檔案,並且cmd為stransferStart
                                        {
                                            string filePath = System.Text.Encoding.UTF8.GetString(receivedData);
                                            fstream = new FileStream(filePath, FileMode.Open);
                                            curFilePointer = 0;
                                            int sendBufferLen = (int)((fstream.Length - curFilePointer) < maxBufferLen ? (fstream.Length - curFilePointer) : maxBufferLen);
                                            byte[] sendBuffer = new byte[sendBufferLen];
                                            fstream.Seek(curFilePointer, SeekOrigin.Begin);
                                            fstream.Read(sendBuffer, 0, sendBuffer.Length);
                                            byte[] sendData = CreateDataPacket(Encoding.UTF8.GetBytes(CommandDefine.stransfering), sendBuffer);
                                            networkStream.Write(sendData, 0, sendData.Length);
                                            networkStream.Flush();
                                        }
                                        break;

                                    case CommandDefine.stransfering:
                                        {
                                            curFilePointer = long.Parse(Encoding.UTF8.GetString(receivedData));    // receive file pointer from client
                                            if (curFilePointer != fstream.Length && curFilePointer < fstream.Length)
                                            {
                                                fstream.Seek(curFilePointer, SeekOrigin.Begin);
                                                int sendBufferLen = (int)((fstream.Length - curFilePointer) < maxBufferLen ? (fstream.Length - curFilePointer) : maxBufferLen);
                                                byte[] sendBuffer = new byte[sendBufferLen];
                                                fstream.Read(sendBuffer, 0, sendBuffer.Length);
                                                byte[] sendData = CreateDataPacket(Encoding.UTF8.GetBytes(CommandDefine.stransfering), sendBuffer);  // send file data to Client
                                                networkStream.Write(sendData, 0, sendData.Length);
                                                networkStream.Flush();                                              
                                            }
                                            else
                                            {
                                                byte[] sendData = CreateDataPacket(Encoding.UTF8.GetBytes(CommandDefine.stransferEnd), Encoding.UTF8.GetBytes("Close"));    // file transfer complete, send Close message
                                                networkStream.Write(sendData, 0, sendData.Length);
                                                networkStream.Flush();
                                                fstream.Close();
                                            }
                                            break;
                                        }
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    catch (SocketException socketException)
                    {
                        string message;
                        message = String.Format("ProcessSocketRequest Fail, errorCode={0}", socketException.ErrorCode);
                        MessageBox.Show(message);
                    }
                    catch (IOException)
                    {
                        
                        return ;
                    }
                    catch (ObjectDisposedException)
                    {
                        
                        return;
                    }
                }

               
                private byte[] ReadDataPacket(NetworkStream ns)
                {
                    byte[] dataBuffer = null;
                    int length = 0;
                    String strDataLength = "";
                    while ((length = ns.ReadByte()) != CommandDefine.dataHead) // read data length
                    {
                        strDataLength += (char)length;
                    }
                    int dataLength = System.Convert.ToInt32(strDataLength);
                    dataBuffer = new byte[dataLength];

                    int byteOffset = 0;
                    while (byteOffset < dataLength)
                    {
                        byteOffset += ns.Read(dataBuffer, byteOffset, dataLength - byteOffset);
                    }
                    return dataBuffer;
                }

                private byte[] CreateDataPacket(byte[] cmd, byte[] data)
                {                  
                    byte[] pacHead = new byte[1];
                    pacHead[0] = CommandDefine.packetHead;
                    byte[] datHead = new byte[1];
                    datHead[0] = CommandDefine.dataHead;
                    byte[] datalength = System.Text.Encoding.UTF8.GetBytes(System.Convert.ToString(data.Length));
                    MemoryStream stream = new MemoryStream();
                    stream.Write(pacHead, 0, pacHead.Length);
                    stream.Write(cmd, 0, cmd.Length);
                    stream.Write(datalength, 0, datalength.Length);
                    stream.Write(datHead, 0, datHead.Length);
                    stream.Write(data, 0, data.Length);
                    return stream.ToArray();
                }
            }
        }
    }
}
