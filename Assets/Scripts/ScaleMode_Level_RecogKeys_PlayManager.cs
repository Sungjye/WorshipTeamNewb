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
    public GameObject gmobjQuizBrickPrefab; // 지금 이 레벨(PickNumber)의 경우는 사운드 브릭

    //private GameObject gmobjCurrentBrick; // 현재 생성되어 있는 브릭
    public List<GameObject> liGmobjCurrentBrick;

    private string sRamdonlySelectedKey;

    void Awake()
    {
        this.liGmobjCurrentBrick = new List<GameObject>();

        // Tentative
        this.sRamdonlySelectedKey = "C";

        //this.SpawnNewBrickS();


    }

#region Public Methods
    public void CheckIfInputIsCorrect(string sTappedKeyObjectName)
    {

        if(Application.isEditor) Debug.Log("Tapped Key Ojbject Name: " + sTappedKeyObjectName );


        if( this.sRamdonlySelectedKey == sTappedKeyObjectName )
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


    }

#endregion

}
