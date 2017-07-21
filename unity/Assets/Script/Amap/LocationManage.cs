using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class LocationManage : MonoBehaviour {

  public Text txtLocation;
  public Text txtInfo;
  private AmapEvent amap;
  private AndroidJavaClass jcu;
  private AndroidJavaObject jou;
  private AndroidJavaObject mLocationClient;
  private AndroidJavaObject mLocationOption;

  public void StartLocation() {
    try {
      txtInfo.text = "start location...";

      txtInfo.text = txtInfo.text + "\r\n";
      jcu = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
      jou = jcu.GetStatic<AndroidJavaObject>("currentActivity");
      txtInfo.text = txtInfo.text + "currentActivity get...";

      txtInfo.text = txtInfo.text + "\r\n";
      mLocationClient = new AndroidJavaObject("com.amap.api.location.AMapLocationClient", jou);
      txtInfo.text = txtInfo.text + "AMapLocationClient get...";

      txtInfo.text = txtInfo.text + "\r\n";
      mLocationOption = new AndroidJavaObject("com.amap.api.location.AMapLocationClientOption");
      txtInfo.text = txtInfo.text + "AMapLocationClientOption get...";

      txtInfo.text = txtInfo.text + "\r\n";
      mLocationClient.Call("setLocationOption", mLocationOption);
      txtInfo.text = txtInfo.text + "setLocationOption...";

      amap = new AmapEvent();
      amap.locationChanged += OnLocationChanged;

      txtInfo.text = txtInfo.text + "\r\n";
      mLocationClient.Call("setLocationListener", amap);
      txtInfo.text = txtInfo.text + "setLocationListener...";

      txtInfo.text = txtInfo.text + "\r\n";
      mLocationClient.Call("startLocation");
      txtInfo.text = txtInfo.text + "startLocation...";

    } catch (Exception ex) {
      txtInfo.text = txtInfo.text + "\r\n";
      txtInfo.text = txtInfo.text + "--------------------";
      txtInfo.text = txtInfo.text + ex.Message;

      EndLocation();
    }
  }

  public void EndLocation() {
    if (amap != null) {
      amap.locationChanged -= OnLocationChanged;
    }

    if (mLocationClient != null) {
      mLocationClient.Call("stopLocation");
      mLocationClient.Call("onDestroy");
    }

    txtLocation.text = "";
  }

  private void OnLocationChanged(AndroidJavaObject amapLocation) {
    if (amapLocation != null) {
      if (amapLocation.Call<int>("getErrorCode") == 0) {
        txtLocation.text = ">>success:";

        try {
          txtLocation.text = txtLocation.text + "\r\n>>定位结果来源:" + amapLocation.Call<int>("getLocationType").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>纬度:" + amapLocation.Call<double>("getLatitude").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>经度:" + amapLocation.Call<double>("getLongitude").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>精度信息:" + amapLocation.Call<float>("getAccuracy").ToString();
          //txtLocation.text = txtLocation.text + "\r\n>>定位时间:" + amapLocation.Call<AndroidJavaObject> ("getTime").ToString ();  
          txtLocation.text = txtLocation.text + "\r\n>>地址:" + amapLocation.Call<string>("getAddress").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>国家:" + amapLocation.Call<string>("getCountry").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>省:" + amapLocation.Call<string>("getProvince").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>城市:" + amapLocation.Call<string>("getCity").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>城区:" + amapLocation.Call<string>("getDistrict").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>街道:" + amapLocation.Call<string>("getStreet").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>门牌:" + amapLocation.Call<string>("getStreetNum").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>城市编码:" + amapLocation.Call<string>("getCityCode").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>地区编码:" + amapLocation.Call<string>("getAdCode").ToString();

          txtLocation.text = txtLocation.text + "\r\n>>海拔:" + amapLocation.Call<double>("getAltitude").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>方向角:" + amapLocation.Call<float>("getBearing").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>定位信息描述:" + amapLocation.Call<string>("getLocationDetail").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>兴趣点:" + amapLocation.Call<string>("getPoiName").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>提供者:" + amapLocation.Call<string>("getProvider").ToString();
          txtLocation.text = txtLocation.text + "\r\n>>卫星数量:" + amapLocation.Call<int>("getSatellites").ToString();
          //txtLocation.text = txtLocation.text + "\r\n>>当前速度:" + amapLocation.Call<string> ("getSpeed").ToString ();  

        } catch (Exception ex) {
          txtLocation.text = txtLocation.text + "\r\n--------------ex-------------:";
          txtLocation.text = txtLocation.text + "\r\n" + ex.Message;
        }

      } else {
        txtLocation.text = ">>amaperror:";
        txtLocation.text = txtLocation.text + ">>getErrorCode:" + amapLocation.Call<int>("getErrorCode").ToString();
        txtLocation.text = txtLocation.text + ">>getErrorInfo:" + amapLocation.Call<string>("getErrorInfo");
      }
    } else {
      txtInfo.text = "amaplocation is null.";
    }
  }
}