using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Compression;
using UnityEditor;

public class Loading : MonoBehaviour {
    
    private TestRest tr;
    private GameObject dialog;

    // Use this for initialization
    void Start ()
    {
        TextManager.Init();

        WriteText();
        
        dialog = GameObject.Find("dialog");
        dialog.SetActive(false);
        tr = gameObject.GetComponent<TestRest>();

        StartCoroutine(CheckVersion());

        
    }
	
	// Update is called once per frame
	void Update () {

	}

    IEnumerator CheckVersion()
    {

        yield return null;

        //InitPict();

        var checkver = tr.CheckVersion();
        var coroutine_checkver = StartCoroutine(checkver);
        yield return coroutine_checkver;
        bool flag = (bool)checkver.Current;
        if (flag)
        {
            dialog.SetActive(true);
            yield return false;
        }
        else
        {
            StartCoroutine(LoadNextScene(false));
        }
        yield return true;
    }
    IEnumerator LoadNextScene(bool getflag)
    {
        if (getflag)
        {
            var getdata = tr.GetData();
            var coroutine_getdata = StartCoroutine(getdata);
            yield return coroutine_getdata;
        }
            
        var sc = gameObject.GetComponent<SceneController>();
        Static.initFlag = true;

        // 連携の確認
        if (Application.platform == RuntimePlatform.Android)
        {
            sc.AndroidCustomUrlLinkCheck();
        }

        if (Static.JointFlag == "true")
        {
            sc.GoStart();
        }
        else
        {
            sc.TopClick(); 
        }
        
    }

    public void ClickDialog(bool download)
    {
        dialog.SetActive(false);
        StartCoroutine(LoadNextScene(download));
    }

    private void WriteText()
    {
        Text text = GameObject.Find("loadtext").GetComponent<Text>();
        text.text = TextManager.Get(TextManager.KEY.DL_VERCHK);

        Text dialogtext = GameObject.Find("dialog").transform.Find("Text").GetComponent<Text>();
        dialogtext.text = TextManager.Get(TextManager.KEY.DL_CHK_1) + "7.5MB" + TextManager.Get(TextManager.KEY.DL_CHK_2)
                            + "\n" + TextManager.Get(TextManager.KEY.DL_SURE);

        Text yestext = GameObject.Find("yes").GetComponentInChildren<Text>();
        yestext.text = TextManager.Get(TextManager.KEY.YES);

        Text notext = GameObject.Find("no").GetComponentInChildren<Text>();
        notext.text = TextManager.Get(TextManager.KEY.NO);
    }
    /*
    public void InitPict()
    {
        DirCopyFromAssetsToPersistent("Floor");
        DirCopyFromAssetsToPersistent("Home");
        DirCopyFromAssetsToPersistent("GetStationPanel");
    }
    
    private void DirCopyFromAssetsToPersistent(string foldername)
    {
        string distpath = Application.persistentDataPath + "/" + foldername;
        if (!Directory.Exists(distpath))
        {
            Directory.CreateDirectory(distpath);
#if UNITY_IPHONE
                UnityEngine.iOS.Device.SetNoBackupFlag(distpath);
#endif
        }

        CopyAsset(foldername, "01");
        CopyAsset(foldername, "02");
        CopyAsset(foldername, "03");
        CopyAsset(foldername, "04");
        CopyAsset(foldername, "05");
        CopyAsset(foldername, "06");
        CopyAsset(foldername, "07");
        CopyAsset(foldername, "08");
        CopyAsset(foldername, "09");
        CopyAsset(foldername, "10");
        CopyAsset(foldername, "11");
        CopyAsset(foldername, "12");
        CopyAsset(foldername, "13");
        CopyAsset(foldername, "14");
        CopyAsset(foldername, "15");
        CopyAsset(foldername, "16");
        CopyAsset(foldername, "17");
        CopyAsset(foldername, "18");
        CopyAsset(foldername, "19");
        CopyAsset(foldername, "20");
        CopyAsset(foldername, "21");
        CopyAsset(foldername, "22");
        CopyAsset(foldername, "23");
        CopyAsset(foldername, "24");

    }

    void CopyAsset(string foldername,string filename)
    {
        string sorcepath = "Assets/" + foldername + "/" + filename;
        string distpath = Application.persistentDataPath + "/" + foldername + "/" + filename;
        if (System.IO.File.Exists(distpath) == false)
        {

            File.Copy(sorcepath, distpath);
#if UNITY_IPHONE
                UnityEngine.iOS.Device.SetNoBackupFlag(distpath);
#endif
        }

    }*/

}
