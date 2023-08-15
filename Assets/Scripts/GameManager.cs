//====================================================================================
//
// 주님, 특히 이 스크립트를 찬찬히 잘 짤 수 있게 해 주십시요...
// 
// 플레이 전체를 주관하는 스크립트
// [주요 하는 일]
//   * 스코어를 관리하는 메서드들이 있음.
//
// 2023.07.10. sjjo. Initial.
// 2023.08.03. sjjo. The methods for scoring system were added.
//
//====================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//---------------------------------------------------------
// [스코어 시스템. Scoring Policy]
// : 스코어링 폴리시를 몇가지 정할 수 있어야.. 스코어링에 의한 동기부여 효과는 처음 해보브로..
public enum eSCORING_POLICY {   POLICY_1_BASIC
                              , POLICY_2_AGGRESSIVE
                            };
//---------------------------------------------------------


//---------------------------------------------------------
// [스코어 시스템. Case ID]
// : 스코어링 관련 상황을 Case ID로 나누어서, 처리를 이해하기 쉽게 함. 다음  sheet 문서 참조: 230727_찬양팀막내_ScoreSystem_v01
public enum eSCORING_CASEID {  
                                // 스케일 모드 (단음 모드)
                                 SM_ITR_0, SM_ITR_1   // Scale Mode. Intro : 그냥 플레이 하기. 
                               , SM_PN_0, SM_PN_1     // Scale Mode. PickNote : 단음 1개 맞추기. 
                                        , SM_PN_6     //                      : 틀렸을 때. 
                               , SM_PPN_0, SM_PPN_1   // Scale Mode. PickPatNote : 단음 여러개 쌓은것 맞추기.
                                        , SM_PPN_6     //                      : 틀렸을 때. 
                               , SM_RK_0, SM_RK_1     // Scale Mode. Recognize Keys: 키 맞추기
                                        , SM_RK_6     //                      : 틀렸을 때. 
                               // 코드 모드
                               , CM_ITR_0, CM_ITR_1     // Code Mode. Intro : 그냥 플레이 하기.
                               , CM_PN_0, CM_PN_1       // Code Mode. PickNumber : 코드 1개 맞추기.
                                        , CM_PN_6       //                       : 틀렸을 때. 
                               , CM_PPN_0, CM_PPN_1     // Code Mode. PickPatNumber : 코드 여러개 쌓은것 맞추기.
                                        , CM_PPN_6      //                       : 틀렸을 때.
                               , CM_MS_0, CM_MS_1       // Code Mode. MatchSound : 텍스트 코드를 보고, (키패드의) 소리를 듣고 맞추기.
                                        , CM_MS_6     //                      : 틀렸을 때. 
                               , CM_RK_0, CM_RK_1     // Code Mode. Recognize Keys: 키 맞추기
                                        , CM_RK_6     //                      : 틀렸을 때. 
                            };

//---------------------------------------------------------

public class GameManager : MonoBehaviour
{


    private static GameManager instance = null;


    [Tooltip("이 값은, 키 알아맞추기 모드에서도 사용한다. 랜덤하게 선택하는 키 값을 넣는다. 왜? 사운드 불러오기 등에도 이 값이 기준으로 사용되기 떄문.")]
    public eAVAILABLEKEYS eSelectedKey; 
    
    public eMUSICMODE eSelectedMusicMode;

    [Tooltip("키 알아맞추기 모드인지, 스케일모드 or 코드모드 분기 팝업에서 확인해야 해서.")]
    public bool bIsRecogKeysMode;


#region Score System Related variable declaration.

    private eSCORING_POLICY eSelectedScoringPolicy;
    private Dictionary<eSCORING_POLICY, Dictionary<eSCORING_CASEID, long>> dicScoringTable;

    public GameObject gmobjScorePanel; // 점수 표시 패널. 현재 활성화된 scene에 붙어 있는 점수 패널을 (각 scene의 start에서) 찾아서 붙인다. 

    // 최대치 넘는지 체크 필요함. 
    public long nl_NoteScore, nl_CodeScore;
    
    public List<GameObject> li_gmobj_CurrentlyExistingBricks;

#endregion

    void Awake()
    {

        if(instance == null)
        {

            // Ref. 1

            //이 클래스 인스턴스가 탄생했을 때 전역변수 instance에 게임매니저 인스턴스가 담겨있지 않다면, 자신을 넣어준다.
            instance = this;
            
            // Ref. 1
            //씬 전환이 되더라도 파괴되지 않게 한다.
            //gameObject만으로도 이 스크립트가 컴포넌트로서 붙어있는 Hierarchy상의 게임오브젝트라는 뜻이지만, 
            //나는 헷갈림 방지를 위해 this를 붙여주기도 한다.
            DontDestroyOnLoad(this.gameObject);

        }else
        {
            // Ref. 1
            //만약 씬 이동이 되었는데 그 씬에도 Hierarchy에 GameMgr이 존재할 수도 있다.
            //그럴 경우엔 이전 씬에서 사용하던 인스턴스를 계속 사용해주는 경우가 많은 것 같다.
            //그래서 이미 전역변수인 instance에 인스턴스가 존재한다면 자신(새로운 씬의 GameMgr)을 삭제해준다.
            Destroy(this.gameObject);

        }


    #region Score System Related variable initialization.
        
        this.gmobjScorePanel = null; // 점수 표시 패널. 현재 활성화된 scene에 붙어 있는 점수 패널을 (각 scene의 start에서) 찾아서 붙인다. 


        // 최대치 넘는지 체크 필요함. 
        this.nl_NoteScore = 0; //1234567;
        this.nl_CodeScore = 0; // 1234567;
        
        this.li_gmobj_CurrentlyExistingBricks = new List<GameObject>();
    #endregion


    }




    // Start is called before the first frame update
    void Start()
    {
        this.eSelectedMusicMode = eMUSICMODE.Code; // 의미는 없지만 그냥 초기값으로.
        this.bIsRecogKeysMode = false;


    #region Score System Related Data Set-up
        this.eSelectedScoringPolicy = eSCORING_POLICY.POLICY_1_BASIC; // 스코어링 정책은, 일단 기본으로. 23.08.03

        this.ScoreSystem_SetUpTheScoreTables();
    #endregion



    }


    // 컨텐츠 매니저 인스턴스에 접근할 수 있는 프로퍼티. static이므로 다른 클래스에서 맘껏 호출할 수 있다.
    public static GameManager Instance
    {
        get
        {
            if( instance == null )
            {
                return null;
            }
            return instance;
        }

    }


    void Update()
    {

        if(Application.platform == RuntimePlatform.Android)
        {

            if(Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

        }


    }

#region Score System related Methods

    private void ScoreSystem_SetUpTheScoreTables()
    {
        // policy 별로, 스코어 테이블 데이터를 (딕셔너리를) 셋업하는 함수. 

        // enum의 장점을 활용해서?
        // 아니지.. 케이스ID를 정확이 알고 넣어야 하므로.. 

        this.dicScoringTable = new  Dictionary<eSCORING_POLICY, Dictionary<eSCORING_CASEID, long>>();

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //===================================
        // Scoring System Policy, Basic.
        Dictionary<eSCORING_CASEID, long> dicPolicyBasic_ScoreTable = new Dictionary<eSCORING_CASEID, long>
                                                        {
                                                            // Score Mode: InTRo.
                                                             {eSCORING_CASEID.SM_ITR_0, 1}
                                                            ,{eSCORING_CASEID.SM_ITR_1, 3}
                                                            // Score Mode: Pick Note.
                                                            ,{eSCORING_CASEID.SM_PN_0, 1}
                                                            ,{eSCORING_CASEID.SM_PN_1, 2}
                                                            ,{eSCORING_CASEID.SM_PN_6, -1} // 23.08.15
                                                            // Score Mode: Pick Pattern Note.
                                                            ,{eSCORING_CASEID.SM_PPN_0, 1}
                                                            ,{eSCORING_CASEID.SM_PPN_1, 2} 
                                                            ,{eSCORING_CASEID.SM_PPN_6, -1} // 23.08.15
                                                            // Score Mode; Recognize Keys.
                                                            ,{eSCORING_CASEID.SM_RK_0, 1}
                                                            ,{eSCORING_CASEID.SM_RK_1, 3} 
                                                            ,{eSCORING_CASEID.SM_RK_6, -1}
                                                            //-------------------------------
                                                            // Code Mode: InTRo.
                                                            ,{eSCORING_CASEID.CM_ITR_0, 1}
                                                            ,{eSCORING_CASEID.CM_ITR_1, 1}
                                                            // Code Mode: Pick Number.
                                                            ,{eSCORING_CASEID.CM_PN_0, 1}
                                                            ,{eSCORING_CASEID.CM_PN_1, 2}
                                                            ,{eSCORING_CASEID.CM_PN_6, -1}
                                                            // Code Mode: Pick Pattern Number.
                                                            ,{eSCORING_CASEID.CM_PPN_0, 1}
                                                            ,{eSCORING_CASEID.CM_PPN_1, 2}
                                                            ,{eSCORING_CASEID.CM_PPN_6, -1}
                                                            // Code Mode: Match Sound.
                                                            ,{eSCORING_CASEID.CM_MS_0, 1}
                                                            ,{eSCORING_CASEID.CM_MS_1, 3}
                                                            ,{eSCORING_CASEID.CM_MS_6, -1}
                                                            // Code Mode; Recognize Keys.
                                                            ,{eSCORING_CASEID.CM_RK_0, 1}
                                                            ,{eSCORING_CASEID.CM_RK_1, 3} 
                                                            ,{eSCORING_CASEID.CM_RK_6, -1}
                                                        };

        this.dicScoringTable.Add(eSCORING_POLICY.POLICY_1_BASIC, dicPolicyBasic_ScoreTable);
        //===================================
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //===================================
        // Scoring System Policy, Aggressive. TENTATIVE!
        Dictionary<eSCORING_CASEID, long> dicPolicyAggressive_ScoreTable = new Dictionary<eSCORING_CASEID, long>
                                                        {
                                                            // Score Mode: InTRo.
                                                             {eSCORING_CASEID.SM_ITR_0, 1}
                                                            ,{eSCORING_CASEID.SM_ITR_1, 3}
                                                            // Score Mode: Pick Note.
                                                            ,{eSCORING_CASEID.SM_PN_0, 1}
                                                            ,{eSCORING_CASEID.SM_PN_1, 4}
                                                            ,{eSCORING_CASEID.SM_PN_6, -2} // Yes, it's an example.
                                                            // Score Mode: Pick Pattern Note.
                                                            ,{eSCORING_CASEID.SM_PPN_0, 1}
                                                            ,{eSCORING_CASEID.SM_PPN_1, 2} 
                                                            ,{eSCORING_CASEID.SM_PPN_6, -1} // 23.08.15
                                                            // Score Mode; Recognize Keys.
                                                            ,{eSCORING_CASEID.SM_RK_0, 1}
                                                            ,{eSCORING_CASEID.SM_RK_1, 3} 
                                                            ,{eSCORING_CASEID.SM_RK_6, -1}
                                                            //-------------------------------
                                                            // Code Mode: InTRo.
                                                            ,{eSCORING_CASEID.CM_ITR_0, 1}
                                                            ,{eSCORING_CASEID.CM_ITR_1, 1}
                                                            // Code Mode: Pick Number.
                                                            ,{eSCORING_CASEID.CM_PN_0, 1}
                                                            ,{eSCORING_CASEID.CM_PN_1, 2}
                                                            ,{eSCORING_CASEID.CM_PN_6, -1}
                                                            // Code Mode: Pick Pattern Number.
                                                            ,{eSCORING_CASEID.CM_PPN_0, 1}
                                                            ,{eSCORING_CASEID.CM_PPN_1, 2}
                                                            ,{eSCORING_CASEID.CM_PPN_6, -1}
                                                            // Code Mode: Match Sound.
                                                            ,{eSCORING_CASEID.CM_MS_0, 1}
                                                            ,{eSCORING_CASEID.CM_MS_1, 3}
                                                            ,{eSCORING_CASEID.CM_MS_6, -1}
                                                            // Code Mode; Recognize Keys.
                                                            ,{eSCORING_CASEID.CM_RK_0, 1}
                                                            ,{eSCORING_CASEID.CM_RK_1, 3} 
                                                            ,{eSCORING_CASEID.CM_RK_6, -1}
                                                        };

        this.dicScoringTable.Add(eSCORING_POLICY.POLICY_2_AGGRESSIVE, dicPolicyAggressive_ScoreTable); // TENTATIVE!
        //===================================
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


    }

    // 디스플레이 패널 싱글톤화. public void ScoreSystem_PleaseUpdateTheScore(GameObject gmobjDisplayPanel, eSCORING_CASEID eCaseID)
    public void ScoreSystem_PleaseUpdateTheScore(eSCORING_CASEID eCaseID)
    {
        // 뭐하는 함수?
        // 스코어를 획득/잃는 상황에서 호출되는 메써드. 
        
        // 현재 노트모드 or 코드모드 를 확인 
        // 케이스ID에 따라서, 해당 모드의 점수를 가/감 연산. 
        // 받은 패널 오프젝트의 스크립트 메써드 중, 화면 업데이트 메써드를 호출해 줌. 

        if( this.gmobjScorePanel == null ) return; // 실수 방지. 

        //===============================
        // 상황에 따른 스코어 계산: 가감.
        //===============================
        this.ScoreSystem_CalculateAndUpdate_Scores_byTheCaseID(eCaseID);

        //===============================
        // (현재 active 되어 있는 스코어 보드중)
        // note 점수 or code 점수 패널에 점수 리프레쉬. 
        //===============================
        this.gmobjScorePanel.GetComponent<Score_Panel_DisplayManager>().RefreshScores();


    }

    private void ScoreSystem_CalculateAndUpdate_Scores_byTheCaseID( eSCORING_CASEID eCaseID )
    {
        // 뭐하는 함수?
        //
        // 현재 (코드에서) 선택된 스코어링 폴리시를 확인해서. 
        // 
        // 케이스ID로 가/감할지 데이터를 가져오고, 
        // 그것을 GM의 싱글톤 스코어 변수에 업데이트 하는 함수. 

        long nlScoreAmount_toAddOrSub = this.dicScoringTable[ this.eSelectedScoringPolicy ][ eCaseID ];

        if( this.eSelectedMusicMode == eMUSICMODE.Scale )
        {
            this.nl_NoteScore += nlScoreAmount_toAddOrSub;
        }
        else if( this.eSelectedMusicMode == eMUSICMODE.Code )
        {
            this.nl_CodeScore += nlScoreAmount_toAddOrSub;
        }
        //else if( )
        //{
        //}
        else
        {
            ; // Do nothing?
            Debug.Assert(false, "Unknown Music Mode!"); // 이건 실제 빌드시에는 포함안됨. 
        }


        // Debug.Log 는 실제 빌드시에 포함된다. 그래서 성능이슈.. 
        // https://reoul.github.io/unity/unity-5/


    }

    public void ScoreSystem_Check_CurrentlyExistingBricks()
    {
        // 뭐하는 함수?
        // 현재 (모드에 상관없이) (연습브릭 및 퀴즈브릭으로)
        // 상판에 존재하는 모든 브릭들 (리스트 데이터에 담긴) 을 디버그 메시지로 표시하는 함수. 
        // 디버깅용. 
        // 
        //GameManager.Instance.ScoreSystem_PleaseUpdateTheScore( this.gmobjScorePanel, eSCORING_CASEID.SM_ITR_1 );
        Debug.Log($"Existing Bricks: {this.li_gmobj_CurrentlyExistingBricks.Count}");

        /*
        for(int liIdx=0; liIdx < this.li_gmobj_CurrentlyExistingBricks.Count; liIdx++)
        {
            //GameManager.Instance.li_gmobj_CurrentlyExistingBricks[liIdx].name;
            Debug.Log($"Name: {this.li_gmobj_CurrentlyExistingBricks[liIdx].name}");
        }
        */

        foreach(GameObject objs in this.li_gmobj_CurrentlyExistingBricks)
        {
            Debug.Log(objs.name);
        }

    }

    //public void ScoreSystem_ScaleMode_Intro_CheckAndApplyScore__BasicHarmonies(GameObject gmobjDisplayPanel)
    public void ScoreSystem_ScaleMode_Intro_CheckAndApplyScore__BasicHarmonies()
    {
        // 뭐하는 함수?
        // 스케일 모드, 인트로(연습) 모드에서, 
        // 기본 화음을 쌓았는지 확인하고, 
        // 쌓았다면 점수를 주는 함수. 

        if( this.gmobjScorePanel == null ) return; // 실수 방지. 

        bool[] bWhaEum = new bool[] {false, false, false, false, false, false, false, false}; // 화음번호가 1부터 시작하므로, 7개+1개 .

        Debug.Log($"Score CnA: Existing Bricks: {this.li_gmobj_CurrentlyExistingBricks.Count}");


        foreach(GameObject objs in this.li_gmobj_CurrentlyExistingBricks)
        {
            Debug.Log($"Score CnA, Existing obj: {objs.name}");

            string sObjName = objs.name;

            // 이런 이름임. 
            // instScaleBrick_C_C4
            // 01234567890123456
            string sObjName_NoteOnly = sObjName.Substring(17); // 문자 열 끝까지.

            string sParsedPinanoKey = ContentsManager.Instance.ParsingTheTappedPianoKey(sObjName_NoteOnly); // e.g. C4는 C로, D4b 은 Db로 변환해 준다.

            ePIANOKEYS eTappedPianoKey = (ePIANOKEYS)System.Enum.Parse(typeof(ePIANOKEYS), sParsedPinanoKey);

            int nCorrespondentKeyValue = ContentsManager.Instance.dicScale_byKeyAndPianoKeys[GameManager.Instance.eSelectedKey][eTappedPianoKey];

            Debug.Log(sObjName_NoteOnly + ", " + nCorrespondentKeyValue.ToString() );

            bWhaEum[nCorrespondentKeyValue] = true; // 이걸 인덱스로 화음 번호를 표현!
        }
        
        //==================================
        // 1도 화음 체크. 
        //==================================        
        if(     (bWhaEum[1] == true)
            &&  (bWhaEum[2] ==  false)
            &&  (bWhaEum[3] == true)
            &&  (bWhaEum[4] ==  false)
            &&  (bWhaEum[5] == true)
            &&  (bWhaEum[6] ==  false)
            &&  (bWhaEum[7] ==  false)
        )
        {        
            if(Application.isEditor) Debug.Log("1 3 5 Harmony!!!"); // 도미솔

          // 화음 해당음 연타 치팅 ^^ 을 방지하려면!
          if( this.li_gmobj_CurrentlyExistingBricks.Count == 3 )
            this.ScoreSystem_PleaseUpdateTheScore( eSCORING_CASEID.SM_ITR_1 );
        }

        //==================================
        // 4도 화음 체크. 
        //==================================        
        if(     (bWhaEum[1] == true)
            &&  (bWhaEum[2] ==  false)
            &&  (bWhaEum[3] ==  false)
            &&  (bWhaEum[4] == true)
            &&  (bWhaEum[5] ==  false)
            &&  (bWhaEum[6] == true)
            &&  (bWhaEum[7] ==  false)
        )
        {        
            if(Application.isEditor) Debug.Log("4 6 1 Harmony!!!"); // 파라도

          // 화음 해당음 연타 치팅 ^^ 을 방지하려면!
          if( this.li_gmobj_CurrentlyExistingBricks.Count == 3 )
            this.ScoreSystem_PleaseUpdateTheScore( eSCORING_CASEID.SM_ITR_1 );
        }

        //==================================
        // 5도 화음 체크. 
        //==================================        
        if(     (bWhaEum[1] ==  false)
            &&  (bWhaEum[2] == true)
            &&  (bWhaEum[3] ==  false)
            &&  (bWhaEum[4] ==  false)
            &&  (bWhaEum[5] == true)
            &&  (bWhaEum[6] ==  false)
            &&  (bWhaEum[7] == true)
        )
        {        
            if(Application.isEditor) Debug.Log("5 7 2 Harmony!!!"); // 솔시레

          // 화음 해당음 연타 치팅 ^^ 을 방지하려면!
          if( this.li_gmobj_CurrentlyExistingBricks.Count == 3 )
            this.ScoreSystem_PleaseUpdateTheScore( eSCORING_CASEID.SM_ITR_1 );
        }

/*
        string sParsedPinanoKey = ContentsManager.Instance.ParsingTheTappedPianoKey(this.name); // e.g. C4는 C로, D4b 은 Db로 변환해 준다. 

        ePIANOKEYS eTappedPianoKey = (ePIANOKEYS)System.Enum.Parse(typeof(ePIANOKEYS), sParsedPinanoKey);
        
        int nCorrespondentKeyValue = ContentsManager.Instance.dicScale_byKeyAndPianoKeys[GameManager.Instance.eSelectedKey][eTappedPianoKey];
*/
    }

#endregion


}
