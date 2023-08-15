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

    void Awake()
    {
        this.liGmobjCurrentBrick = new List<GameObject>();

        this.nUserFocusIndex = 0;

        this.nTentativeNumOfBricks = 3;

        this.SpawnNewBrickS();

        // 현재 이 scene에서 활성화된 스코어 패널을 찾아서 넣어준다. 
        GameManager.Instance.gmobjScorePanel = GameObject.Find("Panel_Scores_NormalSize"); 

    }

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
        this.liGmobjCurrentBrick.Clear();

        // 리스트 인덱스도 클리어. 
        this.nUserFocusIndex = 0;

        Debug.Log("liGmobjCurrentBrick's contents: " + this.liGmobjCurrentBrick.Count);



    }

#endregion

#region Public Methods
    public void CheckIfInputIsCorrect(string sTappedKeyObjectName)
    {

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

                //-------------------
                // 브릭을 다 없애고,
                this.SweepAway_allBricks();

                GameManager.Instance.ScoreSystem_PleaseUpdateTheScore( eSCORING_CASEID.CM_PPN_1 );

                //--------------------
                // 잠시 기다렸다가, 다시 또 브릭세트 생성!
                Invoke("SpawnNewBrickS", 0.7f); // 작아지는 시간 0초,  움직이기 시작하는 시간 0.5초..      
   
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
