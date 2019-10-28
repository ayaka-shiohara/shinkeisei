using MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UnkouController : MonoBehaviour {

    private float timeOut = 15;
    private float timeElapsed;

    private List<AppData> apps;
    private List<DownloadData> downloads;

    private SqliteDatabase sqlite;
    private string query;

    private List<PositionData> position;

    private bool setposflag = true;
    private float scrollfloat = 0.0416667f;

    private Dictionary<string, GameObject> norikae;
    private GameObject norikae_01;
    private GameObject norikae_05;
    private GameObject norikae_11;
    private GameObject norikae_19;
    private GameObject norikae_23;
    private GameObject norikae_24;

    private Dictionary<string, GameObject> bus;
    private GameObject bus_01;
    private GameObject bus_05;
    private GameObject bus_06;
    private GameObject bus_07;
    private GameObject bus_11;
    private GameObject bus_13;
    private GameObject bus_14;
    private GameObject bus_15;
    private GameObject bus_17;
    private GameObject bus_18;
    private GameObject bus_19;
    private GameObject bus_21;
    private GameObject bus_22;
    private GameObject bus_23;

    private bool first = true;
    private GameObject nowOpen;

    // Use this for initialization
    void Start()
    {
        SetNorikae();
        SetBus();

        WriteText();

        if (Static.StationNo == null)
        {
            Static.StationNo = "01";
        }

        apps = new List<AppData>();
        downloads = new List<DownloadData>();

        sqlite = new SqliteDatabase("shinkeisei.db");

        StartCoroutine(LoadNextScene());

        if(Static.FavoStation != "")
        {
            toggleEki(Static.FavoStation);
        }
        first = false;
    }

    IEnumerator LoadNextScene()
    {
        var app = GatApp();
        var coroutine_app = StartCoroutine(app);
        yield return coroutine_app;

        var download = GetDownload();
        var coroutine_download = StartCoroutine(download);
        yield return coroutine_download;

        MakeTrain();
        if (setposflag)
        {
            SetPos();
        }

        if (Static.JointFlag == "true")
        {
            Static.JointClear();
        }
    }

    // Update is called once per frame
    void Update() {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= timeOut)
        {
            MakeTrain();
        }
    }

    public void JumpApp(string name)
    {
        var names = name.Split('-');

        foreach (AppData app in apps)
        {
            if (app.stationid.Equals(names[0]) && app.Rid.Equals(names[1]))
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    Application.OpenURL(app.url);
                    Thread.Sleep(500);
                    foreach (DownloadData dd in downloads)
                    {
                        if (dd.appname == app.appname)
                        {
                            Application.OpenURL(dd.url);
                            break;
                        }
                    }
                }
                else
                {
                    using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                    using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    using (AndroidJavaClass intentStaticClass = new AndroidJavaClass("android.content.Intent"))
                    using (AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri"))
                    using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", app.url))
                    {
                        string actionView = intentStaticClass.GetStatic<string>("ACTION_VIEW");
                        int FLAG_ACTIVITY_NEW_TASK = intentStaticClass.GetStatic<int>("FLAG_ACTIVITY_NEW_TASK");
                        int FLAG_ACTIVITY_CLEAR_TASK = intentStaticClass.GetStatic<int>("FLAG_ACTIVITY_CLEAR_TASK");
                        int orOP = FLAG_ACTIVITY_NEW_TASK | FLAG_ACTIVITY_CLEAR_TASK;

                        using (AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", actionView, uriObject))
                        {
                            try
                            {
                                intent.Call<AndroidJavaObject>("addFlags", orOP);
                                currentActivity.Call("startActivity", intent);
                            }
                            catch (Exception e)
                            {
                                foreach (DownloadData dd in downloads)
                                {
                                    if (dd.appname == app.appname)
                                    {
                                        Application.OpenURL(dd.url);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    IEnumerator GatApp()
    {
        query = "SELECT * FROM train_joint_info;";
        var response = sqlite.ExecuteQuery(query);
        yield return response;

        foreach(DataRow dr in response.Rows)
        {
            if ((int)dr["enableflag"] == 1 && dr["url"].ToString() != "")
            {
                AppData ad = new AppData();

                ad.stationid = dr["stationid"].ToString();
                ad.Rid = dr["Rid"].ToString();
                ad.appname = dr["appname"].ToString();
                ad.url = dr["url"].ToString();

                apps.Add(ad);
            }
        }

        yield return true;
    }
    
    IEnumerator GetDownload()
    {
        string platform = (Application.platform == RuntimePlatform.IPhonePlayer) ? "appstoreurl" : "playstoreurl";

        query = "SELECT * FROM train_download_info;";
        var response = sqlite.ExecuteQuery(query);
        yield return response;

        foreach(DataRow dr in response.Rows)
        {
            DownloadData dd = new DownloadData();

            dd.appname = dr["appname"].ToString();
            dd.url = dr[platform].ToString();

            downloads.Add(dd);
        }

        yield return true;
    }

    IEnumerator GetTrain()
    {
        position = new List<PositionData>();
        var downloadUnkou = DownloadUnkou();
        var coroutine_downloadUnkou = StartCoroutine(downloadUnkou);
        yield return coroutine_downloadUnkou;
        
        var unkou = GetUnkou();
        var coroutine = StartCoroutine(unkou);
        yield return coroutine;

        yield return true;
    }

    IEnumerator GetUnkou()
    {
        string name = "";

        foreach (PositionData pd in position)
        {
            Transform transform;
            GameObject obj;

            string objname = "sec" + pd.sectionid;
            if (pd.sectionid < 100)
            {
                transform = GameObject.Find("nobori").GetComponent<Canvas>().transform;
            }
            else
            {
                transform = GameObject.Find("kudari").GetComponent<Canvas>().transform;
            }

            obj = transform.Find(objname).gameObject;

            obj.SetActive(true);

            if (pd.sectionid == 1 || pd.sectionid == 47 || pd.sectionid == 101 || pd.sectionid == 147)
            {
                if(pd.sectionid == 147 && pd.laststop == "千葉中央")
                {
                    obj.SetActive(false);

                    objname += "-2";
                    obj = transform.Find(objname).gameObject;
                    obj.SetActive(true);
                }
            }

            Text delay = obj.transform.Find("chien").GetComponent<Text>();
            if (pd.delayminute != 0)
            {
                delay.text = "+" + pd.delayminute.ToString();
            }
            else
            {
                delay.text = "";
            }

            Text ikisaki = obj.transform.Find("ikisaki").GetComponent<Text>();
            ikisaki.text = pd.laststop;
        }

        yield return null;
    }
    
    IEnumerator DownloadUnkou()
    {
        string url = "https://trainposinfo.shinkeisei.co.jp/ShinkeiseiMobileWeb/webresources/jp.co.shinkeisei.entity.trainpositioninfo";

        Dictionary<string, string> header = new Dictionary<string, string>();
        header.Add("appkey", "PS8W2qMBYGYJLEKDpqvxWiYC");

        WWW www = new WWW(url, null, header);
        yield return www;

        IList json = (IList)Json.Deserialize(www.text);
        if (www.error == null)
        {
            DestroyTrain();
            foreach (IDictionary data in json)
            {
                PositionData pd = new PositionData();
                pd.laststop = data["laststop"].ToString();
                pd.trainno = ParseInt(data["trainno"]);
                pd.delayminute = ParseInt(data["delayminute"]);

                IDictionary data2 = (IDictionary)data["trainPositionInfoPK"];
                pd.stationid = ParseInt(data2["stationid"]);
                pd.sectionid = ParseInt(data2["sectionid"]);
                pd.sectionid_sub = ParseInt(data2["sectionidSub"]);
                pd.blockno = data2["blockno"].ToString();
                pd.orbitnumber = data2["orbitnumber"].ToString();

                position.Add(pd);
            }
        }
    }

    private void DestroyTrain()
    {
        var nobori_base = GameObject.Find("nobori").GetComponent<Canvas>().transform;
        var kudari_base = GameObject.Find("kudari").GetComponent<Canvas>().transform;
        
        foreach(Transform train in nobori_base)
        {
            train.gameObject.SetActive(false);
        }
        
        foreach (Transform train in kudari_base)
        {
            train.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// 取得データをInteger型にパース
    /// </summary>
    /// <param name="data">取得データ</param>
    /// <returns>パース済みint型</returns>
    private int ParseInt(object data)
    {
        return int.Parse(data.ToString());
    }

    private void SetPos()
    {
        var scroll = GameObject.Find("MainScroll").GetComponent<ScrollRect>();
        var go = GameObject.Find(Static.StationNo);
        var align = 1f;

        var targetRect = go.transform.GetComponent<RectTransform>();
        var contentHeight = scroll.content.rect.height;
        var viewportHeight = scroll.viewport.rect.height;
        // スクロール不要
        if (contentHeight < viewportHeight) scroll.verticalNormalizedPosition = 0f;

        // ローカル座標が contentHeight の上辺を0として負の値で格納されてる
        // これは現在のレイアウト特有なのかもしれないので、要確認
        var targetPos = contentHeight + GetPosY(targetRect) + targetRect.rect.height * align;
        var gap = viewportHeight * align; // 上端〜下端あわせのための調整量
        var normalizedPos = (targetPos - gap) / (contentHeight - viewportHeight);

        normalizedPos = Mathf.Clamp01(normalizedPos);
        scroll.verticalNormalizedPosition = normalizedPos;

        //var scroll = GameObject.Find("MainScroll").GetComponent<ScrollRect>();
        /*var aaa = Screen.height - 1608;// * 0.271f;
        GameObject scroll = GameObject.Find("Content");
        var pos = (float.Parse(Static.StationNo) - 1) * aaa;
        scroll.transform.position = new Vector2(scroll.transform.position.x, scroll.transform.position.y + pos);
        scroll.verticalNormalizedPosition -= pos * scrollfloat;
        if (pos != 0)
        {
            scroll.verticalNormalizedPosition -= 0.005f;
        }*/
    }

    private float GetPosY(RectTransform transform)
    {
        return transform.localPosition.y + transform.rect.y; //pivotによるズレをrect.yで補正
    }

    private void MakeTrain()
    {
        timeElapsed = 0;
        StartCoroutine(GetTrain());
        StartCoroutine(ChangeTime());
    }

    public void Reload()
    {
        setposflag = false;
        Start();
    }

    private IEnumerator ChangeTime()
    {
        Text text = GameObject.Find("UpdateText").GetComponent<Text>();
        string url = "https://trainposinfo.shinkeisei.co.jp/ShinkeiseiMobileWeb/webresources/jp.co.shinkeisei.entity.trainpositionupdate";
        Dictionary<string, string> header = new Dictionary<string, string>();
        header.Add("appkey", "PS8W2qMBYGYJLEKDpqvxWiYC");

        WWW www = new WWW(url, null, header);
        yield return www;

        IList json = (IList)Json.Deserialize(www.text);
        if (www.error == null)
        {
            foreach (IDictionary data in json)
            {
                IDictionary data2 = (IDictionary)data["trainPositionUpdatePK"];
                string date = data2["currentdate"].ToString();
                string time = String.Format("{0:D6}",data2["currenttime"].ToString());

                string year = date.Substring(0, 4);
                string month = date.Substring(4, 2);
                string day = date.Substring(6, 2);

                string hour = time.Substring(0, 2);
                string minute = time.Substring(2, 2);
                string second = time.Substring(4, 2);

                text.text = year + "/" + month + "/" + day + " "
                            + hour + ":" + minute + ":" + second + "　" + TextManager.Get(TextManager.KEY.SOUKOU_UPDATE);
            }
        }
    }

    private void WriteText()
    {
        Text attention = GameObject.Find("attention").transform.Find("Text").GetComponent<Text>();
        attention.text = TextManager.Get(TextManager.KEY.SOUKOU_MSG_1) + "\n" + TextManager.Get(TextManager.KEY.SOUKOU_MSG_2);

        for(int i = 1; i < 25; i++)
        {
            string no = string.Format("{0:D2}", i);

            Text kanji = GameObject.Find(no).transform.Find("ekiname").transform.Find("kanji").GetComponent<Text>();
            string kanjikey = "SL" + no;
            kanji.text = TextManager.Get(kanjikey);

            Text yomi = GameObject.Find(no).transform.Find("ekiname").transform.Find("yomi").GetComponent<Text>();
            string yomikey = "SL" + no + "_2";
            yomi.text = TextManager.Get(yomikey);
        }
    }

    private void SetNorikae()
    {
        if(norikae_01 != null)
        {
            return;
        }
        
        norikae = new Dictionary<string, GameObject>();

        norikae_01 = GameObject.Find("norikae_01");
        norikae.Add("norikae_01", norikae_01);
        norikae_01.SetActive(false);

        norikae_05 = GameObject.Find("norikae_05");
        norikae.Add("norikae_05", norikae_05);
        norikae_05.SetActive(false);

        norikae_11 = GameObject.Find("norikae_11");
        norikae.Add("norikae_11", norikae_11);
        norikae_11.SetActive(false);

        norikae_19 = GameObject.Find("norikae_19");
        norikae.Add("norikae_19", norikae_19);
        norikae_19.SetActive(false);

        norikae_23 = GameObject.Find("norikae_23");
        norikae.Add("norikae_23", norikae_23);
        norikae_23.SetActive(false);

        norikae_24 = GameObject.Find("norikae_24");
        norikae.Add("norikae_24", norikae_24);
        norikae_24.SetActive(false);
    }

    private void SetBus()
    {
        if (bus_01 != null)
        {
            return;
        }

        bus = new Dictionary<string, GameObject>();

        bus_01 = GameObject.Find("bus_01");
        bus.Add("bus_01", bus_01);
        bus_01.SetActive(false);

        bus_05 = GameObject.Find("bus_05");
        bus.Add("bus_05", bus_05);
        bus_05.SetActive(false);

        bus_06 = GameObject.Find("bus_06");
        bus.Add("bus_06", bus_06);
        bus_06.SetActive(false);

        bus_07 = GameObject.Find("bus_07");
        bus.Add("bus_07", bus_07);
        bus_07.SetActive(false);

        bus_11 = GameObject.Find("bus_11");
        bus.Add("bus_11", bus_11);
        bus_11.SetActive(false);

        bus_13 = GameObject.Find("bus_13");
        bus.Add("bus_13", bus_13);
        bus_13.SetActive(false);

        bus_14 = GameObject.Find("bus_14");
        bus.Add("bus_14", bus_14);
        bus_14.SetActive(false);

        bus_15 = GameObject.Find("bus_15");
        bus.Add("bus_15", bus_15);
        bus_15.SetActive(false);

        bus_17 = GameObject.Find("bus_17");
        bus.Add("bus_17", bus_17);
        bus_17.SetActive(false);

        bus_18 = GameObject.Find("bus_18");
        bus.Add("bus_18", bus_18);
        bus_18.SetActive(false);

        bus_19 = GameObject.Find("bus_19");
        bus.Add("bus_19", bus_19);
        bus_19.SetActive(false);

        bus_21 = GameObject.Find("bus_21");
        bus.Add("bus_21", bus_21);
        bus_21.SetActive(false);

        bus_22 = GameObject.Find("bus_22");
        bus.Add("bus_22", bus_22);
        bus_22.SetActive(false);

        bus_23 = GameObject.Find("bus_23");
        bus.Add("bus_23", bus_23);
        bus_23.SetActive(false);
    }

    public void ToggleNorikae(string name)
    {
        GameObject toggle = norikae[name];
        toggle.SetActive(!toggle.activeSelf);

        if(nowOpen == toggle)
        {
            nowOpen = null;
        }
        else
        {
            if (nowOpen != null)
            {
                nowOpen.SetActive(false);
            }
            nowOpen = toggle;
        }
    }

    public void ToggleBus(string name)
    {
        GameObject toggle = bus[name];
        toggle.SetActive(!toggle.activeSelf);

        if (nowOpen == toggle)
        {
            nowOpen = null;
        }
        else
        {
            if (nowOpen != null)
            {
                nowOpen.SetActive(false);
            }
            nowOpen = toggle;
        }
    }

    public void JumpBus(string number)
    {
        string url = "https://bus-vision.jp/skbus/view/selectTimetable.html?stopCd=" + number + "&lang=0";

        Application.OpenURL(url);
    }

    public void toggleEki(string stationno)
    {
        StationColorChange(stationno);

        if (!first)
        {
            if (Static.FavoStation == "")
            {
                Static.setFavo(stationno);
            }
            else
            {
                if (Static.FavoStation == stationno)
                {
                    Static.setFavo("");
                }
                else
                {
                    StationColorChange(Static.FavoStation);

                    Static.setFavo(stationno);
                }
            }
        }
    }

    private void StationColorChange(string stationno)
    {
        Image image = GameObject.Find(stationno).transform.Find("Image").GetComponent<Image>();
        string imagepath = imagepath = "Sprites/station_no/" + stationno;
        if (!image.sprite.name.Contains("y"))
        {
            imagepath += "_y";
        }
        Sprite sprite = Resources.Load<Sprite>(imagepath);
        image.sprite = sprite;
    }

    class AppData
    {
        public string stationid;
        public string Rid;
        public string appname;
        public string url;
    }

    class DownloadData
    {
        public string appname;
        public string url;
    }

    class PositionData
    {
        public int sectionid;
        public int sectionid_sub;
        public int stationid;
        public string blockno;
        public string orbitnumber;
        public int delayminute;
        public int trainno;
        public string laststop;
    }
}
