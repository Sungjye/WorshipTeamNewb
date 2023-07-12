using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackBtnBrick_Control : MonoBehaviour
{
    private string sCurrentSceneName, sBackToThisScene;
    // Start is called before the first frame update
    void Start()
    {
        sCurrentSceneName = SceneManager.GetActiveScene().name;
    }

    private void OnMouseDown()
    {
        if(Application.isEditor) Debug.Log("Mouse Down: " + this.name);

        switch( sCurrentSceneName )
        {
            case "02-01_Code_Intro":
                this.sBackToThisScene = "01-02_KeyList";
                break;
            case "02-02_Scale_Intro_a":
                this.sBackToThisScene = "01-02_KeyList";
                break; 
            default:
                // Do nothing?
                this.sBackToThisScene = "01-01_Mainmenu";
                break;
        }

        SceneManager.LoadScene(this.sBackToThisScene);

    }

}
