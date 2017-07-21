using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using AssemblyCSharp;

public class PlayerItemScript : MonoBehaviour
{
	public GameObject show;
	public Image headerIcon;
	public Image bankerImg;
	public Text nameText;
	public GameObject readyImg;
	public Text scoreText;
	public string dir;
	public GameObject chatAction;
	//public Text offline;//离线字样
	public GameObject offlineImage;
	//离线图片
	public Text chatMessage;
	public GameObject chatPaoPao;
	public GameObject HuFlag;
	public Image biaoqingImage;

	private AvatarVO avatarvo;
	private float showTime = 0;
	private float showChatTime;

	// Update is called once per frame
	void Update()
	{
		if (showTime > 0) {
			showTime -= Time.deltaTime;
			if (showTime <= 0) {
				chatPaoPao.SetActive(false);
			}
		}

		if (showChatTime > 0) {
			showChatTime -= Time.deltaTime;
			if (showChatTime <= 0) {
				chatAction.SetActive(false);
			}
		}
	}

	public void setAvatarVo(AvatarVO value)
	{
		if (value != null) {
			show.SetActive(true);
			avatarvo = value;
			readyImg.SetActive(avatarvo.isReady);
			bankerImg.enabled = avatarvo.main;
			nameText.text = avatarvo.account.nickname;
			scoreText.text = avatarvo.scores + "";
			offlineImage.transform.gameObject.SetActive(!avatarvo.isOnLine);
			Sprite tempSp;
			if (avatarvo.account != null && string.IsNullOrEmpty(avatarvo.account.headicon) == false) {
				if (GlobalDataScript.imageMap.TryGetValue(avatarvo.account.headicon, out tempSp)) {
					headerIcon.sprite = tempSp;
				} else {
					StartCoroutine(LoadImg());
				}
			}
		} else {
			show.SetActive(false);
			nameText.text = "";
			readyImg.SetActive(false);
			bankerImg.enabled = false;
			scoreText.text = "";

//			SpriteRenderer spr = gameObject.GetComponent<SpriteRenderer> ();
//			Texture2D texture =(Texture2D)Resources.Load ("Image/gift");
//			Sprite sp = Sprite.Create (texture, spr.sprite.textureRect, new Vector2 (0.5f, 0.5f));
			headerIcon.sprite = Resources.Load("Image/default_icon", typeof(Sprite)) as Sprite;
		}
	}

	/// <summary>
	/// 加载头像
	/// </summary>
	/// <returns>The image.</returns>
	private IEnumerator LoadImg()
	{ 
		//开始下载图片
		WWW www = new WWW(avatarvo.account.headicon);
		yield return www;
		//下载完成，保存图片到路径filePath
		if (www != null && string.IsNullOrEmpty(www.error)) {
			Texture2D texture2D = www.texture;
			byte[] bytes = texture2D.EncodeToPNG();

			//将图片赋给场景上的Sprite
			Sprite tempSp = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0, 0));
			headerIcon.sprite = tempSp;
			GlobalDataScript.imageMap.Add(avatarvo.account.headicon, tempSp);
		} else {
			Debug.Log("没有加载到图片");
		}
	}


	public void setbankImgEnable(bool flag)
	{
		bankerImg.enabled = flag;
	}

	public void showChatAction(float time)
	{
		showChatTime = time;
		chatAction.SetActive(true);
	}

	public void hideChatAction()
	{
		showChatTime = 0;
		chatAction.SetActive(false);
	}

	public int getUuid()
	{
		int result = -1;
		if (avatarvo != null) {
			result = avatarvo.account.uuid;
		}
		return result;
	}

	public void clean()
	{
		Destroy(headerIcon.gameObject);
		Destroy(bankerImg.gameObject);
		Destroy(nameText.gameObject);
		Destroy(readyImg.gameObject);
	}

	/**设置游戏玩家离线**/
	public void setPlayerOffline()
	{
		offlineImage.transform.gameObject.SetActive(true);
	}

	/**设置游戏玩家上线**/
	public void setPlayerOnline()
	{
		offlineImage.transform.gameObject.SetActive(false);
	}

	private float getMessageSize(string message)
	{
		if (string.IsNullOrEmpty(message) == true)
			return 0;
		Font font = Resources.Load<Font>("font/fzcyt_0");
		int fontsize = 24;
		font.RequestCharactersInTexture(message, fontsize, FontStyle.Normal);
		CharacterInfo characterInfo;
		float width = 0f;
		for (int i = 0; i < message.Length; i++) {
			font.GetCharacterInfo(message [i], out characterInfo, fontsize);
			width += characterInfo.advance;
		}

		return width;
	}

	public void showChatMessage(int index)
	{
		chatPaoPao.SetActive(true);
		biaoqingImage.gameObject.SetActive(false);

		showTime = 4;
		index = index - 1001;
		chatMessage.text = GlobalDataScript.messageBoxContents [index];
		int width = (int)getMessageSize(chatMessage.text);
		int height = 90 + width / 315 * 30;
		if (width < 315)
			width += 20;
		else
			width = 335;
		;
		chatPaoPao.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
	}

	public void showBiaoqing(string message)
	{
		string path = "Image/biaoqing/" + message;
		Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
		if (sprite != null) {
			chatPaoPao.SetActive(true);
			biaoqingImage.gameObject.SetActive(true);
			chatMessage.text = "";
			biaoqingImage.sprite = sprite;
			showTime = 4;
			chatPaoPao.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 120);
		}
	}

	public void showMessage(string message)
	{
		chatPaoPao.SetActive(true);
		biaoqingImage.gameObject.SetActive(false);

		showTime = 4;
		chatMessage.text = message;
		int width = (int)getMessageSize(message);
		int height = 90 + width / 315 * 30;
		if (width < 315)
			width += 20;
		else
			width = 335;
		chatPaoPao.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
	}

	public void displayAvatorIp()
	{
		//userInfoPanel.SetActive (true);
		GameObject obj = PrefabManage.loadPerfab("Prefab/userInfo");
		obj.GetComponent<ShowUserInfoScript>().setUIData(avatarvo);
	}

	public void setHuFlagDisplay()
	{
		HuFlag.SetActive(true);
	}

	public void setHuFlagHidde()
	{
		HuFlag.SetActive(false);
	}

}
