using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class KeyListControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void OnClick_Ckey()
    {
        GameManager.Instance.eSelectedKey = eKEYCODES.C;
        SceneManager.LoadScene("02-01_Code_Intro");
    }

}
