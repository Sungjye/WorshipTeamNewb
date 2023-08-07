///////////////////////////////////////////////////////////////////////////////////////////////////////
// 주님, 주님의 인도하심을 올바로 분별하게 하시고 힘을 주시고 용기를 주시고 주님과 같은 선한 마음을 주십시오.. 
// 
// PickNote : 음 맞추기 퀴즈 단계에서, (위에서 떨어지는) 문제 브릭 프리팹에 컴포넌트로 붙는 스크립트. (제어)
// 
// 하는일
// : 상위 스크립트에서, 이 브릭의 이름을, 해당 키의 특정 음(노트) 값이름으로 생성. 
// : 생성시 해당하는 사운드 출력. 
// : 그림 부분에는 탭 손 모양 (사용이 직관적이도록), 글자는 ? 표
// : 탭하면 해당 사운드 출력.
// : 이 브릭과 사용자가 탭한 브릭의 이름이 같으면, 글자 부분에 정답 음값을 보여주고, 
//   그림 부분에는 맞으면 O와 띵동, 틀리면 X와 부저음.  (23.07.18 TBD.)
// 
// 23.07.18. sjjo. Oh.. Jesus.. I want in You..
//
///////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class Quiz_SoundBrick_typeB_Control : MonoBehaviour
{
    private AudioSource brickSpeaker;

    private Coroutine crPopEffect, crVanishingEffect;
    
    private Vector3 vOrigianlSize;

    // 이 오브젝트의 자체의 이름은, D4b 
    // 음 소리를 내거나, 사용자 입력과 비교하기 위해서, 파싱할 필요가 없을듯.. 

    // Start is called before the first frame update
    void Start()
    {

        if(Application.isEditor) Debug.Log("Quiz brick name: " + this.name); // 

        this.brickSpeaker = this.gameObject.AddComponent<AudioSource>();
        
        this.brickSpeaker.loop = false;


        this.vOrigianlSize = this.transform.localScale; 


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


//        if(Application.isEditor)

/*
        switch( GameManager.Instance.eSelectedKey )
        {
            case eAVAILABLEKEYS.C:
                AmI_Ckey_thenPlaySound(this.name);
                break;
            case eAVAILABLEKEYS.G:
                //AmI_Gkey_thenPlaySound(this.name);
                break;
            default:
                break;
        }
      
*/
        //AmI_Dkey_thenPlaySound(this.name);
        //AmI_Akey_thenPlaySound(this.name);
        //AmI_Ekey_thenPlaySound(this.name);

        //AmI_Fkey_thenPlaySound(this.name);
        // .. 


    }

/*
    private void AmI_Ckey_thenPlaySound(string sMyName)
    {
        // 한꺼번에 해도 되지만, 보기 산만하니까, 키별로 나누어서..

        //-----------
        // C 키
        switch( sMyName )
        {
            case "C4": // C4 음. 
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[0];
                //this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[0];
                break;
            case "D4":
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[2];
                //this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[1];
                break;
            case "E4":
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[4];
                //this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[2];
                break;
            case "F4": // ""instScaleBrick_C_F4":
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[5];
                //this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[3];
                break;
            case "G4": // "instScaleBrick_C_G4":
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[7];
                //this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[4];
                break;
            case "A4": // "instScaleBrick_C_A4":
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[9];
                //this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[5];
                break;
            case "B4": // "instScaleBrick_C_B4":
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[11];
                //this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[6];
                break;
            default:
                this.brickSpeaker.clip = ContentsManager.Instance.AudioClip_Error;
                break;
        }

        brickSpeaker.Play();

    }
*/

    private void AmI_Ckey_thenShowScore(string sMyName)
    {
        // 내 이름과 키에 해당하는 악볼르 보여주기. 
        // 처음과, 틀린 경우에는 보여주지 않고, 맞는 경우에만 보여주기 위해 함수를 나눔. 
        // 한꺼번에 해도 되지만, 보기 산만하니까, 키별로 나누어서..

        //-----------
        // C 키
        switch( sMyName )
        {
            case "C4": // C4 음. 
                //this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[0];
                this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[0];
                break;
            case "D4":
                //this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[2];
                this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[1];
                break;
            case "E4":
                //this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[4];
                this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[2];
                break;
            case "F4": // ""instScaleBrick_C_F4":
                //this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[5];
                this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[3];
                break;
            case "G4": // "instScaleBrick_C_G4":
                //this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[7];
                this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[4];
                break;
            case "A4": // "instScaleBrick_C_A4":
                //this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[9];
                this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[5];
                break;
            case "B4": // "instScaleBrick_C_B4":
                //this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[11];
                this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[6];
                break;
            default:
                //this.brickSpeaker.clip = ContentsManager.Instance.AudioClip_Error;
                this.transform.GetChild(1).gameObject.GetComponent<TextMeshPro>().text = "Err.Mat.";
                break;
        }

        //brickSpeaker.Play();

    }



    #endregion

#region Public Methods regarding to a displaying the brick face.
    public void SetMe_asQuestion()
    {
        // SSS_SSS_PlayManager.cs 에서 부르는 함수. 
        // It is natural to control "my" material using my method, rather than a scrip-wide outside method.

        // Functions
        // > Make my tmp child's string '?'.
        // > Make my object child's material _TapMe_Icon_ (손가락 모양 머티리얼)

        this.transform.GetChild(1).gameObject.GetComponent<TextMeshPro>().text = "?";

        this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matQuiz_Tap_Mark_Image;

    }

    public void SetMe_asCorrect()
    {
        // SSS_SSS_PlayManager.cs 에서 부르는 함수. 

        // Functions
        // > Make my tmp child's string with my real code name. e.g. Dm
        // > Make my object child's material _Correct_

        this.transform.GetChild(1).gameObject.GetComponent<TextMeshPro>().text 
                = this.name;

        //this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matQuiz_O_Mark_Image;
        
        // 맞다 표시 대신에, 맞는 악보를.. 
        this.AmI_Ckey_thenShowScore(this.name);

        // 맞았을 때는, 맞은 음을 한번 플레이 해 주고 사라지기. 
        brickSpeaker.Play();


        Invoke("MovingAway", 0.7f);

    }

    public void SetMe_asWrong()
    {
        // SSS_SSS_PlayManager.cs 에서 부르는 함수. 

        // Functions
        // > Make my tmp child's string '?' still.
        // > Make my object child's material _Wrong_

        PopEffect();

        this.transform.GetChild(1).gameObject.GetComponent<TextMeshPro>().text = "X";

        this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matQuiz_X_Mark_Image;

    }
#endregion

#region Public Methods regarding to a controlling of the brick (me)

    public void MakeMe_Byebye()
    {
        // 나를 사라지게 하는 함수. 
        // 여러개의 다른 인스턴스 브릭들이 겹쳐 있는 상황일 것이므로.. 안 부딪히게 사라져야. 

        this.MovingAway_type3();

        // 중복인듯. Invoke("IveDoneMyRole", 1f);

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
