using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SoundSettingScript : MonoBehaviour {
	public Slider soundSlider;
	public Slider audioSlider;

  public List<Toggle> languageToggleList;
  public List<Text> languageTextList;

  private Color32 selectTextColor = new Color32(239, 76, 74, 255);
  private Color32 noselectTextColor = new Color32(165, 92, 50, 255);

  // Use this for initialization
  void Start () {
    soundSlider.value = SoundCtrl.getInstance().soundValue();
    audioSlider.value = SoundCtrl.getInstance().BMGValue();

    for (int i = 0; i < languageToggleList.Count; i++) {
      languageToggleList[i].onValueChanged.AddListener(onValueChanged);
      languageToggleList[i].isOn = (i == 0);
    }
  }

  private void onValueChanged(bool value) {
    for (int i = 0; i < languageToggleList.Count; i++) {
      if (languageToggleList[i].isOn) {
        languageTextList[i].color = selectTextColor;
        SoundCtrl.language = i;
      } else {
        languageTextList[i].color = noselectTextColor;
      }
    }
  }

    // Update is called once per frame
    void Update () {
	}

  public void soundValueChanged() {
    SoundCtrl.getInstance().setSoundValue(soundSlider.value);
  }

  public void audioValueChanged() {
    SoundCtrl.getInstance().setBMGValue(audioSlider.value);
  }

  public void closeClick() {
    gameObject.SetActive(false);
  }
}
