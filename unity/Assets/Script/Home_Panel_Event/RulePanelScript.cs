using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using AssemblyCSharp;
using System.Collections.Generic;


public class RulePanelScript : MonoBehaviour
{
	
	public void closeDialog()
	{
		SoundCtrl.getInstance().playSoundUI();
		Destroy(this);
		Destroy(gameObject);
	}
}
