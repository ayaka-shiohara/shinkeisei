using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
public class DisplayData : MonoBehaviour {

    Image image;

    bool bigflag = true;
    bool smallflag = false;

    float xsize = 800;
    float ysize = 670;

    // Use this for initialization
    void Start () {
        WriteText();

        image = GameObject.Find("KounaiImage").GetComponent<Image>();
        OpeButton();

        if (Static.StationNo == null)
        {
            Static.StationNo = "01";
        }

        GetEki();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void GetEki()
    {
        Image eki = gameObject.transform.Find("ekidata").gameObject.transform.Find("eki").GetComponent<Image>();
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

        string scene = SceneManager.GetActiveScene().name;
        string imagetype = (scene == "home") ? "Home/" : "Floor/";

        imagepath = Application.persistentDataPath + "/" + imagetype + Static.StationNo;
        if (File.Exists(imagepath) == false)
        {
            string path = imagetype + Static.StationNo;
            Sprite sprite = Resources.Load<Sprite>(path);
            image.sprite = sprite;
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
            image.sprite = ekisprite;
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

    public void BiggerImage()
    {
        OpeFlag();

        float x = (bigflag) ? image.transform.localScale.x + 0.2f : 2.0f;
        float y = (bigflag) ? image.transform.localScale.y + 0.2f : 2.0f;

        var scale = new Vector3(x, y);
        image.transform.localScale = scale;

        var parent = image.transform.parent.gameObject.GetComponent<RectTransform>();
        var size = new Vector2(xsize * x, ysize * y);
        parent.sizeDelta = size;

        if (bigflag)
        {
            MoveCenter();
        }

        OpeButton();
    }

    public void SmallerImage()
    {
        OpeFlag();

        float x = (smallflag) ? image.transform.localScale.x - 0.2f : 1.0f;
        float y = (smallflag) ? image.transform.localScale.y - 0.2f : 1.0f;

        var scale = new Vector3(x, y);
        image.transform.localScale = scale;

        var parent = image.transform.parent.gameObject.GetComponent<RectTransform>();
        var size = new Vector2(xsize * x, ysize * y);
        parent.sizeDelta = size;

        if (smallflag)
        {
            MoveCenter();
        }
        OpeButton();
    }

    private void MoveCenter()
    {
        ScrollRect field = GameObject.Find("KounaiScroll").GetComponent<ScrollRect>();
        field.horizontalNormalizedPosition = 0.5f;
        field.verticalNormalizedPosition = 0.5f;
    }

    private void OpeFlag()
    {
        bigflag = (image.transform.localScale.x < 2.0) ? true : false;
        smallflag = (image.transform.localScale.x > 1.0) ? true : false;
    }

    private void OpeButton()
    {
        OpeFlag();

        Button big = GameObject.Find("Bigger").GetComponent<Button>();
        Button small = GameObject.Find("Smaller").GetComponent<Button>();

        big.interactable = true;
        small.interactable = true;

        if (!bigflag)
        {
            big.interactable = false;
        }

        if (!smallflag)
        {
            small.interactable = false;
        }
    }

    private void WriteText()
    {
        string name = (SceneManager.GetActiveScene().name == "home") ? TextManager.Get(TextManager.KEY.HOMEZU) : TextManager.Get(TextManager.KEY.KOUNAIZU);
        Text title = GameObject.Find("TitleText").GetComponent<Text>();
        title.text = name;

        Text home = GameObject.Find("home").GetComponentInChildren<Text>();
        home.text = TextManager.Get(TextManager.KEY.HOMEZU);

        Text kaisatu = GameObject.Find("kaisatu").GetComponentInChildren<Text>();
        kaisatu.text = TextManager.Get(TextManager.KEY.KOUNAIZU);

        Text soukou = GameObject.Find("soukouichi").GetComponentInChildren<Text>();
        soukou.text = TextManager.Get(TextManager.KEY.MENU_SOUKOU);

        Text jikoku = GameObject.Find("jikokuhyo").GetComponentInChildren<Text>();
        jikoku.text = TextManager.Get(TextManager.KEY.JIKOKU);

        Text modoru = GameObject.Find("return").GetComponentInChildren<Text>();
        modoru.text = TextManager.Get(TextManager.KEY.RETURN);
    }
}
