using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JikokuControll : MonoBehaviour
{
    private List<JikokuData> jikoku;

    private int direction;
    private int daytype;

    private string ikisaki_kudari = "京成津田沼行　　新津田沼行 　　くぬぎ山行 　　千葉中央行";
    private string ikisaki_nobori = "　　松戸行　　　新津田沼行 　　くぬぎ山行 　　千葉中央行";

    // Use this for initialization
    void Start()
    {
        WriteText();

        if (Static.StationNo == null)
        {
            Static.StationNo = "01";
        }

        if (Static.StationNo == "01")
        {
            Static.JikokuNobori = false;
            Button button = GameObject.Find("NoboriBtn").GetComponent<Button>();
            button.interactable = false;
        }
        else if (Static.StationNo == "24")
        {
            Button button = GameObject.Find("KudariBtn").GetComponent<Button>();
            button.interactable = false;
        }

        SetStatus();
        OpeButton();

        GetEki();
        GetJikoku();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GetEki()
    {
        try
        {
            var text = gameObject.transform.Find("attention").gameObject.transform.Find("Text").GetComponent<Text>();
            text.text = Static.getEkiName();
        }
        catch
        {

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

    private void SetStatus()
    {
        Text ikisaki = GameObject.Find("IkisakiText").GetComponent<Text>();

        if (Static.JikokuNobori)
        {
            direction = 0;
            ikisaki.text = ikisaki_nobori;
        }
        else
        {
            direction = 1;
            ikisaki.text = ikisaki_kudari;
        }

        if (Static.JikokuHoliday)
        {
            daytype = 1;
        }
        else
        {
            daytype = 0;
        }
    }

    public void GetJikoku()
    {
        jikoku = new List<JikokuData>();

        SqliteDatabase sqlite = new SqliteDatabase("shinkeisei.db");
        string query = "SELECT * FROM train_schedule_info WHERE stationid = " + Static.StationNo
            +  " and daytype = " + daytype + " and direction = " + direction + ";";
        var response = sqlite.ExecuteQuery(query);

        foreach(DataRow data in response.Rows)
        {
            JikokuData j = new JikokuData();

            j.hour = (int)data["hour"];
            j.minute = (int)data["minute"];
            j.destination = (int)data["destination"];

            jikoku.Add(j);
        }
        
        foreach(JikokuData j in jikoku)
        {
            string element_name = "minutetext_" + j.hour;
            Transform jikoku = GameObject.Find(element_name).transform;

            string path = "Prefab/jikoku/" + j.destination;
            GameObject obj = (GameObject)Resources.Load(path);

            GameObject newobj = (GameObject)Instantiate(obj, jikoku);
            newobj.name = String.Format("{0:D2}", j.minute);
            Text text = newobj.transform.Find("Text").GetComponent<Text>();
            text.text = String.Format("{0:D2}", j.minute);
            
            Vector2 pos = new Vector2(newobj.transform.localPosition.x, newobj.transform.localPosition.y);
            pos.x += newobj.GetComponent<RectTransform>().sizeDelta.x * (float)1.2 * (jikoku.childCount - 1);
            newobj.transform.localPosition = pos;
        }
    }

    public void ToggleDirection()
    {
        Static.JikokuNobori = !Static.JikokuNobori;
        SceneManager.LoadScene("jikoku");
    }

    public void ToggleHoliday()
    {
        Static.JikokuHoliday = !Static.JikokuHoliday;
        SceneManager.LoadScene("jikoku");
    }

    private void OpeButton()
    {
        string name;

        if (Static.JikokuHoliday)
        {
            name = "heijitsu_panel";
        }
        else
        {
            name = "kyujitsu_panel";
        }
        var obj = GameObject.Find(name);
        obj.SetActive(false);

        if (Static.JikokuNobori)
        {
            name = "kudari_panel";
        }
        else
        {
            name = "nobori_panel";
        }
        obj = GameObject.Find(name);
        obj.SetActive(false);
    }

    private void WriteText()
    {
        Text title = GameObject.Find("title").GetComponentInChildren<Text>();
        title.text = TextManager.Get(TextManager.KEY.JIKOKU);

        Text heijitu_btn = GameObject.Find("top").transform.Find("heijitsu").transform.Find("heijitu").GetComponent<Text>();
        heijitu_btn.text = TextManager.Get(TextManager.KEY.JIKOKU_HEIJITU_1);
        Text heidetial_btn = GameObject.Find("top").transform.Find("heijitsu").transform.Find("syousai").GetComponentInChildren<Text>();
        heidetial_btn.text = TextManager.Get(TextManager.KEY.JIKOKU_HEIJITU_2);

        Text heijitu_pan = GameObject.Find("heijitsu_panel").transform.Find("heijitu").GetComponent<Text>();
        heijitu_pan.text = TextManager.Get(TextManager.KEY.JIKOKU_HEIJITU_1);
        Text heidetial_pan = GameObject.Find("heijitsu_panel").transform.Find("syousai").GetComponentInChildren<Text>();
        heidetial_pan.text = TextManager.Get(TextManager.KEY.JIKOKU_HEIJITU_2);

        Text kyujitu_btn = GameObject.Find("top").transform.Find("kyujitsu").transform.Find("kyujitu").GetComponent<Text>();
        kyujitu_btn.text = TextManager.Get(TextManager.KEY.JIKOKU_KYUJITU_1);
        Text kyudetial_btn = GameObject.Find("top").transform.Find("kyujitsu").transform.Find("syousai").GetComponentInChildren<Text>();
        kyudetial_btn.text = TextManager.Get(TextManager.KEY.JIKOKU_KYUJITU_2);

        Text kyujitu_pan = GameObject.Find("kyujitsu_panel").transform.Find("kyujitu").GetComponent<Text>();
        kyujitu_pan.text = TextManager.Get(TextManager.KEY.JIKOKU_KYUJITU_1);
        Text kyudetial_pan = GameObject.Find("kyujitsu_panel").transform.Find("syousai").GetComponentInChildren<Text>();
        kyudetial_pan.text = TextManager.Get(TextManager.KEY.JIKOKU_KYUJITU_2);

        Text nobori_btn_hou = GameObject.Find("NoboriBtn").transform.Find("houkou").GetComponent<Text>();
        nobori_btn_hou.text = TextManager.Get(TextManager.KEY.TO_MATSUDO);
        Text nobori_btn = GameObject.Find("NoboriBtn").transform.Find("nobori").GetComponent<Text>();
        nobori_btn.text = TextManager.Get(TextManager.KEY.NOBORI);

        Text nobori_pan_hou = GameObject.Find("nobori_panel").transform.Find("houkou").GetComponent<Text>();
        nobori_pan_hou.text = TextManager.Get(TextManager.KEY.TO_MATSUDO);
        Text nobori_pan = GameObject.Find("nobori_panel").transform.Find("nobori").GetComponent<Text>();
        nobori_pan.text = TextManager.Get(TextManager.KEY.NOBORI);

        Text kudari_btn_hou = GameObject.Find("KudariBtn").transform.Find("houkou").GetComponent<Text>();
        kudari_btn_hou.text = TextManager.Get(TextManager.KEY.TO_TSUDANUMA);
        Text kudari_btn = GameObject.Find("KudariBtn").transform.Find("kudari").GetComponent<Text>();
        kudari_btn.text = TextManager.Get(TextManager.KEY.KUDARI);

        Text kudari_pan_hou = GameObject.Find("kudari_panel").transform.Find("houkou").GetComponent<Text>();
        kudari_pan_hou.text = TextManager.Get(TextManager.KEY.TO_TSUDANUMA);
        Text kudari_pan = GameObject.Find("kudari_panel").transform.Find("kudari").GetComponent<Text>();
        kudari_pan.text = TextManager.Get(TextManager.KEY.KUDARI);

        Text soukou = GameObject.Find("soukou").GetComponentInChildren<Text>();
        soukou.text = TextManager.Get(TextManager.KEY.MENU_SOUKOU);

        Text jikoku = GameObject.Find("jikokuText").GetComponentInChildren<Text>();
        jikoku.text = TextManager.Get(TextManager.KEY.JIKOKU);

        Text ekiannai = GameObject.Find("kounai").GetComponentInChildren<Text>();
        ekiannai.text = TextManager.Get(TextManager.KEY.MENU_EKIANNAI);
    }

    class JikokuData
    {
        public int hour;
        public int minute;
        public int destination;
    }
}
