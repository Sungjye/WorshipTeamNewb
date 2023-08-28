///////////////////////////////////////////////////////////////////////////////////
// 주님..
//
// 코드 이름이 표시되어서 (위에서 내려오는) 퀴즈 브릭에 붙은 스크립트. 
//
// 
// 23.08.28. sjjo. 연타하면 브릭이, 체크매칭 불가하게 많이 나오는 문제 해결 위해, 코드 추가... 필요 없군. 
//                 CodeMode_Level_MatchSound_Control 의 마우스 업 이벤트에서 새 브릭 스파운 하므로. 주님, 감사합니다!!!
//
///////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class Quiz_TextBrick_typeA_Control : MonoBehaviour
{

    private Coroutine crPopEffect, crVanishingEffect, crMovingTheMark;

    private Transform trChildObject_Image; // 자식: 머티리얼(이미지 표시) 을 가진.
    private Transform trChildObject_Text; // 자식: 텍스트메쉬프로 를 가진.
    private Transform trChildObject_Marker; // 자식: 마커 오브젝트의 트랜스폼.

    void Awake()
    {
        this.trChildObject_Image = this.transform.GetChild(0);
        this.trChildObject_Text = this.transform.GetChild(1);
        this.trChildObject_Marker = this.transform.GetChild(2);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

#region Public Methods regarding to a displaying the brick face.

    public void SetMe_Focused(bool bShowTheFocusMark)
    {

        //this.trChildObject_Marker.gameObject.SetActive(bShowTheFocusMark); // 마크를 보이게, 안보이게. 

        //this.SetToMove_theFocusMarkObject(bShowTheFocusMark); // 마크를 주기적으로 움직이게, 안움직이게. 더 주목되도록. 

        // 움직임 제어가 있어서.. 순서가 중요해서 조건문으로.. (active가 안된 물체를 access하지 않게.)

        if( bShowTheFocusMark )
        {
            this.trChildObject_Marker.gameObject.SetActive(true); // 마크를 보이게

            //this.SetToMove_theFocusMarkObject(true); // 마크를 주기적으로 움직이게. 더 주목되도록. 

        }else
        {
            //this.SetToMove_theFocusMarkObject(false); // 움직이는 마크를 멈춤.

            this.trChildObject_Marker.gameObject.SetActive(false); // 마크를 안보이게.

        }

    }

    public void SetMe_asQuestion(bool bShowTheFocusMark, string sText)
    {
        // SSS_SSS_PlayManager.cs 에서 부르는 함수. 
        // It is natural to control "my" material using my method, rather than a scrip-wide outside method.

        // Functions
        // > Show or hide my FocusMark (ball? or arrow) according to input value.
        // > Make my tmp child's string '?'.
        // > Make my object child's material _TapMe_Icon_ (손가락 모양 머티리얼)


        this.trChildObject_Marker.gameObject.SetActive(bShowTheFocusMark);

        //this.ShowTheFocusMark_withMotion_typeA(bShowTheFocusMark);

        this.trChildObject_Text.gameObject.GetComponent<TextMeshPro>().text = sText; // "?";

        this.trChildObject_Image.gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matQuiz_Tap_Mark_Image;

    }

    public void MakeMe_Byebye()
    {
        // 나를 사라지게 하는 함수. 
        // 여러개의 다른 인스턴스 브릭들이 겹쳐 있는 상황일 것이므로.. 안 부딪히게 사라져야. 

        this.MovingAway_type3();

        Invoke("IveDoneMyRole", 1f);

    }

#endregion


#region Private Methods regarding to a controlling of the brick (me)

    private void MovingAway_type1()
    {
        // 왼쪽으로 쌩 가기. ~~가서 사라지기.~~
        // 사라지는 건 코드 가독성상, 퍼블릭 함수에서. 
        if(Application.isEditor) Debug.Log("MOVE AWAY!");

        this.GetComponent<Rigidbody>().AddForce(Vector3.left*50f, ForceMode.Impulse); // Force, Impulse, Acceleration


    }

    private void MovingAway_type2()
    {
        // 살짝 점프하고, 왼쪽으로 가서 사라지기. 
        if(Application.isEditor) Debug.Log("MOVE AWAY!");

        this.GetComponent<Rigidbody>().AddForce(Vector3.up*5f, ForceMode.Impulse); // Force, Impulse, Acceleration

        Invoke("MovingAway_type1", 0.5f);

    }

    private void MovingAway_type3()
    {
        // 살짝 작아졌다가, 왼쪽으로 쌩 가서 사라지기. 

        //this.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f); // 여유되면 감쇄 계수 적용? 
        this.transform.localScale *= 0.85f;

        Invoke("MovingAway_type1", 0.5f);
        
    }


    private void IveDoneMyRole()
    {
        if(Application.isEditor) Debug.Log("See you then!");

        Destroy(this.transform.gameObject, 0.1f);
    }

#endregion

}
