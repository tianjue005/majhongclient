using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApiCode
{

	//	100 profile
	//	101 app_login__weixin
	//	102 logout
	//	103 staticc__FlowR
	//	104 staticc__ChoiceR
	//	105 staticc__room_opreation

	//心跳包
	public const int HeartRequest = 0;

	public const int LoginSessionRequest = 100;
	//微信登录
	public const int LoginWeChatRequest = 101;
	//退出登录
	public const int LogoutRequest = 102;

	public const int StaticFlowRequest = 103;
	public const int ChooseGameRequest = 104;
	public const int CreateRoomRequest = 105;
	//	public const int JoinRoomRequest = 106;

	public const int CODE_ZERO = 999999;

}
