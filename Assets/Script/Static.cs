using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Static {

    //駅ID
    public static string StationNo;

    //連携用
    public static string JointFlag;
    public static string JointAppname;
    public static string JointStationCode;
    public static bool initFlag;

    //時刻表
    public static bool JikokuNobori;
    public static bool JikokuHoliday;

    //お気に入り駅
    public static string FavoStation;

    public static void JointClear()
    {
        JointAppname = "";
        JointStationCode = "";
        JointFlag = "";
    }

    public static void JikokuClear()
    {
        JikokuNobori = true;
        JikokuHoliday = false;
    }

    public static string getEkiName()
    {
        string str = TextManager.Get(TextManager.KEY.SL01); 

        if (Static.StationNo.Equals("01") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL01);
        }
        else if (Static.StationNo.Equals("02") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL02);
        }
        else if (Static.StationNo.Equals("03") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL03);
        }
        else if (Static.StationNo.Equals("04") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL04);
        }
        else if (Static.StationNo.Equals("05") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL05);
        }
        else if (Static.StationNo.Equals("06") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL06);
        }
        else if (Static.StationNo.Equals("07") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL07);
        }
        else if (Static.StationNo.Equals("08") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL08);
        }
        else if (Static.StationNo.Equals("09") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL09);
        }
        else if (Static.StationNo.Equals("10") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL10);
        }
        else if (Static.StationNo.Equals("11") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL11);
        }
        else if (Static.StationNo.Equals("12") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL12);
        }
        else if (Static.StationNo.Equals("13") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL13);
        }
        else if (Static.StationNo.Equals("14") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL14);
        }
        else if (Static.StationNo.Equals("15") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL15);
        }
        else if (Static.StationNo.Equals("16") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL16);
        }
        else if (Static.StationNo.Equals("17") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL17);
        }
        else if (Static.StationNo.Equals("18") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL18);
        }
        else if (Static.StationNo.Equals("19") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL19);
        }
        else if (Static.StationNo.Equals("20") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL20);
        }
        else if (Static.StationNo.Equals("21") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL21);
        }
        else if (Static.StationNo.Equals("22") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL22);
        }
        else if (Static.StationNo.Equals("23") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL23);
        }
        else if (Static.StationNo.Equals("24") == true)
        {
            str = TextManager.Get(TextManager.KEY.SL24);
        }

        return str;
    }

    public static void setFavo(string no)
    {
        SqliteDatabase sqlite = new SqliteDatabase("shinkeisei.db");
        string sql = @"UPDATE user_info SET station = """ + no + @""";";
        sqlite.ExecuteNonQuery(sql);

        FavoStation = no;
    }
}
