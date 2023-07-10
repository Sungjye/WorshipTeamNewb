using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainmenuControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }



    public void OnClick_NotePracticeMode()
    {
        if( Application.isEditor ) Debug.Log("Note Practice");

        SceneManager.LoadScene("01-02_KeyList");
    }

    public void OnClick_BeatPracticeMode()
    {
        if( Application.isEditor ) Debug.Log("Beat Practice");
    }
}
