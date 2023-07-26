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

    #region Declare list variables to scramble _do codes
    private List<string> sCode_doList_inOrder; // 몇도인지 도를 순서대로 다 넣는 리스트. 
    public List<string> sCode_doList_scrambled; // 도를 중복없이 스크램블 해서 넣은 리스트. 이건, 각 버튼에서 엑세스 해야 해서.  
    #endregion 

    void Awake()
    {
        this.liGmobjCurrentBrick = new List<GameObject>();

        this.nUserFocusIndex = 0;

        this.nTentativeNumOfBricks = 1;

        // 여기서 섞은 이름 데이터를 준비해 놓고, 각 키패드 (물음표) 브릭에서 가져가서 자신의 이름으로 삼는다. 
        this.Prepare_ScrambledCodesBrickName();

        this.SpawnNewBrickS();

        this.tmpGuideText = GameObject.Find("GuideText").GetComponent<TextMeshProUGUI>();

    }

    void Start()
    {

        //this.tmpGuideText.text = "Drag-and-drop the correct sound brick to this code below.";
        this.tmpGuideText.text = "<size=120%>Drag-and-drop</size>\n the correct sound brick\nto this code below.";

        

    }


        
#region Scramble _do (7) codes
    private void Prepare_ScrambledCodesBrickName()
    {    
        // 중복없는 랜덤값 구하기. 
        // 방금 한 2~3분간 생각한 방법이 그대로 있다.. 인줄 알았는데.. 아니네. 주신 지혜로 뒷부분 해결!
        // Ref. https://saens.tistory.com/12


        this.sCode_doList_inOrder = new List<string>(); // 몇도인지 도를 순서대로 다 넣는 리스트. 
        this.sCode_doList_scrambled = new List<string>(); // 도를 중복없이 스크램블 해서 넣은 리스트. 

        int nNumOfDOs = System.Enum.GetValues(typeof(eDO_NUMBER)).Length;
        int nRandomIndex;

        // 일단 데이터 넣기. 
        for(int i=0; i<nNumOfDOs; i++)
        {
            this.sCode_doList_inOrder.Add( ((eDO_NUMBER)(i)).ToString() );
        }


        while( this.sCode_doList_inOrder.Count > 0 )
        {

            nRandomIndex = Random.Range(0, this.sCode_doList_inOrder.Count);
            this.sCode_doList_scrambled.Add( this.sCode_doList_inOrder[nRandomIndex] ); // 현재 남아있는 데이터 중에 랜덤한 인덱스로 차곡차곡 넣기.
            
            this.sCode_doList_inOrder.RemoveAt( nRandomIndex ); // 방금 넣은 데이터는 제거. 중복을 피하기 위해. 

        }

        // 데이터 확인. 주님, 감사합니다!!!
        if( Application.isEditor )
        {
            foreach( var data in this.sCode_doList_scrambled)
            {
                Debug.Log(" Scrambled _Do: " + data );
            }
        }

    }
#endregion


#region Data processing related.

    private void GenerateOneQuizBrick_andAddToList(string sMyNameIs, float fDropPositionY)
    {

        //GameObject gmobjTemp = Instantiate( this.gmobjQuizBrickPrefab, new Vector3(0f, fDropPositionY, 0f), Quaternion.identity );
        // 이 레벨(텍스트-사운드 매칭) 에서는 drag & drop 이라.. 서로 안 부딪히게.. (레이캐스트 사용.)
        GameObject gmobjTemp = Instantiate( this.gmobjQuizBrickPrefab, new Vector3(0f, fDropPositionY, 0.95f), Quaternion.identity );

        gmobjTemp.name = sMyNameIs;

        string sCodeName
            = ContentsManager.Instance.dicCode_byKeyAndDoNum[GameManager.Instance.eSelectedKey][(eDO_NUMBER)System.Enum.Parse(typeof(eDO_NUMBER), sMyNameIs)];

        //gmobjTemp.GetComponent<Quiz_TextBrick_typeA_Control>().SetMe_asQuestion(true);
        // 혹시나.. 비활성화 한 상태에서 트랜스폼 가져오기가 안될 까봐.. 

        gmobjTemp.GetComponent<Quiz_TextBrick_typeA_Control>().SetMe_asQuestion(true, sCodeName);



        this.liGmobjCurrentBrick.Add(gmobjTemp);

        if(Application.isEditor) Debug.Log($"Added. Data Count? : {this.liGmobjCurrentBrick.Count}");

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
            this.liGmobjCurrentBrick[idx].GetComponent<Quiz_TextBrick_typeA_Control>().SetMe_Focused(false); 

            //일단 시나리오상, 여러개 브릭은 안 쓸것 같으므로.. 
            //if(idx == this.nUserFocusIndex) this.liGmobjCurrentBrick[idx].GetComponent<Quiz_TextBrick_typeA_Control>().SetMe_Focused(true); 
            //else this.liGmobjCurrentBrick[idx].GetComponent<Quiz_TextBrick_typeA_Control>().SetMe_Focused(false); 
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


#region Public Methods

    public void TheInputIsCorrect()
    {
        // 뭐하는 함수?
        // 이제는, (이 레벨에서는) 각각의 (사용자가 탭하는) 버튼이 자신의 레이캐스트로, 퀴즈브릭과 자신이 같은지 확인해서
        // 맞았을(이름이 동일할) 경우, 이 함수를 호출해 준다. 
        // 그래서 맞았다는 가정하에서 처리. 

        if(Application.isEditor) Debug.Log($"CORRECT! FocusIndex: {this.nUserFocusIndex} NumOfListData: {this.liGmobjCurrentBrick.Count}");

        //GameObject gmobjFocusedBrick = this.liGmobjCurrentBrick[this.nUserFocusIndex];

        //-------------------
        // 브릭을 다 없애고,
        this.SweepAway_allBricks();

        //--------------------
        // 잠시 기다렸다가, 다시 또 브릭세트 생성!
        //Invoke("SpawnNewBrickS", 0.7f); // 작아지는 시간 0초,  움직이기 시작하는 시간 0.5초..    
        // 드래그 하고 있는 상태에서는 키패드 브릭의 레이캐스크가 계속 있으므로,
        // 새로운 퀴즈 브릭을 생성하면 또 부딪혀서 문제가 생김. 
        // 그래서, 맞췄고, 마우스 업 이벤트가 발생하면, 새로 브릭 스파운..

    }

    public void GiveMeNewQuizBrick()
    {
        // 드래그 하고 있는 상태에서는 키패드 브릭의 레이캐스크가 계속 있으므로,
        // 새로운 퀴즈 브릭을 생성하면 또 부딪혀서 문제가 생김. 
        // 그래서, 맞췄고, 마우스 업 이벤트가 발생하면, 새로 브릭 스파운..

        //--------------------
        // 잠시 기다렸다가, 다시 또 브릭세트 생성!
        Invoke("SpawnNewBrickS", 0.7f); // 작아지는 시간 0초,  움직이기 시작하는 시간 0.5초..   

    }

    public void TheInputIsWrong()
    {
        if(Application.isEditor) Debug.Log($"WRONG.. FocusIndex: {this.nUserFocusIndex} NumOfListData: {this.liGmobjCurrentBrick.Count}");

        //GameObject gmobjFocusedBrick = this.liGmobjCurrentBrick[this.nUserFocusIndex];


        //if(Application.isEditor) Debug.Log($"WRONG.. 2/2 FocusIndex: {this.nUserFocusIndex} NumOfListData: {this.liGmobjCurrentBrick.Count}");
    }

    

#if FALSE
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
#endif
#endregion

}
