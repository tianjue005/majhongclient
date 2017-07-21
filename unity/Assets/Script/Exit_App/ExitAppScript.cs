using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class ExitAppScript : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{
	
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}


	public void exit()
	{
		SoundCtrl.getInstance().playSoundUI();
		CustomSocket.getInstance().sendMsg(new LoginRequest());

		Application.Quit();
		//多态  调用退出登录接口

	}

	public void cancle()
	{
		SoundCtrl.getInstance().playSoundUI();
		ExitAppScript self = GetComponent<ExitAppScript>();
		self = null;
		Destroy(self);
		Destroy(gameObject);
	}
}
