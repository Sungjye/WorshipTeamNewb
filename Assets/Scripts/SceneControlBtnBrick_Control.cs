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
                    // 이걸 안날리면 리스트 빈 오브젝트를 담고 있다. 단음 화음 쌓기, 점수주기 로직관련.
                    GameManager.Instance.li_gmobj_CurrentlyExistingBricks.Clear();
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
                case "03-02_Scale_PickPatNotes": // 23.07.24
                    this.sGoToThisScene = "03-01_Scale_PickNote";
                    break;
                //-----------------------------------------
                case "03-03_Code_MatchSound": // 23.07.24
                    this.sGoToThisScene = "03-02_Code_PickPatNumber";
                    break;
                //-----------------------------------------
                case "04-01_Scale_RecogKeys": // 23.08.04
                    this.sGoToThisScene = "01-02_KeyList";
                    break;
                case "04-01_Code_RecogKeys": // 23.08.07
                    this.sGoToThisScene = "01-02_KeyList";
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
                    // 이걸 안날리면 리스트 빈 오브젝트를 담고 있다. 단음 화음 쌓기, 점수주기 로직관련.
                    GameManager.Instance.li_gmobj_CurrentlyExistingBricks.Clear();
                    this.sGoToThisScene = "03-01_Scale_PickNote";
                    break; 
                //-----------------------------------------
                case "03-01_Code_PickNumber":
                    this.sGoToThisScene = "03-02_Code_PickPatNumber";
                    //this.sGoToThisScene = "StayHere";
                    break;
                case "03-01_Scale_PickNote": // 23.07.24
                    this.sGoToThisScene = "03-02_Scale_PickPatNotes";
                    //this.sGoToThisScene = "StayHere";
                    break;
                //-----------------------------------------
                case "03-02_Code_PickPatNumber": // 23.07.24
                    this.sGoToThisScene = "03-03_Code_MatchSound";
                    //this.sGoToThisScene = "StayHere";
                    break;
                //-----------------------------------------
                case "04-01_Scale_RecogKeys": // 23.08.07
                    //this.sGoToThisScene = "04-01_Code_RecogKeys"; // 키 알아맞추기 모드간에 스위칭..
                    this.sGoToThisScene = "StayHere";
                    break;
                case "04-01_Code_RecogKeys": // 23.08.07
                    //this.sGoToThisScene = "04-01_Scale_RecogKeys"; // 키 알아맞추기 모드간에 스위칭..
                    this.sGoToThisScene = "StayHere";
                    break;
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
