using UnityEditor;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

//build and copy helper
class PerformBuild
{
	//you need to change this var to your android folder name
	//here it means ../android
	static string androidFolderName = "proj.android";

	[MenuItem ("Tools/Build Android &2", false, 1)]
	static void CommandLineBuildAndroid ()
	{
		Debug.Log ("Command line build android version\n------------------\n------------------");

		string[] scenes = GetBuildScenes ();
		string path = GetBuildPath ("build/android") + "/android.apk";

		if (scenes == null || scenes.Length == 0 || path == null)
			return;

		//EditorUtility.DisplayDialog("Objects exported", "Exported objects", "");
		Debug.Log (string.Format ("Path: \"{0}\"", path));

		DeleteDirectory ("../" + androidFolderName + "/assets/bin");
		DeleteDirectory ("../" + androidFolderName + "/assets/libs/armeabi-v7a");
		DeleteDirectory ("../" + androidFolderName + "/assets/libs/x86");

		for (int i = 0; i < scenes.Length; ++i) {
			Debug.Log (string.Format ("Scene[{0}]: \"{1}\"", i, scenes [i]));
		}

		Debug.Log ("Starting Android Build!");
		BuildPipeline.BuildPlayer (scenes, path, BuildTarget.Android, BuildOptions.None);

		CopyDirectory ("Temp/StagingArea/assets/", "../" + androidFolderName + "/assets/");
		CopyDirectory ("Temp/StagingArea/raw/bin/Data/", "../" + androidFolderName + "/assets/bin/Data/");
		CopyDirectory ("Temp/StagingArea/libs/armeabi-v7a", "../" + androidFolderName + "/libs/armeabi-v7a");
		CopyDirectory ("Temp/StagingArea/libs/x86", "../" + androidFolderName + "/libs/x86");

		EditorUtility.DisplayDialog ("", "Completed!", "ok");
	}

	[MenuItem ("Tools/Build iOS New &3", false, 3)]
	static void CommandLineBuildiOS ()
	{
		Debug.Log ("Command line build ios version\n------------------\n------------------");
		PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneAndiPad;
		PlayerSettings.iOS.targetOSVersionString = iOSTargetOSVersion.iOS_7_0.ToString ();
		PlayerSettings.iOS.targetOSVersion = iOSTargetOSVersion.iOS_7_0;
		PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
		PlayerSettings.defaultIsFullScreen = true;
		string[] scenes = GetBuildScenes ();


		DeleteDirectory ("../proj.ios/Classes");
		DeleteDirectory ("../proj.ios/Data");
		DeleteDirectory ("../proj.ios/Libraries");
		DeleteDirectory ("../proj.ios/Unity-iPhone/Images.xcassets/AppIcon.appiconset");
		DeleteDirectory ("../proj.ios/Unity-iPhone/Images.xcassets/LaunchImage.launchimage");

		string path = GetBuildPath (Path.GetFullPath (".") + "/../proj.ios");
		if (scenes == null || scenes.Length == 0 || path == null)
			return;
	
		Debug.Log (string.Format ("Path: \"{0}\"", path));
		for (int i = 0; i < scenes.Length; ++i) {
			Debug.Log (string.Format ("Scene[{0}]: \"{1}\"", i, scenes [i]));
		}
	
		Debug.Log ("Starting Build!");
		BuildPipeline.BuildPlayer (scenes, path, BuildTarget.iOS, BuildOptions.None);

		Debug.Log ("copying icon and launchImage!");
		CopyDirectory ("../icon/AppIcon.appiconset", "../proj.ios/Unity-iPhone/Images.xcassets/AppIcon.appiconset");
		CopyDirectory ("../icon/LaunchImage.launchimage", "../proj.ios/Unity-iPhone/Images.xcassets/LaunchImage.launchimage");
		EditorUtility.DisplayDialog ("", "Completed!", "ok");
	}

	[MenuItem ("Tools/Build iOS Replace &4", false, 2)]
	static void CommandLineBuildiOSX ()
	{
		Debug.Log ("Command line build ios version\n------------------\n------------------");
		PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneAndiPad;
		PlayerSettings.iOS.targetOSVersionString = iOSTargetOSVersion.iOS_7_0.ToString ();
		PlayerSettings.iOS.targetOSVersion = iOSTargetOSVersion.iOS_7_0;
		PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
		PlayerSettings.defaultIsFullScreen = true;

		string[] scenes = GetBuildScenes ();
		string path = GetBuildPath (Path.GetFullPath (".") + "/../build");
		if (scenes == null || scenes.Length == 0 || path == null)
			return;

		Debug.Log (string.Format ("Path: \"{0}\"", path));
		for (int i = 0; i < scenes.Length; ++i) {
			Debug.Log (string.Format ("Scene[{0}]: \"{1}\"", i, scenes [i]));
		}

		Debug.Log ("Starting Build!");
		BuildPipeline.BuildPlayer (scenes, path, BuildTarget.iOS, BuildOptions.None);

		Debug.Log ("copying icon and launchImage!");
		DeleteDirectory ("../proj.ios/Classes");
		DeleteDirectory ("../proj.ios/Data");
		DeleteDirectory ("../proj.ios/Libraries");
		DeleteDirectory ("../proj.ios/Unity-iPhone/Images.xcassets/AppIcon.appiconset");
		DeleteDirectory ("../proj.ios/Unity-iPhone/Images.xcassets/LaunchImage.launchimage");

		CopyDirectory ("../build/Classes", "../proj.ios/Classes");
		CopyDirectory ("../build/Data", "../proj.ios/Data");
		CopyDirectory ("../build/Libraries", "../proj.ios/Libraries");
		CopyDirectory ("../icon/AppIcon.appiconset", "../proj.ios/Unity-iPhone/Images.xcassets/AppIcon.appiconset");
		CopyDirectory ("../icon/LaunchImage.launchimage", "../proj.ios/Unity-iPhone/Images.xcassets/LaunchImage.launchimage");

//		DeleteDirectory ("../build");

		EditorUtility.DisplayDialog ("", "Completed!", "ok");
	}

	static string [] GetBuildScenes ()
	{
		List<string> names = new List<string> ();

		foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes) {
			if (e == null)
				continue;

			if (e.enabled)
				names.Add (e.path);
		}

		return names.ToArray ();
	}

	static string GetBuildPath (string dirPath)
	{
		if (!Directory.Exists (dirPath)) {
			Directory.CreateDirectory (dirPath);
		}

		return dirPath;
	}

	public static void DeleteDirectory (string path)
	{
		DirectoryInfo dir = new DirectoryInfo (path);

		if (dir.Exists) {
			dir.Delete (true);
		}
	}

	public static void CopyDirectory (string sourceDirectory, string destDirectory)
	{
		if (!Directory.Exists (sourceDirectory)) {
			Directory.CreateDirectory (sourceDirectory);
		}

		if (!Directory.Exists (destDirectory)) {
			Directory.CreateDirectory (destDirectory);
		}

		if (sourceDirectory.EndsWith ("/"))
			sourceDirectory = sourceDirectory.Substring (0, sourceDirectory.Length - 1);

		if (destDirectory.EndsWith ("/"))
			destDirectory = destDirectory.Substring (0, destDirectory.Length - 1);

//		Debug.Log ("copy directory: [" + sourceDirectory + "] to:[" + destDirectory + "]");

		CopyFile (sourceDirectory, destDirectory);

		string[] directionName = Directory.GetDirectories (sourceDirectory);

		foreach (string directionPath in directionName) {
			string directionPathTemp = destDirectory + "/"
			                           + directionPath.Substring (sourceDirectory.Length);
			CopyDirectory (directionPath, directionPathTemp);
		}
	}

	public static void CopyFile (string sourceDirectory, string destDirectory)
	{
		string[] fileName = Directory.GetFiles (sourceDirectory);

		foreach (string filePath in fileName) {
			string filePathTemp = destDirectory + "/" + filePath.Substring (sourceDirectory.Length);

			if (File.Exists (filePathTemp)) {
				File.Copy (filePath, filePathTemp, true);
			} else {
				File.Copy (filePath, filePathTemp);
			}
		}
	}
}