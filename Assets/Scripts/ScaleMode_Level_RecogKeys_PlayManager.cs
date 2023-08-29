//=====================================================================================
//
// 주님, 이렇게 키 인식 모드도 할수 있게 해 주셔서 감사합니다. 얼마 안남은 일정이지만, 주님께서 주신 평안 안에서 열심히, 
// 과정도, 결과도 주님 보시기에 선한일이 될 수 있게 하여 주십시요!
// 예수 그리스도의 이름으로 기도드렸습니다, 아멘!
// 
// 하는일 <스케일 단음 모드용, 무슨키의 음구성인지 맞추기 레벨의 플레이매니져>
// > 시작하면, 랜덤하게 키를 선택해서, 그 키에 해당하는 1~7번 음 브릭을 생성.
// > 사용자가 맞는 키를 (하단의 건반에서) 탭하면, (이건 해당 브릭스크립트에서 함)전체 브릭들의 크기를 살짝 줄인 후 왼쪽으로 ‘쌩’ 이동.
// > 사용자가 틀린 키를 (하단의 건반에서) 탭하면, (이건 해당 브릭스크립트에서 함)전체 브릭들에게 틀린 표시를 하고 바로 그냥 사라짐. 
// > 그리고 다음 키를 랜덤하게 선택해서 또 시작. 
//
// 2023.08.04. sjjo. Initial
// 2023.08.29. sjjo. 전반적인 구조의 문제를 근본적으로 해결하기 위해 FSM 을 적용해서, 생성, 맞다/틀리다 처리, 새브릭 스파운 을 처리. 모든 지혜의 근원이신 예수님!
//
//=====================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleMode_Level_RecogKeys_PlayManager : MonoBehaviour
{
    // 지금 이 레벨(PickNumber)의 경우는 사운드 브릭 프리팹,
    // Quiz_SoundBrick_typeB_Control 를 사용
    public GameObject gmobjQuizBrickPrefab; 

    //private GameObject gmobjCurrentBrick; // 현재 생성되어 있는 브릭
    public List<GameObject> liGmobjCurrentBrick;

    private int nTentativeNumOfBricks;

    private ePLAY_STATUS_FORLEVEL ePlay_StateMachine;

    void Awake()
    {
        this.liGmobjCurrentBrick = new List<GameObject>();

        this.nTentativeNumOfBricks = 7; // Tentative 어떤 키이건, 7개의 기본음을 가졌을 것이다. e.g. G키. G A B C D E F#

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
                
                // 틀린 경우에 싹 날아가게 할거면 여기에 별도 처리.

                // 틀리면, 바로 전부 날리고 새로운 브릭세트를 또 문제 내어 주기 위해서. 
                if( FSM_CheckTheTargetBrick_setAsWrong() == true ) this.ePlay_StateMachine = ePLAY_STATUS_FORLEVEL.ANSWERED_WRONG; 
                // 
                break;
            case ePLAY_STATUS_FORLEVEL.ANSWERD_CORRECTLY:
                // 음.. 뭐 해당 인스턴스 브릭에서 이미 사라지는 동작이 시작되었으므로.. 바로..
                this.ePlay_StateMachine = ePLAY_STATUS_FORLEVEL.DESTROYING;
                break;
            case ePLAY_STATUS_FORLEVEL.ANSWERED_WRONG:
                // 틀린 경우에 싹 날아가게 할거면 여기에 별도 처리.
                this.ePlay_StateMachine = ePLAY_STATUS_FORLEVEL.DESTROYING;
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
            //this.nUserFocusIndex = 0;

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
        // 여러개의 패턴 브릭이 존재하는, 이 경우에는 모든 질문 브릭이, 모두다 '맞았음' 으로 세팅 되었는가? 
        // 라는 역할을 하는 함수. 

        bool bResult = true; // 인스턴스 오브젝트들을 돌리다가, 하나라도 아직 정답이 아닌게 있으면 폴스 세팅하기. 

        //if( this.gmobjCurrentBrick == null ) return false; // 혹시나..
        if( this.liGmobjCurrentBrick.Count != this.nTentativeNumOfBricks ) bResult = false; // 혹시나.

        //return this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeB_Control>().bSetMeCorrectOnce;

        for(int idx = 0; idx< this.liGmobjCurrentBrick.Count; idx++)
        {
            if( this.liGmobjCurrentBrick[idx].GetComponent<Quiz_SoundBrick_typeB_Control>().bSetMeCorrectOnce != true )
            {
                bResult = false;
                break; // 하나라도 안되었으면 더 볼 필요 없음. 
            }
        }

        return bResult;

    }
    
    private bool FSM_CheckTheTargetBrick_setAsWrong()
    {
        // 현재 타겟인 질문 브릭이 (한번) '틀렸음' 으로 세팅 되었는가?
        //
        // 여러개의 패턴 브릭이 존재하는, 이 경우에는 모든 질문 브릭이, 모두다 '틀렸음' 으로 세팅 되었는가? 
        // 라는 역할을 하는 함수. 

        bool bResult = true; // 인스턴스 오브젝트들을 돌리다가, 하나라도 아직 정답이 아닌게 있으면 폴스 세팅하기. 

        //if( this.gmobjCurrentBrick == null ) return false; // 혹시나..
        if( this.liGmobjCurrentBrick.Count != this.nTentativeNumOfBricks ) bResult = false; // 혹시나.

        //return this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeB_Control>().bSetMeCorrectOnce;

        for(int idx = 0; idx< this.liGmobjCurrentBrick.Count; idx++)
        {
            if( this.liGmobjCurrentBrick[idx].GetComponent<Quiz_SoundBrick_typeB_Control>().bSetMeWrongOnce != true )
            {
                bResult = false;
                break; // 하나라도 안되었으면 더 볼 필요 없음. 
            }
        }

        return bResult;

    }
#endregion

#region Data processing related.

    //private void GenerateOneQuizBrick_andAddToList(string sMyNameIs, float fDropPositionY)
    private void GenerateOneQuizBrick_andAddToList(string sMyNameIs, float fDropPositionY, int nSizeIndex)
    {

        GameObject gmobjTemp = Instantiate( this.gmobjQuizBrickPrefab, new Vector3(0f, fDropPositionY, 0f), Quaternion.identity );

        gmobjTemp.name = sMyNameIs;

        // 7개 다 쌓기에는 좀 크니까.. 좀 줄여서. 
        //gmobjTemp.transform.localScale *= 0.5f;
        //gmobjTemp.transform.localScale = (gmobjTemp.transform.localScale * 0.5f * ((float)(nSizeIndex) * ) );

        gmobjTemp.transform.localScale = gmobjTemp.transform.localScale * (1f - ((float)(nSizeIndex) * 0.1f)) ;

        // 비활성화 되면 엑세스 할 수 없다? 
        gmobjTemp.GetComponent<Quiz_SoundBrick_typeB_Control>().SetMe_asQuestion();

        this.liGmobjCurrentBrick.Add(gmobjTemp);

    }

#endregion

#region Physical control related.

    private void SpawnNewBrickS()
    {

        
        this.SpawnAllNoteBricks_withRandomlySelectedKey();

        //this.SetFocusMark_byTheIndex();

    }

    private void SpawnAllNoteBricks_withRandomlySelectedKey() 
    {
        // 뭐하는 함수?
        // 밑에서 선택될 랜덤 키에 대해서, 
        // 그 스케일을 구성하는 7개의 음 브릭을 모두 생성하는 함수. 

        float fInitialPosY = 6f;
        float fYspanNotToHitEachOther = 1.5f;

        //int nNumOfBricks = 7; // Tentative 어떤 키이건, 7개의 기본음을 가졌을 것이다. e.g. G키. G A B C D E F#


        // 현재 가용한 키 중에서, 랜덤하게 특정 키를 가져온다. 
        // 현재는, 음원이 다 준비 안되어 있어서, 일단, 주석 처리. 
            // 스트링용. string sRamdonlySelectedKey_inString = ((eAVAILABLEKEYS)( Random.Range(0, System.Enum.GetValues(typeof(eAVAILABLEKEYS)).Length) )).ToString();
        eAVAILABLEKEYS eRamdonlySelectedKey = (eAVAILABLEKEYS)( Random.Range(0, System.Enum.GetValues(typeof(eAVAILABLEKEYS)).Length) );
        //eAVAILABLEKEYS eRamdonlySelectedKey = eAVAILABLEKEYS.C; // Tentative

        GameManager.Instance.eSelectedKey = eRamdonlySelectedKey; // 음원 플레이 등에 사용되므로, 이 싱글톤 변수에 넣어주고 이것을 기준으로 플레이.

        if(Application.isEditor) Debug.Log($"SELECTED KEY: {GameManager.Instance.eSelectedKey}");
        
        string sBrickNameToInstantiate = null;
        for(int nNoteIndex = 0; nNoteIndex < this.nTentativeNumOfBricks; nNoteIndex++)
        {

            // 결국, 스케일 모드에서, 사운드 브릭은, C4, D4 뭐 이런식의 이름을 가지는 브릭을 인스턴시에잇 해야 하므로.. 

            //sQuizScaleNote = ContentsManager.Instance.GetOneBrickName_inePIANOKEYS__Randomly();

            switch(GameManager.Instance.eSelectedKey)
            {
                case eAVAILABLEKEYS.C:
                    sBrickNameToInstantiate = ((e_C_SCALENOTES)(nNoteIndex)).ToString();
                    // 예는 샵이 없어서 안해도 될듯?
                    break;
                case eAVAILABLEKEYS.G:
                    sBrickNameToInstantiate = ((e_G_SCALENOTES)(nNoteIndex)).ToString();
                    // 이 키는 enum 타입에 sharp 이 있어서 이걸 사용해야 함. 
                    sBrickNameToInstantiate = ContentsManager.Instance.CheckAndReplace_sharpString_with_sharpMark(sBrickNameToInstantiate);
                    break;
                case eAVAILABLEKEYS.F:
                    sBrickNameToInstantiate = ((e_F_SCALENOTES)(nNoteIndex)).ToString();
                    // 예는 샵이 없어서 안해도 될듯?
                    //sBrickNameToInstantiate = ContentsManager.Instance.CheckAndReplace_sharpString_with_sharpMark(sBrickNameToInstantiate);
                    break;
                case eAVAILABLEKEYS.D:
                    sBrickNameToInstantiate = ((e_D_SCALENOTES)(nNoteIndex)).ToString();
                    // 이 키는 enum 타입에 sharp 이 있어서 이걸 사용해야 함. 
                    sBrickNameToInstantiate = ContentsManager.Instance.CheckAndReplace_sharpString_with_sharpMark(sBrickNameToInstantiate);
                    break;
                case eAVAILABLEKEYS.A:
                    sBrickNameToInstantiate = ((e_A_SCALENOTES)(nNoteIndex)).ToString();
                    // 이 키는 enum 타입에 sharp 이 있어서 이걸 사용해야 함. 
                    sBrickNameToInstantiate = ContentsManager.Instance.CheckAndReplace_sharpString_with_sharpMark(sBrickNameToInstantiate);
                    break;
                case eAVAILABLEKEYS.E:
                    sBrickNameToInstantiate = ((e_E_SCALENOTES)(nNoteIndex)).ToString();
                    // 이 키는 enum 타입에 sharp 이 있어서 이걸 사용해야 함. 
                    sBrickNameToInstantiate = ContentsManager.Instance.CheckAndReplace_sharpString_with_sharpMark(sBrickNameToInstantiate);
                    break;
/*                    
                case eAVAILABLEKEYS.%:
                    sBrickNameToInstantiate = ((e_%_SCALENOTES)(nNoteIndex)).ToString();
                    // 이 키는 enum 타입에 sharp 이 있어서 이걸 사용해야 함. 
                    sBrickNameToInstantiate = ContentsManager.Instance.CheckAndReplace_sharpString_with_sharpMark(sBrickNameToInstantiate);
                    break;
*/
                default:
                    // 없는 키라면? 음.. C키로?
                    sBrickNameToInstantiate = ((e_C_SCALENOTES)(nNoteIndex)).ToString();
                    break;
            }

            //GenerateOneQuizBrick_andAddToList( sBrickNameToInstantiate, fInitialPosY + (nNoteIndex*fYspanNotToHitEachOther) );
            GenerateOneQuizBrick_andAddToList( sBrickNameToInstantiate, fInitialPosY + (nNoteIndex*fYspanNotToHitEachOther), nNoteIndex );

        }
    }


    private void SweepAway_allBricks()
    {
        // 현재 화면에 보이는 (다 맞춘) 브릭을 모두 사라지게 한다. 
        // 그리고 게임 오브젝트 리스트 데이터도 다 클리어 한다. 

        for(int idx = 0; idx< this.liGmobjCurrentBrick.Count; idx++)
        {
            // 하나식, 인스턴스의 메써드를 호출. 사라질 수 있도록.
            this.liGmobjCurrentBrick[idx].GetComponent<Quiz_SoundBrick_typeB_Control>().MakeMe_Byebye();     
        }

        // 자료형의 데이터도 클리어. 
        // 23.08.29 this.liGmobjCurrentBrick.Clear();
        // 아직 "사라지기 위한 진행과정" 중에 있으므로 클리어 하면 안된다..
        // 노브릭 체크함수에서 처리함. 


        if(Application.isEditor) Debug.Log("ScaleMode_Level_RecogKeys_PlayManager: liGmobjCurrentBrick's contents: " + this.liGmobjCurrentBrick.Count);

    }

#endregion


#region Public Methods
    public void CheckIfInputIsCorrect(string sTappedKeyObjectName)
    {

        //==================================================================
        // State Machine 이 QUESTIONED 모드 일 떄만 브릭이 존재하는 것이므로, 
        // 이 떄에만 체크를 한다. 
        if( this.ePlay_StateMachine != ePLAY_STATUS_FORLEVEL.QUESTIONED) return; // 복붙. 23.08.21 FSM 문서화 및 코딩한 이후...

        if(Application.isEditor) Debug.Log("Tapped Key Ojbject Name: " + sTappedKeyObjectName );


        if( GameManager.Instance.eSelectedKey.ToString() == sTappedKeyObjectName )
        {

            // 브릭 전체를 맞는 효과로 처리!
            foreach(GameObject quizBrickObjs in this.liGmobjCurrentBrick)
            {
                quizBrickObjs.GetComponent<Quiz_SoundBrick_typeB_Control>().SetMe_asCorrect();
            }

            GameManager.Instance.ScoreSystem_PleaseUpdateTheScore( eSCORING_CASEID.SM_RK_0 );


        }else
        {

            // 브릭 전체를 틀렸다는 효과로 처리!
            foreach(GameObject quizBrickObjs in this.liGmobjCurrentBrick)
            {
                quizBrickObjs.GetComponent<Quiz_SoundBrick_typeB_Control>().SetMe_asWrong();
            }

            GameManager.Instance.ScoreSystem_PleaseUpdateTheScore( eSCORING_CASEID.SM_RK_6 );

        }



        //## 맞든 틀리든, 바로 다 날림! 새로운 다음 문제 바로 내주기 ##
        //-------------------
        // 브릭을 다 없애고,
        this.SweepAway_allBricks();

        //--------------------
        // 잠시 기다렸다가, 다시 또 브릭세트 생성!
        // 23.08.29 이제는 FSM에 의해 브릭 스파운. Invoke("SpawnNewBrickS", 0.7f); // 작아지는 시간 0초,  움직이기 시작하는 시간 0.5초.. 

    }

#endregion

}
