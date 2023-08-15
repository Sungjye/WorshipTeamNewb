//=====================================================================================
// Lord Jesus, thank you for making my mind clear and me stay in Your presence!
// 
// 가이드 도움말을 표시하는 TMP Text Prefab에 붙는 매니져 스크립트. 
// 현재 무슨 scene인지 확인해서 해당하는 적절한 도움말을 표시한다. 
// 
// 
// 2023.08.15. sjjo. Intial. @JLIB with Ellim
//
//=====================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.SceneManagement;

public class GuideText_DisplayManager : MonoBehaviour
{
    private TextMeshProUGUI tmpGuideText;

    void Awake()
    {
        this.tmpGuideText = GetComponent<TextMeshProUGUI>();

    }

    // Start is called before the first frame update
    void Start()
    {
        this.tmpGuideText.text = this.GetGuideText_accordingToTheScene();

    }

    // 처음 시작할 때, 블링킹이나, pop 효과 주려면 이 스립트에서 작성하면 될듯..?

    private string GetGuideText_accordingToTheScene()
    {


        string sCurrentSceneName = SceneManager.GetActiveScene().name;

        string sResultGuideText = null;

        switch( sCurrentSceneName )
        {
            case "01-01_Mainmenu":
                sResultGuideText = sCurrentSceneName; // Tentative
                break; 
            case "01-02_KeyList":
                sResultGuideText = sCurrentSceneName; // Tentative
                break; 
            //-----------------------------------------
            case "02-01_Code_Intro":
                sResultGuideText = sCurrentSceneName; // Tentative
                break;
            case "02-02_Scale_Intro_a":
                sResultGuideText = sCurrentSceneName; // Tentative
                break; 
            //-----------------------------------------
            case "03-01_Code_PickNumber":
                sResultGuideText = sCurrentSceneName; // Tentative
                break;
            case "03-01_Scale_PickNote":
                sResultGuideText = sCurrentSceneName; // Tentative
                break;
            //-----------------------------------------
            case "03-02_Code_PickPatNumber":
                sResultGuideText = sCurrentSceneName; // Tentative
                break;
            case "03-02_Scale_PickPatNotes": 
                sResultGuideText = sCurrentSceneName; // Tentative
                break;
            //-----------------------------------------
            case "03-03_Code_MatchSound": 
                sResultGuideText = 
                    "<size=120%>Drag-and-drop</size>\n the correct sound brick\nto this code below.";
                break;
            //-----------------------------------------
            case "04-01_Scale_RecogKeys": 
                sResultGuideText = sCurrentSceneName; // Tentative
                break;
            case "04-01_Code_RecogKeys": 
                sResultGuideText = sCurrentSceneName; // Tentative
                break;
            default:
                // Do nothing?
                sResultGuideText = null;
                break;
        }

        return sResultGuideText;

    }

}
