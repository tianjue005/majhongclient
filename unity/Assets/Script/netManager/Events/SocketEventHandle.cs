using System;
using UnityEngine;
using System.Collections.Generic;
using LitJson;

/// <summary>
/// 消息分发类
/// </summary>
public class SocketEventHandle : MonoBehaviour
{
	private static SocketEventHandle _instance;

	public delegate void ServerCallBackEvent(ClientResponse response);

	public delegate void ServerDisconnectCallBackEvent();

	public ServerCallBackEvent LoginWeChatCallBack;

	public ServerCallBackEvent LoginCallBack;
	//登录回调
	public ServerCallBackEvent ChooseGameCallBack;

	public ServerCallBackEvent CreateRoomCallBack;
	//创建房间回调

	//	public ServerCallBackEvent JoinRoomCallBack;
	//	//加入房间回调

	public ServerCallBackEvent StartGameNotice;
	//
	public ServerCallBackEvent pickCardCallBack;
	//自己摸牌

	public ServerCallBackEvent otherPickCardCallBack;
	//别人摸牌通知

	public ServerCallBackEvent putOutCardCallBack;
	//出牌通知

	public ServerCallBackEvent otherUserJointRoomCallBack;

	public ServerCallBackEvent PengCardCallBack;
	//碰牌回调

	public ServerCallBackEvent GangCardCallBack;
	//杠牌回调

	public ServerCallBackEvent HupaiCallBack;
	//胡牌回调

	public ServerCallBackEvent FinalGameOverCallBack;
	//全局结束回调

	public ServerCallBackEvent gangCardNotice;
	//

	public ServerCallBackEvent btnActionShow;
	//碰杠行为按钮显示

	public ServerCallBackEvent outRoomCallback;
	//退出房间回调

	public ServerCallBackEvent dissoliveRoomResponse;

	public ServerCallBackEvent gameReadyNotice;
	//准备游戏通知返回

	public ServerCallBackEvent micInputNotice;
	public ServerCallBackEvent messageBoxNotice;

	public ServerCallBackEvent serviceErrorNotice;
	//错误信息返回

	public ServerCallBackEvent backLoginNotice;
	//玩家断线重连

	public ServerCallBackEvent RoomBackResponse;
	//掉线后返回房间

	public ServerCallBackEvent cardChangeNotice;
	//房卡数据变化

	public ServerCallBackEvent offlineNotice;
	//离线通知

	public ServerCallBackEvent onlineNotice;
	//上线通知
	//public ServerCallBackEvent rewardRequestCallBack;//投资请求返回

	public ServerCallBackEvent giftResponse;
	//奖品回调

	public ServerCallBackEvent returnGameResponse;

	public ServerCallBackEvent gameFollowBanderNotice;
	//跟庄

	public ServerCallBackEvent gameBroadcastNotice;
	//游戏公告

	public ServerDisconnectCallBackEvent disConnetNotice;
	//断线
	public ServerCallBackEvent contactInfoResponse;
	//联系方式回调
	public ServerCallBackEvent zhanjiResponse;
	//房间战绩返回数据
	public ServerCallBackEvent zhanjiDetailResponse;
	//房间战绩返回数据

	public ServerCallBackEvent gameBackPlayResponse;
	//回放返回数据
	public ServerCallBackEvent otherTeleLogin;
	//其他设备登陆账户

	public ServerCallBackEvent TestMesgCallBack;
	//测试消息的回调

	public ServerCallBackEvent buhuaResponse;
	public ServerCallBackEvent huaGangResponse;
	public ServerCallBackEvent timeWaitResponse;
	public ServerCallBackEvent scoreResponse;
	public ServerCallBackEvent buhuaBeginResponse;

	//private List<ClientResponse> callBackResponseList;


	private List<ClientResponse> callBackResponseList;

	private bool isDisconnet = false;
	public bool isWaiting = false;
	private float timeWait = 0;


	public SocketEventHandle()
	{
		callBackResponseList = new List<ClientResponse>();
	}

	void Awake()
	{
		_instance = this;
	}

	void Start()
	{
		isWaiting = false;
		SocketEventHandle.getInstance();
		SocketEventHandle.getInstance().timeWaitResponse = timeWaitCallback;
	}

	public static bool checkInstance()
	{
		if (_instance != null)
			return true;
		return false;
	}

	public static SocketEventHandle getInstance()
	{
		if (_instance == null) {
			_instance = (SocketEventHandle)UnityEngine.Object.FindObjectOfType(typeof(SocketEventHandle));
		}
		return _instance;
	}

	void FixedUpdate()
	{
		if (timeWait > 0) {
			timeWait -= Time.fixedDeltaTime;
			if (timeWait <= 0) {
				timeWait = 0;
				timeWaitFinish();
			}
		}

		while (callBackResponseList.Count > 0) {
			if (isWaiting)
				break;
			ClientResponse response = callBackResponseList [0];
			callBackResponseList.RemoveAt(0);
			dispatchHandle(response);
		}

		if (isDisconnet) {
			isDisconnet = false;
			disConnetNotice();
		}
	}

	private void dispatchHandle(ClientResponse response)
	{
		Debug.Log("receive apiCode=" + response.headCode + " message: " + response.message);
		switch (response.headCode) {
			case APIS.CLOSE_RESPONSE:
				TipsManagerScript.getInstance().setTips("服务器关闭了");
				CustomSocket.getInstance().closeSocket();
				break;
			case ApiCode.LoginSessionRequest:
//			case APIS.LOGIN_RESPONSE:
				if (LoginCallBack != null)
					LoginCallBack(response);
				break;

			case ApiCode.LoginWeChatRequest:
				if (LoginWeChatCallBack != null)
					LoginWeChatCallBack(response);
				break;

			case ApiCode.ChooseGameRequest:
				if (ChooseGameCallBack != null)
					ChooseGameCallBack(response);
				break;

			case ApiCode.CreateRoomRequest:
				if (CreateRoomCallBack != null) {
					CreateRoomCallBack(response);
				}
				break;
//			case ApiCode.JoinRoomRequest:
//				if (JoinRoomCallBack != null) {
//					JoinRoomCallBack(response);
//				}
//				break;
			case APIS.STARTGAME_RESPONSE_NOTICE:
				if (StartGameNotice != null) {
					StartGameNotice(response);
				}
				break;
			case APIS.PICKCARD_RESPONSE:
				if (pickCardCallBack != null) {
					pickCardCallBack(response);
				}
				break;
			case APIS.OTHER_PICKCARD_RESPONSE_NOTICE:
				if (otherPickCardCallBack != null) {
					otherPickCardCallBack(response);
				}
				break;
			case APIS.CHUPAI_RESPONSE:
				if (putOutCardCallBack != null) {
					putOutCardCallBack(response);
				}
				break;
			case APIS.JOIN_ROOM_NOICE:
				if (otherUserJointRoomCallBack != null) {
					otherUserJointRoomCallBack(response);
				}
				break;
			case APIS.PENGPAI_RESPONSE:
				if (PengCardCallBack != null) {
					PengCardCallBack(response);
				}
				break;
			case APIS.GANGPAI_RESPONSE:
				if (GangCardCallBack != null) {
					GangCardCallBack(response);
				}
				break;
			case APIS.OTHER_GANGPAI_NOICE:
				if (gangCardNotice != null) {
					gangCardNotice(response);
				}
				break;
			case APIS.RETURN_INFO_RESPONSE:
				if (btnActionShow != null) {
					btnActionShow(response);
				}
				break;
			case APIS.HUPAI_RESPONSE:
				if (HupaiCallBack != null) {
					HupaiCallBack(response);
				}
				break;
			case APIS.HUPAIALL_RESPONSE:
				if (FinalGameOverCallBack != null) {
					FinalGameOverCallBack(response);
				}
				break;

			case APIS.OUT_ROOM_RESPONSE:
				if (outRoomCallback != null) {
					outRoomCallback(response);
				}
				break;
			case APIS.HeadRESPONSE:
				break;
			case APIS.DISSOLIVE_ROOM_RESPONSE:
				if (dissoliveRoomResponse != null) {
					dissoliveRoomResponse(response);
				}
				break;
			case APIS.PrepareGame_MSG_RESPONSE:
				if (gameReadyNotice != null) {
					gameReadyNotice(response);
				}
				break;
			case APIS.MicInput_Response:
				if (micInputNotice != null) {
					micInputNotice(response);
				}
				break;
			case APIS.MessageBox_Notice:
				if (messageBoxNotice != null) {
					messageBoxNotice(response);
				}
				break;
			case APIS.ERROR_RESPONSE:
				if (serviceErrorNotice != null) {
					serviceErrorNotice(response);
				}
				break;
			case APIS.BACK_LOGIN_RESPONSE:
				if (RoomBackResponse != null) {
					RoomBackResponse(response);
				}

				break;
			case APIS.CARD_CHANGE:
				if (cardChangeNotice != null) {
					cardChangeNotice(response);
				}
				break;
			case APIS.OFFLINE_NOTICE:
				if (offlineNotice != null) {
					offlineNotice(response);
				}
				break;
			case APIS.RETURN_ONLINE_RESPONSE:

				if (returnGameResponse != null) {
					returnGameResponse(response);
				}
				break;
			case APIS.PRIZE_RESPONSE:
				if (giftResponse != null) {
					giftResponse(response);
				}
				break;

			case APIS.Game_FollowBander_Notice:
				if (gameFollowBanderNotice != null) {
					gameFollowBanderNotice(response);
				}
				break;


			case APIS.ONLINE_NOTICE:
				if (onlineNotice != null) {
					onlineNotice(response);
				}
				break;

			case APIS.GAME_BROADCAST:
				if (gameBroadcastNotice != null) {
					gameBroadcastNotice(response);
				}
				break;

			case APIS.CONTACT_INFO_RESPONSE:
				if (contactInfoResponse != null) {
					contactInfoResponse(response);
				}
				break;
			case APIS.ZHANJI_REPORTER_REPONSE:
				if (zhanjiResponse != null) {
					zhanjiResponse(response);
				}
				break;
			case APIS.ZHANJI_DETAIL_REPORTER_REPONSE:
				if (zhanjiDetailResponse != null) {
					zhanjiDetailResponse(response);
				}
				break;
			case APIS.GAME_BACK_PLAY_RESPONSE:
				if (gameBackPlayResponse != null) {
					gameBackPlayResponse(response);
				}
				break;
			case APIS.TIP_MESSAGE:
				TipsManagerScript.getInstance().setTips(response.message);
				break;
			case APIS.OTHER_TELE_LOGIN:
				if (otherTeleLogin != null) {
					otherTeleLogin(response);
				}
				break;

			case APIS.BUHUA_RESPONSE:
				if (buhuaResponse != null) {
					buhuaResponse(response);
				}
				break;

			case APIS.BUHUA_BEGIN_RESPONSE:
				if (buhuaBeginResponse != null) {
					buhuaBeginResponse(response);
				}
				break;

			case APIS.HUAGANG_RESPONSE:
				if (huaGangResponse != null) {
					huaGangResponse(response);
				}
				break;

			case APIS.SCORE_RESPONSE:
				if (scoreResponse != null) {
					scoreResponse(response);
				}
				break;

			case APIS.TIME_WAIT_RESPONSE:
				if (timeWaitResponse != null) {
					timeWaitResponse(response);
				}
				break;
		}
	}

	public void addResponse(ClientResponse response)
	{
		callBackResponseList.Add(response);
	}


	public void noticeDisConect()
	{
		isDisconnet = true;
	}

	private void timeWaitCallback(ClientResponse response)
	{
		JsonData data = JsonMapper.ToObject(response.message);
		int delta = (int)data ["time"];
		timeWait += delta;
		isWaiting = true;
	}

	public void timeWaitFinish()
	{
		isWaiting = false;
	}
}

