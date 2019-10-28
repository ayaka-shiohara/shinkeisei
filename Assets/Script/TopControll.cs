using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TopControll : MonoBehaviour
{

    ScrollRect scrollrect;
    int count;
    int length;

    private string before = "<body>";
    private string after = "</body>";
    private string br = "<br />";

    private GameObject red;
    private GameObject yellow;
    private GameObject green;
    private GameObject syousai;

    private GameObject text1;
    private GameObject text2;
    private GameObject text3;
    private GameObject text4;

    // Use this for initialization
    void Start()
    {
        WriteText();

        SqliteDatabase sqlite = new SqliteDatabase("shinkeisei.db");
        string query = "SELECT * FROM user_info;";
        var response = sqlite.ExecuteQuery(query);

        Static.FavoStation = response.Rows[0]["station"].ToString();
        if (Static.FavoStation != "")
        {
            Static.StationNo = Static.FavoStation;
        }
        else
        {
            Static.StationNo = "01";
        }

        scrollrect = GameObject.Find("Scroll").GetComponent<ScrollRect>();
        scrollrect.horizontalNormalizedPosition = 0;
        count = 0;
        length = 1;
        StartCoroutine(ChangeInfo());

        Dropdown langdrop = GameObject.Find("LangDrop").GetComponent<Dropdown>();
        langdrop.value = (int)response.Rows[0]["language"];

        StartCoroutine(CheckDialog());
    }

    // Update is called once per frame
    void Update()
    {
        count++;
        if (count >= length)
        {
            if (scrollrect.horizontalNormalizedPosition < 0.998)
            {
                scrollrect.horizontalNormalizedPosition += (float)0.003;
            }
            else
            {
                scrollrect.horizontalNormalizedPosition = 0;
            }
            count = 0;
        }
    }

    IEnumerator ChangeInfo()
    {
        Text infotext = GameObject.Find("InfoText").GetComponent<Text>();

        SqliteDatabase sqlite = new SqliteDatabase("shinkeisei.db");
        string query = "SELECT * FROM url_info WHERE type = 2";
        var response = sqlite.ExecuteQuery(query);
        string url = response.Rows[0]["url"].ToString();

        WWW www = new WWW(url);
        yield return www;

        var text = MakeText(www.text);
        ChangeTextSize(text);
        infotext.text = text;
    }

    private string MakeText(string original)
    {
        string[] del = { "<br />" };

        string[] temps = original.Split(del, StringSplitOptions.None);
        string temp = temps[1];
        return temp;
    }

    public void Reload()
    {
        Start();
    }

    public void changeLang()
    {
        Dropdown langdrop = GameObject.Find("LangDrop").GetComponent<Dropdown>();

        SqliteDatabase sqlite = new SqliteDatabase("shinkeisei.db");
        string sql = "UPDATE user_info SET language = " + langdrop.value;
        sqlite.ExecuteNonQuery(sql);
        
        TextManager.Init();
        Start();
    }

    private void WriteText()
    {
        Text unkou = GameObject.Find("hyoudai").GetComponent<Text>();
        unkou.text = TextManager.Get(TextManager.KEY.UNKOU_TITLE);
        
        Text soukou = GameObject.Find("soukouichi").GetComponentInChildren<Text>();
        soukou.text = TextManager.Get(TextManager.KEY.MENU_SOUKOU);

        Text jikoku = GameObject.Find("kakueki").GetComponentInChildren<Text>();
        jikoku.text = TextManager.Get(TextManager.KEY.MENU_EKIINFO_1);

        /*Text ekiinfo = GameObject.Find("kounaiannai").GetComponentInChildren<Text>();
        ekiinfo.text = TextManager.Get(TextManager.KEY.MENU_EKIANNAI);*/
    }

    private void ChangeTextSize(string text)
    {
        RectTransform rect = GameObject.Find("InfoText").GetComponent<RectTransform>();
        RectTransform rectsc = GameObject.Find("Content").GetComponent<RectTransform>();

        int textlength = text.Length * 50;
        int anchore = length / 2 - 100;
        length = text.Length / 20;

        rect.sizeDelta = new Vector2(textlength, rect.sizeDelta.y);
        rectsc.sizeDelta = new Vector2(textlength + 50, rectsc.sizeDelta.y);

    }

    IEnumerator CheckDialog()
    {
        string url = "http://tech.ttc-net.co.jp/ShinkeiseiMobileWeb/webresources/jp.co.shinkeisei.entity.serviceinfo";
        WWW kanriwww = new WWW(url);
        yield return kanriwww;

        string kanriText = kanriwww.text;
        //kanriText = "お知らせテストお知らせテストお知らせテストお知らせテスト";

        if (kanriText != "")
        {
            GameObject dialog = GameObject.Find("Canvas").transform.Find("kanriDialog").gameObject;
            dialog.SetActive(true);
        }
    }
}
