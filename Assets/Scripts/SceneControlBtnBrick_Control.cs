using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControlBtnBrick_Control : MonoBehaviour
{
    private string sCurrentSceneName, sGoToThisScene;
    // Start is called before the first frame update
    void Start()
    {
        sCurrentSceneName = SceneManager.GetActiveScene().name;
    }

    private void OnMouseDown()
    {
        if(Application.isEditor) Debug.Log("Mouse Down: " + this.name);

        if( this.name == "BackBtnBrick" )
        {
            // "뒤로" 브릭 버튼용
            switch( sCurrentSceneName )
            {
                case "02-01_Code_Intro":
                    this.sGoToThisScene = "01-02_KeyList";
                    break;
                case "02-02_Scale_Intro_a":
                    this.sGoToThisScene = "01-02_KeyList";
                    break; 
                //-----------------------------------------
                case "03-01_Code_PickNumber":
                    this.sGoToThisScene = "02-01_Code_Intro";
                    break;
                case "03-01_Scale_PickNote":
                    this.sGoToThisScene = "02-02_Scale_Intro_a";
                    break;
                //-----------------------------------------
                case "03-02_Code_PickPatNumber":
                    this.sGoToThisScene = "03-01_Code_PickNumber";
                    break;
                default:
                    // Do nothing?
                    this.sGoToThisScene = "01-01_Mainmenu";
                    break;
            }


        }else if( this.name == "NextBtnBrick" )
        {
            // "다음" 브릭 버튼용
            switch( sCurrentSceneName )
            {
                case "02-01_Code_Intro":
                    this.sGoToThisScene = "03-01_Code_PickNumber";                    
                    break;
                case "02-02_Scale_Intro_a":
                    this.sGoToThisScene = "03-01_Scale_PickNote";
                    break; 
                //-----------------------------------------
                case "03-01_Code_PickNumber":
                    this.sGoToThisScene = "03-02_Code_PickPatNumber";
                    //this.sGoToThisScene = "StayHere";
                    break;
                case "03-01_Scale_PickNote":
                    //this.sGoToThisScene = "03-02_Scale_@";
                    this.sGoToThisScene = "StayHere";
                    break;
                //-----------------------------------------
                default:
                    // Do nothing?
                    //this.sGoToThisScene = "01-01_Mainmenu";
                    this.sGoToThisScene = "StayHere";
                    break;
            }

        }


        if( this.sGoToThisScene != "StayHere" ) SceneManager.LoadScene(this.sGoToThisScene);


    }

}
