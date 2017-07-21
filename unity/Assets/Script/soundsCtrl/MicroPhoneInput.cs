using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class MicroPhoneInput : MonoBehaviour
{

	private static MicroPhoneInput m_instance;

	public float sensitivity = 100;
	public float loudness = 0;

	private static string[] micArray = null;

	const int HEADER_SIZE = 44;

	const int RECORD_TIME = 10;
	List<int> userList;
	private AudioClip redioclip;

	public static MicroPhoneInput getInstance()
	{
		if (m_instance == null) {
			micArray = Microphone.devices;
			if (micArray.Length == 0) {
			}
			foreach (string deviceStr in Microphone.devices) {
				Debug.Log("device name = " + deviceStr);
			}
			if (micArray.Length == 0) {
			}

			GameObject MicObj = new GameObject("MicObj");
			m_instance = MicObj.AddComponent<MicroPhoneInput>();
		}
		return m_instance;
	}

	public void StartRecord(List<int> _userList)
	{
		userList = _userList;
		SoundCtrl.playAudio.Stop();
		if (micArray.Length == 0) {
			return;
		}
		//GetComponent<AudioSource>().loop = false;  
		//GetComponent<AudioSource>().mute = true;  
		redioclip = Microphone.Start("inputMicro", false, 30, 8000); //22050    
		while (!(Microphone.GetPosition(null) > 0)) {
		}
		//	GetComponent<AudioSource>().Play ();  
		//倒计时   
		//StartCoroutine(TimeDown());  
	}

	public float StopRecord()
	{
		if (micArray.Length == 0) {
			return 0;
		}
		if (!Microphone.IsRecording(null)) {
			return 0;
		}
		int lastTime = Microphone.GetPosition(null);
		Microphone.End(null);
		if (lastTime <= 0)
			return 0;
		float[] samples = new float[redioclip.samples]; //
		redioclip.GetData(samples, 0);
		float[] clipSamples = new float[lastTime];
		Array.Copy(samples, clipSamples, clipSamples.Length - 1);
		redioclip = AudioClip.Create("playRecordClip", clipSamples.Length, 1, 8000, false);
		redioclip.SetData(clipSamples, 0);

		SoundCtrl.playAudio.clip = redioclip;
		ChatSocket.getInstance().sendMsg(new MicInputRequest(userList, GetClipData()));
		PlayRecord();
		return redioclip.length;
	}

	public Byte[] GetClipData()
	{
		if (SoundCtrl.playAudio.clip == null) {
			return null;
		}

		float[] samples = new float[SoundCtrl.playAudio.clip.samples];
		SoundCtrl.playAudio.clip.GetData(samples, 0);


		Byte[] outData = new byte[samples.Length * 2];
		int rescaleFactor = 32767; //to convert float to Int16   
		for (int i = 0; i < samples.Length; i++) {
			short temshort = (short)(samples [i] * rescaleFactor);
			Byte[] temdata = System.BitConverter.GetBytes(temshort);
			outData [i * 2] = temdata [0];
			outData [i * 2 + 1] = temdata [1];
		}
		if (outData == null || outData.Length <= 0) {
			return null;
		}
		return outData;
	}

	public float PlayClipData(byte[] data)
	{
		if (data.Length == 0) {
			return 0;
		}

		int i = 0;
		List<short> result = new List<short>();
		while (data.Length - i >= 2) {
			result.Add(BitConverter.ToInt16(data, i));
			i += 2;
		}
		Int16[] arr = result.ToArray();//这就是你要的
		return PlayClipData(arr);
	}

	public float PlayClipData(Int16[] intArr)
	{
		if (intArr.Length == 0) {
			return 0;
		}

		float[] samples = new float[intArr.Length];
		int rescaleFactor = 32767;
		for (int i = 0; i < intArr.Length; i++) {
			samples [i] = (float)intArr [i] / rescaleFactor;
		}
		SoundCtrl.playAudio.clip = AudioClip.Create("playRecordClip", samples.Length, 1, 8000, false);
		SoundCtrl.playAudio.clip.SetData(samples, 0);
		SoundCtrl.playAudio.mute = false;
		SoundCtrl.playAudio.volume = 1;
		SoundCtrl.playAudio.Play();
		return SoundCtrl.playAudio.clip.length;
	}

	private void PlayRecord()
	{
		if (SoundCtrl.playAudio.clip == null) {
			return;
		}
		SoundCtrl.playAudio.mute = false;
		SoundCtrl.playAudio.loop = false;
		SoundCtrl.playAudio.volume = 1;
		SoundCtrl.playAudio.Play();

	}

	public float GetAveragedVolume()
	{
		float[] data = new float[256];
		float a = 0;
		SoundCtrl.playAudio.GetOutputData(data, 0);
		foreach (float s in data) {
			a += Mathf.Abs(s);
		}
		return a / 256;
	}

	private IEnumerator TimeDown()
	{
		int time = 0;
		while (time < RECORD_TIME) {
			if (!Microphone.IsRecording(null)) { //如果没有录制   
				yield break;
			}
			yield return new WaitForSeconds(1);
			time++;
		}
		if (time >= RECORD_TIME) {
			StopRecord();
		}
		yield return 0;
	}

	public void micInputNotice(ClientResponse response)
	{
		if (GlobalDataScript.soundToggle) {
			byte[] data = response.bytes;
			int i = 0;
			List<short> result = new List<short>();
			while (data.Length - i >= 2) {
				result.Add(BitConverter.ToInt16(data, i));
				i += 2;
			}
			Int16[] arr = result.ToArray();//这就是你要的
			PlayClipData(arr);
		}
	}

	//save to localhost
	public bool Save(string filename)
	{

		AudioClip clip = SoundCtrl.playAudio.clip;

		if (!filename.ToLower().EndsWith(".wav")) {
			filename += ".wav";
		}

		string filepath = Path.Combine(Application.persistentDataPath, filename);
		// Make sure directory exists if user is saving to sub dir.  
		Directory.CreateDirectory(Path.GetDirectoryName(filepath));

		using (FileStream fileStream = CreateEmpty(filepath)) {

			ConvertAndWrite(fileStream, clip);

			WriteHeader(fileStream, clip);
		}

		return true; // TODO: return false if there's a failure saving the file  
	}

	private FileStream CreateEmpty(string filepath)
	{
		FileStream fileStream = new FileStream(filepath, FileMode.Create);
		byte emptyByte = new byte();

		for (int i = 0; i < HEADER_SIZE; i++) { //preparing the header  
			fileStream.WriteByte(emptyByte);
		}

		return fileStream;
	}

	private void ConvertAndWrite(FileStream fileStream, AudioClip clip)
	{

		float[] samples = new float[clip.samples];

		clip.GetData(samples, 0);

		Int16[] intData = new Int16[samples.Length];

		Byte[] bytesData = new Byte[samples.Length * 2];

		int rescaleFactor = 32767; //to convert float to Int16  

		for (int i = 0; i < samples.Length; i++) {
			intData [i] = (short)(samples [i] * rescaleFactor);
			Byte[] byteArr = new Byte[2];
			byteArr = BitConverter.GetBytes(intData [i]);
			byteArr.CopyTo(bytesData, i * 2);
		}

		fileStream.Write(bytesData, 0, bytesData.Length);
	}

	private void WriteHeader(FileStream fileStream, AudioClip clip)
	{

		int hz = clip.frequency;
		int channels = clip.channels;
		int samples = clip.samples;

		fileStream.Seek(0, SeekOrigin.Begin);

		Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
		fileStream.Write(riff, 0, 4);

		Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
		fileStream.Write(chunkSize, 0, 4);

		Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
		fileStream.Write(wave, 0, 4);

		Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
		fileStream.Write(fmt, 0, 4);

		Byte[] subChunk1 = BitConverter.GetBytes(16);
		fileStream.Write(subChunk1, 0, 4);

		UInt16 two = 2;
		UInt16 one = 1;

		Byte[] audioFormat = BitConverter.GetBytes(one);
		fileStream.Write(audioFormat, 0, 2);

		Byte[] numChannels = BitConverter.GetBytes(channels);
		fileStream.Write(numChannels, 0, 2);

		Byte[] sampleRate = BitConverter.GetBytes(hz);
		fileStream.Write(sampleRate, 0, 4);

		Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2  
		fileStream.Write(byteRate, 0, 4);

		UInt16 blockAlign = (ushort)(channels * 2);
		fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

		UInt16 bps = 16;
		Byte[] bitsPerSample = BitConverter.GetBytes(bps);
		fileStream.Write(bitsPerSample, 0, 2);

		Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
		fileStream.Write(datastring, 0, 4);

		Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
		fileStream.Write(subChunk2, 0, 4);

		//      fileStream.Close();  
	}
}