using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeStation : MonoBehaviour {

    public Image image;
    private Sprite sprite;
    private string path;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        image = GetComponent<Image>();
        Static.StationNo = image.name;
        path = "Sprites/Rosenzu/active/" + Static.StationNo;
        sprite = Resources.Load<Sprite>(path);
        image.sprite = sprite;
        ChangeStationInfo(col);        
    }

    void OnTriggerExit2D(Collider2D col)
    {
        image = this.GetComponent<Image>();
        path = "Sprites/Rosenzu/inactive/" + image.name;
        sprite = Resources.Load<Sprite>(path);
        image.sprite = sprite;
    }

    void ChangeStationInfo(Collider2D col)
    {
        //    image = col.gameObject.transform.parent.gameObject.transform.Find("ekidata")
        //            .gameObject.transform.Find("eki").GetComponent<Image>();

        //    string imagepath = Application.persistentDataPath + "/GetStationPanel/" + Static.StationNo;
        //    if (!File.Exists(imagepath))
        //    {
        //        Debug.Log(imagepath + " is not found.");
        //        imagepath = "Assets/GetStationPanel/" + Static.StationNo;
        //    }

        //    sprite = null;
        //    Texture2D texture = Texture2DFromFile(imagepath);
        //    if (texture)
        //    {
        //        //Texture2DからSprite作成
        //        sprite = SpriteFromTexture2D(texture);
        //    }
        //    texture = null;

        //    image.sprite = sprite;
        image = col.gameObject.transform.parent.gameObject.transform.Find("ekidata")
                .gameObject.transform.Find("eki").GetComponent<Image>();
        string imagepath = Application.persistentDataPath + "/GetStationPanel/" + Static.StationNo;
        if (File.Exists(imagepath) == false)
        {
            string path = "GetStationPanel/" + Static.StationNo;
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
}
