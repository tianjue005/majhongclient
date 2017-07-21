using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;
using System.IO;
using System.Text;

public class TestMsgRequest : ClientRequest
{
	public TestMsgRequest(string sendMsg)
	{
		headCode = APIS.TESTSENDMSG;
		if (sendMsg == null) {
			JSONObject jsonnode = new JSONObject();
			byte[] buffer = new byte[1024];
			string user_json_path = FileIO.LOCAL_RES_PATH + "wlcaption.json";
			try {
				FileStream stream = new FileStream(user_json_path, FileMode.Open);
				stream.Read(buffer, 0, (int)stream.Length);
				string user_json = Encoding.UTF8.GetString(buffer);
				jsonnode = JSON.Parse(user_json).AsObject;
			} catch (Exception e) {
				Debug.Log("Read user json not found. path:" + user_json_path + "error" + e);
			}

			sendMsg = jsonnode.ToString();
			TestMsgVo msgVo = new TestMsgVo();
			msgVo.version = jsonnode ["version"];
			msgVo.name = jsonnode ["name"];
			msgVo.age = jsonnode ["age"];
			msgVo.address = jsonnode ["address"];
			msgVo.phone = jsonnode ["phone"];
			Debug.Log("sendMsg--" + sendMsg);
		}
		messageContent = sendMsg;
	}

}

