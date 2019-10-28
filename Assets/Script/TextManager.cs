using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 文字列管理クラス(多言語対応)
/// </summary>
public static class TextManager
{
    // 文字列格納、検索用ディクショナリー
    private static Dictionary<string, string> sDictionary = new Dictionary<string, string>();

    /// <summary>
    /// 検索キー
    /// </summary>
    public enum KEY
    {
        DL_CHK_1,
        DL_CHK_2,
        DL_SURE,
        YES,
        NO,
        DL_VERCHK,
        DL_LOADING_1,
        DL_LOADING_2,
        DL_EXPANDING_1,
        DL_EXPANDING_2,
        DL_UPDATING_1,
        DL_UPDATING_2,
        DL_COMPLETE,
        DL_WHILE,
        DL_ERROR,
        DL_PLEASE,
        DL_RETRY_1,
        DL_RETRY_2,
        JIKOKU,
        KOUNAIZU,
        ANNNAIZU,
        HOMEZU,
        URL,
        LNKURL,
        INFO_STOP,
        INFO_DELAY,
        INFO_UNKYU,
        INFO_TYOKUTU,
        INFO_INFO,
        INFO_NORMAL,
        INFO_DETAIL,
        MENU_SOUKOU,
        MENU_EKIINFO_1,
        MENU_EKIINFO_2,
        MENU_HOMEPAGE,
        UNKOU_TITLE,
        UNKOU_CLOSE,
        SOUKOU_MSG_1,
        SOUKOU_MSG_2,
        SOUKOU_UPDATE,
        TO_MATSUDO,
        TO_TSUDANUMA,
        NOBORI,
        KUDARI,
        MENU_EKIANNAI,
        KAKUDAI,
        SYUKUSYO,
        JIKOKU_HEIJITU_1,
        JIKOKU_HEIJITU_2,
        JIKOKU_KYUJITU_1,
        JIKOKU_KYUJITU_2,
        RETURN,
        KANRI_TITLE,
        SL01,
        SL02,
        SL03,
        SL04,
        SL05,
        SL06,
        SL07,
        SL08,
        SL09,
        SL10,
        SL11,
        SL12,
        SL13,
        SL14,
        SL15,
        SL16,
        SL17,
        SL18,
        SL19,
        SL20,
        SL21,
        SL22,
        SL23,
        SL24,
        SL01_2,
        SL02_2,
        SL03_2,
        SL04_2,
        SL05_2,
        SL06_2,
        SL07_2,
        SL08_2,
        SL09_2,
        SL10_2,
        SL11_2,
        SL12_2,
        SL13_2,
        SL14_2,
        SL15_2,
        SL16_2,
        SL17_2,
        SL18_2,
        SL19_2,
        SL20_2,
        SL21_2,
        SL22_2,
        SL23_2,
        SL24_2
    };

    /// <summary>
    /// 使用言語
    /// </summary>
    public enum LANGUAGE
    {
        JAPANESE,
        ENGLISH,
        CHINESE,
        KOREAN
    }

    /// <summary>
    /// 文字列初期化
    /// </summary>
    /// <param name="lang">使用言語</param>
    public static void Init()
    {
        LANGUAGE lang;

        SqliteDatabase sqlite = new SqliteDatabase("shinkeisei.db");
        string query = "SELECT language FROM user_info;";
        var response = sqlite.ExecuteQuery(query);

        if ((int)response.Rows[0]["language"] == 1)
        {
            lang = LANGUAGE.ENGLISH;
        }
        else if ((int)response.Rows[0]["language"] == 2)
        {
            lang = LANGUAGE.CHINESE;
        }
        else if ((int)response.Rows[0]["language"] == 3)
        {
            lang = LANGUAGE.KOREAN;
        }
        else
        {
            lang = LANGUAGE.JAPANESE;
        }

        // リソースファイルパス決定
        string filePath;
        if (lang == LANGUAGE.JAPANESE)
        {
            filePath = "Text/japanese";
        }
        else if (lang == LANGUAGE.ENGLISH)
        {
            filePath = "Text/english";
        }
        else if (lang == LANGUAGE.CHINESE)
        {
            filePath = "Text/chinese";
        }
        else if (lang == LANGUAGE.KOREAN)
        {
            filePath = "Text/korean";
        }
        else
        {
            throw new Exception("TextManager Init failed.");
        }

        // ディクショナリー初期化
        sDictionary.Clear();
        TextAsset csv = Resources.Load<TextAsset>(filePath);
        StringReader reader = new StringReader(csv.text);
        while (reader.Peek() > -1)
        {
            string[] values = reader.ReadLine().Split('\t');
            sDictionary.Add(values[0], values[1].Replace("\\n", "\n"));
        }
    }

    /// <summary>
    /// 文字列取得
    /// </summary>
    /// <param name="key">文字列取得キー</param>
    /// <returns>キーに該当する文字列</returns>
    public static string Get(KEY key)
    {
        return Get(Enum.GetName(typeof(KEY), key));
    }

    /// <summary>
    /// 文字列取得
    /// </summary>
    /// <param name="key">文字列取得キー</param>
    /// <returns>キーに該当する文字列</returns>
    public static string Get(string key)
    {
        return sDictionary[key];
    }
}