using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class MessageBoxPanel : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{
	}

	public void exit()
	{
		SoundCtrl.getInstance().playSoundUI();
		gameObject.SetActive(false);
	}
}
