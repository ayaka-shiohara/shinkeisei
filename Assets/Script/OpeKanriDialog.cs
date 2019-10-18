using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OpeKanriDialog : MonoBehaviour
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
        StartCoroutine(GetKanri());
    }

    // Update is called once per frame
    void Update()
    {
        Start();
    }
    
    public void CloseDialog()
    {
        this.gameObject.SetActive(false);
    }
    
    IEnumerator GetKanri()
    {
        string url = "http://tech.ttc-net.co.jp/ShinkeiseiMobileWeb/webresources/jp.co.shinkeisei.entity.serviceinfo";
        WWW kanriwww = new WWW(url);
        yield return kanriwww;

        string kanriText = kanriwww.text;
        //kanriText = "お知らせテストお知らせテストお知らせテストお知らせテスト";

        if (kanriText != "")
        {
            this.gameObject.SetActive(true);

            Text text = GameObject.Find("kanriText").GetComponent<Text>();
            text.text = kanriText;
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    private void WriteText()
    {
        Text kanrititle = GameObject.Find("kanriTitle").GetComponent<Text>();
        kanrititle.text = TextManager.Get(TextManager.KEY.KANRI_TITLE);
        
        Text close = GameObject.Find("kanriButton").GetComponentInChildren<Text>();
        close.text = TextManager.Get(TextManager.KEY.UNKOU_CLOSE);
    }
}
