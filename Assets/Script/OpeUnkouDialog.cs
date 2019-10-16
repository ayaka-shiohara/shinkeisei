using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeUnkouDialog : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.gameObject.SetActive(false);
        OpeBase(false);
    }
	
	// Update is called once per frame
	void Update () {

    }

    void OpeBase(bool update)
    {
        Canvas bases = gameObject.transform.parent.gameObject.transform.Find("dialogbase").GetComponent<Canvas>();
        bases.gameObject.SetActive(update);
    }

    public void OpenDialog()
    {
        gameObject.SetActive(true);
        OpeBase(true);
    }

    public void CloseDialog()
    {
        Start();
    }
}
