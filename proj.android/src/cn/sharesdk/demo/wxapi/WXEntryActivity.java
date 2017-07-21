package cn.sharesdk.demo.wxapi;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.widget.Toast;

import com.tencent.mm.sdk.constants.ConstantsAPI;
import com.tencent.mm.sdk.modelbase.BaseReq;
import com.tencent.mm.sdk.modelbase.BaseResp;
import com.tencent.mm.sdk.modelmsg.SendAuth;
import com.tencent.mm.sdk.openapi.IWXAPI;
import com.tencent.mm.sdk.openapi.IWXAPIEventHandler;
import com.tencent.mm.sdk.openapi.WXAPIFactory;
import com.unity3d.player.UnityPlayer;

import org.json.JSONObject;

import cn.sharesdk.demo.utils.Debug;
import cn.sharesdk.demo.utils.OkHttpUtils;
import okhttp3.Response;

public class WXEntryActivity extends Activity implements IWXAPIEventHandler {
    public static final String WEIXIN_APP_ID = "wx4868b35061f87885";
    private static final String APP_SECRET = "64020361b8ec4c99936c0e3999a9f249";
    private IWXAPI mWeixinAPI;
    private static String uuid;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        mWeixinAPI = WXAPIFactory.createWXAPI(this, WEIXIN_APP_ID, true);
        mWeixinAPI.handleIntent(this.getIntent(), this);
    }

    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);
        setIntent(intent);
        mWeixinAPI.handleIntent(intent, this);//必须调用此句话
    }

    @Override
    public void onReq(BaseReq arg0) {
        // TODO Auto-generated method stub
    }

    /**
     * 发送到微信请求的响应结果
     * <p/>
     * （1）用户同意授权后得到微信返回的一个code，将code替换到请求地址GetCodeRequest里的CODE，同样替换APPID和SECRET
     * （2）将新地址newGetCodeRequest通过HttpClient去请求，解析返回的JSON数据
     * （3）通过解析JSON得到里面的openid （用于获取用户个人信息）还有 access_token
     * （4）同样地，将openid和access_token替换到GetUnionIDRequest请求个人信息的地址里
     * （5）将新地址newGetUnionIDRequest通过HttpClient去请求，解析返回的JSON数据
     * （6）通过解析JSON得到该用户的个人信息，包括unionid
     */


    private static String authCode = "";

    @Override
    public void onResp(BaseResp baseResp) {
        //微信登录为getType为1，分享为0
        if (baseResp.getType() == ConstantsAPI.COMMAND_SENDAUTH) {
            //登录回调
            SendAuth.Resp resp = (SendAuth.Resp) baseResp;
            Debug.Log("on resp code: " + resp.errCode);
            switch (resp.errCode) {
                case BaseResp.ErrCode.ERR_OK:
                    String code = String.valueOf(resp.code);
                    //获取用户信息
                    Debug.Log("wechat reponse, authCode= " + authCode + " code=" + code);
                    if (authCode != null && authCode.equals(code)) {
                    } else {
                        authCode = code;
                        getAccessToken(code);
                    }
                    break;
                case BaseResp.ErrCode.ERR_AUTH_DENIED://用户拒绝授权
                    break;
                case BaseResp.ErrCode.ERR_USER_CANCEL://用户取消
                    break;
                default:
                    break;
            }
        } else if (baseResp.getType() == ConstantsAPI.COMMAND_SENDMESSAGE_TO_WX) {
            //分享成功回调
            switch (baseResp.errCode) {
                case BaseResp.ErrCode.ERR_OK:
                    //分享成功
                    Toast.makeText(WXEntryActivity.this, "分享成功", Toast.LENGTH_LONG).show();
                    break;
                case BaseResp.ErrCode.ERR_USER_CANCEL:
                    //分享取消
                    Toast.makeText(WXEntryActivity.this, "分享取消", Toast.LENGTH_LONG).show();
                    break;
                case BaseResp.ErrCode.ERR_AUTH_DENIED:
                    //分享拒绝
                    Toast.makeText(WXEntryActivity.this, "分享拒绝", Toast.LENGTH_LONG).show();
                    break;
            }
        }
        finish();
    }

    private void getAccessToken(String code) {
        //获取授权
        String http = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + WEIXIN_APP_ID + "&secret=" + APP_SECRET
                + "&code=" + code + "&grant_type=authorization_code";
        Debug.Log(" get access token : " + http);
        OkHttpUtils.ResultCallback resultCallback = new OkHttpUtils.ResultCallback() {
            @Override
            public void onSuccess(Response response) {
                Debug.Log("get access token success");
                try {
                    String result = response.body().string();
                    JSONObject jsonObject = new JSONObject(result);
                    String access = jsonObject.getString("access_token");
                    String openId = jsonObject.getString("openid");
                    Debug.Log("access: " + access + "  openid: " + openId);
//                    getUserInfo(access, openId);

                    UnityPlayer.UnitySendMessage("PlatformBridge", "WeChatLoginCallBack", result);
                } catch (Exception e) {
                    onFailure(e);
                }
            }

            @Override
            public void onFailure(Exception e) {
                e.printStackTrace();
                Toast.makeText(WXEntryActivity.this, "登录失败2", Toast.LENGTH_SHORT).show();
            }
        };
        OkHttpUtils.get(http, resultCallback);
    }

    private void getUserInfo(String access, String openId) {
        //获取个人信息
        Debug.Log("get userInfo: access_token=" + access + "  openId=" + openId);

        String getUserInfo = "https://api.weixin.qq.com/sns/userinfo?access_token=" + access + "&openid=" + openId + "";
        OkHttpUtils.ResultCallback resultCallback = new OkHttpUtils.ResultCallback() {
            @Override
            public void onSuccess(Response response) {
                try {
                    JSONObject jsonObject = new JSONObject(response.body().string());
                    String nickname = jsonObject.getString("nickname");
                    int sex = Integer.parseInt(jsonObject.get("sex").toString());
                    String headimgurl = jsonObject.getString("headimgurl");
                    Debug.Log("get userInfo success: nickname=" + nickname + " sex:" + sex + "  headImgUrl=" + headimgurl);
                    Toast.makeText(WXEntryActivity.this, jsonObject.toString(), Toast.LENGTH_LONG).show();
                    finish();
                } catch (Exception e) {
                    onFailure(e);
                }
            }

            @Override
            public void onFailure(Exception e) {
                e.printStackTrace();
                Toast.makeText(WXEntryActivity.this, "登录失败1", Toast.LENGTH_SHORT).show();
            }
        };
        OkHttpUtils.get(getUserInfo, resultCallback);
    }
}
