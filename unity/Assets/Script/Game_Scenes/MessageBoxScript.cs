using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using AssemblyCSharp;
using LitJson;

public class MessageBoxScript : MonoBehaviour
{
	public GameObject showFramebtn;
	public GameObject showFrameMask;
	public MyMahjongScript myMaj;
	private bool isShowFrame = false;

	public InputField input;
	public GameObject panelWord;
	public GameObject panelBiaoqing;
	public Image wordImage;
	public Image biaoqingImage;

	public void btnClick(int index)
	{
		SoundCtrl.getInstance().playSoundUI();
		SoundCtrl.getInstance().playMessageBoxSound(index);
		CustomSocket.getInstance().sendMsg(new MessageBoxRequest(1
			, GlobalDataScript.loginResponseData.account.uuid
			, index.ToString())
		);
		if (myMaj != null) {
			myMaj.playerItems [0].showChatMessage(index);
		}
		hidePanel();
	}

	public void sendMessage()
	{
		SoundCtrl.getInstance().playSoundUI();
		if (myMaj != null) {
			if (string.IsNullOrEmpty(input.text) == false) {
				CustomSocket.getInstance().sendMsg(
					new MessageBoxRequest(0
						, GlobalDataScript.loginResponseData.account.uuid
						, input.text)
				);
				hidePanel();
			}
		}
	}

	public void sendBiaoqing(string code)
	{
		SoundCtrl.getInstance().playSoundUI();
		if (myMaj != null) {
			CustomSocket.getInstance().sendMsg(new MessageBoxRequest(2, GlobalDataScript.loginResponseData.account.uuid, code));
			hidePanel();
		}
	}

	public void selectWord()
	{
		wordImage.sprite = Resources.Load("Image/button_yellow_left", typeof(Sprite)) as Sprite;
		biaoqingImage.sprite = Resources.Load("Image/button_green_right", typeof(Sprite)) as Sprite;
		panelBiaoqing.SetActive(false);
		panelWord.SetActive(true);
	}

	public void selectBiaoqing()
	{
		wordImage.sprite = Resources.Load("Image/button_green_left", typeof(Sprite)) as Sprite;
		biaoqingImage.sprite = Resources.Load("Image/button_yellow_right", typeof(Sprite)) as Sprite;
		panelWord.SetActive(false);
		panelBiaoqing.SetActive(true);
	}

	public void showPanel()
	{
		showFrameMask.SetActive(true);
		gameObject.transform.DOLocalMove(new Vector3(267, 0), 0.4f);
		isShowFrame = true;
		showFramebtn.SetActive(false);
		SoundCtrl.getInstance().playSoundUI();
	}

	public void hidePanel()
	{
		showFrameMask.SetActive(false);
		gameObject.transform.DOLocalMove(new Vector3(667, 0), 0.4f);
		isShowFrame = false;
		showFramebtn.SetActive(true);
		SoundCtrl.getInstance().playSoundUI();
	}
}
