//=====================================================================================
// 주님, 주님꼐서 주신 지혜로 하는 모든 일들이 주님 보시기에 선한일이 되게 하여 주십시요!!
// 예수님의 이름으로 기도드렸습니다, 아멘!
// 
// 하는일 <코드 모드용, 무슨키의 코드 구성인지 맞추기 레벨의 플레이매니져> (1도, 4도 5도만 인스턴시에잇해주기)
// > 시작하면, 랜덤하게 키를 선택해서, 그 키에 해당하는 1, 4, 5도의 코드음 브릭을 생성.
// > 사용자가 맞는 키를 (하단의 건반에서) 탭하면, (이건 해당 브릭스크립트에서 함)전체 브릭들의 크기를 살짝 줄인 후 왼쪽으로 ‘쌩’ 이동.
// > 사용자가 틀린 키를 (하단의 건반에서) 탭하면, (이건 해당 브릭스크립트에서 함)전체 브릭들에게 틀린 표시를 하고 바로 그냥 사라짐. 
// > 그리고 다음 키를 랜덤하게 선택해서 또 시작. 
//
// 2023.08.07. sjjo. Initial
//
//=====================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeMode_Level_RecogKeys_PlayManager : MonoBehaviour
{

    // 지금 이 레벨(PickNumber)의 경우는 사운드 브릭 프리팹,
    // Quiz_SoundBrick_typeA_Control 를 사용 
    public GameObject gmobjQuizBrickPrefab; 

    // 현재 생성되어 있는 브릭들
    public List<GameObject> liGmobjCurrentBrick;

    void Awake()
    {
        this.liGmobjCurrentBrick = new List<GameObject>();


        this.SpawnNewBrickS();


    }

#region Data processing related.

    //private void GenerateOneQuizBrick_andAddToList(string sMyNameIs, float fDropPositionY)
    private void GenerateOneQuizBrick_andAddToList(string sMyNameIs, string sMyTextIs, float fDropPositionY)
    {

        GameObject gmobjTemp = Instantiate( this.gmobjQuizBrickPrefab, new Vector3(0f, fDropPositionY, 0f), Quaternion.identity );

        gmobjTemp.name = sMyNameIs;

        // 이건 물음표로 해서 될일이 아니고, 몇도 화음인지 표시해 줘야 함. 
        //gmobjTemp.GetComponent<Quiz_SoundBrick_typeA_Control>().SetMe_asDoNumber( sMyNameIs.Substring( sMyNameIs.Length-4, 4) ); // _3do 이 스트링만 텍스트 영역에 표시.
        // 로마자 번호로 표시해 주기..
        gmobjTemp.GetComponent<Quiz_SoundBrick_typeA_Control>().SetMe_asDoNumber( sMyTextIs ); // I IV, V

        this.liGmobjCurrentBrick.Add(gmobjTemp);

    }

#endregion

#region Physical control related.

    private void SpawnNewBrickS()
    {
        
        this.SpawnAllCodeBricks_withRandomlySelectedKey();

    }

    private void SpawnAllCodeBricks_withRandomlySelectedKey()
    {
        // 뭐하는 함수?
        // 밑에서 선택될 랜덤 키에 대해서,
        // 그 스케일의 1도, 4도, 5도 화음 브릭 3개를 생성하는 함수.

        float fInitialPosY = 6f;
        float fYspanNotToHitEachOther = 1.5f;

        //int nNumOfBricks = 3; // Tentative. 

        //---------------------------------------------------
        // 일단, 1도 4도 5도 3개의 브릭만 인스턴시에잇하자. 

        int nTempBrickIndex = 0; // 일단은 인스턴시에잇 높이 지정용..

        //---------------------------------------------------
        // 현재 가용한 키 중에서, 랜덤하게 특정 키를 가져온다. 
        // 현재는, 음원이 다 준비 안되어 있어서, 일단, 주석 처리. 
            // 스트링용. string sRamdonlySelectedKey_inString = ((eAVAILABLEKEYS)( Random.Range(0, System.Enum.GetValues(typeof(eAVAILABLEKEYS)).Length) )).ToString();
        eAVAILABLEKEYS eRamdonlySelectedKey = (eAVAILABLEKEYS)( Random.Range(0, System.Enum.GetValues(typeof(eAVAILABLEKEYS)).Length) );
        //eAVAILABLEKEYS eRamdonlySelectedKey = eAVAILABLEKEYS.C; // Tentative        

        GameManager.Instance.eSelectedKey = eRamdonlySelectedKey; // 음원 플레이 등에 사용되므로, 이 싱글톤 변수에 넣어주고 이것을 기준으로 플레이.

        if(Application.isEditor) Debug.Log($"SELECTED KEY: {GameManager.Instance.eSelectedKey}");

        string sTempInstBrickName = null;
        //----------------
        // 1도 브릭 생성. 
        sTempInstBrickName = "instCodeBrick_" + GameManager.Instance.eSelectedKey.ToString()+ "_" + (eDO_NUMBER._1do).ToString();
        GenerateOneQuizBrick_andAddToList( sTempInstBrickName, "I", fInitialPosY + (nTempBrickIndex*fYspanNotToHitEachOther) );
        nTempBrickIndex++;

        //----------------
        // 4도 브릭 생성. 
        sTempInstBrickName = "instCodeBrick_" + GameManager.Instance.eSelectedKey.ToString()+ "_" + (eDO_NUMBER._4do).ToString();
        GenerateOneQuizBrick_andAddToList( sTempInstBrickName, "IV", fInitialPosY + (nTempBrickIndex*fYspanNotToHitEachOther) );
        nTempBrickIndex++;

        //----------------
        // 5도 브릭 생성. 
        sTempInstBrickName = "instCodeBrick_" + GameManager.Instance.eSelectedKey.ToString()+ "_" + (eDO_NUMBER._5do).ToString();
        GenerateOneQuizBrick_andAddToList( sTempInstBrickName, "V", fInitialPosY + (nTempBrickIndex*fYspanNotToHitEachOther) );
        nTempBrickIndex++;

    }

    private void SweepAway_allBricks()
    {
        // 현재 화면에 보이는 (다 맞춘) 브릭을 모두 사라지게 한다. 
        // 그리고 게임 오브젝트 리스트 데이터도 다 클리어 한다. 

        for(int idx = 0; idx< this.liGmobjCurrentBrick.Count; idx++)
        {
            // 하나식, 인스턴스의 메써드를 호출. 사라질 수 있도록.
            this.liGmobjCurrentBrick[idx].GetComponent<Quiz_SoundBrick_typeA_Control>().MakeMe_Byebye();     
        }

        // 자료형의 데이터도 클리어. 
        this.liGmobjCurrentBrick.Clear();


        Debug.Log("CodeMode_Level_RecogKeys_PlayManager: liGmobjCurrentBrick's contents: " + this.liGmobjCurrentBrick.Count);

    }

#endregion

#region Public Methods
    public void CheckIfInputIsCorrect(string sTappedKeyObjectName)
    {


        if(Application.isEditor) Debug.Log("Tapped Key Ojbject Name: " + sTappedKeyObjectName );


        if( GameManager.Instance.eSelectedKey.ToString() == sTappedKeyObjectName )
        {

            // 브릭 전체를 맞는 효과로 처리!
            foreach(GameObject quizBrickObjs in this.liGmobjCurrentBrick)
            {
                quizBrickObjs.GetComponent<Quiz_SoundBrick_typeA_Control>().SetMe_asCorrect();
            }



        }else
        {

            // 브릭 전체를 틀렸다는 효과로 처리!
            foreach(GameObject quizBrickObjs in this.liGmobjCurrentBrick)
            {
                quizBrickObjs.GetComponent<Quiz_SoundBrick_typeA_Control>().SetMe_asWrong();
            }

        }


        //-------------------
        // 브릭을 다 없애고,
        this.SweepAway_allBricks();

        //--------------------
        // 잠시 기다렸다가, 다시 또 브릭세트 생성!
        Invoke("SpawnNewBrickS", 0.7f); // 작아지는 시간 0초,  움직이기 시작하는 시간 0.5초.. 

    }

#endregion

}
