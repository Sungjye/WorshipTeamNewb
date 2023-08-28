//=====================================================================================
//
// 선하신 주님, 언제나 주님만을 믿고 따르겠습니다. 
// 주님의 말씀만을 전심으로 순전하게 믿고 실천하겠습니다! 주님, 인도하여 주십시오. 
// 아버지 하나님의 선하신 뜻을 위해서 항상 사용하여 주십시오!
// 예수님의 이름으로 기도드렸습니다!
//
// 하는일 <스케일 단음 모드용>
// > 시작하면 랜덤하게 브릭 3~4개를 생성.
// > 사용자가 1개를 맞추면 해당 브릭의 코드를 표시. 
// > 사용자가 (쌓여 있는) 모든 브릭을 다 맞추면 전체 브릭들의 크기를 살짝 줄인 후 왼쪽으로 ‘쌩’ 이동.
// > (TBD)난이도 조절 위해 일단 보류. ~~사용자가 도중에 틀리면, (쌓여 있는) 모든 브릭이 다 빠르게 쪼그라들면서 사라짐.
// > 카메라 시야면 좋겠지만 일단, 고정된 시간 이후로. ~~사용자가 맞추고 브릭들이 왼쪽으로 이동하여 카메라 시야에서 사라지면, 새로운 브릭을 생성.
// > (TBD) Score 와 Combo 를 표시해 주기. 
//
// 2023.07.24. sjjo. Initial.
// 2023.08.28. sjjo. 전반적인 구조의 문제를 근본적으로 해결하기 위해 FSM 을 적용해서, 생성, 맞다/틀리다 처리, 새브릭스파운 을 처리.
//                   주님, 말씀으로 저를 채워 주시니 감사합니다! Did you also embrace him with your heart? Did he get inside you?
// 
//=====================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleMode_Level_PickPatNotes_PlayManager : MonoBehaviour
{
    private int nUserFocusIndex; // 사용자가 맞추기 시작하면서 몇번째의 브릭인지를 확인하기 위한 값. 

    public GameObject gmobjQuizBrickPrefab; // 지금 이 레벨(PickNumber)의 경우는 사운드 브릭

    //private GameObject gmobjCurrentBrick; // 현재 생성되어 있는 브릭
    public List<GameObject> liGmobjCurrentBrick;

    private int nTentativeNumOfBricks;

    public GameObject gmobjDebugText;


    private ePLAY_STATUS_FORLEVEL ePlay_StateMachine;

    void Awake()
    {
        this.liGmobjCurrentBrick = new List<GameObject>();

        this.nUserFocusIndex = 0;

        this.nTentativeNumOfBricks = 3;

        //this.SpawnNewBrickS();
        // 바로 생성하는 대신, FSM에서..
        this.ePlay_StateMachine = ePLAY_STATUS_FORLEVEL.START;


        // 현재 이 scene에서 활성화된 스코어 패널을 찾아서 넣어준다. 
        GameManager.Instance.gmobjScorePanel = GameObject.Find("Panel_ScoreDisplay"); 

        //LevelGuideText_typeA
        this.gmobjDebugText = GameManager.Instance.DebugMsgOnScreen_Setup();

    }



#region FSM related methods.
    // 음.. FSM 사용안하고 했었는데.. 이렇게 하면 delay 를 이리저리 주는 사이나, 
    // 여기 저기 시간의 구멍에서 (막 들어오는) 사용자의 입력과 스파운 사이에서.. 여러개가 막 스파운되거나, out of index 등이 발생한다. (pattern 모드인 경우)
    // 그래서 어쩔수 없이 이제야 FSM을 사용해서 다시 구현.. 23.08.21
    // 주님, 문서화의 마음과 구현할 수 있는 지혜를 주셔서 감사합니다!

    void FixedUpdate()
    {
        this.Tick_TheStateMachine();
    }

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

    /*
    private bool FSM_NoBrickExist()
    {
        // 현재 이판에(scene에.. ^^) 질문 브릭이 존재하는가?

        bool bResult = false;
        
        // 이걸로 체크 됨?
        //이건 복붙한 내용. if( this.gmobjCurrentBrick == null ) bResult = true; 

        // 이것이.. 실제 각자의 브릭인스턴스 스크립트에서 디스트로이 되는 시점과, 여기 게임오브젝트 리스트에서 클리어 하는 시점이 달라서..
        // 사용에 주의를 기울여야 함.. 
        if( this.liGmobjCurrentBrick.Count == 0 ) bResult = true;
        
        if(Application.isEditor) Debug.Log($"FSM_NoBrickExist: {bResult}. liGmobjCurrentBrick.Count: {this.liGmobjCurrentBrick.Count}");

        return bResult;
    }
    */

    private bool FSM_NoBrickExist()
    {
        // 현재 이판에(scene에.. ^^) 질문 브릭이 존재하는가?
        //
        // 이게.. 각자의 인스턴스 브릭에서 디스트로이가 되면, 리스트의 데이터는 남아 있되, 내용이 null 일거다. 
        // 그래서 이방식으로 변경. 23.08.28

        bool bAreAllBricks_Null_Result = true;

        for(int idx = 0; idx< this.liGmobjCurrentBrick.Count; idx++)
        {
            if( this.liGmobjCurrentBrick[idx] != null )
            {
                bAreAllBricks_Null_Result = false;
                break; // 하나라도 널이 아니면 더 볼 필요 없음. 
            }
        }

        if( bAreAllBricks_Null_Result == true )
        {
            // 모든 브릭이 null 이란 것은, 각자의 인스턴스 스크립트에서 디스트로이를 "다" 했다는 뜻. 그래서 실제로는 브릭이 하나도 없다는 뜻. 

            // 자료형의 데이터 클리어. 
            this.liGmobjCurrentBrick.Clear();
            // 리스트 인덱스도 클리어. 
            this.nUserFocusIndex = 0;

        }

        if(Application.isEditor) Debug.Log($"FSM_NoBrickExist: {bAreAllBricks_Null_Result}. liGmobjCurrentBrick.Count: {this.liGmobjCurrentBrick.Count}");

        return bAreAllBricks_Null_Result;
    }

    private void FSM_SpawnNewBrick()
    {

        // 이제 기다릴 필요 없음. 앞의 START 상태에서 브릭 (다) 사라질 떄까지 기다렸으므로. 
        this.SpawnNewBrickS();

    }

    private bool FSM_CheckTheTargetNumberOfBricksExists()
    {
        // 이 판에서 원하는 개수 만큼의 질문 브릭이 생성되어 있는가?

        bool bResult = false;

        // 이걸로 체크 됨?
        // if( this.gmobjCurrentBrick != null ) bResult = true; 

        if( this.liGmobjCurrentBrick.Count == this.nTentativeNumOfBricks ) bResult = true; 


        if(Application.isEditor) Debug.Log($"FSM_CheckTheTargetNumberOfBricksExists: {bResult}");

        return bResult;
    }

    private bool FSM_CheckTheTargetBrick_setAsCorrect()
    {
        // 현재 타겟인 질문 브릭이 '맞았음' 으로 세팅 되었는가?
        //
        // 여러개의 패턴 브릭이 존재하는, 이 경우에는 모든 질문 브릭이, 모두다 '맞앚음' 으로 세팅 되었는가? 
        // 라는 역할을 하는 함수. 

        bool bResult = true; // 인스턴스 오브젝트들을 돌리다가, 하나라도 아직 정답이 아닌게 있으면 폴스 세팅하기. 

        //if( this.gmobjCurrentBrick == null ) return false; // 혹시나..
        if( this.liGmobjCurrentBrick.Count != this.nTentativeNumOfBricks ) bResult = false; // 혹시나.

        //return this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeB_Control>().bSetMeCorrectOnce;

        for(int idx = 0; idx< this.liGmobjCurrentBrick.Count; idx++)
        {
            if( this.liGmobjCurrentBrick[idx].GetComponent<Quiz_SoundBrick_typeB2_Control>().bSetMeCorrectOnce != true )
            {
                bResult = false;
                break; // 하나라도 안되었으면 더 볼 필요 없음. 
            }
        }

        return bResult;

    }
#endregion

#region Data processing related.

    private void GenerateOneQuizBrick_andAddToList(string sMyNameIs, float fDropPositionY)
    {

        GameObject gmobjTemp = Instantiate( this.gmobjQuizBrickPrefab, new Vector3(0f, fDropPositionY, 0f), Quaternion.identity );

        gmobjTemp.name = sMyNameIs;

        // 비활성화 되어서 엑세스 할 수 없다? gmobjTemp.GetComponent<Quiz_SoundBrick_typeB2_Control>().SetMe_asQuestion(false);
        gmobjTemp.GetComponent<Quiz_SoundBrick_typeB2_Control>().SetMe_asQuestion(true);

        this.liGmobjCurrentBrick.Add(gmobjTemp);

    }

    //private string GetOneBrickName_inNoteNameString_itfKey__Randomly()
    //{
        // 리턴값: e_C_SCALENOTES 등의 이넘 데이터 타입들과 호환돠는 데이터를 랜덤으로 구해서, 그걸 스트링으로 바꾸어 리턴하는 함수. 
        // 왜? 이 이름으로 브릭을 만들어야, 사용자 키보드 입력이 있을 떄, 내정된 규칙으로 해당 브릭을 찾지...

        // 컨텐츠 매니져에 있는거 갖다 쓰자. 만들어 놓고도 잊는다..

    //}

#endregion


#region Physical control related.

    //private void SpawnNewBrics(int nNumOfBricks, eSCALE_PATTERN_RULE ePattern) // SCALE_PATTERN_1DO_WHA_UM, 
    private void SpawnNewBrickS()
    {

        // 일단 랜덤하게 한번 생성해 보고.
        this.SpawnNewBricks_Randomly(this.nTentativeNumOfBricks);

        this.SetFocusMark_byTheIndex();

    }

    private void SpawnNewBricks_Randomly(int nNumOfBricks)
    {
        float fInitialPosY = 6f;
        float fYspanNotToHitEachOther = 1.5f;

        string sQuizScaleNote;
        //string sQuizeBrickName;

        for(int i = 0; i<nNumOfBricks; i++)
        {
            sQuizScaleNote = ContentsManager.Instance.GetOneBrickName_inePIANOKEYS__Randomly();
            //sQuizeBrickName = "instCodeBrick_" + GameManager.Instance.eSelectedKey.ToString()+ "_" + eQuizCodeNumber.ToString();
            
            // 잘 이해 안되는 내 주석이지만. 
            // 스케일 모드는, 악보를 표시할 때 빼고는 그냥 이름 inst 이런거 붙이지 말고 이름그대로 사용하자! 
            // 스케일을 섞어서, 브릭을 생성해 낼 일도 없을듯.. 
            // 나중에 악보 표시나, 소리 매칭할 때, 컨트롤 스크립트에서 현재 선택된 key (조) 를 확인해서 서브루틴 돌리기로..             
            
            //sQuizeBrickName = sQuizScaleNote;

            GenerateOneQuizBrick_andAddToList(/*sQuizeBrickName*/ sQuizScaleNote, fInitialPosY + (i*fYspanNotToHitEachOther) );

        }
    }


    private void SetFocusMark_byTheIndex()
    {
        // 표시 마크를 보이게 or 보이지 않게

        if( this.nUserFocusIndex >= this.liGmobjCurrentBrick.Count ) return; // 이런 경우가 마지막 브릭에서 있다. 

        for(int idx = 0; idx< this.liGmobjCurrentBrick.Count; idx++)
        {
            if(idx == this.nUserFocusIndex) this.liGmobjCurrentBrick[idx].GetComponent<Quiz_SoundBrick_typeB2_Control>().SetMe_Focused(true); 
            else this.liGmobjCurrentBrick[idx].GetComponent<Quiz_SoundBrick_typeB2_Control>().SetMe_Focused(false); 
        }
    }

    private void SweepAway_allBricks()
    {
        // 현재 화면에 보이는 (다 맞춘) 브릭을 모두 사라지게 한다. 
        // 그리고 게임 오브젝트 리스트 데이터도 다 클리어 한다. 

        for(int idx = 0; idx< this.liGmobjCurrentBrick.Count; idx++)
        {
            // 하나식, 인스턴스의 메써드를 호출. 사라질 수 있도록.
            this.liGmobjCurrentBrick[idx].GetComponent<Quiz_SoundBrick_typeB2_Control>().MakeMe_Byebye();     
        }

        // 자료형의 데이터도 클리어. 
        // 23.08.28. this.liGmobjCurrentBrick.Clear();

        // 음.. 이건 리스트에 담긴 데이터가 실제 인스턴스라면, 각자의 인스턴스 스크립트에서 디스트로이하면 여기서도 날아간다?
        // 그러면 이런 이상한 타이밍 즉, 아직 각자의 인스턴스 브릭들이 "사라지고 있는 진행중" 에 클리어 하지 않아도 된다?!
        // 아니군. 노브릭 체크 함수를 보시오. 


        // 리스트 인덱스도 클리어. 
        // 23.08.28. 클리어 시점을 맞추자. this.nUserFocusIndex = 0;

        if(Application.isEditor) Debug.Log("ScaleMode_Level_PickPatNotes_PlayManager: liGmobjCurrentBrick's contents: " + this.liGmobjCurrentBrick.Count);

    }

#endregion

#region Public Methods
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
        if( this.ePlay_StateMachine != ePLAY_STATUS_FORLEVEL.QUESTIONED) return; // 복붙. 23.08.21 FSM 문서화 및 코딩한 이후...

        // 탭된 (버튼 역할인) 3D 오브젝트의 이름 자체가, ePIANOKEYS 타입의 이름. 
        // 그래서 변환해서 바로 인덱싱 하면 됨.. 이라고 생각할 수 있지만, 

        // 피아노 키 값이 나중에 뭐가될지 C# or Db 뭐가 될지 모르므로.. 있는지 확인해야 함. 
        // ref. https://learn.microsoft.com/ko-kr/dotnet/api/system.enum.isdefined?view=net-7.0 
        //---------------------------------------------
        // 지금 탭된 피아노 키가 무엇인지 확인부터해서.. 
        // 어떤 것을 인스턴시에잇 할지 결정!
        string sParsedPinanoKey = ContentsManager.Instance.ParsingTheTappedPianoKey(sTappedKeyObjectName); // e.g. C4는 C로, D4b 은 Db로 변환해 준다. 

        if( System.Enum.IsDefined( typeof(ePIANOKEYS),  sParsedPinanoKey ) ) 
        {

        // 인덱스의 범위를 확인해서 처리해야!!
        //===================================================================
        // 인덱스가 끝까지 왔는지 확인 후, 지금 인스턴스 브릭들 사라짐 처리. 
        //if( this.nUserFocusIndex < this.liGmobjCurrentBrick.Count) 
        //{

            if( (this.nUserFocusIndex < 0) || (this.nUserFocusIndex >= this.liGmobjCurrentBrick.Count) )
            {
                GameManager.Instance.DebugMsgOnScreen(this.gmobjDebugText, $"Out of index: {this.nUserFocusIndex} of {this.liGmobjCurrentBrick.Count}" );
                //this.gmobjDebugText
            }

            // 현재 포커스된 오브젝트를 가져와서.. 
            GameObject gmobjFocusedBrick = this.liGmobjCurrentBrick[this.nUserFocusIndex];



            //if( ContentsManager.Instance.sCodeMode_Level_PickNumber_QuizBrickName 
            if( gmobjFocusedBrick.name
                    == sTappedKeyObjectName )
            {
                // 맞았음. 

                if(Application.isEditor) Debug.Log("Correct!");

                // 여기서 이 맞는 브릭은 사라지게 함. 
                // 여기서 맞게 처리하는 것은, (패턴 학습효과를 위해) 사라지게 하면 안됨. 
                gmobjFocusedBrick.GetComponent<Quiz_SoundBrick_typeB2_Control>().SetMe_asCorrect(); 

                // 맞으면 다음 브릭 생성!
                //this.SpawnNewBrick();

                //Invoke("SpawnNewBrick", 0.7f);

                // 맞으면 인덱스 증가. 
                this.nUserFocusIndex++;

                GameManager.Instance.ScoreSystem_PleaseUpdateTheScore( eSCORING_CASEID.SM_PPN_0 );

            }else
            {
                // 틀렸음. 
                Debug.Log("Wrong... Brick: " + gmobjFocusedBrick.name 
                                + " Tapped: " + sTappedKeyObjectName);

                //--------------------------------------------------
                // 1단계. 틀리면 그냥 틀렸다고 표시만 해줌. 
                // 나중에는 브릭들 무너지고.. 없어지고, 새로 시작.. ? ^^;
                gmobjFocusedBrick.GetComponent<Quiz_SoundBrick_typeB2_Control>().SetMe_asWrong();

                GameManager.Instance.ScoreSystem_PleaseUpdateTheScore( eSCORING_CASEID.SM_PPN_6 );

            }

            //---------------------------
            // 브릭 앞의 마크도 처리. 
            this.SetFocusMark_byTheIndex();

            //---------------------------------------------------------------------
            // 유저입력이 다시 올 떄까지 기다리지 않고, 인덱스가 끝까지 간 경우를 
            // 지금 바로 감지하기!!!
            // 이미 증가 되었을 것이므로. 
            if( this.nUserFocusIndex >= this.liGmobjCurrentBrick.Count) 
            {

                // 끝까지 (다 맞춰서) 간 경우. 

                GameManager.Instance.ScoreSystem_PleaseUpdateTheScore( eSCORING_CASEID.SM_PPN_1 );

                //-------------------
                // 브릭을 다 없애고,
                this.SweepAway_allBricks();

                //--------------------
                // 잠시 기다렸다가, 다시 또 브릭세트 생성!
                // 23.08.28 이제는 FSM에 의해 브릭 스파운. Invoke("SpawnNewBrickS", 0.7f); // 작아지는 시간 0초,  움직이기 시작하는 시간 0.5초..      
   
            }


        }else
        {
            // 정상적이지 않은 것은? 예를 들어, D4b 이 아니고, C4# 뭐 이렇게 피아노 건반 이름을 개발자가 실수로 잘못 만들고 사용자가 탭한 경우. 

            if(Application.isEditor)
            {
                Debug.Log("Error! The Tapped key does not exist in the piano key enumerete type!!!");
            }
        }




    }

#endregion




}
