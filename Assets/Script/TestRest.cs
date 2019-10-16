using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniJSON;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using System;

public class TestRest : MonoBehaviour
{
    private Slider slider;

    private SqliteDatabase sqlite;
    private string url;
    private string query;

    private bool jikokuflag = false;
    private bool floorflag = false;
    private bool homeflag = false;
    private bool urlflag = false;
    private bool jointflag = false;
    private bool downloadflag = false;

    private int ver_jikoku;
    private int ver_floor;
    private int ver_home;
    private int ver_url;
    private int ver_joint;
    private int ver_download;

    private VersionData server_v;
    private VersionData client_v;

    // Use this for initialization
    void Start()
    {
        sqlite = new SqliteDatabase("shinkeisei.db");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator CheckVersion()
    {
        // バージョン取得
        var enumerator = GetVersion();
        var coroutine = StartCoroutine(enumerator);
        yield return coroutine;
        bool ret = (bool)enumerator.Current;

        if (ret == true)
        {

            // ダウンロードすべき
            yield return true;
        }
        else
        {
            yield return false;
        }
    }
    public IEnumerator GetData()
    {
        // プログレスバー更新
        ChangeProgress();
        yield return null;

        // URLの取得
        var enumerator = GetUrl();
        var coroutine = StartCoroutine(enumerator);
        yield return coroutine;
        bool ret = (bool)enumerator.Current;
        while (ret == false)
        {

            for (int i = 0; i < 10; i++)
            {
                WriteText(TextManager.Get(TextManager.KEY.DL_ERROR) + "..." + TextManager.Get(TextManager.KEY.DL_PLEASE)
                                            + TextManager.Get(TextManager.KEY.DL_RETRY_1) + (10 - i) + TextManager.Get(TextManager.KEY.DL_RETRY_2));
                yield return null;
                yield return new WaitForSeconds(1); //1秒待つ            }

            }
            enumerator = GetUrl();
            coroutine = StartCoroutine(enumerator);
            yield return coroutine;
            ret = (bool)enumerator.Current;
        }

        // プログレスバー更新
        ChangeProgress();
        yield return null;

        // アプリケーション連携URL
        enumerator = GetJoint();
        coroutine = StartCoroutine(enumerator);
        yield return coroutine;
        ret = (bool)enumerator.Current;
        while (ret == false)
        {

            for (int i = 0; i < 10; i++)
            {
                WriteText(TextManager.Get(TextManager.KEY.DL_ERROR) + "..." + TextManager.Get(TextManager.KEY.DL_PLEASE)
                                            + TextManager.Get(TextManager.KEY.DL_RETRY_1) + (10 - i) + TextManager.Get(TextManager.KEY.DL_RETRY_2));
                yield return null;
                yield return new WaitForSeconds(1); //1秒待つ            }

            }
            enumerator = GetJoint();
            coroutine = StartCoroutine(enumerator);
            yield return coroutine;
            ret = (bool)enumerator.Current;
        }

        // プログレスバー更新
        ChangeProgress();
        yield return null;

        // アプリケーションダウンロードURL
        enumerator = GetDownload();
        coroutine = StartCoroutine(enumerator);
        yield return coroutine;
        ret = (bool)enumerator.Current;
        while (ret == false)
        {

            for (int i = 0; i < 10; i++)
            {
                WriteText(TextManager.Get(TextManager.KEY.DL_ERROR) + "..." + TextManager.Get(TextManager.KEY.DL_PLEASE)
                                            + TextManager.Get(TextManager.KEY.DL_RETRY_1) + (10 - i) + TextManager.Get(TextManager.KEY.DL_RETRY_2));
                yield return null;
                yield return new WaitForSeconds(1); //1秒待つ            }

            }
            enumerator = GetDownload();
            coroutine = StartCoroutine(enumerator);
            yield return coroutine;
            ret = (bool)enumerator.Current;
        }

        // プログレスバー更新
        ChangeProgress();
        yield return null;

        // 時刻表の取得
        enumerator = GetJikoku();
        coroutine = StartCoroutine(enumerator);
        yield return coroutine;
        ret = (bool)enumerator.Current;
        while (ret == false)
        {

            for (int i = 0; i < 10; i++)
            {
                WriteText(TextManager.Get(TextManager.KEY.DL_ERROR) + "..." + TextManager.Get(TextManager.KEY.DL_PLEASE)
                                            + TextManager.Get(TextManager.KEY.DL_RETRY_1) + (10 - i) + TextManager.Get(TextManager.KEY.DL_RETRY_2));
                yield return null;
                yield return new WaitForSeconds(1); //1秒待つ            }

            }
            enumerator = GetJikoku();
            coroutine = StartCoroutine(enumerator);
            yield return coroutine;
            ret = (bool)enumerator.Current;
        }

        // プログレスバー更新
        ChangeProgress();
        yield return null;


        // 構内図の取得
        enumerator = GetFloor();
        coroutine = StartCoroutine(enumerator);
        yield return coroutine;
        ret = (bool)enumerator.Current;
        while (ret == false)
        {

            for (int i = 0; i < 10; i++)
            {
                WriteText(TextManager.Get(TextManager.KEY.DL_ERROR) + "..." + TextManager.Get(TextManager.KEY.DL_PLEASE)
                                            + TextManager.Get(TextManager.KEY.DL_RETRY_1) + (10 - i) + TextManager.Get(TextManager.KEY.DL_RETRY_2));
                yield return null;
                yield return new WaitForSeconds(1); //1秒待つ            }

            }
            enumerator = GetFloor();
            coroutine = StartCoroutine(enumerator);
            yield return coroutine;
            ret = (bool)enumerator.Current;
        }


        // プログレスバー更新
        ChangeProgress();
        yield return null;


        // 駅看板の更新
        enumerator = GetStationPanel();
        coroutine = StartCoroutine(enumerator);
        yield return coroutine;
        ret = (bool)enumerator.Current;
        while (ret == false)
        {

            for (int i = 0; i < 10; i++)
            {
                WriteText(TextManager.Get(TextManager.KEY.DL_ERROR) + "..." + TextManager.Get(TextManager.KEY.DL_PLEASE)
                                            + TextManager.Get(TextManager.KEY.DL_RETRY_1) + (10 - i) + TextManager.Get(TextManager.KEY.DL_RETRY_2));
                yield return null;
                yield return new WaitForSeconds(1); //1秒待つ            }

            }
            enumerator = GetStationPanel();
            coroutine = StartCoroutine(enumerator);
            yield return coroutine;
            ret = (bool)enumerator.Current;
        }

        // プログレスバー更新
        ChangeProgress();
        yield return null;

        // ホーム図の更新
        enumerator = GetHome();
        coroutine = StartCoroutine(enumerator);
        yield return coroutine;
        ret = (bool)enumerator.Current;
        while (ret == false)
        {

            for (int i = 0; i < 10; i++)
            {
                WriteText(TextManager.Get(TextManager.KEY.DL_ERROR) + "..." + TextManager.Get(TextManager.KEY.DL_PLEASE)
                                            + TextManager.Get(TextManager.KEY.DL_RETRY_1) + (10 - i) + TextManager.Get(TextManager.KEY.DL_RETRY_2));
                yield return null;
                yield return new WaitForSeconds(1); //1秒待つ            }

            }
            enumerator = GetHome();
            coroutine = StartCoroutine(enumerator);
            yield return coroutine;
            ret = (bool)enumerator.Current;
        }

        // プログレスバー更新
        ChangeProgress();
        yield return null;



        // 更新の完了
        LoadComplete();
        yield return null;

        // バージョン更新
        WriteVersion();

        yield return true;

    }

   
    /// <summary>
    /// 時刻表
    /// </summary>
    /// <returns></returns>
    IEnumerator GetJikoku()
    {
        bool bret = true;
        if (jikokuflag == true)
        {
            WriteText(TextManager.Get(TextManager.KEY.DL_LOADING_1) + TextManager.Get(TextManager.KEY.JIKOKU) + TextManager.Get(TextManager.KEY.DL_LOADING_2) + "...");
            yield return null;

            DeleteSql("train_schedule_info");

            List<JikokuCsvData> JiCsv = new List<JikokuCsvData>();
            var header = AddHeader();

            url = "https://trainposinfo.shinkeisei.co.jp/ShinkeiseiMobileWeb/webresources/jp.co.shinkeisei.entity.trainschedulecsv";

            WWW trainschedulewww = new WWW(url, null, header);
            yield return trainschedulewww;

            if (trainschedulewww.error == null)
            {
                WriteText(TextManager.Get(TextManager.KEY.DL_UPDATING_1) + TextManager.Get(TextManager.KEY.JIKOKU) + TextManager.Get(TextManager.KEY.DL_UPDATING_2)
                                    + "...\n" + TextManager.Get(TextManager.KEY.DL_WHILE));
                yield return null;

                IList json = (IList)Json.Deserialize(trainschedulewww.text);
                foreach (IDictionary data in json)
                {
                    JikokuCsvData f = new JikokuCsvData();

                    f.filename = data["filename"].ToString();
                    f.csvpath = data["csvpath"].ToString();

                    JiCsv.Add(f);
                }


                //string path = "";
                string text = "";
                foreach (JikokuCsvData schedulecsv in JiCsv)
                {
                    var download = DownloadScheduleCsv(schedulecsv);
                    var coroutine = StartCoroutine(download);

                    yield return coroutine;
                    text = (string)download.Current;
                }


                // 読み込み
                List<string> liststring = new List<string>();
                StringReader csvReader = new StringReader(text);
                if (csvReader != null)
                {
                    string record;

                    while (csvReader.Peek() > -1)
                    {
                        // 一行を取り出す
                        record = csvReader.ReadLine();

                        liststring.Add(record);
                    }

                }
                else
                {
                    Debug.Log("Error!");
                    Debug.Log(trainschedulewww.error);
                    ver_jikoku = client_v.train_schedule_info;
                    bret = false;
                }
                text = "";


                query = "";
                for (int row = 0; row < liststring.Count; row++)
                {


                    string line = liststring[row];
                    if (row % 100 == 0)
                    {
                        WriteText(TextManager.Get(TextManager.KEY.DL_UPDATING_1) + TextManager.Get(TextManager.KEY.JIKOKU) + TextManager.Get(TextManager.KEY.DL_UPDATING_2)
                                    + "(" + (row + 1) + "/" + liststring.Count + ")\n" + TextManager.Get(TextManager.KEY.DL_WHILE));
                        yield return null;

                        if (query != "")
                        {
                            try
                            {
                                query += ";";
                                sqlite.ExecuteNonQuery(query);
                            }
                            catch
                            {
                                Debug.Log("Error!");
                                Debug.Log(trainschedulewww.error);
                                ver_jikoku = client_v.train_schedule_info;
                                bret = false;
                            }

                        }

                        query = "INSERT INTO train_schedule_info VALUES(" + line + ")";
                    }
                    else
                    {
                        query += ",(" + line + ")";
                    }
                }
                if (query != "")
                {
                    WriteText(TextManager.Get(TextManager.KEY.DL_UPDATING_1) + TextManager.Get(TextManager.KEY.JIKOKU) + TextManager.Get(TextManager.KEY.DL_UPDATING_2)
                                + "(" + liststring.Count + "/" + liststring.Count + ")\n" + TextManager.Get(TextManager.KEY.DL_WHILE));
                    yield return null;
                    try
                    {
                        query += ";";
                        sqlite.ExecuteNonQuery(query);
                    }
                    catch
                    {
                        Debug.Log("Error!");
                        Debug.Log(trainschedulewww.error);
                        ver_jikoku = client_v.train_schedule_info;
                        bret = false;
                    }

                }
                bret = true;
            }
            else
            {
                Debug.Log("Error!");
                Debug.Log(trainschedulewww.error);
                ver_jikoku = client_v.train_schedule_info;
                bret = false;
            }
        }
        else
        {
            bret = true;
        }
        yield return bret;
    }

    /// <summary>
    /// 構内図
    /// </summary>
    /// <returns></returns>
    IEnumerator GetFloor()
    {
        bool bret = true;
        if (floorflag == true)
        {
            Debug.Log("floor");
            WriteText(TextManager.Get(TextManager.KEY.DL_LOADING_1) + TextManager.Get(TextManager.KEY.KOUNAIZU) + TextManager.Get(TextManager.KEY.DL_LOADING_2) + "...");
            yield return null;

            List<PictureData> Floor = new List<PictureData>();
            string folderpath = Application.persistentDataPath + "/Floor";
            //string folderpath = "Assets/Resources/Sprites/Floor";

            var header = AddHeader();

            url = "https://trainposinfo.shinkeisei.co.jp/ShinkeiseiMobileWeb/webresources/jp.co.shinkeisei.entity.trainfloorinfo";

            WWW trainfloorwww = new WWW(url, null, header);
            yield return trainfloorwww;

            if (trainfloorwww.error == null)
            {
                //ChangeProgress();
                WriteText(TextManager.Get(TextManager.KEY.DL_EXPANDING_1) + TextManager.Get(TextManager.KEY.KOUNAIZU) + TextManager.Get(TextManager.KEY.DL_EXPANDING_2) + "...");
                yield return null;

                IList json = (IList)Json.Deserialize(trainfloorwww.text);
                foreach (IDictionary data in json)
                {
                    PictureData f = new PictureData();

                    f.stationid = data["stationid"].ToString();
                    f.picturepath = data["picturepath"].ToString();

                    Floor.Add(f);
                }

                if (!Directory.Exists(folderpath))
                {
                    Directory.CreateDirectory(folderpath);
#if UNITY_IPHONE
                UnityEngine.iOS.Device.SetNoBackupFlag(folderpath);
#endif
                }


                for (int row = 0; row < Floor.Count; row++)
                {
                    PictureData floor = Floor[row];

                    WriteText(TextManager.Get(TextManager.KEY.DL_UPDATING_1) + TextManager.Get(TextManager.KEY.KOUNAIZU) + TextManager.Get(TextManager.KEY.DL_UPDATING_2)
                                + "(" + (row + 1) + "/" + Floor.Count + ")\n" + TextManager.Get(TextManager.KEY.DL_WHILE));
                    yield return null;

                    string path = folderpath + "/" + floor.stationid;
                    var download = DownloadPicture(floor, path);
                    var coroutine = StartCoroutine(download);

                    yield return coroutine;
                    if ((bool)download.Current == false)
                    {
                        bret = false;
                    }

                }
            }
            else
            {
                Debug.Log("Error!");
                Debug.Log(trainfloorwww.error);
                ver_floor = client_v.train_floor_info;
                bret = false;
            }
        }
        else
        {
            bret = true;
        }
        Debug.Log("GetFloorEnd");
        yield return bret;
    }

    /// <summary>
    /// 駅案内(構内図とセットで更新)
    /// </summary>
    /// <returns></returns>
    IEnumerator GetStationPanel()
    {
        bool bret = true;
        if (floorflag == true)
        {
            Debug.Log("floor");
            WriteText(TextManager.Get(TextManager.KEY.DL_LOADING_1) + TextManager.Get(TextManager.KEY.ANNNAIZU) + TextManager.Get(TextManager.KEY.DL_LOADING_2) + "...");
            yield return null;

            List<PictureData> Floor = new List<PictureData>();
            string folderpath = Application.persistentDataPath + "/GetStationPanel";
            //string folderpath = "Assets/Resources/Sprites/Floor";

            var header = AddHeader();

            url = "https://trainposinfo.shinkeisei.co.jp/ShinkeiseiMobileWeb/webresources/jp.co.shinkeisei.entity.stationinfo";

            WWW trainfloorwww = new WWW(url, null, header);
            yield return trainfloorwww;

            if (trainfloorwww.error == null)
            {
                //ChangeProgress();
                WriteText(TextManager.Get(TextManager.KEY.DL_EXPANDING_1) + TextManager.Get(TextManager.KEY.ANNNAIZU) + TextManager.Get(TextManager.KEY.DL_EXPANDING_2) + "...");
                yield return null;

                IList json = (IList)Json.Deserialize(trainfloorwww.text);
                foreach (IDictionary data in json)
                {
                    PictureData f = new PictureData();

                    f.stationid = data["stationid"].ToString();
                    f.picturepath = data["picturepath"].ToString();

                    Floor.Add(f);
                }

                if (!Directory.Exists(folderpath))
                {
                    Directory.CreateDirectory(folderpath);
#if UNITY_IPHONE
                UnityEngine.iOS.Device.SetNoBackupFlag(folderpath);
#endif

                }


                for (int row = 0; row < Floor.Count; row++)
                {
                    PictureData floor = Floor[row];

                    WriteText(TextManager.Get(TextManager.KEY.DL_UPDATING_1) + TextManager.Get(TextManager.KEY.ANNNAIZU) + TextManager.Get(TextManager.KEY.DL_UPDATING_2)
                                + "(" + (row + 1) + "/" + Floor.Count + ")\n" + TextManager.Get(TextManager.KEY.DL_WHILE));
                    yield return null;

                    string path = folderpath + "/" + floor.stationid;
                    var download = DownloadPicture(floor, path);
                    var coroutine = StartCoroutine(download);

                    yield return coroutine;
                    if ((bool)download.Current == false)
                    {
                        bret = false;
                    }

                }
            }
            else
            {
                Debug.Log("Error!");
                Debug.Log(trainfloorwww.error);
                ver_floor = client_v.train_floor_info;
                bret = false;
            }
        }
        else
        {
            bret = true;
        }
        Debug.Log("GetFloorEnd");
        yield return bret;
    }

    /// <summary>
    /// ホーム図
    /// </summary>
    /// <returns></returns>
    IEnumerator GetHome()
    {
        bool bret = true;
        if (homeflag == true)
        {
            Debug.Log("home");
            WriteText(TextManager.Get(TextManager.KEY.DL_LOADING_1) + TextManager.Get(TextManager.KEY.HOMEZU) + TextManager.Get(TextManager.KEY.DL_LOADING_2) + "...");
            yield return null;

            List<PictureData> Home = new List<PictureData>();
            string folderpath = Application.persistentDataPath + "/Home";
            //string folderpath = "Assets/Resources/Sprites/Home";
            var header = AddHeader();

            url = "https://trainposinfo.shinkeisei.co.jp/ShinkeiseiMobileWeb/webresources/jp.co.shinkeisei.entity.trainhomeinfo";

            WWW trainhomewww = new WWW(url, null, header);
            yield return trainhomewww;

            if (trainhomewww.error == null)
            {

                WriteText(TextManager.Get(TextManager.KEY.DL_EXPANDING_1) + TextManager.Get(TextManager.KEY.HOMEZU) + TextManager.Get(TextManager.KEY.DL_EXPANDING_2) + "...");
                yield return null;

                IList json = (IList)Json.Deserialize(trainhomewww.text);
                foreach (IDictionary data in json)
                {
                    PictureData h = new PictureData();

                    h.stationid = data["stationid"].ToString();
                    h.picturepath = data["picturepath"].ToString();

                    Home.Add(h);
                }

                if (!Directory.Exists(folderpath))
                {
                    Directory.CreateDirectory(folderpath);
#if UNITY_IPHONE
                UnityEngine.iOS.Device.SetNoBackupFlag(folderpath);
#endif

                }

                for (int row = 0; row < Home.Count; row++)
                {
                    PictureData home = Home[row];
                    
                    WriteText(TextManager.Get(TextManager.KEY.DL_UPDATING_1) + TextManager.Get(TextManager.KEY.HOMEZU) + TextManager.Get(TextManager.KEY.DL_UPDATING_2)
                                + "(" + (row + 1) + "/" + Home.Count + ")\n" + TextManager.Get(TextManager.KEY.DL_WHILE));
                    yield return null;

                    string path = folderpath + "/" + home.stationid;
                    //StartCoroutine(DownloadHome(home, path));
                    var download = DownloadPicture(home, path);
                    var coroutine = StartCoroutine(download);

                    yield return coroutine;
                    if((bool)download.Current == false)
                    {
                        bret = false;
                    }
                }
            }
            else
            {
                Debug.Log("Error!");
                Debug.Log(trainhomewww.error);
                ver_home = client_v.train_home_info;
                bret = false;
            }
        }
        else
        {
            bret = true;
        }
        yield return bret;
    }



    /// <summary>
    /// ホームページURL
    /// </summary>
    /// <returns></returns>
    IEnumerator GetUrl()
    {
        bool bret = true;
        if (urlflag == true)
        {
            WriteText(TextManager.Get(TextManager.KEY.DL_LOADING_1) + TextManager.Get(TextManager.KEY.URL) + TextManager.Get(TextManager.KEY.DL_LOADING_2) + "...");
            yield return null;
            Debug.Log("url");

            DeleteSql("url_info");

            List<UrlData> Url = new List<UrlData>();
            var header = AddHeader();

            url = "https://trainposinfo.shinkeisei.co.jp/ShinkeiseiMobileWeb/webresources/jp.co.shinkeisei.entity.urlinfo";

            WWW urlwww = new WWW(url, null, header);
            yield return urlwww;

            if (urlwww.error == null)
            {
                //ChangeProgress();
                WriteText(TextManager.Get(TextManager.KEY.DL_EXPANDING_1) + TextManager.Get(TextManager.KEY.URL) + TextManager.Get(TextManager.KEY.DL_EXPANDING_2) + "...");
                yield return null;
                IList json = (IList)Json.Deserialize(urlwww.text);

                foreach (IDictionary data in json)
                {
                    UrlData u = new UrlData();

                    u.type = ParseInt(data["pagetype"]);
                    u.url = data["url"].ToString();

                    Url.Add(u);
                }

                foreach (UrlData url in Url)
                {
                    query = "INSERT INTO url_info VALUES(" + url.type + "," + MakeStr(url.url) + ");";
                    try
                    {
                        sqlite.ExecuteNonQuery(query);
                    }
                    catch
                    {
                        ver_url = client_v.url_info;
                        bret = false;
                    }
                }
            }
            else
            {
                Debug.Log("Error!");
                Debug.Log(urlwww.error);
                ver_url = client_v.url_info;
                bret = false;
            }
        }
        else
        {
            bret = true;
        }
        yield return bret;
    }

    /// <summary>
    /// アプリケーション連携URL
    /// </summary>
    /// <returns></returns>
    IEnumerator GetJoint()
    {
        bool bret = true;
        if (jointflag == true)
        {
            Debug.Log("joint");
            WriteText(TextManager.Get(TextManager.KEY.DL_LOADING_1) + TextManager.Get(TextManager.KEY.LNKURL) + TextManager.Get(TextManager.KEY.DL_LOADING_2) + "...");
            yield return null;

            DeleteSql("train_joint_info");

            List<JointData> Joint = new List<JointData>();
            var header = AddHeader();

            url = "https://trainposinfo.shinkeisei.co.jp/ShinkeiseiMobileWeb/webresources/jp.co.shinkeisei.entity.trainjointinfo";

            WWW jointwww = new WWW(url, null, header);
            yield return jointwww;

            if (jointwww.error == null)
            {
                //ChangeProgress();
                WriteText(TextManager.Get(TextManager.KEY.DL_EXPANDING_1) + TextManager.Get(TextManager.KEY.LNKURL) + TextManager.Get(TextManager.KEY.DL_EXPANDING_2) + "...");
                yield return null;
                IList json = (IList)Json.Deserialize(jointwww.text);

                foreach (IDictionary data in json)
                {
                    JointData j = new JointData();

                    IDictionary data2 = (IDictionary)data["trainJointInfoPK"];
                    j.stationid = ParseInt(data2["stationid"]);
                    j.Rid = ParseInt(data2["rid"]);
                    j.appname = data["appname"].ToString();
                    j.url = data["url"].ToString();
                    j.enableflag = ((bool)data["enableflag"] == true) ? 1 : 0;
                    j.memo = data["memo"].ToString();

                    Joint.Add(j);
                }

                foreach (JointData joint in Joint)
                {
                    var aaa = joint.url.Length;
                    query = "INSERT INTO train_joint_info VALUES("
                        + joint.stationid + "," + joint.Rid + ","
                        + MakeStr(joint.appname) + "," + MakeStr(joint.url) + ","
                        + joint.enableflag + "," + MakeStr(joint.memo) + ");";
                    try
                    {
                        sqlite.ExecuteNonQuery(query);
                    }
                    catch
                    {
                        ver_joint = client_v.train_joint_info;
                        bret = false;
                    }
                }
            }
            else
            {
                Debug.Log("Error!");
                Debug.Log(jointwww.error);
                ver_joint = client_v.train_joint_info;
                bret = false;
            }
        }
        else
        {
            bret = true;
        }
        yield return bret;
    }

    /// <summary>
    /// アプリケーションダウンロードURL
    /// </summary>
    /// <returns></returns>
    IEnumerator GetDownload()
    {
        bool bret = true;
        if (downloadflag == true)
        {
            Debug.Log("download");
            WriteText(TextManager.Get(TextManager.KEY.DL_LOADING_1) + TextManager.Get(TextManager.KEY.LNKURL) + TextManager.Get(TextManager.KEY.DL_LOADING_2) + "...");
            yield return null;

            DeleteSql("train_download_info");

            List<DownloadData> Download = new List<DownloadData>();
            var header = AddHeader();

            url = "https://trainposinfo.shinkeisei.co.jp/ShinkeiseiMobileWeb/webresources/jp.co.shinkeisei.entity.traindownloadinfo";

            WWW downloadwww = new WWW(url, null, header);
            yield return downloadwww;

            if (downloadwww.error == null)
            {
                //ChangeProgress();
                WriteText(TextManager.Get(TextManager.KEY.DL_EXPANDING_1) + TextManager.Get(TextManager.KEY.LNKURL) + TextManager.Get(TextManager.KEY.DL_EXPANDING_2) + "...");
                yield return null;
                IList json = (IList)Json.Deserialize(downloadwww.text);

                foreach (IDictionary data in json)
                {
                    DownloadData d = new DownloadData();

                    d.appname = data["appname"].ToString();
                    d.appstoreurl = data["appstoreurl"].ToString();
                    d.playstoreurl = data["playstoreurl"].ToString();

                    Download.Add(d);
                }

                foreach (DownloadData download in Download)
                {
                    query = "INSERT INTO train_download_info VALUES("
                        + MakeStr(download.appname) + "," + MakeStr(download.appstoreurl)
                        + "," + MakeStr(download.playstoreurl) + ");";
                    try
                    {
                        sqlite.ExecuteNonQuery(query);
                    }
                    catch
                    {
                        ver_download = client_v.train_download_info;
                        bret = false;
                    }
                }
            }
            else
            {
                Debug.Log("Error!");
                Debug.Log(downloadwww.error);
                ver_download = client_v.train_download_info;
                bret = false;
            }
        }
        else
        {
            bret = true;
        }
        yield return bret;
    }

    /// <summary>
    /// バージョン確認
    /// </summary>
    /// <returns></returns>
    IEnumerator GetVersion()
    {
        bool bret = false;
        List<VersionData> Version = new List<VersionData>();
        var header = AddHeader();

        url = "https://trainposinfo.shinkeisei.co.jp/ShinkeiseiMobileWeb/webresources/jp.co.shinkeisei.entity.versioninfo";
        

        WWW versionwww = new WWW(url, null, header);
        yield return versionwww;

        if (versionwww.error == null)
        {
            IList jsons = (IList)Json.Deserialize(versionwww.text);
            var json = (IDictionary)jsons[0];

            server_v = new VersionData();
            client_v = new VersionData();

            server_v.id = ParseInt(json["id"]);
            server_v.train_schedule_info = ParseInt(json["trainScheduleInfo"]);
            server_v.train_floor_info = ParseInt(json["trainFloorInfo"]);
            server_v.train_home_info = ParseInt(json["trainHomeInfo"]);
            server_v.url_info = ParseInt(json["urlInfo"]);
            server_v.train_joint_info = ParseInt(json["trainJointInfo"]);
            server_v.train_download_info = ParseInt(json["trainDownloadInfo"]);

            query = "SELECT * FROM version_info;";
            var sqlversion = sqlite.ExecuteQuery(query);
            foreach (DataRow dr in sqlversion.Rows)
            {
                client_v.id = ParseInt(dr["id"]);
                client_v.train_schedule_info = ParseInt(dr["train_schedule_info"]);
                client_v.train_floor_info = ParseInt(dr["train_floor_info"]);
                client_v.train_home_info = ParseInt(dr["train_home_info"]);
                client_v.url_info = ParseInt(dr["url_info"]);
                client_v.train_joint_info = ParseInt(dr["train_joint_info"]);
                client_v.train_download_info = ParseInt(dr["train_download_info"]);
            }

            if (server_v.train_schedule_info != client_v.train_schedule_info)
            {
                jikokuflag = true;
                bret = true;
            }
            if (server_v.train_floor_info != client_v.train_floor_info)
            {
                floorflag = true;
                bret = true;
            }
            if (server_v.train_home_info != client_v.train_home_info)
            {
                homeflag = true;
                bret = true;
            }
            if (server_v.url_info != client_v.url_info)
            {
                urlflag = true;
                bret = true;
            }
            if (server_v.train_joint_info != client_v.train_joint_info)
            {
                jointflag = true;
                bret = true;
            }
            if (server_v.train_download_info != client_v.train_download_info)
            {
                downloadflag = true;
                bret = true;
            }

            ver_jikoku = server_v.train_schedule_info;
            ver_floor = server_v.train_floor_info;
            ver_home = server_v.train_home_info;
            ver_url = server_v.url_info;
            ver_joint = server_v.train_joint_info;
            ver_download = server_v.train_download_info;
            
        }
        else
        {
            Debug.Log("Error!");
            Debug.Log(versionwww.error);
            bret = false;
        }
        yield return bret;
    }



    /// <summary>
    /// DBのバージョン更新
    /// </summary>
    public void WriteVersion()
    {
        DeleteSql("version_info");

        query = "INSERT INTO version_info VALUES("
            + server_v.id + "," + ver_jikoku + ","
            + ver_floor + "," + ver_home + ","
            + ver_url + "," + ver_joint + ","
            + ver_download + ");";
        sqlite.ExecuteNonQuery(query);
    }

    /// <summary>
    /// ヘッダーにキーを追加
    /// </summary>
    /// <returns></returns>
    private Dictionary<string, string> AddHeader()
    {
        Dictionary<string, string> hash = new Dictionary<string, string>();
        hash.Add("appkey", "PS8W2qMBYGYJLEKDpqvxWiYC");
        return hash;
    }
    private Dictionary<string, string> AddHeaderPost()
    {
        Dictionary<string, string> hash = new Dictionary<string, string>();
        hash.Add("appkey", "PS8W2qMBYGYJLEKDpqvxWiYC");
        hash.Add("Content-Type", "application/json");
        return hash;
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

    /// <summary>
    /// バーの進捗を変更
    /// </summary>
    /// <param name="progress"></param>
    private void ChangeProgress()
    {
        if (slider == null)
        {
            slider = GameObject.Find("Slider").GetComponent<Slider>();
            slider.value = 0;
        }
        slider.value += (float)0.125;
    }

    /// <summary>
    /// 時刻表csvを取得
    /// </summary>
    /// <param name="csv"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    /// 
    IEnumerator DownloadScheduleCsv(JikokuCsvData schedulecsv)
    {
        //var url = schedulecsv.csvpath.Replace("8080", "8084");
        string text = "";
        using (WWW www = new WWW(schedulecsv.csvpath))
        {
            yield return www;

            if (www.error == null)
            {
                text = www.text;
            }

        }

        yield return text;
    }

    /// <summary>
    /// 構内図画像を取得
    /// </summary>
    /// <param name="floor"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    IEnumerator DownloadPicture(PictureData data, string path)
    {
        bool ret = true;
        Debug.Log(path);
        if (File.Exists(path) == true)
        {
            try
            {
                File.Delete(path);
            }
            catch
            {
                Debug.Log("Error!");
            }
        }

        using (WWW www = new WWW(data.picturepath))
        {
            yield return www;

            if (www.error == null)
            {
                File.WriteAllBytes(path, www.bytes);
#if UNITY_IPHONE
                UnityEngine.iOS.Device.SetNoBackupFlag(path);
#endif
            }
            else
            {
                ret = false;
                Debug.Log("Error!");
                Debug.Log(www.error);
            }
        }
        yield return ret;
    }

    private void WriteText(string str)
    {
        Text text = GameObject.Find("loadtext").GetComponent<Text>();
        text.text = str;
    }

    /// <summary>
    /// ダウンロード完了
    /// </summary>
    public void LoadComplete()
    {
        WriteText(TextManager.Get(TextManager.KEY.DL_COMPLETE));
        slider.value = 1;
    }

    /// <summary>
    /// 文字列を''で囲む
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private string MakeStr(string str)
    {
        return "'" + str + "'";
    }

    private string Makepath(string path)
    {
        string sep = "/Assets/";
        string temp = path.Remove(0, path.IndexOf(sep) + sep.Length);
        temp = "Assets/" + temp;
        return temp;
    }

    /// <summary>
    /// データを削除
    /// </summary>
    /// <param name="name"></param>
    private void DeleteSql(string name)
    {
        query = "DELETE FROM " + name + ";";
        sqlite.ExecuteNonQuery(query);
    }

    /*以下、ダウンロードデータの一時保存クラス*/

    class JikokuData
    {
        public int stationid;
        public int daytype;
        public int direction;
        public int hour;
        public int minute;
        public int destination;
    }

    class JikokuCsvData
    {
        public string csvpath;
        public string filename;
    }

    class PictureData
    {
        public string stationid;
        public string picturepath;
    }
    /*
    class FloorData
    {
        public string stationid;
        public string picturepath;
    }

    class HomeData
    {
        public string stationid;
        public string picturepath;
    }
    */
    class UrlData
    {
        public int type;
        public string url;
    }

    class JointData
    {
        public int stationid;
        public int Rid;
        public string appname;
        public string url;
        public int enableflag;
        public string memo;
    }

    class DownloadData
    {
        public string appname;
        public string appstoreurl;
        public string playstoreurl;
    }

    class VersionData
    {
        public int id;
        public int train_schedule_info;
        public int train_floor_info;
        public int train_home_info;
        public int url_info;
        public int train_joint_info;
        public int train_download_info;
    }
}
