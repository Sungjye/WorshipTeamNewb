using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class KeyListControl : MonoBehaviour
{
    public GameObject gmobjPanel_PopupMenu; 

    // Start is called before the first frame update
    void Start()
    {
        this.gmobjPanel_PopupMenu = GameObject.Find("Panel_PopupMenu");


        this.gmobjPanel_PopupMenu.SetActive(false);

    }


    public void OnClick_Ckey()
    {
        GameManager.Instance.eSelectedKey = eKEYCODES.C;
        //SceneManager.LoadScene("02-01_Code_Intro");

        this.gmobjPanel_PopupMenu.SetActive(true);
    }


    public void OnClick_Popup_ScaleMode_Selected()
    {
        if(Application.isEditor) Debug.Log("Scale mode");

        GameManager.Instance.eSelectedMusicMode = eMUSICMODE.Scale;

        //SceneManager.LoadScene("02-02_Scale_Intro");
    }

    public void OnClick_Popup_CodeMode_Selected()
    {
        if(Application.isEditor) Debug.Log("Code mode");

        GameManager.Instance.eSelectedMusicMode = eMUSICMODE.Code;

        SceneManager.LoadScene("02-01_Code_Intro");
    }

    public void OnClick_Popup_Close()
    {
        this.gmobjPanel_PopupMenu.SetActive(false);

    }


    public void OnClick_KeyList_Close()
    {
        this.gmobjPanel_PopupMenu.SetActive(false);

        SceneManager.LoadScene("01-01_Mainmenu");

    }

}
