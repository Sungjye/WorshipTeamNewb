//=====================================================================================
//
// 주님, 감사합니다, 차분히 할 수 있게 인도해 주십시요!
// 
// 현재 이 레벨의 플레이를 주관하는 스크립트.
// 해당 레벨: PickNumber : 브릭의 소리를 듣고 맞는 화음 '번호' 를 맞추는 레벨. 
//
// 하는일
// > 시작하면 랜덤하게 브릭 1개를 생성.
// > 사용자가 맞추면 이 브릭을 왼쪽으로 ‘쌩’ 이동하고
// > 사용자가 틀리면 계속 두기. 
// > 사용자가 맞추고 이 브릭이 왼쪽으로 이동하여 카메라 시야에서 사라지면, 새로운 브릭을 생성
// > (TBD) Score 와 Combo 를 표시해 주기. 
//
// 2023.07.14. sjjo. Initial.
// 2023.08.28. sjjo. 전반적인 구조의 문제를 근본적으로 해결하기 위해 FSM 을 적용해서, 생성, 맞다/틀리다 처리, 새브릭스파운 을 처리. Did you take God into your mind only?
//
//=====================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class CodeMode_Level_PickNumber_PlayManager : MonoBehaviour
{
    // 사운드 브릭이 될 수도 있고, 글자 브릭이 될 수도 있고 그림 브릭이 될 수도 있고..
    public GameObject gmobjQuizBrickPrefab; // 지금 이 레벨(PickNumber)의 경우는 사운드 브릭

    //public int nQuizTotalCount; // 있다 사용하겠지만..  어찌면, 전체를 주관하는 GameManager에 두어야 할지도?

    private GameObject gmobjCurrentBrick; // 현재 생성되어 있는 브릭

    private ePLAY_STATUS_FORLEVEL ePlay_StateMachine;


    // Start is called before the first frame update
    //void Start()
    // There is a self-checking of a instace object name 
    // in the prefab script Start function
    // to play its sound. 
    // Therefore it should be instantiated before the run of the prefab script's Start function.
    void Awake()
    {
        //this.SpawnNewBrick();
        this.ePlay_StateMachine = ePLAY_STATUS_FORLEVEL.START;

        // 현재 이 scene에서 활성화된 스코어 패널을 찾아서 넣어준다. 
        GameManager.Instance.gmobjScorePanel = GameObject.Find("Panel_ScoreDisplay"); 

    }

    #region FSM related methods.

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

        return this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeA_Control>().bSetMeCorrectOnce;
    }

    private void SpawnNewBrick()
    {
        this.gmobjCurrentBrick = Instantiate( this.gmobjQuizBrickPrefab, new Vector3(0f, 6f, 0f), Quaternion.identity );

        //--------------------------------------------------
        // 오브젝트 자체의 이름을 정하기
        // 이걸, 몇도 이렇게 해야.. 다른 레벨에서의 생성 규칙도 일관성 있어지고.. 
        //And the code name is just a name in the system of harmonies (화성) 
        // 
        // Refer to. CodeMode_Level_0_Control.cs > OnMouseDown()

        // Tentative. Randomly chosen code number
        //eDO_NUMBER eQuizCodeNumber = eDO_NUMBER._2do; // Randomly or a certain pattern.
        eDO_NUMBER eQuizCodeNumber = GetOneBrickName_ineDO_NUMBER__Randomly();
        
        this.gmobjCurrentBrick.name = "instCodeBrick_" + GameManager.Instance.eSelectedKey.ToString()+ "_" + eQuizCodeNumber.ToString();

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
        this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeA_Control>().SetMe_asQuestion();


    }
    #endregion

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


    public void CheckIfInputIsCorrect(string sTappedKeyObjectName)
    {
        // This function is called from the each key tap of a user.
        //
        // Functions
        // > Compare the tapped key object name with the quiz brick name
        // > Set the quiz brick components according to the result.

        if( this.ePlay_StateMachine != ePLAY_STATUS_FORLEVEL.QUESTIONED) return; // 23.08.21 FSM 문서화 및 코딩한 이후...

        // 탭된 (버튼 역할인) 3D 오브젝트의 이름 자체가, eDO_NUMBER 타입의 이름. 
        // 그래서 변환해서 바로 인덱싱 하면 됨.
        string sTappedCodeName_inTermsOfTheSelectedKey = ContentsManager.Instance.dicCode_byKeyAndDoNum[GameManager.Instance.eSelectedKey][(eDO_NUMBER)System.Enum.Parse(typeof(eDO_NUMBER), sTappedKeyObjectName)];

        // F#m 이런 코드의 enum 타입은 Fsharpm 이다. 
        // 딕셔너리로 찾은, Fsharpm 과 같은 스트링을, F#m 이렇게 바꾸어 주어야, 피아노 건반 탭한 것과 비교 및 화면 표시등에 사용할 수 있다. 
        // ( F#m 인데. Fsharpm 이렇게 표시되면 어색함..)
        sTappedCodeName_inTermsOfTheSelectedKey = ContentsManager.Instance.CheckAndReplace_sharpString_with_sharpMark( sTappedCodeName_inTermsOfTheSelectedKey );


        //if( ContentsManager.Instance.sCodeMode_Level_PickNumber_QuizBrickName 
        if( this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeA_Control>().sMyDictionariedCodeName
                == sTappedCodeName_inTermsOfTheSelectedKey )
        {
            if(Application.isEditor) Debug.Log("Correct!");

                       
            // 여기서 이 맞는 브릭은 사라지게 함. 
            this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeA_Control>().SetMe_asCorrect(); 

            // 맞았을 떄, 연타로 치면, 
            GameManager.Instance.ScoreSystem_PleaseUpdateTheScore( eSCORING_CASEID.CM_PN_0 );

            // 맞으면 다음 브릭 생성!
            //this.SpawnNewBrick();
            // 이제 브릭생성은 FSM에서. Invoke("SpawnNewBrick", 0.7f);

        }else
        {
            if(Application.isEditor) 
            {
            Debug.Log("Wrong... Brick: " + this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeA_Control>().sMyDictionariedCodeName // + ContentsManager.Instance.sCodeMode_Level_PickNumber_QuizBrickName 
                            + " Tapped: " + sTappedCodeName_inTermsOfTheSelectedKey);
            }

            this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeA_Control>().SetMe_asWrong();

            GameManager.Instance.ScoreSystem_PleaseUpdateTheScore( eSCORING_CASEID.CM_PN_6 );

        }

    }


}
