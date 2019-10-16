using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public sealed class SceneController : MonoBehaviour
{

    private string url;
    private AndroidJavaClass ajc;

    
   

    // Use this for initialization
    void Start()
    {
        if(SceneManager.GetActiveScene().name != "jikoku")
        {
            Static.JikokuClear();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TopClick()
    {
        SceneManager.LoadScene("top");
    }

    public void EkiClick()
    {
        SceneManager.LoadScene("eki");
    }

    public void JikokuClick()
    {
        //string name = "jikoku_";

        //if (Static.StationNo == "24")
        //{
        //    name += "nobori";
        //}
        //else
        //{
        //    name += "kudari";
        //}

        //DateTime dt = DateTime.Now;
        //int dow = (int)dt.DayOfWeek;

        //if(dow == 0 || dow == 6)
        //{
        //    name += "_kyu";
        //}
        string name = "jikoku";

        SceneManager.LoadScene(name);
    }

    public void KounaiClick()
    {
        SceneManager.LoadScene("home");
    }

    public void ToggleJikokuDay()
    {
        string scene = SceneManager.GetActiveScene().name;
        string houkou = scene.Remove(0, scene.IndexOf('_')).Substring(0, 7);
        string newscene = "jikoku" + houkou;

        if (!scene.Contains("_kyu"))
        {
            newscene += "_kyu";
        }

        SceneManager.LoadScene(newscene);
    }

    public void ToggleJikokuHoukou()
    {
        string scene = SceneManager.GetActiveScene().name;
        string newscene = "jikoku";

        if (!scene.Contains("kudari"))
        {
            newscene += "_kudari";
        }
        else
        {
            newscene += "_nobori";
        }
        
        if (scene.Contains("_kyu"))
        {
            newscene += "_kyu";
        }

        SceneManager.LoadScene(newscene);
    }

    public void ToggleKounai()
    {
        string scene = SceneManager.GetActiveScene().name;
        if (scene == "home")
        {
            SceneManager.LoadScene("kaisatu");
        }
        else
        {
            KounaiClick();
        }
    }

    public void BuildingClick()
    {
        url = "https://www.shinkeisei.co.jp/station/navi_matsudo_floorguide/";

        Application.OpenURL(url);
    }

    public void MapClick(string url)
    {
        Application.OpenURL(url);
    }

    public void UnkouClick()
    {
        SceneManager.LoadScene("unkou");
    }

    public void HomepageClick()
    {
        SqliteDatabase sqlite = new SqliteDatabase("shinkeisei.db");
        string query = "SELECT * FROM url_info WHERE type = 1";
        var response = sqlite.ExecuteQuery(query);

        url = response.Rows[0]["url"].ToString();
        Application.OpenURL(url);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Debug.Log("applicationWillResignActive or onPause");
        }
        else
        {
            Debug.Log("applicationDidBecomeActive or onResume");
            //ajc = new AndroidJavaClass("com.example.shinkeisei_plugin_lib.IntentReceiveActivity");
            //Static.JointFlag = ajc.CallStatic<String>("getJointflag");

            if (Static.initFlag == true)
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    // iphoneの場合はOpenURLに連携URLの通知が来る
                    
                }
                else if (Application.platform == RuntimePlatform.Android)
                {

                    AndroidCustomUrlLinkCheck();
                    GoStart();
                }

            }
        }
    }

    public void AndroidCustomUrlLinkCheck()
    {

        // 連携の確認
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass ajc = new AndroidJavaClass("com.example.shinkeisei_plugin_lib.IntentReceiveActivity");
            Static.JointFlag = ajc.CallStatic<String>("getJointflag");

            if (Static.JointFlag == "true")
            {
                string temp;
                ajc = new AndroidJavaClass("com.example.shinkeisei_plugin_lib.IntentReceiveActivity");
                try
                {
                    ajc.CallStatic("setJointflag", "false");
                    Static.JointAppname = ajc.CallStatic<string>("getKid");
                    temp = ajc.CallStatic<string>("getSid");
                    Static.StationNo = temp.Remove(0, 2);
                }
                catch
                {
                    Static.JointFlag = "false";
                }


                Debug.Log("Static.JointAppname :" + Static.JointAppname);
                Debug.Log("Static.StationNo :" + Static.StationNo);
            }

        }
    }

    //public static class NativeIntentParameterApi
    //{
    //    [DllImport("__Internal")]
    //    private static extern string getUrlString();

    //    public static string GetUrlString()
    //    {
    //        return getUrlString();
    //    }
    //}

    public void GoStart()
    {
        if (Static.JointFlag == "true")
        {
            Static.JointFlag = "false";

            WriteLog();

            UnkouClick();
        }

    }

    private void WriteLog()
    {
        string autoinc, campanyname, accesslogdate, stationid = "";
        autoinc = "0";
        campanyname = Static.JointAppname;
        accesslogdate = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff");
        stationid = Static.StationNo;
        string json = "{\"autoincid\":" + autoinc + ",\"campanyname\":\"" + campanyname + "\",\"accesslogdate\":\"" + accesslogdate + "\",\"stationid\":" + stationid + "}";
        var postlog = PostLinkLog(json);
        var coroutine = StartCoroutine(postlog);
        //yield return coroutine;
    }
    /// <summary>
    /// 連携Logの書き込み
    /// </summary>
    /// <returns></returns>
    public IEnumerator PostLinkLog(string json)
    {
        bool bret = false;

        byte[] postData = System.Text.Encoding.UTF8.GetBytes(json);
        url = "https://trainposinfo.shinkeisei.co.jp/ShinkeiseiMobileWeb/webresources/jp.co.shinkeisei.entity.loginfo";


        var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("appkey", "PS8W2qMBYGYJLEKDpqvxWiYC");
        yield return request.SendWebRequest();
        if (request.error == null)
        {
        }
        else
        {
            Debug.Log("Error! PostLinkLog");
            Debug.Log(request.error);
        }
    }

    public static class NativeBinding
    {
        [DllImport("__Internal")]
        public static extern string OnOpenURLListener_GetOpenURLString();
    }

    /// <summary>
    /// iPhoneからURL連携のパラメータを本メソッドに通知
    /// </summary>
    /// <param name="url"></param>
    public void OnOpenURL(string url)
    {
        var temp = url;
        Debug.Log("SceneController: url:" + temp);
        if (temp.Contains("?"))
        {
            var tempstr = temp.Remove(0, temp.IndexOf('?'));
            var tempparams = tempstr.Split('&');
            bool check1 = false;
            bool check2 = false;
            foreach (string param in tempparams)
            {
                if (param.Contains("kid"))
                {
                    try
                    {
                        Static.JointAppname = param.Remove(0, param.IndexOf('=') + 1);
                        check1 = true;
                    }catch
                    {

                    }

                }
                else if (param.Contains("sid"))
                {
                    try
                    {
                        var pstr = param.Remove(0, param.IndexOf('=') + 1);
                        Static.StationNo = pstr.Remove(0, 2);
                        check2 = true;
                    }
                    catch
                    {

                    }

                }
            }
            if(check1 == true && check2 == true)
            {
                Static.JointFlag = "true";
            }
            
        }
        if (Static.initFlag == true)
        {
            GoStart();
        }
    }

    // バス表示テスト
    public void Unkou1Click()
    {
        SceneManager.LoadScene("unkou 1");
    }

    public void BusClick()
    {
        url = "https://bus-vision.jp/skbus/view/searchStop.html";
        Application.OpenURL(url);
    }

    public void BusStationClick()
    {
        url = "https://bus-vision.jp/skbus/view/selectTimetable.html?stopCd=120180&lang=0";
        Application.OpenURL(url);
    }

    public void Top1Click()
    {
        SceneManager.LoadScene("top 1");
    }
}
