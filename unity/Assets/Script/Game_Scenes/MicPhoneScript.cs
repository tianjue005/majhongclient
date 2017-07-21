using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using AssemblyCSharp;

public class MicPhoneScript : MonoBehaviour
{
	public float WholeTime = 10;
	public GameObject InputGameObject;
	private Boolean btnDown = false;
	public Slider circle;
	public MyMahjongScript myScript;

	void Start()
	{
		SocketEventHandle.getInstance().micInputNotice += micInputNotice;
	}

	void OnDestroy()
	{
		if (SocketEventHandle.checkInstance()) {
			SocketEventHandle.getInstance().micInputNotice -= micInputNotice;
		}
	}

	// Update is called once per frame
	void Update()
	{
	}

	void FixedUpdate()
	{
		if (btnDown) {
			WholeTime -= Time.deltaTime;
			circle.value = WholeTime;
			if (WholeTime <= 0) {
				OnPointerUp();
			}
		}
	}

	public void OnPointerDown()
	{
		if (myScript.avatarList != null && myScript.avatarList.Count > 1) {
			btnDown = true;
			circle.gameObject.SetActive(true);
			InputGameObject.SetActive(true);
			WholeTime = 10;
			circle.value = WholeTime;
			MicroPhoneInput.getInstance().StartRecord(getUserList());
		} else {
			TipsManagerScript.getInstance().setTips("房间里只有你一个人，不能发送语音");
		}
	}

	public void OnPointerUp()
	{
		if (btnDown) {
			btnDown = false;
			circle.gameObject.SetActive(false);
			InputGameObject.SetActive(false);
			if (myScript.avatarList != null && myScript.avatarList.Count > 1) {
				float time = MicroPhoneInput.getInstance().StopRecord();
				myScript.soundActionPlay(time);
			} else {

			}
		}
	}

	private List<int> getUserList()
	{
		List<int> userList = new List<int>();
		for (int i = 0; i < myScript.avatarList.Count; i++) {
			if (myScript.avatarList [i].account.uuid != GlobalDataScript.loginResponseData.account.uuid) {
				userList.Add(myScript.avatarList [i].account.uuid);
			}
		}
		return userList;
	}

	public void micInputNotice(ClientResponse response)
	{
		if (GlobalDataScript.soundToggle) {
			float time = MicroPhoneInput.getInstance().PlayClipData(response.bytes);
			int sendUUid = int.Parse(response.message);
			if (sendUUid > 0) {
				for (int i = 0; i < myScript.playerItems.Count; i++) {
					if (myScript.playerItems [i].getUuid() != -1) {
						if (sendUUid == myScript.playerItems [i].getUuid()) {
							myScript.playerItems [i].showChatAction(time);
						}
					}
				}
			}
		}
	}
}
