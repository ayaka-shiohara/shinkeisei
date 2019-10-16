using MiniJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class StationController : MonoBehaviour {

    private float timeOut = 15;
    private float timeElapsed;

    private SqliteDatabase sqlite;
    private string query;

    private List<PositionData> position;

    private bool setposflag = true;
    //private float scrollfloat = 0.04167f;
   
    private float scrollfloat = 0.04347f;

    // Use this for initialization
    void Start () {
        WriteText();
        if (Static.StationNo == null)
        {
            Static.StationNo = "01";
        }
        GetEki();

        SetPos();
        sqlite = new SqliteDatabase("shinkeisei.db");

        StartCoroutine(LoadNextScene());
    }
    void GetEki()
    {
        SqliteDatabase sqlite = new SqliteDatabase("shinkeisei.db");

        Image eki = gameObject.transform.Find("ekidata").gameObject.transform.Find("eki").GetComponent<Image>();
        /*
        string path = "Sprites/station/" + Static.StationNo;
        Sprite ekisprite = Resources.Load<Sprite>(path);
        */
        /*
        string imagepath = Application.persistentDataPath + "/GetStationPanel/" + Static.StationNo;
        if (System.IO.File.Exists(imagepath) == false)
        {

            string path = "GetStationPanel/" + Static.StationNo;
            Sprite sprite = Resources.Load<Sprite>(path);
            eki.sprite = sprite;

        }
        else
        {
            Sprite ekisprite = null;
            Texture2D texture = Texture2DFromFile(imagepath);
            if (texture)
            {
                //Texture2DからSprite作成
                ekisprite = SpriteFromTexture2D(texture);
            }
            texture = null;
            eki.sprite = ekisprite;
        }*/
        //string imagepath = Application.persistentDataPath + "/GetStationPanel/" + Static.StationNo;
        //if (!File.Exists(imagepath))
        //{
        //    Debug.Log(imagepath + " is not found.");
        //    imagepath = "Assets/GetStationPanel/" + Static.StationNo;
        //}
        //Sprite ekisprite = null;
        //Texture2D texture = Texture2DFromFile(imagepath);
        //if (texture)
        //{
        //    //Texture2DからSprite作成
        //    ekisprite = SpriteFromTexture2D(texture);
        //}
        //texture = null;
        //eki.sprite = ekisprite;
        string imagepath = Application.persistentDataPath + "/GetStationPanel/" + Static.StationNo;
        if (File.Exists(imagepath) == false)
        {
            string path = "GetStationPanel/" + Static.StationNo;
            Sprite sprite = Resources.Load<Sprite>(path);
            eki.sprite = sprite;
        }
        else
        {
            Sprite ekisprite = null;
            Texture2D texture = Texture2DFromFile(imagepath);
            if (texture)
            {
                //Texture2DからSprite作成
                ekisprite = SpriteFromTexture2D(texture);
            }
            texture = null;
            eki.sprite = ekisprite;
        }

    }
    public Texture2D Texture2DFromFile(string path)
    {
        Texture2D texture = null;
        if (File.Exists(path))
        {
            //byte取得
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader bin = new BinaryReader(fileStream);
            byte[] readBinary = bin.ReadBytes((int)bin.BaseStream.Length);
            bin.Close();
            fileStream.Dispose();
            fileStream = null;
            if (readBinary != null)
            {
                //横サイズ
                int pos = 16;
                int width = 0;
                for (int i = 0; i < 4; i++)
                {
                    width = width * 256 + readBinary[pos++];
                }
                //縦サイズ
                int height = 0;
                for (int i = 0; i < 4; i++)
                {
                    height = height * 256 + readBinary[pos++];
                }
                //byteからTexture2D作成
                texture = new Texture2D(width, height);
                texture.LoadImage(readBinary);
            }
            readBinary = null;
        }
        return texture;
    }
    public Sprite SpriteFromTexture2D(Texture2D texture)
    {
        Sprite sprite = null;
        if (texture)
        {
            //Texture2DからSprite作成
            sprite = Sprite.Create(texture, new UnityEngine.Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
        return sprite;
    }
    IEnumerator LoadNextScene()
    {
        var train = GetTrain();
        var coroutine_train = StartCoroutine(train);
        yield return coroutine_train;
    }

    // Update is called once per frame
    void Update () {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= timeOut)
        {
            timeElapsed = 0;
            StartCoroutine(GetTrain());
        }
    }

    /// <summary>
    /// 列車情報をダウンロードして更新
    /// </summary>
    /// <returns></returns>
    IEnumerator GetTrain()
    {
        position = new List<PositionData>();
        //yield return StartCoroutine(DownloadUnkou());
        var download = DownloadUnkou();
        var coroutine_download = StartCoroutine(download);
        yield return coroutine_download;

        //StartCoroutine(GetUnkou());
        var unkou = GetUnkou();
        var coroutine_unkou = StartCoroutine(unkou);
        yield return coroutine_unkou;

        yield return true;
    }

    /// <summary>
    /// 運行情報をダウンロード
    /// </summary>
    /// <returns></returns>
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

        yield return true;
    }

    /// <summary>
    /// 列車情報を画面に表示
    /// </summary>
    /// <returns></returns>
    IEnumerator GetUnkou()
    {
        var box = GameObject.Find("TrainBox").GetComponent<Canvas>().transform;
        string name;
        GameObject obj;

        foreach (PositionData pd in position)
        {
            name = "sec" + pd.sectionid;
            obj = box.Find(name).gameObject;
            obj.SetActive(true);
        }
        
        yield return null;
    }

    private void DestroyTrain()
    {
        var box = GameObject.Find("TrainBox").GetComponent<Canvas>().transform;

        foreach (Transform train in box)
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

    public void ChangeStation()
    {
        ScrollRect train = GameObject.Find("Train").GetComponent<ScrollRect>();
        ScrollRect station = GameObject.Find("Rosenzu").GetComponent<ScrollRect>();
        
        train.verticalNormalizedPosition = station.verticalNormalizedPosition;
    }

    public void SetPos()
    {
        var scroll = GameObject.Find("Rosenzu").GetComponent<ScrollRect>();
        //float pos = (float.Parse(Static.StationNo) > 9) ? float.Parse(Static.StationNo) : float.Parse(Static.StationNo);
        float pos = float.Parse(Static.StationNo) - 1;

        scroll.verticalNormalizedPosition -= pos * scrollfloat;
    }

    public void WriteText()
    {
        Text soukou = GameObject.Find("soukouichi").GetComponentInChildren<Text>();
        soukou.text = TextManager.Get(TextManager.KEY.MENU_SOUKOU);

        Text jikoku = GameObject.Find("jikokuhyo").GetComponentInChildren<Text>();
        jikoku.text = TextManager.Get(TextManager.KEY.JIKOKU);

        Text ekiannai = GameObject.Find("kounai").GetComponentInChildren<Text>();
        ekiannai.text = TextManager.Get(TextManager.KEY.MENU_EKIANNAI);
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
