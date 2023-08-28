//=====================================================================================
//
// 주님..
// 
// 현재 이 레벨의 플레이를 주관하는 스크립트.
// 해당 레벨: PickPatNumber : 패턴 세트로 내려온 브릭들의 소리를 듣고 맞는 화음 '번호' 를 순서대로 맞추는 레벨. 
//
// 하는일
// > 시작하면 랜덤하게 브릭 3~4개를 생성.
// > 사용자가 1개를 맞추면 해당 브릭의 코드를 표시. 
// > 사용자가 (쌓여 있는) 모든 브릭을 다 맞추면 전체 브릭들의 크기를 살짝 줄인 후 왼쪽으로 ‘쌩’ 이동.
// > 사용자가 도중에 틀리면, (쌓여 있는) 모든 브릭이 다 빠르게 쪼그라들면서 사라짐.
// > 사용자가 맞추고 브릭들이 왼쪽으로 이동하여 카메라 시야에서 사라지면, 새로운 브릭을 생성.
// > (TBD) Score 와 Combo 를 표시해 주기. 
//
// 2023.07.19. sjjo. Initial.
// 2023.08.28. sjjo. 전반적인 구조의 문제를 근본적으로 해결하기 위해 FSM 을 적용해서, 생성, 맞다/틀리다, 새브릭스파운 을 처리. Did He get inside you?
//
//=====================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeMode_Level_PickPatNumber_PlayManager : MonoBehaviour
{

    private int nUserFocusIndex; // 사용자가 맞추기 시작하면서 몇번째의 브릭인지를 확인하기 위한 값. 

    public GameObject gmobjQuizBrickPrefab; // 지금 이 레벨(PickNumber)의 경우는 사운드 브릭

    //private GameObject gmobjCurrentBrick; // 현재 생성되어 있는 브릭
    public List<GameObject> liGmobjCurrentBrick;

    private int nTentativeNumOfBricks;

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
            if( this.liGmobjCurrentBrick[idx].GetComponent<Quiz_SoundBrick_typeA2_Control>().bSetMeCorrectOnce != true )
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

        gmobjTemp.GetComponent<Quiz_SoundBrick_typeA2_Control>().SetMe_asQuestion(true);
        // 혹시나.. 비활성화 한 상태에서 트랜스폼 가져오기가 안될 까봐.. 

        this.liGmobjCurrentBrick.Add(gmobjTemp);

    }

    private eDO_NUMBER GetOneBrickName_ineDO_NUMBER__Randomly()
    {
        // Ref. 
        // https://www.reddit.com/r/Unity3D/comments/ax1tqf/unity_tip_random_item_from_enum/
        // https://afsdzvcx123.tistory.com/entry/C-%EB%AC%B8%EB%B2%95-C-Enum-Count-%EA%B0%80%EC%A0%B8%EC%98%A4%EB%8A%94-%EB%B0%A9%EB%B2%95 


        //int nUpperBound = (int)(eDO_NUMBER._7do);
        int nUpperBound = System.Enum.GetValues(typeof(eDO_NUMBER)).Length;

        // random function for integer has the upper bound value which is not include itself.
        //nUpperBound++;

        eDO_NUMBER eRandomDo = (eDO_NUMBER)( Random.Range(0, nUpperBound) );


        return eRandomDo;
    }
#endregion

#region Physical control related.
    //private void SpawnNewBrickS(int nNumOfBricks, eCODE_PATERN_RULE ePattern) // CODE_PATTERN_RANDOM, CODE_PATTERN_1564
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

        eDO_NUMBER eQuizCodeNumber;
        string sQuizeBrickName;

        for(int i = 0; i<nNumOfBricks; i++)
        {
            eQuizCodeNumber = GetOneBrickName_ineDO_NUMBER__Randomly();
            sQuizeBrickName = "instCodeBrick_" + GameManager.Instance.eSelectedKey.ToString()+ "_" + eQuizCodeNumber.ToString();

            GenerateOneQuizBrick_andAddToList(sQuizeBrickName, fInitialPosY + (i*fYspanNotToHitEachOther) );
        }
    }


    private void SetFocusMark_byTheIndex()
    {
        // 표시 마크를 보이게 or 보이지 않게

        if( this.nUserFocusIndex >= this.liGmobjCurrentBrick.Count ) return; // 이런 경우가 마지막 브릭에서 있다. 

        for(int idx = 0; idx< this.liGmobjCurrentBrick.Count; idx++)
        {
            if(idx == this.nUserFocusIndex) this.liGmobjCurrentBrick[idx].GetComponent<Quiz_SoundBrick_typeA2_Control>().SetMe_Focused(true); 
            else this.liGmobjCurrentBrick[idx].GetComponent<Quiz_SoundBrick_typeA2_Control>().SetMe_Focused(false); 
        }
    }

    private void SweepAway_allBricks()
    {
        // 현재 화면에 보이는 (다 맞춘) 브릭을 모두 사라지게 한다. 
        // 그리고 게임 오브젝트 리스트 데이터도 다 클리어 한다. 

        for(int idx = 0; idx< this.liGmobjCurrentBrick.Count; idx++)
        {
            // 하나식, 인스턴스의 메써드를 호출. 사라질 수 있도록.
            this.liGmobjCurrentBrick[idx].GetComponent<Quiz_SoundBrick_typeA2_Control>().MakeMe_Byebye();     
        }

        // 자료형의 데이터도 클리어. 
        // 23.08.28. this.liGmobjCurrentBrick.Clear();

        // 리스트 인덱스도 클리어. 
        // 23.08.28. 클리어 시점을 맞추자. this.nUserFocusIndex = 0;

        Debug.Log("liGmobjCurrentBrick's contents: " + this.liGmobjCurrentBrick.Count);



    }

#endregion

#region Public Methods
    public void CheckIfInputIsCorrect(string sTappedKeyObjectName)
    {

        if( this.ePlay_StateMachine != ePLAY_STATUS_FORLEVEL.QUESTIONED) return; // 복붙. 23.08.21 FSM 문서화 및 코딩한 이후...


        string sTappedCodeName_inTermsOfTheSelectedKey = ContentsManager.Instance.dicCode_byKeyAndDoNum[GameManager.Instance.eSelectedKey][(eDO_NUMBER)System.Enum.Parse(typeof(eDO_NUMBER), sTappedKeyObjectName)];

        // F#m 이런 코드의 enum 타입은 Fsharpm 이다. 
        // 딕셔너리로 찾은, Fsharpm 과 같은 스트링을, F#m 이렇게 바꾸어 주어야, 피아노 건반 탭한 것과 비교 및 화면 표시등에 사용할 수 있다. 
        // ( F#m 인데. Fsharpm 이렇게 표시되면 어색함..)
        sTappedCodeName_inTermsOfTheSelectedKey = ContentsManager.Instance.CheckAndReplace_sharpString_with_sharpMark( sTappedCodeName_inTermsOfTheSelectedKey );

        // 인덱스의 범위를 확인해서 처리해야!!
        //===================================================================
        // 인덱스가 끝까지 왔는지 확인 후, 지금 인스턴스 브릭들 사라짐 처리. 
        //if( this.nUserFocusIndex < this.liGmobjCurrentBrick.Count) 
        //{

            // 현재 포커스된 오브젝트를 가져와서.. 
            GameObject gmobjFocusedBrick = this.liGmobjCurrentBrick[this.nUserFocusIndex];


            //if( ContentsManager.Instance.sCodeMode_Level_PickNumber_QuizBrickName 
            if( gmobjFocusedBrick.GetComponent<Quiz_SoundBrick_typeA2_Control>().sMyDictionariedCodeName
                    == sTappedCodeName_inTermsOfTheSelectedKey )
            {
                // 맞았음. 

                Debug.Log("Correct!");

                // 여기서 이 맞는 브릭은 사라지게 함. 
                // 여기서 맞게 처리하는 것은, (패턴 학습효과를 위해) 사라지게 하면 안됨. 
                gmobjFocusedBrick.GetComponent<Quiz_SoundBrick_typeA2_Control>().SetMe_asCorrect(); 

                // 맞으면 다음 브릭 생성!
                //this.SpawnNewBrick();

                //Invoke("SpawnNewBrick", 0.7f);

                // 맞으면 인덱스 증가. 
                this.nUserFocusIndex++;

                GameManager.Instance.ScoreSystem_PleaseUpdateTheScore( eSCORING_CASEID.CM_PPN_0 );

            }else
            {
                // 틀렸음. 
                Debug.Log("Wrong... Brick: " + gmobjFocusedBrick.GetComponent<Quiz_SoundBrick_typeA2_Control>().sMyDictionariedCodeName // + ContentsManager.Instance.sCodeMode_Level_PickNumber_QuizBrickName 
                                + " Tapped: " + sTappedCodeName_inTermsOfTheSelectedKey);

                //--------------------------------------------------
                // 1단계. 틀리면 그냥 틀렸다고 표시만 해줌. 
                // 나중에는 브릭들 무너지고.. 없어지고, 새로 시작.. ? ^^;
                gmobjFocusedBrick.GetComponent<Quiz_SoundBrick_typeA2_Control>().SetMe_asWrong();

                GameManager.Instance.ScoreSystem_PleaseUpdateTheScore( eSCORING_CASEID.CM_PPN_6 );

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

                GameManager.Instance.ScoreSystem_PleaseUpdateTheScore( eSCORING_CASEID.CM_PPN_1 );

                //-------------------
                // 브릭을 다 없애고,
                this.SweepAway_allBricks();

                //--------------------
                // 잠시 기다렸다가, 다시 또 브릭세트 생성!
                // 23.08.28 이제는 FSM에 의해 브릭 스파운. Invoke("SpawnNewBrickS", 0.7f); // 작아지는 시간 0초,  움직이기 시작하는 시간 0.5초..      
   
            }
/*
        }else
        {
            // 끝까지 (다 맞춰서) 간 경우. 

            //-------------------
            // 브릭을 다 없애고,
            this.SweepAway_allBricks();

            //--------------------
            // 잠시 기다렸다가, 다시 또 브릭세트 생성!
            Invoke("SpawnNewBrick", 1f); // 작아지는 시간 0초,  움직이기 시작하는 시간 0.5초..             
    
        }
*/


    }

#endregion

}
