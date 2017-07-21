using System;
using System.IO;
using System.Text;
using UnityEngine;
using LitJson;

namespace AssemblyCSharp
{
	public class LoginRequest:ClientRequest
	{
		public LoginRequest(string data)
		{
			headCode = APIS.LOGIN_REQUEST;
			if (data == null) {
				SimpleJSON.JSONObject jsonNode = new SimpleJSON.JSONObject();
				byte[] buffer = new byte[1024];
				string user_json_path = FileIO.LOCAL_RES_PATH + "user.json";
				try {
					FileStream stream = new FileStream(user_json_path, FileMode.Open);
					stream.Read(buffer, 0, (int)stream.Length);
					string user_json = Encoding.UTF8.GetString(buffer);
					jsonNode = SimpleJSON.JSON.Parse(user_json).AsObject;
				} catch (Exception e) {
					Debug.Log("Read user json not found. path(" + user_json_path + "), error(" + e + ")");
					System.Random r = new System.Random();
					string rand_id = r.Next(9999, 99999) + "" + r.Next(9999, 99999);
					jsonNode ["openId"] = rand_id;
					jsonNode ["nickName"] = rand_id;
					jsonNode ["unionid"] = rand_id;
					jsonNode ["sex"] = 1;
					FileStream stream = new FileStream(user_json_path, FileMode.CreateNew);
					byte[] resutl = Encoding.UTF8.GetBytes(jsonNode.ToString());
					stream.Write(resutl, 0, (int)resutl.Length);
					stream.Flush();
					stream.Close();
				}

				jsonNode ["IP"] = GlobalDataScript.getInstance().getIpAddress();

				data = jsonNode.ToString();

				LoginVo loginvo = new LoginVo();
				loginvo.openId = jsonNode ["openId"];
				loginvo.nickName = jsonNode ["nickName"];
				loginvo.unionid = jsonNode ["unionid"];
				loginvo.headIcon = jsonNode ["headimgurl"];
				loginvo.province = jsonNode ["province"];
				loginvo.city = jsonNode ["city"];
				loginvo.sex = jsonNode ["sex"];
				loginvo.IP = jsonNode ["IP"];

				GlobalDataScript.loginVo = loginvo;
				GlobalDataScript.loginResponseData = new AvatarVO();
				GlobalDataScript.loginResponseData.account = new Account();
				GlobalDataScript.loginResponseData.account.city = loginvo.city;
				GlobalDataScript.loginResponseData.account.openid = loginvo.openId;
				GlobalDataScript.loginResponseData.account.nickname = loginvo.nickName;
				GlobalDataScript.loginResponseData.account.headicon = loginvo.headIcon;
				GlobalDataScript.loginResponseData.account.unionid = loginvo.city;
				GlobalDataScript.loginResponseData.account.sex = loginvo.sex;
				GlobalDataScript.loginResponseData.IP = loginvo.IP;
			}
			messageContent = data;

		}

		/**用于重新登录使用**/

		//退出登录
		public LoginRequest()
		{
			headCode = APIS.QUITE_LOGIN;
			if (GlobalDataScript.loginResponseData != null) {
				messageContent = GlobalDataScript.loginResponseData.account.uuid + "";
			}
		}
	}
}

