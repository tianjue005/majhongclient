using System;

/***
 * 登录请求的微信数据定义
 * 
 */
[Serializable]
public class LoginVo
{
	public String openId;

	public String nickName;

	public String headIcon;

	public String unionid;

	public String province;

	public String city;

	public int sex;

	public string IP;

	public LoginVo()
	{
	}

}

