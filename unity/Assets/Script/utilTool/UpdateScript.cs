using System.Collections;
using UnityEngine;
using AssemblyCSharp;

public class UpdateScript
{
	private ServiceVersionVo serviceVersionVo = new ServiceVersionVo();

	private string currentVersion = Application.version;
	//当前软件版本号

	private string serverVersion;
	//服务器上软件版本号

	private string downloadPath;
	//应用下载链接

	/**
	 * 检测升级
	 */
	public IEnumerator  updateCheck()
	{
		WWW www = new WWW(APIS.UPDATE_INFO_JSON_URL);
		yield return www;
		byte[] buffer = www.bytes;
		string result = System.Text.Encoding.UTF8.GetString(buffer);
		Debug.Log("version = " + result);

		var resultJson = SimpleJSON.JSON.Parse(result);
		var versions = resultJson ["versions"];
		if (versions != null && versions.IsArray) {
			for (int i = 0; i < versions.Count; i++) {
				Version v = new Version();
				v.title = versions [i] ["title"];
				v.note = versions [i] ["note"];
				v.url = versions [i] ["url"];
				v.version = versions [i] ["version"];
				string platform = versions [i] ["platform"];
				switch (platform) {
					case "android":
						serviceVersionVo.android = v;
						break;
					case "ios":
						serviceVersionVo.ios = v;
						serviceVersionVo.ios.url += "l=zh&mt=8";
						break;
				}
			}
		}
		compareVersion();
	}

	//对比版本虚
	public void compareVersion()
	{
		int currentVerCode;//当前版本号数字
		int serverVerCode;//服务器上版本号数字
		currentVersion = currentVersion.Replace(".", "");
		currentVerCode = int.Parse(currentVersion);
		Version versionTemp = new Version();//版本信息
		if (Application.platform == RuntimePlatform.Android) {
			versionTemp = serviceVersionVo.android;
		} else if (Application.platform == RuntimePlatform.IPhonePlayer) {
			versionTemp = serviceVersionVo.ios;
		}

		if (versionTemp != null && versionTemp.version != null) {
			serverVersion = versionTemp.version;
			serverVersion = serverVersion.Replace(".", "");
			serverVerCode = int.Parse(serverVersion);
			if (serverVerCode > currentVerCode) {//服务器上有新版本 	
				string note = versionTemp.note;
				downloadPath = versionTemp.url;

				TipsManagerScript.getInstance().loadDialog(versionTemp.title, versionTemp.note, onSureClick, onCancel);
			}
		}
	}

	public void onSureClick()
	{
		if (downloadPath != null) {
			Application.OpenURL(downloadPath);
		}
	}

	public void onCancel()
	{
	}

}
