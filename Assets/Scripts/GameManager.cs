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

using TMPro;

// 나도 사용. #region EthanKorTest
using System;
using System.Linq;
using System.IO; // 여기서 저장을 하므로, 여기서 선언.. 
//#endregion


//---------------------------------------------------------
// [각 scene (레벨의) 플레이 매니져를 상태머신으로 구현하기 위해.]
// : 스파우닝 시점과, 사라지는 효과 사라지는 시점에 따라, out of index 되거나, 연타시에 중복으로 인스턴시에잇되는 문제 해결을 위해.
public enum ePLAY_STATUS_FORLEVEL { START, SPAWN, QUESTIONED, ANSWERD_CORRECTLY, ANSWERED_WRONG, DESTROYING };

//---------------------------------------------------------

//---------------------------------------------------------
// [스코어 시스템. Scoring Policy]
// : 스코어링 폴리시를 몇가지 정할 수 있어야.. 스코어링에 의한 동기부여 효과는 처음 해보므로..
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

    private Dictionary<eSCORING_CASEID, string> dicScoredReasonMessageTable; // 나중에는 다국어용으로 json 만들어서 불러서 딕셔너리 세팅해야?


    public GameObject gmobjScorePanel; // 점수 표시 패널. 현재 활성화된 scene에 붙어 있는 점수 패널을 (각 scene의 start에서) 찾아서 붙인다. 

    // 최대치 넘는지 체크 필요함. 
    public long nl_NoteScore, nl_CodeScore;
    
    public List<GameObject> li_gmobj_CurrentlyExistingBricks;

#endregion

#if ETHAN_KOR_TEST // https://docs.unity3d.com/kr/2021.2/Manual/PlatformDependentCompilation.html 플레이어 설정의 Other Settings 패널을 연 후 Scripting Define Symbols 텍스트 상자로 이동하시기 바랍니다. 
#region EthanKorTest
    public static readonly string sChoSung = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";
    public static readonly string sJungSung = "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ";
    public static readonly string sJONGSung = " ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ";

    public static readonly string[] sArCho = {"ㄱ","ㄲ","ㄴ","ㄷ","ㄸ","ㄹ","ㅁ","ㅂ","ㅃ","ㅅ","ㅆ","ㅇ","ㅈ","ㅉ","ㅊ","ㅋ","ㅌ","ㅍ","ㅎ"};
    public static readonly string[] sArJung = {"ㅏ","ㅐ","ㅑ","ㅒ","ㅓ","ㅔ","ㅕ","ㅖ","ㅗ","ㅘ","ㅙ","ㅚ","ㅛ","ㅜ","ㅝ","ㅞ","ㅟ","ㅠ","ㅡ","ㅢ","ㅣ"};
    public static readonly string[] sArJONG = {" ","ㄱ","ㄲ","ㄳ","ㄴ","ㄵ","ㄶ","ㄷ","ㄹ","ㄺ","ㄻ","ㄼ","ㄽ","ㄾ","ㄿ","ㅀ","ㅁ","ㅂ","ㅄ","ㅅ","ㅆ","ㅇ","ㅈ","ㅊ","ㅋ","ㅌ","ㅍ","ㅎ"};
    //public static readonly string[] sArJong = " ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ";

    // 이든이가 배열한 순서.
    public static readonly string[] sAr_ET_Cho = {"ㄱ","ㄲ","ㄴ","ㄷ","ㄸ","ㄹ","ㅁ","ㅂ","ㅃ","ㅅ","ㅆ","ㅇ","ㅈ","ㅉ","ㅊ","ㅋ","ㅌ","ㅍ","ㅎ"};
    public static readonly string[] sAr_ET_Jung = {"ㅏ","ㅑ","ㅓ","ㅕ","ㅗ","ㅛ","ㅜ","ㅠ","ㅡ","ㅣ","ㅐ","ㅒ","ㅔ","ㅖ","ㅢ","ㅚ","ㅘ","ㅝ","ㅙ","ㅟ","ㅞ"};
    public static readonly string[] sAr_ET_JONG = {" ","ㄱ","ㄲ","ㄳ","ㄴ","ㄶ","ㄵ","ㄷ","ㄹ","ㄺ","ㄻ","ㄼ","ㄽ","ㄾ","ㄿ","ㅀ","ㅁ","ㅂ","ㅄ","ㅅ","ㅆ","ㅇ","ㅈ","ㅊ","ㅋ","ㅌ","ㅍ","ㅎ"};

    private static readonly ushort 유니코드첫한국어 = 0xAC00;
    // 워닝 제거용. private static readonly ushort 유니코드마지막한국어 = 0xD79F;

//{"ㅏ","ㅑ","ㅓ","ㅕ","ㅗ","ㅛ","ㅜ","ㅠ","ㅡ","ㅣ","ㅐ","ㅒ","ㅔ","ㅖ","ㅢ","ㅚ","ㅝ","ㅙ"};
//{"ㅏ","ㅐ","ㅑ","ㅒ","ㅓ","ㅔ","ㅕ","ㅖ","ㅗ","ㅘ","ㅙ","ㅚ","ㅛ","ㅜ","ㅝ","ㅞ","ㅟ","ㅠ","ㅡ","ㅢ","ㅣ"};
//{"ㅏ","ㅑ","ㅓ","ㅕ","ㅗ","ㅛ","ㅜ","ㅠ","ㅡ","ㅣ","ㅐ","ㅒ","ㅔ","ㅖ","ㅢ","ㅚ","ㅘ","ㅝ","ㅙ","ㅟ","ㅞ"};

    public string sLocalSavePath;
    public string sFilename;
#endregion
#endif

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

#if ETHAN_KOR_TEST
#region EthanKorTest
        sLocalSavePath = Application.persistentDataPath + "/"; // Ref. https://coding-of-today.tistory.com/178?category=984992 

        //sFilename = "versesOfLifeData.json";
        // 22.11.08. 앱이름, MyVerses로 바꾸기 위해. 
        this.sFilename = "Ethan_Data.txt";


        //if(Application.isEditor) this.Ethan_Kor_Test(); // 디파인을 넣어도, 빌드를 하지 않는 이상, 에디터에서는 실행된다. 그래서 처음에 시간이 너무 걸려서 일단 막음. 
#endregion
#endif
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

        this.dicScoredReasonMessageTable = new Dictionary<eSCORING_CASEID, string>
                                                        {
                                                            // Score Mode: InTRo.
                                                             {eSCORING_CASEID.SM_ITR_0, "Tap & Listen!"}
                                                            ,{eSCORING_CASEID.SM_ITR_1, "Yes, that's a harmony!"}
                                                            // Score Mode: Pick Note.
                                                            ,{eSCORING_CASEID.SM_PN_0, "Correct!"}
                                                            ,{eSCORING_CASEID.SM_PN_1, "Excellent!"}
                                                            ,{eSCORING_CASEID.SM_PN_6, "<color=#ff0000ff>Wrong...</color>"} // Yes, it's an example.
                                                            // Score Mode: Pick Pattern Note.
                                                            ,{eSCORING_CASEID.SM_PPN_0, "1"}
                                                            ,{eSCORING_CASEID.SM_PPN_1, "2"} 
                                                            ,{eSCORING_CASEID.SM_PPN_6, "-1"} // 23.08.15
                                                            // Score Mode; Recognize Keys.
                                                            ,{eSCORING_CASEID.SM_RK_0, "1"}
                                                            ,{eSCORING_CASEID.SM_RK_1, "3"} 
                                                            ,{eSCORING_CASEID.SM_RK_6, "-1"}
                                                            //-------------------------------
                                                            // Code Mode: InTRo.
                                                            ,{eSCORING_CASEID.CM_ITR_0, "1"}
                                                            ,{eSCORING_CASEID.CM_ITR_1, "1"}
                                                            // Code Mode: Pick Number.
                                                            ,{eSCORING_CASEID.CM_PN_0, "Correct!"}
                                                            ,{eSCORING_CASEID.CM_PN_1, "Excellent!"}
                                                            ,{eSCORING_CASEID.CM_PN_6, "<color=#ff0000ff>Wrong...</color>"}
                                                            // Code Mode: Pick Pattern Number.
                                                            ,{eSCORING_CASEID.CM_PPN_0, "1"}
                                                            ,{eSCORING_CASEID.CM_PPN_1, "2"}
                                                            ,{eSCORING_CASEID.CM_PPN_6, "-1"}
                                                            // Code Mode: Match Sound.
                                                            ,{eSCORING_CASEID.CM_MS_0, "Correct!"}
                                                            ,{eSCORING_CASEID.CM_MS_1, "Excellent!"}
                                                            ,{eSCORING_CASEID.CM_MS_6, "<color=#ff0000ff>Wrong...</color>"}
                                                            // Code Mode; Recognize Keys.
                                                            ,{eSCORING_CASEID.CM_RK_0, "1"}
                                                            ,{eSCORING_CASEID.CM_RK_1, "3"} 
                                                            ,{eSCORING_CASEID.CM_RK_6, "-1"}
                                                        };




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
        //this.gmobjScorePanel.GetComponent<Score_Panel_DisplayManager>().RefreshScores();
        this.gmobjScorePanel.GetComponent<Score_Panel_DisplayManager>().RefreshScores( this.dicScoredReasonMessageTable[eCaseID] );


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
        Debug.Log($"=======\nExisting Bricks: {this.li_gmobj_CurrentlyExistingBricks.Count}");

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

        Debug.Log("=======");

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

#region On-Screen debug function realted.

public GameObject DebugMsgOnScreen_Setup()
{
    GameObject gmobjSearched_debugTextObj = GameObject.Find("LevelGuideText_typeA");

    return gmobjSearched_debugTextObj;
}

public void DebugMsgOnScreen(GameObject gmobjTargetTextObj, string sWhatToDisplay)
{
    // 뭐하는 함수?
    // 각 개별의 scene 플레이매니져 스크립트에서 호출되어서,
    // 인자로 받은 그 scene의 디버그 메시지 창에 념겨받은 디버그 텍스트를 표시한다. 

    // 왜 이렇게 별도로 만듦?
    // 나중에 릴리즈 버전 만들때는 이것을 그냥 return 으로 끝낼 수 있으니. 

    if( gmobjTargetTextObj != null)
        gmobjTargetTextObj.GetComponent<TextMeshProUGUI>().text = "<color=#ff0000ff>" + sWhatToDisplay + "</color>";
    




}

#endregion

#if ETHAN_KOR_TEST
#region EthanKorTest
    private void Ethan_Kor_Test()
    {

        char cResult;

        //cResult = 글자만들기( sChoSung[0], sJungSung[0], sJONGSung[0]);
        cResult = Jo_MakeHangul( "ㄷ", "ㅏ", "ㄹ");

        //cResult = 글자만들기( sChoSung.Substring(2, 1), sJungSung.Substring(1, 1), sJONGSung.Substring(1, 1));


        //Debug.Log("ETHAN 1: " + sChoSung.IndexOf("ㅁ").ToString());

        Debug.Log("ETHAN 2: " + cResult.ToString() +", "+ sChoSung.Length.ToString() + ", " + sChoSung.Substring(2, 1));

/*
        for(int ChoIdx=0; ChoIdx<sChoSung.Length; ChoIdx++ )
        {
            for(int JungIdx=0; JungIdx<sJungSung.Length; JungIdx++ )
            {
                char cR = 글자만들기( sChoSung.Substring(ChoIdx, 1), 
                                    sJungSung.Substring(JungIdx, 1), 
                                    sJONGSung.Substring(0, 1)
                                    );
                Debug.Log("ETHAN: " + cR.ToString() );
            }
        }
    */    


        if((sArCho.Length != sAr_ET_Cho.Length) || (sArJung.Length != sAr_ET_Jung.Length) || (sArJONG.Length != sAr_ET_JONG.Length) )
        {
            Debug.Log("ETHAN: Check the length!!!");
        //    return;
        }

        /*
        //=======================================
        // 유니코드 숫자 증가 기준으로 뽑기. 
        for(int ChoIdx=0; ChoIdx<sArCho.Length; ChoIdx++ )
        {
            for(int JungIdx=0; JungIdx<sArJung.Length; JungIdx++ )
            {
                //int a, b, c;

                int nUcode = 유니코드첫한국어 + (ChoIdx * 21 + JungIdx) * 28 + 0;
                char cTemp = Convert.ToChar(nUcode);

                Debug.Log("ETHAN: " + cTemp.ToString() );
            }
        }        
        //=======================================
        */

        string sDataToSave = null;
        string sBreakChar = "\n";
        //=======================================
        // 이든이가 정의한, 기준으로 뽑기. 
        // 초성 자음 단독, 먼저 해달라고 해서.. 
        for(int ChoIdx=0; ChoIdx<sAr_ET_Cho.Length; ChoIdx++ )
        {
            sDataToSave += (sAr_ET_Cho[ChoIdx] + sBreakChar);
        }

        // 전체 구성글자. 
        for(int ChoIdx=0; ChoIdx<sAr_ET_Cho.Length; ChoIdx++ )
        {
            for(int JungIdx=0; JungIdx<sAr_ET_Jung.Length; JungIdx++ )
            {
                for(int JONGIdx=0; JONGIdx<sAr_ET_JONG.Length; JONGIdx++ )
                {
                    char cTemp = Jo_MakeHangul(
                                                sAr_ET_Cho[ChoIdx]
                                                , sAr_ET_Jung[JungIdx]
                                                , sAr_ET_JONG[JONGIdx] // tentative.
                                                );

                    Debug.Log("ETHAN: " + cTemp.ToString() );

                    sDataToSave += (cTemp.ToString() + sBreakChar);
                }
            }
        }        
        //=======================================

        //=======================================
        // 파일로 저장.

        if( SaveDataToTheLocalDeviceAsJsonFile( sLocalSavePath + sFilename, sDataToSave ) )
        {
            Debug.Log("ETHAN: File saving has succeeded!");
        }else
        {
            Debug.Log("ETHAN: File saving has failed...");
        }

    }

    private char Jo_MakeHangul(string Cho, string Jung, string JONG)
    {

        //int indexA = Array.IndexOf(strArr, "A");

        int 초성위치, 중성위치, 종성위치;
        int 유니코드;

        초성위치 = Array.IndexOf(sArCho, Cho);    // sChoSung 위치
        중성위치 = Array.IndexOf(sArJung, Jung);   // sJungSung 위치
        종성위치 = Array.IndexOf(sArJONG, JONG);   // sJONGSung 위치

        Debug.Log($"{Cho},{Jung},{JONG} = {초성위치}, {중성위치}, {종성위치}");

        // 앞서 만들어 낸 계산식
        유니코드 = 유니코드첫한국어 + (초성위치 * 21 + 중성위치) * 28 + 종성위치;

        // 코드값을 문자로 변환
        char 임시 = Convert.ToChar(유니코드);

        return 임시;

    }


    private char 글자만들기(string Cho, string Jung, string JONG)
    {
        int 초성위치, 중성위치, 종성위치;
        int 유니코드;

        초성위치 = sChoSung.IndexOf(Cho);    // sChoSung 위치
        중성위치 = sJungSung.IndexOf(Jung);   // sJungSung 위치
        종성위치 = sJONGSung.IndexOf(JONG);   // sJONGSung 위치

        Debug.Log($"{Cho},{Jung},{JONG} = {초성위치}, {중성위치}, {종성위치}");

        // 앞서 만들어 낸 계산식
        유니코드 = 유니코드첫한국어 + (초성위치 * 21 + 중성위치) * 28 + 종성위치;

        // 코드값을 문자로 변환
        char 임시 = Convert.ToChar(유니코드);

        return 임시;
    }

    public bool SaveDataToTheLocalDeviceAsJsonFile(string sPathAndJsonFilename_withExtention, string sDataToSave_JsonFormat)
    {

        // 뭐하는 함수? 함수 기능 수정. 23.02.06
        // 개인 묶음/소그룹 묶음 인지를 나타내는 싱글톤 값을 확인해서, 
        // 어떤 파일명 (개인 묶음용 소그룹 묶음용 ) 으로 저장할지 판단해서 파일로 저장. 
        // 이게.. 
        // 이 함수는, 암송 메인 부터, 구절 CRUD (개인 및 소그룹도?) 에서 널리 쓰이므로.. 
        // 이렇게 1원화 하고 플래그봐서 하도록 수정. 

        //------------------------------------------------------
        // 파일로 저장한다. : 소그룹 묶음을 소그룹 묶음 데이터의 (정해진) 파일이름으로. 
        //------------------------------------------------------
        // 파일로 저장한다. : 개인 묶음을 개인 묶음 데이터의 (정해진) 파일이름으로. 

        /* 23.02.13
        //string sPathAndJsonFilename_withExtention = sLocalSavePath + sFilename;
        string sPathAndJsonFilename_withExtention = null;

        //----------------------------
        // 모드에 따라서 엑세스할 파일 이름 결정. 
        if( GameManager.Instance.bIsItPersonalList == true )
        {
            sPathAndJsonFilename_withExtention = sLocalSavePath + sFilename;

        }else
        {
            sPathAndJsonFilename_withExtention = sLocalSavePath + sFilename_SharedVersesData;

        }
        */

        

        //----------------------------
        // 트라이 캐치로 파일 오퍼레이션.
        try
        {
            #if UNITY_EDITOR || UNITY_ANDROID
                File.WriteAllText(sPathAndJsonFilename_withExtention, sDataToSave_JsonFormat);

            #elif UNITY_ANDROID
                File.WriteAllText(sPathAndJsonFilename_withExtention, sDataToSave_JsonFormat);

            #elif UNITY_IOS
                File.WriteAllText(sPathAndJsonFilename_withExtention, sDataToSave_JsonFormat); // IOS 출시는 신중하게..
                if(Application.isEditor) Debug.Log("Unity iPhone");

            #else
                File.WriteAllText(sPathAndJsonFilename_withExtention, sDataToSave_JsonFormat);

            #endif

            /* 이제 팝업쓰면서 일단 없애기
            //------------------------------------------------------
            // 디버그 메시지.             
            GameManager.Instance.MOBILE_SCREEN_Debug("Saved in the local folder: "+ sPathAndJsonFilename_withExtention);
            Debug.Log("OK: File saving is succeeded to this path with this name: " + sPathAndJsonFilename_withExtention);
            */

            //GameManager.Instance.bSemaphore_FileWritingIsInProgress = false;
            return true;

        }catch(Exception eFilesavingError)
        {

            if(Application.isEditor)
            {
                //string sTempDebugMsg = "ERROR: Failed to save the file on the device: " + eFilesavingError;
                string sTempDebugMsg = "ERROR: Failed to save the file on the device. The path and filename:  " + sPathAndJsonFilename_withExtention
                                            + " The detailed error message: " + eFilesavingError;

                /* 이제 팝업쓰면서 일단 없애기            
                GameManager.Instance.MOBILE_SCREEN_Debug(sTempDebugMsg);
                */

                Debug.Log(sTempDebugMsg);

                Debug.Assert(false, sTempDebugMsg);
            }

            // 유니티, 에러 팝업 띄우기 찾아서 넣기. 

            // 팝업메시지.. 는, 부모 오브젝트가 있어야 하므로, 각각의 씬에서~~

            //GameManager.Instance.bSemaphore_FileWritingIsInProgress = false;
            return false;

        }

    }

#endregion
#endif

}
