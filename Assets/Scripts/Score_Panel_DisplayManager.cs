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
// 2023.08.18. sjjo. 이벤트 팝업 메시지 표시하기 위해 상위 패널을 하나 더 둠. 상위 부모인 패널의 컴포넌트로 옮김. 
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

    public GameObject gmobjEventMsgPopup_prefab; 
    private GameObject instGmobj_MsgPopUp;

    private string sDISPLAYFORMAT;

    
    
    private Coroutine crTextPopEffect;
    private Coroutine crDestroyTheMsgBox;

    private Vector3 v3OriginalTextBoxSize;


    void Awake()
    {
        this.sDISPLAYFORMAT = "N0"; // "#,#";

        this.crTextPopEffect = null;

        this.gmobjNoteScorePanel = this.transform.GetChild(0).GetChild(0).gameObject;
        this.gmobjCodeScorePanel = this.transform.GetChild(0).GetChild(1).gameObject;
        this.gmobjChosenKeyScalePanel = this.transform.GetChild(0).GetChild(2).gameObject;

        this.gmobjNoteScoreText = this.transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
        this.gmobjCodeScoreText = this.transform.GetChild(0).GetChild(1).GetChild(1).gameObject;
        this.gmobjChosenKeyScaleText = this.transform.GetChild(0).GetChild(2).GetChild(0).gameObject;

        // 사이즈가 팝핑으로 변하는 중에 또 점수 따거나/잃으면, 그 현재 크기 기준에서 움직여서 점점 커진다!
        // 이문제를 막기 위해. 
        this.v3OriginalTextBoxSize = this.gmobjNoteScoreText.transform.localScale; // 노트점수 텍스트나, 코드점수 텍스트나 원래크기는 같을 것이므로 아무거나 넣어서 같이 사용.

        /*
        this.gmobjNoteScorePanel = this.transform.GetChild(0).gameObject;
        this.gmobjCodeScorePanel = this.transform.GetChild(1).gameObject;
        this.gmobjChosenKeyScalePanel = this.transform.GetChild(2).gameObject;

        this.gmobjNoteScoreText = this.transform.GetChild(0).GetChild(1).gameObject;
        this.gmobjCodeScoreText = this.transform.GetChild(1).GetChild(1).gameObject;
        this.gmobjChosenKeyScaleText = this.transform.GetChild(2).GetChild(0).gameObject;

        // 사이즈가 팝핑으로 변하는 중에 또 점수 따거나/잃으면, 그 현재 크기 기준에서 움직여서 점점 커진다!
        // 이문제를 막기 위해. 
        this.v3OriginalTextBoxSize = this.gmobjNoteScoreText.transform.localScale; 
        */



    }

    // Start is called before the first frame update
    void Start()
    {



        //this.gmobjNoteScoreText.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.nl_NoteScore.ToString(this.sDISPLAYFORMAT);
        //this.gmobjCodeScoreText.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.nl_CodeScore.ToString(this.sDISPLAYFORMAT);

        //==============================================================================================
        // 현재 이 스코어 패널이 어느 scene에 표시되었냐에 따라서, 자식들의 표시/안표시 여부를 결정 먼저 하고. 
        this.ActivateMyChildren_asNOTEmodeOrCODEmode_byGMval();

        //=====================================================
        // 일단 시작하면서 점수를 업데이트 해준다. 
        this.RefreshScores(false);


        

    }

    #if FALSE
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


    public void RefreshScores()
    {
        // 점수 스코어를 팝 효과와 함께 리프래쉬
        // (나중에는 롤링 효과도?)

        if( this.gmobjNoteScorePanel.activeSelf ) this.gmobjNoteScoreText.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.nl_NoteScore.ToString(this.sDISPLAYFORMAT);
        if( this.gmobjCodeScorePanel.activeSelf ) this.gmobjCodeScoreText.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.nl_CodeScore.ToString(this.sDISPLAYFORMAT);

        // 선택된 키를 표시해야 하는 모드인 경우 표시를 해준다. 
        if( this.gmobjChosenKeyScalePanel.activeSelf ) 
                this.gmobjChosenKeyScaleText.GetComponent<TextMeshProUGUI>().text = ContentsManager.Instance.GetTheChosenKeySacleText_toDisplay();

    }
    #endif

    public void RefreshScores(bool bDoPopEffect)
    {
        // 점수 스코어를 팝 효과와 함께 리프래쉬
        // + 일단 팝핑 효과를 선택할 수 있게 하는 함수. 

        if( this.gmobjNoteScorePanel.activeSelf ) 
        {
            this.gmobjNoteScoreText.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.nl_NoteScore.ToString(this.sDISPLAYFORMAT);

            if(bDoPopEffect) this.ScoringEffect_Pop(this.gmobjNoteScoreText);
        }

        if( this.gmobjCodeScorePanel.activeSelf )
        {
            this.gmobjCodeScoreText.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.nl_CodeScore.ToString(this.sDISPLAYFORMAT);

            if(bDoPopEffect) this.ScoringEffect_Pop(this.gmobjCodeScoreText);
        } 

        // 선택된 키를 표시해야 하는 모드인 경우 표시를 해준다. 
        if( this.gmobjChosenKeyScalePanel.activeSelf ) 
        {
            this.gmobjChosenKeyScaleText.GetComponent<TextMeshProUGUI>().text = ContentsManager.Instance.GetTheChosenKeySacleText_toDisplay();

            
        }

    }

    public void RefreshScores()
    {
        // 점수 스코어를 팝 효과와 함께 리프래쉬
        // + 일단 팝핑 효과.

        if( this.gmobjNoteScorePanel.activeSelf ) 
        {
            this.gmobjNoteScoreText.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.nl_NoteScore.ToString(this.sDISPLAYFORMAT);

            this.ScoringEffect_Pop(this.gmobjNoteScoreText);
        }

        if( this.gmobjCodeScorePanel.activeSelf )
        {
            this.gmobjCodeScoreText.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.nl_CodeScore.ToString(this.sDISPLAYFORMAT);

            this.ScoringEffect_Pop(this.gmobjCodeScoreText);
        } 

        // 선택된 키를 표시해야 하는 모드인 경우 표시를 해준다. 
        if( this.gmobjChosenKeyScalePanel.activeSelf ) 
        {
            this.gmobjChosenKeyScaleText.GetComponent<TextMeshProUGUI>().text = ContentsManager.Instance.GetTheChosenKeySacleText_toDisplay();

            
        }

        //this.ShowEventMessage("Test Message!");

    }

    public void RefreshScores(string sScoredEventMessage)
    {
        // 점수 스코어를 팝 효과와 함께 리프래쉬
        // + 일단 팝핑 효과.

        if( this.gmobjNoteScorePanel.activeSelf ) 
        {
            this.gmobjNoteScoreText.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.nl_NoteScore.ToString(this.sDISPLAYFORMAT);

            this.ScoringEffect_Pop(this.gmobjNoteScoreText);
        }

        if( this.gmobjCodeScorePanel.activeSelf )
        {
            this.gmobjCodeScoreText.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.nl_CodeScore.ToString(this.sDISPLAYFORMAT);

            this.ScoringEffect_Pop(this.gmobjCodeScoreText);
        } 

        // 선택된 키를 표시해야 하는 모드인 경우 표시를 해준다. 
        if( this.gmobjChosenKeyScalePanel.activeSelf ) 
        {
            this.gmobjChosenKeyScaleText.GetComponent<TextMeshProUGUI>().text = ContentsManager.Instance.GetTheChosenKeySacleText_toDisplay();

            
        }

        this.ShowEventMessage(sScoredEventMessage);

    }


    private void ShowEventMessage(string sMessage)
    {
        // 뭐하는 함수?
        // 스코어의 득/실 시에, 즉 스코어 이벤트 발생시, 그 이유를 팝업으로 표시해주는 함수. 
        // 팝업 패널을 인스턴시에잇하고, 일정시간이 지난 뒤에 없애준다. 
        // 여러개가 뜰 수 있게 때문에 리스트로 관리해야할 듯. 

        //GameObject instGmobj_MsgPopUp = Instantiate(gmobjEventMsgPopup_prefab);
        //this.liGmobj_MsgPopUp.Add(instGmobj_MsgPopUp);



        if( this.instGmobj_MsgPopUp != null)
        {
            // 기존에 팝업이 있다면 없애주고.. 
            Destroy(this.instGmobj_MsgPopUp);
            this.instGmobj_MsgPopUp = null;

            // 기존 팝업 때문에 돌고 있던 코루틴도 즉시 멈춤.
            StopCoroutine(this.crDestroyTheMsgBox);
        }

        // 그리고 나서 생성해야!
        //this.instGmobj_MsgPopUp = Instantiate(gmobjEventMsgPopup_prefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        this.instGmobj_MsgPopUp = Instantiate(gmobjEventMsgPopup_prefab);
        this.instGmobj_MsgPopUp.transform.SetParent(this.transform, false); // 월드포지션 좌표계 off. 헐.. 이거 안하면, 한쪽으로 치우쳐서 팝업이 이상하게 표시됨!!!

        // 메시지 넣기. 
        this.instGmobj_MsgPopUp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = sMessage;

        this.DelayedDestroyTheMsgBox( this.instGmobj_MsgPopUp );

        
    }

    private void DelayedDestroyTheMsgBox(GameObject gmobjObjectToDestroy)
    {
        if( this.crDestroyTheMsgBox != null ) StopCoroutine(this.crDestroyTheMsgBox);

        this.crDestroyTheMsgBox = StartCoroutine( DestroyTheMsgBox_afterAwhile(gmobjObjectToDestroy) );

    }

    IEnumerator DestroyTheMsgBox_afterAwhile(GameObject gmobjTargetObject)
    {
        yield return null;

        yield return new WaitForSeconds(1f);

        Destroy(gmobjTargetObject);
    }


    private void ScoringEffect_Pop(GameObject gmobjTargetObject)
    {
        // 서로 독립적으로 팝핑하는.. 음 일단..

        if( this.crTextPopEffect != null ) StopCoroutine( this.crTextPopEffect );

        //this.crTextPopEffect = StartCoroutine( MakeMe_Pop_TypeB(gmobjTargetObject, 0.15f, 0.04f) );
        this.crTextPopEffect = StartCoroutine( MakeMe_Pop_TypeC(gmobjTargetObject, 0.7f, 10f, 0.2f) );
        // 최대 50%까지 커지고, 20개의 프래임으로 0.5초동안 (사인파형으로) 팝 해줘.

    }

    IEnumerator MakeMe_Pop_TypeC(GameObject gmobjTarget, float fMaxIncSize, float fFrames, float fPerSec)
    {
        // 사인파형으로 팝핑 효과를 내는 코루틴 함수. 
        // fFPS는 이 효과 전체를 어떤 프레임 레이트로 돌릴지를 나타내는 값. 

        yield return null; // 다음 프레임까지 기다렸다가. 

        // 삼각함수 사용.
        // Ref. https://docs.unity3d.com/kr/2021.3/Manual/class-Mathf.html

        float fPeriod = fPerSec/fFrames; // e.g. 1/30 

        float fMaxAngle_inRadian = Mathf.PI; // 즉 180도. 

        //float fIncStepIncAngle_inDegreeps = 180f / fFPS; // 90도를 30프레임으로 나누면 1프레임이 fIncAngle_inDegree
        //float fIncStepIncAngle_inRadian = fIncStepIncAngle_inDegreeps * Mathf.Deg2Rad;
        float fIncStepIncAngle_inRadian = fMaxAngle_inRadian / fFrames;

        float fCurrentAngle_inRadian = 0f;
        float fResultYval = 0f;
        //int nTempCnt = 0;

        //Debug.Log(System.DateTime.Now.ToString("HHmmss"));
        /*
var now = DateTime.Now.ToLocalTime();
var span = (now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
int timestamp = (int)span.TotalSeconds;
        */
        Vector3 v3TargetMaxSize = this.v3OriginalTextBoxSize * fMaxIncSize;

        while( fCurrentAngle_inRadian <= fMaxAngle_inRadian)
        {
            fCurrentAngle_inRadian += fIncStepIncAngle_inRadian;

            fResultYval = Mathf.Sin( fCurrentAngle_inRadian);

            //Debug.Log($"Sine val: {nTempCnt}) {fResultYval} "); nTempCnt++;
            gmobjTarget.transform.localScale =  this.v3OriginalTextBoxSize + ( v3TargetMaxSize * fResultYval );

            yield return new WaitForSeconds( fPeriod );
        }

        //Debug.Log(System.DateTime.Now.ToString("HH:mm:ss"));


        //----------------------
        // 안전하게, 원래 사이즈로. 
        gmobjTarget.transform.localScale = this.v3OriginalTextBoxSize;

        //yield return new WaitForSeconds( 1f );

        //float fSineVale = Mathf.
        //Debug.Log(  );


    }

//#if FALSE
    IEnumerator MakeMe_Pop_TypeB(GameObject gmobjTarget, float fMaxIncSize, float fInterval) // 0.3f, 0.03f
    {
        // 팝핑 효과를 내는 코루틴 함수를, 보다 일반적으로 만들기. 23.08.17
        // 가능하다면 sine 함수 적용해서 보다 느낌있게 팝핑하기?
        //
        // 자신(메인 번들 리스트 아이템 하나)을 살짝 크기를 키웠다가 원래 사이즈로 만드는 코루틴.

        // 다음 프레임까지 깔끔하게 기다렸다가!
        yield return null;

        //Vector3 vOrigianlSize = gmobjTarget.transform.localScale; // 최초의 크기를 넣어두고..


        float fSizeSpan = 0.03f;
        //Vector3 vNewSize = new Vector3(1.1f, 1.1f, 1.1f);

        //----------------------
        // 커지는 단계
        for(float fSizeInc = 0f; fSizeInc < fMaxIncSize; fSizeInc += fSizeSpan)
        {
            Vector3 vNewSize = new Vector3(fSizeInc, fSizeInc, fSizeInc);

            gmobjTarget.transform.localScale = this.v3OriginalTextBoxSize + vNewSize;

            yield return new WaitForSeconds(fInterval);
        }

        //----------------------
        // 잠시 멈추는 단계
        //yield return new WaitForSeconds(0.3f);

         Vector3 vChangedSize = gmobjTarget.transform.localScale;

        //----------------------
         // 작아지는 단계 
         // fSizeSpan = 0.03f; // 작아지는 속도는 좀 빠르게?

        //for(float fSizeInc = fMaxIncSize; fSizeInc > 0f; fSizeInc -= fSizeSpan)
        for(float fSizeInc = 0f; fSizeInc < fMaxIncSize; fSizeInc += fSizeSpan)
        //for(float fSizeInc = 0f; fSizeInc < fMaxIncSize; fSizeInc += fSizeSpan)// 살짝 작아졌다가?
        {
            Vector3 vNewSize = new Vector3(fSizeInc, fSizeInc, fSizeInc);

            //gmobjTarget.transform.localScale = vOrigianlSize - vNewSize;
            gmobjTarget.transform.localScale = vChangedSize - vNewSize;

            yield return new WaitForSeconds(fInterval);
        }

        //----------------------
        // 안전하게, 원래 사이즈로. 
         gmobjTarget.transform.localScale = this.v3OriginalTextBoxSize;

    }
//#endif

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
