using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OpeDialog : MonoBehaviour
{

    private string number;
    private string Kaigyou = "\n";
    private Canvas can;
    private string[] texts;

    private string before = "<body>";
    private string after = "</body>";
    private string br = "<br />";

    // Use this for initialization
    void Start()
    {
        WriteText();

        this.gameObject.SetActive(false);
        OpeBase(false);
    }

    // Update is called once per frame
    void Update()
    {
        WriteText();
    }

    public void OpenTopDialog()
    {
        gameObject.SetActive(true);
        OpeBase(true);
        StartCoroutine(ChangeText());
    }
    
    public void CloseDialog()
    {
        this.gameObject.SetActive(false);
        OpeBase(false);
    }

    void OpeBase(bool update)
    {
        Canvas bases = gameObject.transform.parent.gameObject.transform.Find("dialogbase").GetComponent<Canvas>();
        bases.gameObject.SetActive(update);
    }

    IEnumerator ChangeText()
    {
        char atmark = '@';
        string https = "https://";

        Text infotext = GameObject.Find("setsumei").GetComponent<Text>();

        SqliteDatabase sqlite = new SqliteDatabase("shinkeisei.db");
        string query = "SELECT * FROM url_info WHERE type = 2";
        var response = sqlite.ExecuteQuery(query);
        string url = response.Rows[0]["url"].ToString();

        WWW www = new WWW(url);
        yield return www;

        var text = MakeText(www.text);
        infotext.text = text;
    }

    private string MakeText(string original)
    {
        string temp = original.Replace(br, "");
        return temp;
    }

    private void WriteText()
    {
        Text hyoudai = GameObject.Find("hyoudai").GetComponent<Text>();
        hyoudai.text = TextManager.Get(TextManager.KEY.UNKOU_TITLE);

        Text close = GameObject.Find("close").GetComponentInChildren<Text>();
        close.text = TextManager.Get(TextManager.KEY.UNKOU_CLOSE);
    }
}
