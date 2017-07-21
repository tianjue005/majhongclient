using UnityEngine;
using UnityEditor;
using System.IO;
using System;

[ExecuteInEditMode]
public class EdWatchForSaveAssets : SaveAssetsProcessor
{
//	static string[] OnWillSaveAssets (string[] paths)
//	{
//		Debug.Log ("OnWillSaveAssets");
//		foreach (string path in paths) {
//			Debug.Log (path);
//			string ext = Path.GetExtension (path);
//
//			if ((String.Compare (ext, ".unity", StringComparison.OrdinalIgnoreCase) == 0)) {
//				BroadcastAll ("OnSave", null);
//			}
//		}
//		return paths;
//	}

	public static void BroadcastAll (string fun, System.Object msg)
	{
//		GameObject[] gos = (GameObject[])GameObject.FindObjectsOfType (typeof(GameObject));
//		foreach (GameObject go in gos) {
//			if (go.transform.parent == null) {
//				go.gameObject.BroadcastMessage (fun, msg, SendMessageOptions.DontRequireReceiver);
//			}
//		}
	}
}
