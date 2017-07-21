using System;
using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using AssemblyCSharp;
using System.IO;

public class CustomSocket
{
	//reset对象
	//public ManualResetEvent connectDone = new ManualResetEvent(false);
	//tcp客户端,即是与服务端通讯的组件
	TcpClient tcpclient = new TcpClient();
	//网络流
	NetworkStream stream;

	//	//接受到的数据
	//	public byte[] databuffer = new byte[StateObject.BufferSize];
	//	//数据缓冲区;
	//	public int offset = 0;
	//	//处理的位置
	//	public int end = 0;
	//数据缓冲区的长度

	byte[] sources;
	byte[] headBytes;

	int waitLen = 0;
	bool isWait = false;

	private int disConnectCount = 0;

	public bool hasStartTimer = false;
	public bool isConnected = false;

	private static CustomSocket _instance;

	public static CustomSocket getInstance()
	{
		if (_instance == null) {
			_instance = new CustomSocket();
			//_instance.Connect ();
		}

		return _instance;
	}

	public CustomSocket()
	{
//		HeadRequest head = new HeadRequest();
//		headBytes = head.ToBytes();
		headBytes = new ClientRequest(ApiCode.HeartRequest).ToBytes();
	}

	/// <summary>
	/// 连接到服务器
	/// </summary>
	/// <param name="ip">服务器IP</param>
	/// <returns></returns>
	/// 
	public void Connect()
	{
		//disCountTimer ();

		try {
			if (APIS.useDNS == false) {
				//IPAddress[] address = Dns.GetHostAddresses();
				tcpclient = new TcpClient();
				//防止延迟,即时发送!
				tcpclient.NoDelay = true;
				Debug.Log("connect to :" + APIS.SocketUrl + "  port:" +APIS.SocketPort);
				tcpclient.BeginConnect(APIS.SocketUrl, APIS.SocketPort, new AsyncCallback(ConnectCallback), tcpclient);
			} else {
				IPAddress[] address = Dns.GetHostAddresses(APIS.DNS);
				tcpclient = new TcpClient(address [0].AddressFamily);
				//防止延迟,即时发送!
				tcpclient.NoDelay = true;
				Debug.Log("connect to :" + APIS.DNS+" ip:" +address[0] + "  port:" +APIS.DNS_PORT);
				tcpclient.BeginConnect(address [0], APIS.DNS_PORT, new AsyncCallback(ConnectCallback), tcpclient);
			}
		} catch (Exception ex) {
			//设置标志,连接服务端失败!
			showMessageTip("服务器断开连接，请重新运行程序或稍后再试");
			//	ReConnectScript.getInstance().ReConnectToServer(); 
			Debug.Log(ex.ToString());
			isConnected = false;
		}
	}

	/// <summary>
	/// 关闭网络流
	/// </summary>
	private void DisConnect()
	{
		if (tcpclient != null) {
			tcpclient.Close();
			tcpclient = null;
		}
		if (stream != null) {
			stream.Close();
			stream = null;
		}
	}

	public void sendMsg(ClientRequest client)
	{
		Debug.LogFormat("send msg, head code:" + client.headCode);
		SendData(client.ToBytes());
	}

	/// <summary>
	/// 发送数据
	/// </summary>
	private void SendData(byte[] data)
	{
		try {
			Debug.Log("total length:" + data.Length);
            string str = "";
            for (int i = 0; i < data.Length; i++)
            {
                str += data[i] + " ";
            }
            Debug.Log(str);
			if (stream != null) {
				stream.Write(data, 0, data.Length);
			} else {
				showMessageTip("服务器断开连接，请重新运行程序或稍后再试");
				isConnected = false;
				SocketEventHandle.getInstance().noticeDisConect();
				//ReConnectScript.getInstance().ReConnectToServer(); 
				//Connect();
			}
			Debug.Log("send over");
		} catch (Exception ex) {
			Debug.Log(ex.ToString());
			//	showMessageTip("服务器断开连接，请重新运行程序或稍后再试");
			isConnected = false;
			SocketEventHandle.getInstance().noticeDisConect();
			//ReConnectScript.getInstance().ReConnectToServer(); 
			//Connect();
		}

	}

	/// <summary>
	/// 发送心跳包
	/// </summary>
	/// *
	public bool SendHeartBeatData()
	{
		try {
			if (stream != null && tcpclient.Connected) {
				stream.Write(headBytes, 0, headBytes.Length);
				Debug.Log("custom socket send head data length: " + headBytes.Length);
				return true;
			} else {
				isConnected = false;
				SocketEventHandle.getInstance().noticeDisConect();
				return false;
			}
		} catch (Exception ex) {
			Debug.Log(ex.ToString());
			isConnected = false;
			showMessageTip("服务器已断开连接，请重新登录");
			isConnected = false;
			SocketEventHandle.getInstance().noticeDisConect();
			return false;
		}
	}

	private void showMessageTip(string message)
	{
		ClientResponse temp = new ClientResponse();
		temp.headCode = APIS.TIP_MESSAGE;
		temp.message = message;
		SocketEventHandle.getInstance().addResponse(temp);
	}

	/// <summary>
	/// 异步连接的回调函数
	/// </summary>
	/// <param name="ar"></param>
	private void ConnectCallback(IAsyncResult ar)
	{
		//connectDone.Set();
		if ((tcpclient != null) && (tcpclient.Connected)) {
			stream = tcpclient.GetStream();
			asyncread(tcpclient);
			isConnected = true;
			Debug.Log("服务器已经连接!");
		}
		TcpClient t = (TcpClient)ar.AsyncState;
		try {
			t.EndConnect(ar);
		} catch (Exception ex) {
			//设置标志,连接服务端失败!
			Debug.Log(ex.ToString());
			//	tcpclient.BeginConnect(APIS.SocketUrl, 1101, new AsyncCallback(ConnectCallback), tcpclient);


		}
	}

	/// <summary>
	/// 异步读TCP数据
	/// </summary>
	/// <param name="sock"></param>
	private void asyncread(TcpClient sock)
	{
		StateObject state = new StateObject();
		state.client = sock;
		NetworkStream stream;
		try {
			stream = sock.GetStream();
			if (stream.CanRead) {
				try {
					stream.BeginRead(state.buffer, 0, StateObject.BufferSize,
						new AsyncCallback(TCPReadCallBack), state);

				} catch (Exception ex) {
					//设置标志,连接服务端失败!

					Debug.Log(ex.ToString());
				}
			}
		} catch (Exception ex) {
			//设置标志,连接服务端失败!
			// NetManaged.isConnectServer = false;
			// NetManaged.surcessstate = 0;
			Debug.Log(ex.ToString());
		}

	}

	/// <summary>
	/// TCP读数据的回调函数
	/// </summary>
	/// <param name="ar"></param>
	private void TCPReadCallBack(IAsyncResult ar)
	{
		StateObject state = (StateObject)ar.AsyncState;
		//主动断开时
		if ((state.client == null) || (!state.client.Connected)) {
			closeSocket();
			return;
		}
		int numberOfBytesRead;
		NetworkStream mas = state.client.GetStream();
		numberOfBytesRead = mas.EndRead(ar);
		state.totalBytesRead += numberOfBytesRead;
		if (numberOfBytesRead > 0) {
			byte[] dd = new byte[numberOfBytesRead];
			Array.Copy(state.buffer, 0, dd, 0, numberOfBytesRead);
			if (isWait) {
				byte[] temp = new byte[sources.Length + dd.Length];
				sources.CopyTo(temp, 0);
				dd.CopyTo(temp, sources.Length);
				sources = temp;
				if (sources.Length >= waitLen) {
					ReceiveCallBack(sources.Clone() as byte[]);
					isWait = false;
					waitLen = 0;
				}
			} else {
				sources = null;
				ReceiveCallBack(dd);
			}
			mas.BeginRead(state.buffer, 0, StateObject.BufferSize,
				new AsyncCallback(TCPReadCallBack), state);
		} else {
			//被动断开时 
			mas.Close();
			state.client.Close();
			mas = null;
			state = null;
			//设置标志,连接服务端失败!

			Debug.Log("客户端被动断开");
		}
	}

	/// <summary>
	/// 读取大端序的int
	/// </summary>
	/// <param name="value"></param>
	public int ReadInt(byte[] intbytes)
	{
		Array.Reverse(intbytes);
		return BitConverter.ToInt32(intbytes, 0);
	}

	public short ReadShort(byte[] intbytes)
	{
		Array.Reverse(intbytes);
		return BitConverter.ToInt16(intbytes, 0);
	}

	private void ReceiveCallBack(byte[] m_receiveBuffer)
	{
		//通知调用端接收完毕
		try {
//				Debug.Log("m_receiveBuffer======" + m_receiveBuffer.Length);
//				for (int i = 0; i < m_receiveBuffer.Length; i++) {
//					if (i % 4 == 0)
//						Debug.Log("12312");
//					Debug.Log("-=-- " + m_receiveBuffer [i]);
//				}
			MemoryStream ms = new MemoryStream(m_receiveBuffer);
			BinaryReader buffers = new BinaryReader(ms, UTF8Encoding.Default);
			readBuffer(buffers);
		} catch (Exception ex) {
			Debug.Log("socket exception:" + ex.Message);
			throw new Exception(ex.Message);
		}
	}

	private void readBuffer(BinaryReader buffers)
	{
		int lens = ReadInt(buffers.ReadBytes(4));
		disConnectCount = 0;
		if (!hasStartTimer && lens == 20) {
			startTimer();
			hasStartTimer = true;
		}

		Debug.Log("lengs ====>>  " + lens + "  " + buffers.BaseStream.Length);
//		for(int i = 0;i<buffers.BaseStream.Length){
//			Debug.Log("-- " + ReadInt(buffers.ReadBytes(4));
//		}

		if (lens > buffers.BaseStream.Length) {
			waitLen = lens;
			isWait = true;
			buffers.BaseStream.Position = 0;
			byte[] dd = new byte[buffers.BaseStream.Length];
			byte[] temp = buffers.ReadBytes((int)buffers.BaseStream.Length);
			Array.Copy(temp, 0, dd, 0, (int)buffers.BaseStream.Length);
			if (sources == null) {
				sources = dd;
			}
			return;
		}
		int headcode = ReadInt(buffers.ReadBytes(4));
		int assistId = ReadInt(buffers.ReadBytes(4));
		int handleCode = ReadInt(buffers.ReadBytes(4));
		int reserveCode = ReadInt(buffers.ReadBytes(4));

//		string message = Encoding.UTF8.GetString(buffers.ReadBytes(lens - 20));
		ClientResponse response = new ClientResponse();
		response.headCode = headcode;
		response.assistId = assistId;
		response.handleCode = handleCode;
		response.reserveCode = reserveCode;
		if (lens > 21) {
			response.bytes = buffers.ReadBytes(lens - 21);
		}
		if (headcode != APIS.HeadRESPONSE) {
			Debug.Log("response msg,  head code: " + response.headCode);
		}
		SocketEventHandle.getInstance().addResponse(response);

		buffers.ReadBytes(1);//read the EOF symbol

		if (buffers.BaseStream.Position < buffers.BaseStream.Length) {
			readBuffer(buffers);
		}
	}

	public void closeSocket()
	{
		DisConnect();
	}

	System.Timers.Timer t;

	private void startTimer()
	{
		if (t == null) {
			t = new System.Timers.Timer(20000);   //实例化Timer类，设置间隔时间为10000毫秒；   
			t.Elapsed += new System.Timers.ElapsedEventHandler(timeout); //到达时间的时候执行事件；   
			t.AutoReset = true;   //设置是执行一次（false）还是一直执行(true)；   
			t.Enabled = true;     //是否执行System.Timers.Timer.Elapsed事件；   
		} else {
			t.Start();
		}
	}

	public void timeout(object source, System.Timers.ElapsedEventArgs e)
	{
		disConnectCount += 1;
		if (disConnectCount >= 15) {
			t.Stop();

			disConnectCount = 0;
			isConnected = false;
			SocketEventHandle.getInstance().noticeDisConect();
			return;
		}
	}
}
