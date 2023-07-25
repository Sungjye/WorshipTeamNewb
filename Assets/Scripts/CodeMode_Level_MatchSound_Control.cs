//====================================================================================
// 
// 주님, 찬찬히 잘 짤 수 있게 도와 주십시요!
//
// 코드 텍스트가 퀴즈로 나오는 모드. 이 레벨에서 사용자가 탭하는 물음표 비밀 키패드 브릭 각각에 붙는 스크립트.
//
// : 이번 레벨에서 이 control 스크립트는 다른 레벨의 (사용자 탭하는 키패드 브릭에 붙는) control 스크립트와는 
// : 상당히 다른 역할을 하는데.. : 텍스트 히든 사운드 브릭
// 
// 하는일
//  1 단계
//   > 이 레벨의 플레이어 스크립트를 통해, 자신의 히든 (뮤지컬) 코드가 무엇인지 받아온다. 
//   > 그것으로 자신의 오브젝트 이름을 정한다. 표시되는 이름은 ? 표로 한다. 
//   > 탭 되면, 컨텐츠매니저 퍼블릭 함수로, 자신의 이름에 해당하는 음을 플레이 한다. 
//  2 단계
//   > drag 를 시작하면, 손 위치에 따라 움직인다. 
//   > drop 을 퀴즈 텍스트 브릭위치에 놓고 맞으면, 플레이어 매니져에 맞다고 알리고, 자신의 원래 위치로 순간이동한다. (퀴즈브릭은 사라지고)
//   > drop 이 그외의 경우라면, 이동경로가 보이게, 자신의 원래 위치로 이동한다. 
// 
// 2023.07.25. sjjo. Initial.
// 
//====================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class CodeMode_Level_MatchSound_Control : MonoBehaviour
{
    public GameObject gmobjPlayManager;


    private AudioSource brickSpeaker;

    // Start is called before the first frame update
    void Start()
    {
        // int.Parse(this.name.Substring(1,1))
        string sMyNewName = this.gmobjPlayManager.GetComponent<CodeMode_Level_MatchSound_PlayManager>().sCode_doList_scrambled[ (int.Parse(this.name.Substring(1,1)) -1) ]; // 이름은 1부터, 인덱스는 0부터라 1을 빼줌. 

        this.name = sMyNewName;

        // Audio related settings.
        this.brickSpeaker = this.gameObject.AddComponent<AudioSource>();        
        this.brickSpeaker.loop = false;        


        // tentative. To check if it works.
        this.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = sMyNewName;


    }



    private void OnMouseDown()
    {
        //mousePosition = Input.mousePosition - GetMousePos();
        if(Application.isEditor) Debug.Log("Mouse Down: " + this.name);

        //PopEffect_inTermsOf_Size();

       // sCodeMode_Tapped_Keypad_inTermsOfTheSelectedKey

        // this.gmobjPlayManager.GetComponent<CodeMode_Level_PickPatNumber_PlayManager>().CheckIfInputIsCorrect(this.name);

        this.brickSpeaker.clip = ContentsManager.Instance.Check_WhoAmI_AndPlaySound_CodeOrNote(this.name);

        this.brickSpeaker.Play();

    }

}
