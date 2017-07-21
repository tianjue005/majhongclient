using UnityEngine;
using DG.Tweening;

public class SoundToggleScript : MonoBehaviour {
	public GameObject showFramebtn;
	public GameObject showFrameMask;
	public GameObject soundSetting;
	private bool isShowFrame = false;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void openClick(){
    SoundCtrl.getInstance().playSoundUI();
		GlobalDataScript.soundToggle = true;
	}

	public void closeClick(){
    SoundCtrl.getInstance().playSoundUI();
		GlobalDataScript.soundToggle = false;
	}

  public void settingClick() {
    SoundCtrl.getInstance().playSoundUI();
    if (isShowFrame) hideSettingFrame();
    soundSetting.SetActive(true);
  }

	public void clicksettingbtn(){
    SoundCtrl.getInstance().playSoundUI();
		if (!isShowFrame) {
			showSettingframe ();
		} else {
			hideSettingFrame ();
		}
	}

	private void showSettingframe(){
    showFrameMask.SetActive(true);
    gameObject.transform.DOLocalMove (new Vector3(412, 0), 0.4f);
    isShowFrame = true;
    showFramebtn.SetActive(false);
  }

  private void hideSettingFrame(){
    showFrameMask.SetActive(false);
    gameObject.transform.DOLocalMove (new Vector3(667, 0), 0.4f);
    isShowFrame = false;
    showFramebtn.SetActive(true);
  }
}
