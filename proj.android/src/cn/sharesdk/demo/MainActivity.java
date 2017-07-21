package cn.sharesdk.demo;

import android.os.Bundle;
import android.widget.Toast;

import com.tencent.mm.sdk.modelmsg.SendAuth;
import com.tencent.mm.sdk.openapi.IWXAPI;
import com.tencent.mm.sdk.openapi.WXAPIFactory;
import com.unity3d.player.UnityPlayerActivity;

import cn.sharesdk.demo.utils.Debug;
import cn.sharesdk.demo.wxapi.WXEntryActivity;

public class MainActivity extends UnityPlayerActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
//        setContentView(R.layout.main);

//        findViewById(R.id.buttonLogin).setOnClickListener(new View.OnClickListener() {
//
//            @Override
//            public void onClick(View v) {
//                loginToWeiXin();
//            }
//        });
    }

    public void wechatLogin() {
        loginToWeiXin();
    }

    private void loginToWeiXin() {
        IWXAPI mApi = WXAPIFactory.createWXAPI(this, WXEntryActivity.WEIXIN_APP_ID, true);
        mApi.registerApp(WXEntryActivity.WEIXIN_APP_ID);

        Debug.Log("login to wechat.");

        if (mApi != null && mApi.isWXAppInstalled()) {
            SendAuth.Req req = new SendAuth.Req();
            req.scope = "snsapi_userinfo";
            req.state = "wechat_sdk_demo_test_neng";
            mApi.sendReq(req);
        } else
            Toast.makeText(this, "用户未安装微信", Toast.LENGTH_SHORT).show();
    }

}
