
using System;
using System.IO;
using UnityEngine;
using System.Text;

public class ClientRequest
{
	protected int Len = 0;
	public int headCode;
	public int assistId;
	public int handleCode;
	public int reserveCode;

	public string messageContent = "";
	public int totelLenght;
	public byte[] content;

	public ClientRequest()
	{
	}

	public ClientRequest(int headCode)
	{
		this.headCode = headCode;
	}

	public ClientRequest SetContent<T>(T t)
	{
		this.content = Serialize<T>(t);
		Debug.Log("Serialize content: " + content.Length);
		return this;
	}

	public void setData()
	{
	}

	/// <summary>
	/// 写入大端序的int
	/// </summary>
	/// <param name="value"></param>
	public byte[] WriterInt(int value)
	{
		byte[] bs = BitConverter.GetBytes(value);
		Array.Reverse(bs);
		return bs;
	}

	/// <summary>
	/// Writes the short.
	/// </summary>
	/// <returns>The short.</returns>
	/// <param name="value">Value.</param>
	public byte[] WriteShort(short value)
	{
		byte[] bs = BitConverter.GetBytes(value);
		Array.Reverse(bs);
		return bs;
	}

	public byte[] WriterString(string value)
	{
		byte[] result = Encoding.UTF8.GetBytes(value);
		return result;
	}

	private static string EOF_MESSAGE = "\nE\nO\nF\n";

	public byte[] ToBytes()
	{
		byte[] _bytes; //自定义字节数组，用以装载消息协议
		using (MemoryStream memoryStream = new MemoryStream()) { //创建内存流
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream, UTF8Encoding.Default); //以二进制写入器往这个流里写内容

			bool isEmpty = content == null || content.Length == 0;
			Len = 27 + (isEmpty ? 0 : content.Length);

			binaryWriter.Write(WriterInt(Len));
			binaryWriter.Write(WriterInt(headCode)); 
			binaryWriter.Write(WriterInt(assistId)); 
			binaryWriter.Write(WriterInt(handleCode)); 
			binaryWriter.Write(WriterInt(reserveCode));
			if (!isEmpty) {
				binaryWriter.Write(content); //write the messge content
			}
			binaryWriter.Write(WriterString(EOF_MESSAGE));//write the EOF symbol

			_bytes = memoryStream.ToArray(); 
			binaryWriter.Close();
		}
		totelLenght = _bytes.Length;
		return _bytes; //返回填充好消息协议对象的自定义字节数组
	}

	public static byte[] Serialize<T>(T t)
	{
		using (MemoryStream ms = new MemoryStream()) {
			ProtoBuf.Serializer.Serialize<T>(ms, t);
			return ms.ToArray();
		}
	}

	/// <summary>
	/// 反序列化
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="content"></param>
	/// <returns></returns>
	public static T DeSerialize<T>(byte[] content)
	{
		using (MemoryStream ms = new MemoryStream(content)) {
			T t = ProtoBuf.Serializer.Deserialize<T>(ms);
			return t;
		}
	}
}

