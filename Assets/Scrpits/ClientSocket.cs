using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

public class ClientSocket : MonoBehaviour {
    private IPAddress ip;
    private EndPoint port;
    private Socket clientSocket;

    private float waitTime = 5;  //超时时间
    private float connectTime = 0;//实际连接时间

	void Start () {
        InitSocket();

        Thread t = new Thread(StartConnect);
        t.Start(); 
	}
	
    void InitSocket()
    {
        ip = new IPAddress(new byte[] { 192, 168, 0, 128 });
        port = new IPEndPoint(ip, 1090);

        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    void StartConnect()
    {
        //连接服务器
        //clientSocket.Close();   //防止客户端重复连接同一个socket会导致卡死
        clientSocket.Connect(port);
        connectTime = 0;

        //等待连接成功
        while(!clientSocket.Connected)
        {
            connectTime += 0.3f;
            //...超时处理
            if(connectTime >waitTime)
            {
                Debug.Log("连接超时");
                return;
            }
        }

        Debug.Log("连接成功");

        MemoryStream newMs = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(newMs);
        bw.Write("确认连接成功");
        bw.Write(100);
        bw.Flush();
        bw.Close();

        clientSocket.Send(newMs.ToArray());

        while(clientSocket.Connected)
        {
            byte[] data = new byte[1024];
            clientSocket.Receive(data);
            Debug.Log("接收到服务器的协议");

            MemoryStream ms = new MemoryStream(data);
            BinaryReader br = new BinaryReader(ms);
            Debug.Log("Name:" + br.ReadString());
            Debug.Log("Passwrd:" + br.ReadString());
        }
    }
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDestroy()
    {
        clientSocket.Close();
        clientSocket = null;
    }
}

public class Login
{
    public string name;
    public string password;
}

public class LoginAck
{
    public string Result;
    public string RoleID;
}
