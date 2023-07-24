//=====================================================================================
//
// Oh Lord Jesus, please guide me to follow you.
// 
// 현재 이 레벨의 플레이를 주관하는 스크립트.
// 해당 레벨: Code Match Sound : 코드 이름이 표시된 브릭이 내려오면, 물음표로 표시된 키패드(랜덤하게 섞인) 브릭의 소리를 듣고
//                              Drag & Drop 해서 맞추는 레벨. 
//
// 하는일
// > 시작하면 랜덤하게 브릭 n개를 생성.
// > 사용자가, 키패드 영역에 있는 물음표 사운드 브릭을 탭해서 소리를 들어봄. 
// > 맞다고 생각하는, 키패드 사운드 브릭을 위로 D&D 함. 
// > 
// > 
// > (TBD) Score 와 Combo 를 표시해 주기. 
//
// 2023.07.24. sjjo. Initial.
//
//=====================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class CodeMode_Level_MatchSound_PlayManager : MonoBehaviour
{

    private int nUserFocusIndex; // 사용자가 맞추기 시작하면서 몇번째의 브릭인지를 확인하기 위한 값. 

    public GameObject gmobjQuizBrickPrefab; // 지금 이 레벨(PickNumber)의 경우는 사운드 브릭

    //private GameObject gmobjCurrentBrick; // 현재 생성되어 있는 브릭
    public List<GameObject> liGmobjCurrentBrick;

    private int nTentativeNumOfBricks;

    public TextMeshProUGUI tmpGuideText;

    void Awake()
    {
        this.liGmobjCurrentBrick = new List<GameObject>();

        this.nUserFocusIndex = 0;

        this.nTentativeNumOfBricks = 1;

        this.SpawnNewBrickS();

        this.tmpGuideText = GameObject.Find("GuideText").GetComponent<TextMeshProUGUI>();

    }

    void Start()
    {

        this.tmpGuideText.text = "Drag-and-drop the correct sound brick to this code below.";
    }

#region Data processing related.

    private void GenerateOneQuizBrick_andAddToList(string sMyNameIs, float fDropPositionY)
    {

        GameObject gmobjTemp = Instantiate( this.gmobjQuizBrickPrefab, new Vector3(0f, fDropPositionY, 0f), Quaternion.identity );

        gmobjTemp.name = sMyNameIs;

        string sCodeName
            = ContentsManager.Instance.dicCode_byKeyAndDoNum[GameManager.Instance.eSelectedKey][(eDO_NUMBER)System.Enum.Parse(typeof(eDO_NUMBER), sMyNameIs)];

        //gmobjTemp.GetComponent<Quiz_TextBrick_typeA_Control>().SetMe_asQuestion(true);
        // 혹시나.. 비활성화 한 상태에서 트랜스폼 가져오기가 안될 까봐.. 

        gmobjTemp.GetComponent<Quiz_TextBrick_typeA_Control>().SetMe_asQuestion(true, sCodeName);

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
            //sQuizeBrickName = "instCodeBrick_" + GameManager.Instance.eSelectedKey.ToString()+ "_" + eQuizCodeNumber.ToString();
            sQuizeBrickName = eQuizCodeNumber.ToString();

            GenerateOneQuizBrick_andAddToList(sQuizeBrickName, fInitialPosY + (i*fYspanNotToHitEachOther) );
        }
    }


    private void SetFocusMark_byTheIndex()
    {
        // 표시 마크를 보이게 or 보이지 않게

        if( this.nUserFocusIndex >= this.liGmobjCurrentBrick.Count ) return; // 이런 경우가 마지막 브릭에서 있다. 

        for(int idx = 0; idx< this.liGmobjCurrentBrick.Count; idx++)
        {
            if(idx == this.nUserFocusIndex) this.liGmobjCurrentBrick[idx].GetComponent<Quiz_TextBrick_typeA_Control>().SetMe_Focused(true); 
            else this.liGmobjCurrentBrick[idx].GetComponent<Quiz_TextBrick_typeA_Control>().SetMe_Focused(false); 
        }
    }

    private void SweepAway_allBricks()
    {
        // 현재 화면에 보이는 (다 맞춘) 브릭을 모두 사라지게 한다. 
        // 그리고 게임 오브젝트 리스트 데이터도 다 클리어 한다. 

        for(int idx = 0; idx< this.liGmobjCurrentBrick.Count; idx++)
        {
            // 하나식, 인스턴스의 메써드를 호출. 사라질 수 있도록.
            this.liGmobjCurrentBrick[idx].GetComponent<Quiz_TextBrick_typeA_Control>().MakeMe_Byebye();     
        }

        // 자료형의 데이터도 클리어. 
        this.liGmobjCurrentBrick.Clear();

        // 리스트 인덱스도 클리어. 
        this.nUserFocusIndex = 0;

        Debug.Log("liGmobjCurrentBrick's contents: " + this.liGmobjCurrentBrick.Count);



    }

#endregion

#if FALSE
#region Public Methods
    public void CheckIfInputIsCorrect(string sTappedKeyObjectName)
    {

        string sTappedCodeName_inTermsOfTheSelectedKey = ContentsManager.Instance.dicCode_byKeyAndDoNum[GameManager.Instance.eSelectedKey][(eDO_NUMBER)System.Enum.Parse(typeof(eDO_NUMBER), sTappedKeyObjectName)];

        // 인덱스의 범위를 확인해서 처리해야!!
        //===================================================================
        // 인덱스가 끝까지 왔는지 확인 후, 지금 인스턴스 브릭들 사라짐 처리. 
        //if( this.nUserFocusIndex < this.liGmobjCurrentBrick.Count) 
        //{

            // 현재 포커스된 오브젝트를 가져와서.. 
            GameObject gmobjFocusedBrick = this.liGmobjCurrentBrick[this.nUserFocusIndex];


            //if( ContentsManager.Instance.sCodeMode_Level_PickNumber_QuizBrickName 
            if( gmobjFocusedBrick.GetComponent<Quiz_TextBrick_typeA_Control>().sMyDictionariedCodeName
                    == sTappedCodeName_inTermsOfTheSelectedKey )
            {
                // 맞았음. 

                Debug.Log("Correct!");

                // 여기서 이 맞는 브릭은 사라지게 함. 
                // 여기서 맞게 처리하는 것은, (패턴 학습효과를 위해) 사라지게 하면 안됨. 
                gmobjFocusedBrick.GetComponent<Quiz_TextBrick_typeA_Control>().SetMe_asCorrect(); 

                // 맞으면 다음 브릭 생성!
                //this.SpawnNewBrick();

                //Invoke("SpawnNewBrick", 0.7f);

                // 맞으면 인덱스 증가. 
                this.nUserFocusIndex++;

            }else
            {
                // 틀렸음. 
                Debug.Log("Wrong... Brick: " + gmobjFocusedBrick.GetComponent<Quiz_TextBrick_typeA_Control>().sMyDictionariedCodeName // + ContentsManager.Instance.sCodeMode_Level_PickNumber_QuizBrickName 
                                + " Tapped: " + sTappedCodeName_inTermsOfTheSelectedKey);

                //--------------------------------------------------
                // 1단계. 틀리면 그냥 틀렸다고 표시만 해줌. 
                // 나중에는 브릭들 무너지고.. 없어지고, 새로 시작.. ? ^^;
                gmobjFocusedBrick.GetComponent<Quiz_TextBrick_typeA_Control>().SetMe_asWrong();

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
#endif
}
