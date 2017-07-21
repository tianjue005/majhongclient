using UnityEngine;
using System.Collections;

/**
 * sound control class
 * 
 * author :kevin
 * 
 * */
public class SoundCtrl
{

	private Hashtable soudHash = new Hashtable();

	private static SoundCtrl _instance;

	private static AudioSource audioS;
	private static AudioSource soundS;
	private static AudioSource soundBMG;
	private static AudioSource soundGame;
	private static AudioSource soundL;
	private static AudioSource soundB;
	private static AudioSource soundT;
	private static AudioSource soundR;

	public static AudioSource playAudio;

	private static int language_ = 0;
	// 0 nanjing, 1 putong

	public static int language {
		get { return language_; }
		set { language_ = value; }
	}

	public static SoundCtrl getInstance()
	{
		if (_instance == null) {
			_instance = new SoundCtrl();
			audioS = GameObject.Find("MyAudio").GetComponent<AudioSource>();
			soundS = GameObject.Find("Sound").GetComponent<AudioSource>();
			soundBMG = GameObject.Find("SoundBMG").GetComponent<AudioSource>();
			soundGame = GameObject.Find("SoundGame").GetComponent<AudioSource>();
			soundL = GameObject.Find("SoundL").GetComponent<AudioSource>();
			soundB = GameObject.Find("SoundB").GetComponent<AudioSource>();
			soundT = GameObject.Find("SoundT").GetComponent<AudioSource>();
			soundR = GameObject.Find("SoundR").GetComponent<AudioSource>();
			playAudio = GameObject.Find("GamePlayAudio").GetComponent<AudioSource>();
		}

		return _instance;
	}

	public float soundValue()
	{
		return soundS.volume;
	}

	public float BMGValue()
	{
		return soundBMG.volume;
	}

	public void setSoundValue(float value)
	{
		audioS.volume = value;
		soundS.volume = value;
		soundL.volume = value;
		soundB.volume = value;
		soundT.volume = value;
		soundR.volume = value;
	}

	public void setBMGValue(float value)
	{
		soundBMG.volume = value;
	}

	public AudioSource selectSound(string dir)
	{
		switch (dir) {
			case DirectionEnum.Left:
				return soundL;
			case DirectionEnum.Bottom:
				return soundB;
			case DirectionEnum.Top:
				return soundT;
			case DirectionEnum.Right:
				return soundR;
			default:
				return soundGame;
		}
	}

	public void playSound(int cardPoint, int sex, string dir)
	{
		if (GlobalDataScript.soundToggle) {
			string path = "Sounds/";
			if (language == 1)
				path += "normal/";
			else
				path += "normal/";
			if (sex == 1) {
				path += "boy/" + (cardPoint + 1);
			} else {
				path += "girl/" + (cardPoint + 1);
			}
			AudioClip temp = (AudioClip)soudHash [path];
			if (temp == null) {
				temp = GameObject.Instantiate(Resources.Load(path)) as AudioClip;
				soudHash.Add(path, temp);
			}
			var sound = selectSound(dir);
			sound.clip = temp;
			sound.loop = false;
			sound.Play();
		}
	}

	public void playMessageBoxSound(int codeIndex)
	{
		if (GlobalDataScript.soundToggle) {
			string path = "Sounds/other/" + codeIndex;
			AudioClip temp = (AudioClip)soudHash [path];
			if (temp == null) {
				temp = GameObject.Instantiate(Resources.Load(path)) as AudioClip;
				soudHash.Add(path, temp);
			}
			audioS.clip = temp;
			audioS.loop = false;
			audioS.Play();
		}
	}

	public void playBGM()
	{
		string path = "Sounds/mjBGM";
		AudioClip temp = (AudioClip)soudHash [path];
		if (temp == null) {
			temp = GameObject.Instantiate(Resources.Load(path)) as AudioClip;
			soudHash.Add(path, temp);
		}
		soundBMG.clip = temp;
		soundBMG.loop = true;
		soundBMG.Play();
		if (GlobalDataScript.soundToggle) {
			soundBMG.mute = false;
		} else {
			soundBMG.mute = true;
		}
	}

	public void playGameBGM()
	{
		int rand = Random.Range(0, 2);
		string path = "Sounds/gameBGM";
		if (rand == 1)
			path = "Sounds/gameBGM2";
		AudioClip temp = (AudioClip)soudHash [path];
		if (temp == null) {
			temp = GameObject.Instantiate(Resources.Load(path)) as AudioClip;
			soudHash.Add(path, temp);
		}
		soundBMG.clip = temp;
		soundBMG.loop = true;
		soundBMG.Play();
		if (GlobalDataScript.soundToggle) {
			soundBMG.mute = false;
		} else {
			soundBMG.mute = true;
		}
	}


	public void stopBGM()
	{
		soundBMG.loop = false;
		soundBMG.Stop();
	}

	public void playSoundByAction(string str, int sex, string dir)
	{
		string path = "Sounds/";
		if (language == 1)
			path += "normal/";
		else
			path += "normal/";
		if (sex == 1) {
			path += "boy/" + str;
		} else {
			path += "girl/" + str;
		}
		AudioClip temp = (AudioClip)soudHash [path];
		if (temp == null) {
			temp = GameObject.Instantiate(Resources.Load(path)) as AudioClip;
			soudHash.Add(path, temp);
		}
		var sound = selectSound(dir);
		sound.clip = temp;
		sound.loop = false;
		sound.Play();
	}

	public void playSoundGame(string str)
	{
		string path = "Sounds/other/" + str;
		AudioClip temp = (AudioClip)soudHash [path];
		if (temp == null) {
			temp = GameObject.Instantiate(Resources.Load(path)) as AudioClip;
			soudHash.Add(path, temp);
		}
		soundGame.clip = temp;
		soundGame.loop = false;
		soundGame.Play();
	}

	public void playSoundOther(string str)
	{
		string path = "Sounds/other/" + str;
		AudioClip temp = (AudioClip)soudHash [path];
		if (temp == null) {
			temp = GameObject.Instantiate(Resources.Load(path)) as AudioClip;
			soudHash.Add(path, temp);
		}
		soundS.clip = temp;
		soundS.loop = false;
		soundS.Play();
	}

	public void playSoundUI(bool main = false)
	{
		string path = "";
		if (main)
			path = "Sounds/other/click01";
		else
			path = "Sounds/other/click02";
		AudioClip temp = (AudioClip)soudHash [path];
		if (temp == null) {
			temp = GameObject.Instantiate(Resources.Load(path)) as AudioClip;
			soudHash.Add(path, temp);
		}
		soundS.clip = temp;
		soundS.loop = false;
		soundS.Play();
	}

}
