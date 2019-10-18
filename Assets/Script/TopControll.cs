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
        if (red == null)
        {
            red = GameObject.Find("red");
            yellow = GameObject.Find("yellow");
            green = GameObject.Find("green");
            syousai = GameObject.Find("syousai");

            text1 = GameObject.Find("Text1");
            text2 = GameObject.Find("Text2");
            text3 = GameObject.Find("Text3");
            text4 = GameObject.Find("Text4");
        }
        
        red.SetActive(true);
        yellow.SetActive(true);
        green.SetActive(true);
        syousai.SetActive(true);
        
        text1.SetActive(true);
        text2.SetActive(true);
        text3.SetActive(true);
        text4.SetActive(true);

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
        StartCoroutine(ChangeInfo());

        Dropdown langdrop = GameObject.Find("LangDrop").GetComponent<Dropdown>();
        langdrop.value = (int)response.Rows[0]["language"];
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ChangeInfo()
    {
        //Text infotext = GameObject.Find("InfoText").GetComponent<Text>();

        SqliteDatabase sqlite = new SqliteDatabase("shinkeisei.db");
        string query = "SELECT * FROM url_info WHERE type = 2";
        var response = sqlite.ExecuteQuery(query);
        string url = response.Rows[0]["url"].ToString();

        WWW www = new WWW(url);
        yield return www;

        var text = MakeText(www.text);
        changeButton(text);
    }

    private string MakeText(string original)
    {
        string temp = "      " + original.Replace(br, "");
        return temp;
    }

    public void Reload()
    {
        Start();
    }

    private void changeButton(string text)
    {
        if (text.Contains("新京成線は、ただいま運転を見合わせています。") || text.Contains("【新京成線 全線運転見合わせ】") || text.Contains("【新京成線 一部区間運転見合わせ】"))
        {
            yellow.SetActive(false);
            green.SetActive(false);
        }
        else if (text.Contains("新京成線は、平常通り運転しています。"))
        {
            syousai.SetActive(false);
        }
        else
        {
            green.SetActive(false);

            GameObject text1 = GameObject.Find("Text1");
            GameObject text2 = GameObject.Find("Text2");
            GameObject text3 = GameObject.Find("Text3");
            GameObject text4 = GameObject.Find("Text4");

            if (text.Contains("【新京成線 遅延】"))
            {
                text2.SetActive(false);
                text3.SetActive(false);
                text4.SetActive(false);
            }
            else if (text.Contains("【新京成線 一部列車運休】"))
            {
                text1.SetActive(false);
                text3.SetActive(false);
                text4.SetActive(false);
            }
            else if (text.Contains("【新京成線 直通運転中止】"))
            {
                text1.SetActive(false);
                text2.SetActive(false);
                text4.SetActive(false);
            }
            else
            {
                text1.SetActive(false);
                text2.SetActive(false);
                text3.SetActive(false);
            }
        }
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
        Text redtext = GameObject.Find("red").GetComponentInChildren<Text>();
        redtext.text = TextManager.Get(TextManager.KEY.INFO_STOP);
        Text yellow1 = GameObject.Find("Text1").GetComponent<Text>();
        yellow1.text = TextManager.Get(TextManager.KEY.INFO_DELAY);
        Text yellow2 = GameObject.Find("Text2").GetComponent<Text>();
        yellow2.text = TextManager.Get(TextManager.KEY.INFO_UNKYU);
        Text yellow3 = GameObject.Find("Text3").GetComponent<Text>();
        yellow3.text = TextManager.Get(TextManager.KEY.INFO_TYOKUTU);
        Text yellow4 = GameObject.Find("Text4").GetComponent<Text>();
        yellow4.text = TextManager.Get(TextManager.KEY.INFO_INFO);
        Text greentext = GameObject.Find("green").GetComponentInChildren<Text>();
        greentext.text = TextManager.Get(TextManager.KEY.INFO_NORMAL);
        Text syousai = GameObject.Find("syousai").GetComponent<Text>();
        syousai.text = TextManager.Get(TextManager.KEY.INFO_DETAIL);

        Text soukou = GameObject.Find("button2").GetComponentInChildren<Text>();
        soukou.text = TextManager.Get(TextManager.KEY.MENU_SOUKOU);

        Text ekiinfo = GameObject.Find("button3").GetComponentInChildren<Text>();
        ekiinfo.text = TextManager.Get(TextManager.KEY.MENU_EKIINFO_1) + "\n(" + TextManager.Get(TextManager.KEY.MENU_EKIINFO_2) + ")";
        
        Text homepage = GameObject.Find("button4").GetComponentInChildren<Text>();
        homepage.text = TextManager.Get(TextManager.KEY.MENU_HOMEPAGE);
    }
}
