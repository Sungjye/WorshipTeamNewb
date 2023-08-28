//=====================================================================================
//
// 주님, 이렇게 스케일 모드도 퀴즈 레벨을 시작할 수 있게 해 주셔서 감사합니다!!!
//
// 현재 이 레벨의 플레이를 주관하는 스크립트.
// 해당 레벨: PickNote : 건반의 음 브릭의 소리를 듣고 맞는 그 음의 건반을 '구체적으로'  탭해서 맞추는 레벨. 
//                    (구체적 이란, C 가 아니라, C4)
//
// 하는일
// > 시작하면 랜덤하게 브릭 1개를 생성.
// > 사용자가 맞추면 이 브릭을 왼쪽으로 ‘쌩’ 이동하고
// > 사용자가 틀리면 계속 두기. 
// > 사용자가 맞추고 이 브릭이 왼쪽으로 이동하여 카메라 시야에서 사라지면, 새로운 브릭을 생성
// > (TBD) Score 와 Combo 를 표시해 주기. 
//
// 2023.07.18. sjjo. Initial.
// 2023.08.21. sjjo. 전반적인 구조의 문제를 근본적으로 해결하기 위해 FSM 을 적용해서, 생성, 맞다/틀리다 처리, 새브릭스파운 을 처리. 주님께서 지혜와 마음 주셔서, 할 수 있었습니다! 감사합니다, 모든 지혜의 근원이신 예수님!
// 
//=====================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class ScaleMode_Level_PickNote_PlayManager : MonoBehaviour
{
    // 사운드 브릭이 될 수도 있고, 글자 브릭이 될 수도 있고 그림 브릭이 될 수도 있고..
    public GameObject gmobjQuizBrickPrefab; // 지금 이 레벨(PickNote)의 경우는 사운드 브릭

    //public int nQuizTotalCount; // 있다 사용하겠지만..  어찌면, 전체를 주관하는 GameManager에 두어야 할지도?

    private GameObject gmobjCurrentBrick; // 현재 생성되어 있는 브릭


    private ePLAY_STATUS_FORLEVEL ePlay_StateMachine;

    void Awake()
    {
        this.ePlay_StateMachine = ePLAY_STATUS_FORLEVEL.START;
        //this.SpawnNewBrick();

        // 현재 이 scene에서 활성화된 스코어 패널을 찾아서 넣어준다. 
        GameManager.Instance.gmobjScorePanel = GameObject.Find("Panel_ScoreDisplay"); 
    }

    void FixedUpdate()
    {
        this.Tick_TheStateMachine();
    }

    #region FSM related methods.
    // 음.. FSM 사용안하고 했었는데.. 이렇게 하면 delay 를 이리저리 주는 사이나, 
    // 여기 저기 시간의 구멍에서 (막 들어오는) 사용자의 입력과 스파운 사이에서.. 여러개가 막 스파운되거나, out of index 등이 발생한다. (pattern 모드인 경우)
    // 그래서 어쩔수 없이 이제야 FSM을 사용해서 다시 구현.. 23.08.21
    // 주님, 문서화의 마음과 구현할 수 있는 지혜를 주셔서 감사합니다!

    private void Tick_TheStateMachine()
    {
        // 상태머신 틱틱 돌리기. 

        // 참조: 구글 슬라이드, 230719_찬양팀막내_TechDoc_v01
        // PlayerManager 구조

        switch( this.ePlay_StateMachine )
        {
            case ePLAY_STATUS_FORLEVEL.START:
                if( FSM_NoBrickExist() == true ) this.ePlay_StateMachine = ePLAY_STATUS_FORLEVEL.SPAWN;
                //else ; Stay this state.
                break;
            case ePLAY_STATUS_FORLEVEL.SPAWN:
                FSM_SpawnNewBrick();

                if( FSM_CheckTheTargetNumberOfBricksExists() == true ) this.ePlay_StateMachine = ePLAY_STATUS_FORLEVEL.QUESTIONED;
                //else ; Stay this state.
                break;
            case ePLAY_STATUS_FORLEVEL.QUESTIONED:
                // 사용자가 키 탭 할 때마다, 여기의 체크 함수가 호출될 테고.. 거기서 검사 및 set 할 테고..

                if( FSM_CheckTheTargetBrick_setAsCorrect() == true ) this.ePlay_StateMachine = ePLAY_STATUS_FORLEVEL.ANSWERD_CORRECTLY;
                // else // 틀린 경우에 싹 날아가게 할거면 여기에 별도 처리.
                break;
            case ePLAY_STATUS_FORLEVEL.ANSWERD_CORRECTLY:
                // 음.. 뭐 해당 인스턴스 브릭에서 이미 사라지는 동작이 시작되었으므로.. 바로..
                this.ePlay_StateMachine = ePLAY_STATUS_FORLEVEL.DESTROYING;
                break;
            case ePLAY_STATUS_FORLEVEL.ANSWERED_WRONG:
                // 틀린 경우에 싹 날아가게 할거면 여기에 별도 처리.
                break;
            case ePLAY_STATUS_FORLEVEL.DESTROYING:
                // if( FSM_DoesAnyBrickExists() == true ) this.ePlay_StateMachine = ePLAY_STATUS_FORLEVEL.START;
                // 음.. 어차피 START 스테이트 에서 동일하게 브릭이 하나도 없을 때까지, 즉 디스트로잉이 끝날 떄까지 기다리므로.. 
                // 바로 가자. 역시 컨셉과 코드의 차이..
                this.ePlay_StateMachine = ePLAY_STATUS_FORLEVEL.START;
                break;
            default:
                this.ePlay_StateMachine = ePLAY_STATUS_FORLEVEL.START;
                break;
        }
    }

    private bool FSM_NoBrickExist()
    {
        // 현재 이판에(scene에.. ^^) 질문 브릭이 존재하는가?

        bool bResult = false;
        // 이걸로 체크 됨?
        if( this.gmobjCurrentBrick == null ) bResult = true; 
        
        if(Application.isEditor) Debug.Log($"FSM_NoBrickExist: {bResult}");

        return bResult;
    }

    private void FSM_SpawnNewBrick()
    {

        // 이제 기다릴 필요 없음. 앞의 START 상태에서 브릭 (다) 사라질 떄까지 기다렸으므로. 
        this.SpawnNewBrick();

    }

    private bool FSM_CheckTheTargetNumberOfBricksExists()
    {
        // 이 판에서 원하는 개수 만큼의 질문 브릭이 생성되어 있는가?

        bool bResult = false;

        // 이걸로 체크 됨?
        if( this.gmobjCurrentBrick != null ) bResult = true; 
        
        if(Application.isEditor) Debug.Log($"FSM_CheckTheTargetNumberOfBricksExists: {bResult}");

        return bResult;
    }

    private bool FSM_CheckTheTargetBrick_setAsCorrect()
    {
        // 현재 타겟인 질문 브릭이 '맞았음' 으로 세팅 되었는가?

        if( this.gmobjCurrentBrick == null ) return false; // 혹시나..

        return this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeB_Control>().bSetMeCorrectOnce;
    }

    private void SpawnNewBrick()
    {
        this.gmobjCurrentBrick = Instantiate( this.gmobjQuizBrickPrefab, new Vector3(0f, 6f, 0f), Quaternion.identity );

        //--------------------------------------------------
        // 오브젝트 자체의 이름을 정하기
        // 이걸, 구제적인 노트번호 D4b 이렇게 해야.. 다른 레벨에서의 생성 규칙도 일관성 있어지고.. 
        //And the note name should be a absolute name among the piano keyboard. 
        // 
        // Refer to. CodeMode_Level_0_Control.cs > OnMouseDown()

        // Tentative. Randomly chosen note among the available keys in the piano keyboard which are defined in ContentsManager.
        //ePIANOKEYS eQuizScaleNote = ePIANOKEYS.Db; // Randomly or a certain pattern.
        // ePIANOKEYS eQuizScaleNote = GetOneBrickName_inePIANOKEYS__Randomly();
        string sQuizScaleNote =  ContentsManager.Instance.GetOneBrickName_inePIANOKEYS__Randomly();

        //-------------------------------------------
        // 인스턴시에잇된 오브젝트 자체의 이름 정하기:
        // 인스턴시에잇된 (하늘에서 떨어지는) 스케일 브릭 + 현재선택된 키, 사용자가 누른 어떤 키인지를 나타내는 값.        
        //this.gmobjCurrentBrick.name = "instScaleBrick_" + GameManager.Instance.eSelectedKey.ToString()+ "_" + eQuizScaleNote.ToString();
        // 스케일 모드는, 악보를 표시할 때 빼고는 그냥 이름 inst 이런거 붙이지 말고 이름그대로 사용하자! 
        // 스케일을 섞어서, 브릭을 생성해 낼 일도 없을듯.. 
        // 나중에 악보 표시나, 소리 매칭할 때, 컨트롤 스크립트에서 현재 선택된 key (조) 를 확인해서 서브루틴 돌리기로.. 
        //this.gmobjCurrentBrick.name = "instScaleBrick_" + GameManager.Instance.eSelectedKey.ToString()+ "_" + eQuizScaleNote.ToString();
        this.gmobjCurrentBrick.name = sQuizScaleNote;

        //--------------------------------------------------
        // 오브젝트 자식중, 이미지 브릭의 머티리얼(이미지) 을 정하기.

        //--------------------------------------------------
        // Set the text of the child object for this instantiated object 
        // with searching the Dictionary according to the selected code-key. (e.g. C key)
        /*
        this.gmobjCurrentBrick.transform.GetChild(1).gameObject.GetComponent<TextMeshPro>().text 
                    = ContentsManager.Instance.dicCode_byKeyAndDoNum[GameManager.Instance.eSelectedKey][eQuizCodeNumber] ;
        */
        // Nope. use the method in the spawned object. 
        // Type B sound birck is for a scale mode. 23.07.18
        this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeB_Control>().SetMe_asQuestion();


    }

    #endregion


    #region Public Methods which is/are called from User's tapping control scripts.
    public void CheckIfInputIsCorrect(string sTappedKeyObjectName)
    {
        // This function is called from the each key tap of a user.
        //
        // Functions
        // > Compare the tapped key object name with the quiz brick name
        // > Set the quiz brick components according to the result.


        //==================================================================
        // State Machine 이 QUESTIONED 모드 일 떄만 브릭이 존재하는 것이므로, 
        // 이 떄에만 체크를 한다. 
        if( this.ePlay_StateMachine != ePLAY_STATUS_FORLEVEL.QUESTIONED) return; // 23.08.21 FSM 문서화 및 코딩한 이후...

        // 탭된 (버튼 역할인) 3D 오브젝트의 이름 자체가, ePIANOKEYS 타입의 이름. 
        // 그래서 변환해서 바로 인덱싱 하면 됨.. 이라고 생각할 수 있지만, 

        // 피아노 키 값이 나중에 뭐가될지 C# or Db 뭐가 될지 모르므로.. 있는지 확인해야 함. 
        // ref. https://learn.microsoft.com/ko-kr/dotnet/api/system.enum.isdefined?view=net-7.0 
        //---------------------------------------------
        // 지금 탭된 피아노 키가 무엇인지 확인부터해서.. 
        // 어떤 것을 인스턴시에잇 할지 결정!
        string sParsedPinanoKey = ContentsManager.Instance.ParsingTheTappedPianoKey(sTappedKeyObjectName); // e.g. C4는 C로, D4b 은 Db로 변환해 준다. 
        
        //string sTappedNoteName_inTermsOfTheSelectedKey;

    //if( this.li_gmobj_CurrentlyExistingBricks.Count == 1 ) // 연타하면 브릭이 아직 남아 있는데도 마구 생성되는 문제를 막기 위해. 23.08.21
    // 아니다, 바닥에 또는 어딘가에 "착지" 한 다음에 맞추든 틀리든 할수 있게 해야. 
    //if( this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeB_Control>().bAmI_Landed == true )
    // 다시. 
    //if( GameManager.Instance.li_gmobj_CurrentlyExistingBricks.Count <= 1 ) // 연타하면 브릭이 아직 남아 있는데도 마구 생성되는 문제를 막기 위해. 23.08.21  음.. 작을수는 없긴 한데..
    //{
        if( System.Enum.IsDefined( typeof(ePIANOKEYS),  sParsedPinanoKey ) ) 
        {
            // 값이 enum 정의에 존재하면 
            // 일단, 체크를 해볼 수 있는 정상적인 (피아노) 키패드 입력값이라는 뜻!
            // 정상적이지 않은 것은? 예를 들어, D4b 대신 C4# 뭐 이렇게 피아노 건반 이름을 개발자가 실수로 잘못 만들고 사용자가 탭한 경우. 

            // 자, 정상적인 키 입력이었다면, 넘어온 오브젝트의 이름이 C4 나 D4b 이런식일 테므로, 
            // 1. 이 탭된 오브젝트의 값과, 방금 이 스크립트가 인스턴시에잇 한 오브젝트의 이름이 같은지 확인. 
            // 2. 같다면 맞다고 처리.
            // 3. 다르다면, 틀렸다고 처리. 

            //sTappedNoteName_inTermsOfTheSelectedKey = ContentsManager.Instance.dicScale_byKeyAndPianoKeys[GameManager.Instance.eSelectedKey][(ePIANOKEYS)System.Enum.Parse(typeof(ePIANOKEYS), sTappedKeyObjectName)];

            // 스케일 모드는, 악보를 표시할 때 빼고는 그냥 이름 inst 이런거 붙이지 말고 이름그대로 사용하자! 

            if( this.gmobjCurrentBrick.name
                    == sTappedKeyObjectName )
            {
                if(Application.isEditor) Debug.Log("Correct!");
                
                //if(Application.isEditor) Debug.Log($"Correct! {GameManager.Instance.li_gmobj_CurrentlyExistingBricks.Count}");

                // 여기서 이 맞는 브릭은 사라지게 함. 
                this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeB_Control>().SetMe_asCorrect(); 

                GameManager.Instance.ScoreSystem_PleaseUpdateTheScore( eSCORING_CASEID.SM_PN_0 );

                // 맞으면 다음 브릭 생성!
                //this.SpawnNewBrick();
                //Invoke("SpawnNewBrick", 0.7f);

                // 맞는 브릭이 사라지는 시퀀스를 시작한다는건 딱 1개 있다는 뜻. 
                // 아.. 이 0.7 초사이에 맞는거 2연타 하면 1개 이상 생성됨. 
                //if( GameManager.Instance.li_gmobj_CurrentlyExistingBricks.Count == 1 ) // 연타하면 브릭이 아직 남아 있는데도 마구 생성되는 문제를 막기 위해. 23.08.21  
                //Invoke("SpawnNewBrick", 0.7f);

            }else
            {
                // if(Application.isEditor) Debug.Log("Wrong..");
                //Debug.Log("Wrong... Brick: " + this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeB_Control>().sMyDictionariedCodeName // + ContentsManager.Instance.sCodeMode_Level_PickNumber_QuizBrickName 
                //                + " Tapped: " + sTappedNoteName_inTermsOfTheSelectedKey);
                if(Application.isEditor)
                {
                    Debug.Log("Wrong... Brick: " + this.gmobjCurrentBrick.name
                                + " Tapped: " + sTappedKeyObjectName);
                }

                this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeB_Control>().SetMe_asWrong();

                GameManager.Instance.ScoreSystem_PleaseUpdateTheScore( eSCORING_CASEID.SM_PN_6 );

            }


        }else
        {
            // 정상적이지 않은 것은? 예를 들어, D4b 이 아니고, C4# 뭐 이렇게 피아노 건반 이름을 개발자가 실수로 잘못 만들고 사용자가 탭한 경우. 

            if(Application.isEditor)
            {
                Debug.Log("Error! The Tapped key does not exist in the piano key enumerete type!!!");
            }
        }

    //}else
    //{
        // 브릭이 1개 이상이면, 처리하지 않고 처리 될 떄까지 기다림. 
    //    return;
    //}


    }

    #endregion

}
