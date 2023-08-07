///////////////////////////////////////////////////////////////////////////////////////////////////////
// 주님, 감사하고! 또 주님만을 사랑하고 또 주님만을 신뢰합니다!
// 
// PickPatNotes : 음 브릭 "여러게 쌓아" 맞추기 퀴즈 단계에서, (위에서 떨어지는) 문제 브릭 프리팹에 컴포넌트로 붙는 스크립트. (제어)
// 
// 하는일
// : 상위 스크립트에서, 이 브릭의 이름을, 해당 키의 특정 음(노트) 값이름으로 생성. 
// : 생성시 해당하는 사운드 출력. 
// : 그림 부분에는 탭 손 모양 (사용이 직관적이도록), 글자는 텍스트 오브젝트에 표시.
// : 자신의 현재 위치를 나타내는 FocusMark를, 자신의 차례인 경우에만 표시. 
// : 탭하면 해당 사운드 출력.
// : 이 브릭과 사용자가 탭한 브릭의 이름이 같으면, 글자 부분에 정답 음값을 보여주고, 
//   그림 부분에는 맞으면 O와 띵동, 틀리면 X와 부저음. 
// 
// 23.07.24. sjjo. 주님, 과정과 결과, 진행 모든 것이 But seek His kingdom 인 일이 되게 하여 주십시오. 
//                 저의 생각과 제가 원하는 것에, 주님의 이름을 함부로 붙이지 않게, But seek His kingdom 이라고 말씀하신
//                 그 참 의미를, 제가 감히 왜곡시키거나 잘못 추구하지 않도록 주님, 늘 저를 인도해 주십시오!
//                 저의 주인되신 목자되신, 구원자 되신 예수 그리스도의 이름으로 기도드렸습니다, 아멘!
//
///////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class Quiz_SoundBrick_typeB2_Control : MonoBehaviour
{
    private AudioSource brickSpeaker;

    private Coroutine crPopEffect, crVanishingEffect, crMovingTheMark;
    
    private Vector3 vOrigianlSize;

    //============================================================
    // 마커 오브젝트 관련
    private Vector3 v3Marker_InitialPosition; // 마커의 처음 위치.
    private Vector3 v3Marker_NewPosition; // 매번 계산하는 마커의 이동 위치 그릇.
    //private float fMarker_DeltaSpan; // 좌(우)로 이동 가능한 x 최대값. 
    //private float fMarker_Speed; // 이동 속도. 

    //============================================================
    // 이 오브젝트의 자체의 이름은, D4b 
    // 음 소리를 내거나, 사용자 입력과 비교하기 위해서, 파싱할 필요가 없을듯.. 

    //============================================================
    // 자식오브젝트 들을 제어하기 위해. 
    // 이해하기 쉽게, 깔끔하게 이렇게 넣어서 사용하자.
    private Transform trChildObject_Image; // 자식: 머티리얼(이미지 표시) 을 가진.
    private Transform trChildObject_Text; // 자식: 텍스트메쉬프로 를 가진.
    private Transform trChildObject_Marker; // 자식: 마커 오브젝트의 트랜스폼.

    void Awake()
    {
        // 왜 코드모드에서는 잘 되던게..  위치 옮겨보기. 아니다.. 아이쿠.. 겨우 찾았네.. 2번 자식에 이 스크립트를 또 실수로 붙여놨네.. 
        // UnityException: Transform child out of bounds
        // Quiz_SoundBrick_typeB2_Control.Awake () (at Assets/Scripts/Quiz_SoundBrick_typeB2_Control.cs:55)

        this.trChildObject_Image = this.transform.GetChild(0);
        this.trChildObject_Text = this.transform.GetChild(1);
        this.trChildObject_Marker = this.transform.GetChild(2);
    }


    // Start is called before the first frame update
    void Start()
    {

        if(Application.isEditor) Debug.Log("Quiz brick name: " + this.name); // 

        this.brickSpeaker = this.gameObject.AddComponent<AudioSource>();
        
        this.brickSpeaker.loop = false;


        this.vOrigianlSize = this.transform.localScale; 


        //============================================================
        // (자신이 focus되었다는 것을 표시하는) 마커 오브젝트 제어 관련
        this.v3Marker_InitialPosition = this.trChildObject_Marker.position; // 마커의 최초 위치를 저장해두고..
        //this.fMarker_DeltaSpan = 1f; // 구현에 실패해서 안쓰지만 일단 두기. 23.07.24 // 좌(우)로 이동 가능한 x 최대값. 
        //this.fMarker_Speed = 1f; // 구현에 실패해서 안쓰지만 일단 두기. 23.07.24 // 이동 속도. 



        Check_WhoAmI_AndPlaySound();


    }

    #region Visual effects when a user tapped.

    void PopEffect()
    {
        if( crPopEffect != null ) StopCoroutine(crPopEffect); 

        // 효과 코루틴 시작.
        // 모바일에서 움직임이 끊어져서.. 고정 시간으로 변경: 25fps
        crPopEffect = StartCoroutine( MakeMe_Pop(0.15f, 0.04f) );

    }

    IEnumerator MakeMe_Pop(float fMaxIncSize, float fInterval)
    {
        // 주의! 이 브릭이 정방향 일때만..

        yield return null; // 다음 프레임까지 깔끔하게 기다림. 

        float fSizeSpan = 0.03f;

        //Vector3 vNewSize = new Vector3(1.1f, 1.1f, 1.1f);

        //----------------------
        // 커지는 단계
        for(float fSizeInc = 0f; fSizeInc < fMaxIncSize; fSizeInc += fSizeSpan)
        {
            Vector3 vNewSize = new Vector3(fSizeInc, fSizeInc, fSizeInc);

            this.transform.localScale = vOrigianlSize + vNewSize;

            yield return new WaitForSeconds(fInterval);
        }

        //----------------------
        // 잠시 멈추는 단계
        yield return new WaitForSeconds(0.5f);

        //this.transform.localScale = this.vOrigianlSize;

        //Vector3 vChangedSize = this.transform.localScale;
        /*
        //----------------------
        // 작아지는 단계 
        // : 거의 원래대로 만드는 함수. 
        // : 비선형적으로 작아지며, 끝에는 사라지기. 
        // Ref. https://docs.unity3d.com/ScriptReference/Mathf.Sin.html 
        //
        fSizeSpan = 0.06f; // 작아지는 속도는 좀 빠르게?
        //

        //for(float fSizeInc = fMaxIncSize; fSizeInc > 0f; fSizeInc -= fSizeSpan)
        for(float fSizeInc = 0f; fSizeInc < 1f; fSizeInc += fSizeSpan)
        //for(float fSizeInc = 0f; fSizeInc < fMaxIncSize; fSizeInc += fSizeSpan)// 살짝 작아졌다가?
        {
            Vector3 vNewSize = new Vector3(fSizeInc, fSizeInc, fSizeInc);

            //this.transform.localScale = vOrigianlSize - vNewSize;
            this.transform.localScale = vChangedSize - vNewSize;

            yield return new WaitForSeconds(fInterval);
        }
        */

        //----------------------
        // 사라지는 단계
        // 
        //Destroy(this.transform.gameObject, 0.1f);


    }

    void VanishingEffect()
    {

        if( crVanishingEffect != null ) StopCoroutine(crVanishingEffect); 

        // 효과 코루틴 시작.

        // 모바일에서 움직임이 끊어져서.. 고정 시간으로 변경: 25fps
        crVanishingEffect = StartCoroutine( MakeMe_Disappear_typeA(0.15f, 0.04f) );

    }

    IEnumerator MakeMe_Disappear_typeA(float fMaxIncSize, float fInterval)
    {
        // 주의! 이 브릭이 정방향 일때만..

        yield return null; // 다음 프레임까지 깔끔하게 기다림. 

        float fSizeSpan = 0.03f;

        //Vector3 vNewSize = new Vector3(1.1f, 1.1f, 1.1f);

        //----------------------
        // 커지는 단계
        for(float fSizeInc = 0f; fSizeInc < fMaxIncSize; fSizeInc += fSizeSpan)
        {
            Vector3 vNewSize = new Vector3(fSizeInc, fSizeInc, fSizeInc);

            this.transform.localScale = vOrigianlSize + vNewSize;

            yield return new WaitForSeconds(fInterval);
        }

        //----------------------
        // 잠시 멈추는 단계
        yield return new WaitForSeconds(1f);

        /*
        Vector3 vChangedSize = this.transform.localScale;

        //----------------------
        // 작아지는 단계 
        // : 거의 원래대로 만드는 함수. 
        // : 비선형적으로 작아지며, 끝에는 사라지기. 
        // Ref. https://docs.unity3d.com/ScriptReference/Mathf.Sin.html 
        //
        fSizeSpan = 0.06f; // 작아지는 속도는 좀 빠르게?
        //

        //for(float fSizeInc = fMaxIncSize; fSizeInc > 0f; fSizeInc -= fSizeSpan)
        for(float fSizeInc = 0f; fSizeInc < 1f; fSizeInc += fSizeSpan)
        //for(float fSizeInc = 0f; fSizeInc < fMaxIncSize; fSizeInc += fSizeSpan)// 살짝 작아졌다가?
        {
            Vector3 vNewSize = new Vector3(fSizeInc, fSizeInc, fSizeInc);

            //this.transform.localScale = vOrigianlSize - vNewSize;
            this.transform.localScale = vChangedSize - vNewSize;

            yield return new WaitForSeconds(fInterval);
        }
        */

        //----------------------
        // 사라지는 단계
        // 
        Destroy(this.transform.gameObject, 0.1f);


    }

    #endregion

    #region SoundRelated

    private void Check_WhoAmI_AndPlaySound()
    {

        this.brickSpeaker.clip = ContentsManager.Instance.Check_WhoAmI_AndPlaySound_CodeOrNote( this.name );

        brickSpeaker.Play();

    }


    private void AmI_Ckey_thenShowScore(string sMyName)
    {
        // 내 이름과 키에 해당하는 악보를 보여주기. 
        // 처음과, 틀린 경우에는 보여주지 않고, 맞는 경우에만 보여주기 위해 함수를 나눔. 
        // 한꺼번에 해도 되지만, 보기 산만하니까, 키별로 나누어서..

        //-----------
        // C 키
        switch( sMyName )
        {
            case "C4": // C4 음. 
                //this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[0];
                this.trChildObject_Image.gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[0];
                break;
            case "D4":
                //this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[2];
                this.trChildObject_Image.gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[1];
                break;
            case "E4":
                //this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[4];
                this.trChildObject_Image.gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[2];
                break;
            case "F4": // ""instScaleBrick_C_F4":
                //this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[5];
                this.trChildObject_Image.gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[3];
                break;
            case "G4": // "instScaleBrick_C_G4":
                //this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[7];
                this.trChildObject_Image.gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[4];
                break;
            case "A4": // "instScaleBrick_C_A4":
                //this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[9];
                this.trChildObject_Image.gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[5];
                break;
            case "B4": // "instScaleBrick_C_B4":
                //this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[11];
                this.trChildObject_Image.gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[6];
                break;
            default:
                //this.brickSpeaker.clip = ContentsManager.Instance.AudioClip_Error;
                this.trChildObject_Text.gameObject.GetComponent<TextMeshPro>().text = "Err.Mat.";
                break;
        }

        //brickSpeaker.Play();

    }



    #endregion

#region Public Methods regarding to a displaying the brick face.

    public void SetMe_Focused(bool bShowTheFocusMark)
    {


        // 움직임 제어가 있어서.. 순서가 중요해서 조건문으로.. (active가 안된 물체를 access하지 않게.)

        if( bShowTheFocusMark )
        {
            this.trChildObject_Marker.gameObject.SetActive(true); // 마크를 보이게

            this.SetToMove_theFocusMarkObject(true); // 마크를 주기적으로 움직이게. 더 주목되도록. 

        }else
        {
            this.SetToMove_theFocusMarkObject(false); // 움직이는 마크를 멈춤.

            this.trChildObject_Marker.gameObject.SetActive(false); // 마크를 안보이게.

        }

    }

    public void SetMe_asQuestion(bool bShowTheFocusMark)
    {
        // SSS_SSS_PlayManager.cs 에서 부르는 함수. 
        // It is natural to control "my" material using my method, rather than a scrip-wide outside method.

        // Functions
        // > Show or hide my FocusMark (ball? or arrow) according to input value.
        // > Make my tmp child's string '?'.
        // > Make my object child's material _TapMe_Icon_ (손가락 모양 머티리얼)


        this.trChildObject_Marker.gameObject.SetActive(bShowTheFocusMark);

        //this.ShowTheFocusMark_withMotion_typeA(bShowTheFocusMark);

        this.trChildObject_Text.gameObject.GetComponent<TextMeshPro>().text = "?";

        this.trChildObject_Image.gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matQuiz_Tap_Mark_Image;

    }

    public void SetMe_asQuestion()
    {
        // SSS_SSS_PlayManager.cs 에서 부르는 함수. 
        // It is natural to control "my" material using my method, rather than a scrip-wide outside method.

        // Functions
        // > Show or hide my FocusMark (ball? or arrow) according to input value.
        // > Make my tmp child's string '?'.
        // > Make my object child's material _TapMe_Icon_ (손가락 모양 머티리얼)

        this.trChildObject_Marker.gameObject.SetActive(false);

        this.trChildObject_Text.gameObject.GetComponent<TextMeshPro>().text = "?";

        this.trChildObject_Image.gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matQuiz_Tap_Mark_Image;

    }

    public void SetMe_asCorrect()
    {
        // SSS_SSS_PlayManager.cs 에서 부르는 함수. 

        // Functions
        // > Make my tmp child's string with my real code name. e.g. Dm
        // > Make my object child's material _Correct_

        this.trChildObject_Marker.gameObject.SetActive(false); // 맞았으니, 포커스 마크는 없애고. 

        this.trChildObject_Text.gameObject.GetComponent<TextMeshPro>().text 
                = this.name;

        //this.trChildObject_Image.gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matQuiz_O_Mark_Image;
        
        // 맞다 표시 대신에, 맞는 악보를.. 
        this.AmI_Ckey_thenShowScore(this.name);

        // 맞았을 때는, 맞은 음을 한번 플레이 해 주고 사라지기. 
        brickSpeaker.Play();

        // 이제는, 패턴이 다 완성되고 나서 사라져야 함! 23.07.24        
        //Invoke("MovingAway", 0.7f);

    }

    public void SetMe_asWrong()
    {
        // SSS_SSS_PlayManager.cs 에서 부르는 함수. 

        // Functions
        // > Make my tmp child's string '?' still.
        // > Make my object child's material _Wrong_

        // 틀린 경우, 해당 문제 브릭을 통 튀게.. 정신차리고 탭하라고.. ㅎ
        // 굴러떨어진 브릭 다 처리할 수 있을때, 판단해 보고 살리기. 23.07.24
        PopEffect();

        this.trChildObject_Text.gameObject.GetComponent<TextMeshPro>().text = "X";

        this.trChildObject_Image.gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matQuiz_X_Mark_Image;

    }

    public void MakeMe_Byebye()
    {
        // 나를 사라지게 하는 함수. 
        // 여러개의 다른 인스턴스 브릭들이 겹쳐 있는 상황일 것이므로.. 안 부딪히게 사라져야. 

        this.MovingAway_type3();

        Invoke("IveDoneMyRole", 1f);

    }

#endregion

#region Public Methods regarding to a controlling of the brick (me)

    private void SetToMove_theFocusMarkObject(bool bAmIactive)
    {

        // 포커스 마크 입체 도형을 움직임. 

        if( bAmIactive )
        {

            if( crMovingTheMark != null ) StopCoroutine( crMovingTheMark );

            // 새로 움직일 때는 마커 위치를 원위치 하고 시작!
            //this.trChildObject_Marker.position = this.v3Marker_InitialPosition;

            //this.crMovingTheMark = StartCoroutine( MovingTheFocusMark_withMotion_typeA( 0.04f ) );
            //this.crMovingTheMark = StartCoroutine( MovingTheFocusMark_withMotion_typeB( 0.5f ) );
            this.crMovingTheMark = StartCoroutine( MovingTheFocusMark_withMotion_typeC( 0.04f ) );

        }else
        {

            if( crMovingTheMark != null ) StopCoroutine( crMovingTheMark );

            // 멈출 때도 마커 위치를 원위치!
            //this.trChildObject_Marker.position = this.v3Marker_InitialPosition;

        }

    }

    IEnumerator MovingTheFocusMark_withMotion_typeC(float fFrameTimePeriod)
    {
        // 다 안되니.. 일단 돌게.. 마커는 큐브로 바꾸고. 

        yield return null;

        while( true )
        {
            this.trChildObject_Marker.Rotate(10f, 0f, 0f); // x축 기준으로 돌기. 단위시간당 10도. 

            yield return new WaitForSeconds( fFrameTimePeriod );
        }
    }

    private void MovingAway()
    {
        //MovingAway_type1();
        MovingAway_type2();


    }

    private void MovingAway_type1()
    {
        // 왼쪽으로 가서 사라지기.
        if(Application.isEditor) Debug.Log("MOVE AWAY!");

        this.GetComponent<Rigidbody>().AddForce(Vector3.left*50f, ForceMode.Impulse); // Force, Impulse, Acceleration

        Invoke("IveDoneMyRole", 1f);

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

    private void OnMouseDown()
    {
        PopEffect();

        Check_WhoAmI_AndPlaySound();

    }


}
