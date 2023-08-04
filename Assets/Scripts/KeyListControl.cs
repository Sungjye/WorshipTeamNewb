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


#region Private Methods

    //private BranchScenes_AccordingTo

#endregion


    public void OnClick_Ckey()
    {
        GameManager.Instance.eSelectedKey = eAVAILABLEKEYS.C;
        //SceneManager.LoadScene("02-01_Code_Intro");

        this.gmobjPanel_PopupMenu.SetActive(true);
    }

//================================
// 키 알아맞추기는 맨 마지막에 배치. 
    public void OnClick_RecogKeys()
    {
        GameManager.Instance.eSelectedKey = eAVAILABLEKEYS.NONE; // 일단 실제로 모드 들어갈 때 랜덤으로 생성하게.
        //SceneManager.LoadScene("02-01_Code_Intro");

        this.gmobjPanel_PopupMenu.SetActive(true);
    }


    public void OnClick_Popup_ScaleMode_Selected()
    {
        if(Application.isEditor) Debug.Log("Scale mode");

        GameManager.Instance.eSelectedMusicMode = eMUSICMODE.Scale;

        if( GameManager.Instance.eSelectedKey == eAVAILABLEKEYS.NONE )
        {
            SceneManager.LoadScene("04-01_Scale_RecogKeys");
        }else
        {
            SceneManager.LoadScene("02-02_Scale_Intro_a");
        }
    }

    public void OnClick_Popup_CodeMode_Selected()
    {
        if(Application.isEditor) Debug.Log("Code mode");

        GameManager.Instance.eSelectedMusicMode = eMUSICMODE.Code;


        if( GameManager.Instance.eSelectedKey == eAVAILABLEKEYS.NONE )
        {
            // TBD.
            SceneManager.LoadScene("02-01_Code_Intro");
            
        }else
        {
            SceneManager.LoadScene("02-01_Code_Intro");
        }
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
