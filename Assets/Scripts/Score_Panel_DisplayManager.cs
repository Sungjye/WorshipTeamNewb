//===================================================================================
// 주님, 시간이 지날수록 마음이 조급해 지고, 코딩도 한글자 칠 때마다 마음이 바빠집니다. 
// 주님, 찬찬히, 주님의 인도하심을 구하며 할 수 있도록 도와 주십시요!
// 만민의 구원자 되신 예수님의 이름으로 기도드렸습니다, 아멘!
// 
// 뭐하는 스크립트?
// : 스코어 2가지를 표시하는 최상위 패널에 붙어서, GameManager 에서 스코어 값을 받아와 그대로 표시만 하는 함수. 
// : 시작할 때 표시하고, 스코어 업데이트 함수가 불리면 표시함. 
// 
// 2023.07.27. sjjo. Initial.
// 2023.08.14. sjjo. 무슨 키인지 표시하는 모드 child 추가. 
// 
//===================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using UnityEngine.SceneManagement;

public class Score_Panel_DisplayManager : MonoBehaviour
{

    private GameObject gmobjNoteScorePanel;
    private GameObject gmobjCodeScorePanel;
    private GameObject gmobjChosenKeyScalePanel;

    private GameObject gmobjNoteScoreText;
    private GameObject gmobjCodeScoreText;
    private GameObject gmobjChosenKeyScaleText;

    private string sDISPLAYFORMAT;

    // Start is called before the first frame update
    void Start()
    {
        this.sDISPLAYFORMAT = "N0"; // "#,#";

        this.gmobjNoteScorePanel = this.transform.GetChild(0).gameObject;
        this.gmobjCodeScorePanel = this.transform.GetChild(1).gameObject;
        this.gmobjChosenKeyScalePanel = this.transform.GetChild(2).gameObject;

        this.gmobjNoteScoreText = this.transform.GetChild(0).GetChild(1).gameObject;
        this.gmobjCodeScoreText = this.transform.GetChild(1).GetChild(1).gameObject;
        this.gmobjChosenKeyScaleText = this.transform.GetChild(2).GetChild(0).gameObject;


        //this.gmobjNoteScoreText.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.nl_NoteScore.ToString(this.sDISPLAYFORMAT);
        //this.gmobjCodeScoreText.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.nl_CodeScore.ToString(this.sDISPLAYFORMAT);

        //==============================================================================================
        // 현재 이 스코어 패널이 어느 scene에 표시되었냐에 따라서, 자식들의 표시/안표시 여부를 결정 먼저 하고. 
        this.ActivateMyChildren_asNOTEmodeOrCODEmode_byGMval();

        //=====================================================
        // 일단 시작하면서 점수를 업데이트 해준다. 
        this.RefreshScores();


        

    }

    public void RefreshScores()
    {
        //this.gmobjNoteScoreText.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.nl_NoteScore.ToString(this.sDISPLAYFORMAT);
        //this.gmobjCodeScoreText.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.nl_CodeScore.ToString(this.sDISPLAYFORMAT);

        if( this.gmobjNoteScorePanel.activeSelf ) this.gmobjNoteScoreText.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.nl_NoteScore.ToString(this.sDISPLAYFORMAT);
        if( this.gmobjCodeScorePanel.activeSelf ) this.gmobjCodeScoreText.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.nl_CodeScore.ToString(this.sDISPLAYFORMAT);

        // 선택된 키를 표시해야 하는 모드인 경우 표시를 해준다. 
        if( this.gmobjChosenKeyScalePanel.activeSelf ) 
                this.gmobjChosenKeyScaleText.GetComponent<TextMeshProUGUI>().text = ContentsManager.Instance.GetTheChosenKeySacleText_toDisplay();

    }


    // 각 scene에서 이 패널이 인스턴시에엣 될 때마다, 처음에 한번 호출되면 되므로. 
    private void ActivateMyChildren_asNOTEmodeOrCODEmode_byGMval() 
    {
        // 2개의 자식(노트모드 스코어보드, 코드모드 스코어보드)을 보이게 안보이게 하는 함수. 
        // 
        // 일단, 현재의 scene 이 다음의 종류에 따라서, 나누기. 
        // 
        // 절대음감 키리스트 모드라면, 
        //      둘다 표시해 주기.
        // Quiz 관련 scene 이라면,
        //      GM 의 변수값을 확인해서, 
        //      Note 모드 이면, Note 스코어만 표시. 
        //      Code 모드 이면, Code 스코어만 표시. 
        // 
        

        switch( SceneManager.GetActiveScene().name )
        {
            //-- Key List Scene.
            case "01-02_KeyList":
                gmobjNoteScorePanel.SetActive(true);
                gmobjCodeScorePanel.SetActive(true);
                gmobjChosenKeyScalePanel.SetActive(false);
                break;
            //-- Scale 단음
            case "02-02_Scale_Intro_a":
            case "03-01_Scale_PickNote":
            case "03-02_Scale_PickPatNotes":
                gmobjNoteScorePanel.SetActive(true);
                gmobjCodeScorePanel.SetActive(false);
                gmobjChosenKeyScalePanel.SetActive(true);
                break;
            case "04-01_Scale_RecogKeys": // 키 알아맞추기 모드는 별도!
                gmobjNoteScorePanel.SetActive(true);
                gmobjCodeScorePanel.SetActive(false);
                gmobjChosenKeyScalePanel.SetActive(false);
                break;
            //-- Code 코드
            case "02-01_Code_Intro":
            case "03-01_Code_PickNumber":
            case "03-02_Code_PickPatNumber":
            case "03-03_Code_MatchSound":
                gmobjNoteScorePanel.SetActive(false);
                gmobjCodeScorePanel.SetActive(true);
                gmobjChosenKeyScalePanel.SetActive(true);
                break;            
            case "04-01_Code_RecogKeys": // 키 알아맞추기 모드는 별도!
                gmobjNoteScorePanel.SetActive(false);
                gmobjCodeScorePanel.SetActive(true);
                gmobjChosenKeyScalePanel.SetActive(false);
                break;
            default:
                // Show both of them.
                break;
        }
          
    }




}
