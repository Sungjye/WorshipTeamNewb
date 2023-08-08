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


    void Awake()
    {
        this.liGmobjCurrentBrick = new List<GameObject>();


        this.SpawnNewBrickS();


    }



#region Data processing related.

    private void GenerateOneQuizBrick_andAddToList(string sMyNameIs, float fDropPositionY)
    {

        GameObject gmobjTemp = Instantiate( this.gmobjQuizBrickPrefab, new Vector3(0f, fDropPositionY, 0f), Quaternion.identity );

        gmobjTemp.name = sMyNameIs;

        // 7개 다 쌓기에는 좀 크니까.. 좀 줄여서. 
        gmobjTemp.transform.localScale *= 0.5f;

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

        int nNumOfBricks = 7; // Tentative 어떤 키이건, 7개의 기본음을 가졌을 것이다. e.g. G키. G A B C D E F#


        // 현재 가용한 키 중에서, 랜덤하게 특정 키를 가져온다. 
        // 현재는, 음원이 다 준비 안되어 있어서, 일단, 주석 처리. 
            // 스트링용. string sRamdonlySelectedKey_inString = ((eAVAILABLEKEYS)( Random.Range(0, System.Enum.GetValues(typeof(eAVAILABLEKEYS)).Length) )).ToString();
        eAVAILABLEKEYS eRamdonlySelectedKey = (eAVAILABLEKEYS)( Random.Range(0, System.Enum.GetValues(typeof(eAVAILABLEKEYS)).Length) );
        //eAVAILABLEKEYS eRamdonlySelectedKey = eAVAILABLEKEYS.C; // Tentative

        GameManager.Instance.eSelectedKey = eRamdonlySelectedKey; // 음원 플레이 등에 사용되므로, 이 싱글톤 변수에 넣어주고 이것을 기준으로 플레이.

        if(Application.isEditor) Debug.Log($"SELECTED KEY: {GameManager.Instance.eSelectedKey}");
        
        string sBrickNameToInstantiate = null;
        for(int nNoteIndex = 0; nNoteIndex<nNumOfBricks; nNoteIndex++)
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
/*                    
                case eAVAILABLEKEYS.F:
                    sBrickNameToInstantiate = ((e_F_SCALENOTES)(nNoteIndex)).ToString();
                    break;
*/
                default:
                    // 없는 키라면? 음.. C키로?
                    sBrickNameToInstantiate = ((e_C_SCALENOTES)(nNoteIndex)).ToString();
                    break;
            }

            GenerateOneQuizBrick_andAddToList( sBrickNameToInstantiate, fInitialPosY + (nNoteIndex*fYspanNotToHitEachOther) );

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
        this.liGmobjCurrentBrick.Clear();


        Debug.Log("ScaleMode_Level_RecogKeys_PlayManager: liGmobjCurrentBrick's contents: " + this.liGmobjCurrentBrick.Count);

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
                quizBrickObjs.GetComponent<Quiz_SoundBrick_typeB_Control>().SetMe_asCorrect();
            }



        }else
        {

            // 브릭 전체를 틀렸다는 효과로 처리!
            foreach(GameObject quizBrickObjs in this.liGmobjCurrentBrick)
            {
                quizBrickObjs.GetComponent<Quiz_SoundBrick_typeB_Control>().SetMe_asWrong();
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
