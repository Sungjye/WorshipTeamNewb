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

    public void OnClick_Fkey()
    {
        GameManager.Instance.eSelectedKey = eAVAILABLEKEYS.F;
        this.gmobjPanel_PopupMenu.SetActive(true);
    }

    public void OnClick_Gkey()
    {
        GameManager.Instance.eSelectedKey = eAVAILABLEKEYS.G;
        this.gmobjPanel_PopupMenu.SetActive(true);
    }

    public void OnClick_Dkey()
    {
        GameManager.Instance.eSelectedKey = eAVAILABLEKEYS.D;
        this.gmobjPanel_PopupMenu.SetActive(true);
    }

    public void OnClick_Akey()
    {
        GameManager.Instance.eSelectedKey = eAVAILABLEKEYS.A;
        this.gmobjPanel_PopupMenu.SetActive(true);
    }
    public void OnClick_Ekey()
    {
        GameManager.Instance.eSelectedKey = eAVAILABLEKEYS.E;
        this.gmobjPanel_PopupMenu.SetActive(true);
    }

//================================
// 키 알아맞추기는 맨 마지막에 배치. 
    public void OnClick_RecogKeys()
    {
        GameManager.Instance.bIsRecogKeysMode = true;
        //GameManager.Instance.eSelectedKey = eAVAILABLEKEYS.NONE; // 일단 실제로 모드 들어갈 때 랜덤으로 생성하게.        
        // 라기 보다는.. 의미상으로, 키 선택이 없는 모드로.. 키 인식 모드는.. 
        //SceneManager.LoadScene("02-01_Code_Intro");

        this.gmobjPanel_PopupMenu.SetActive(true);
    }


    public void OnClick_Popup_ScaleMode_Selected()
    {
        if(Application.isEditor) Debug.Log("Scale mode");

        GameManager.Instance.eSelectedMusicMode = eMUSICMODE.Scale;

        if( GameManager.Instance.bIsRecogKeysMode == true )
        {
            SceneManager.LoadScene("04-01_Scale_RecogKeys");
            GameManager.Instance.bIsRecogKeysMode = false; // 리셋해줘야 또 쓰지. 안그러면 이 모드로만 감..
        }else
        {
            SceneManager.LoadScene("02-02_Scale_Intro_a");
        }
    }

    public void OnClick_Popup_CodeMode_Selected()
    {
        if(Application.isEditor) Debug.Log("Code mode");

        GameManager.Instance.eSelectedMusicMode = eMUSICMODE.Code;


        if( GameManager.Instance.bIsRecogKeysMode == true )
        {
            SceneManager.LoadScene("04-01_Code_RecogKeys");
            GameManager.Instance.bIsRecogKeysMode = false; // 리셋해줘야 또 쓰지. 안그러면 이 모드로만 감..
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
