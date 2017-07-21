using System;
using UnityEngine;

/**
 * 服务器返回错误
 */
public class ServiceErrorListener
{
	public ServiceErrorListener()
	{
		SocketEventHandle.getInstance().serviceErrorNotice += serviceErrorNotice;
	}

	public void serviceErrorNotice(ClientResponse response)
	{
		TipsManagerScript.getInstance().setTips(response.message);
	}
}

