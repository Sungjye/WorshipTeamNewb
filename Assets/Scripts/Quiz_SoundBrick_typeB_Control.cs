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
// 23.08.21. sjjo. 연타하면 브릭이, 체크매칭 불가하게 많이 나오는 문제 해결 위해, 코드 추가.
//
///////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class Quiz_SoundBrick_typeB_Control : MonoBehaviour
{
    public bool bSetMeCorrectOnce; // 23.08.21 사용자 입력이 나의 정체와 (한번) 맞았음을 나타내는 플래그. 한번 true로 셋되면 없어지기 전까지는 변화 없어야 함. (사용자 막연타에 대응)

    public bool bSetMeWrongOnce; // 23.08.29 사용자 입력의 나의 정체와 (한번) 틀렸음을 나타내는 플래그. ScaleMode_Level_RecogKeys_PlayManager 클래스에서만 쓰임.. 왜? 틀리면 바로 다 치우고, 새로운 문제 내기 위해서. 

    private AudioSource brickSpeaker;

    private Coroutine crPopEffect, crVanishingEffect;
    
    private Vector3 vOrigianlSize;

    private Coroutine crRemoveMyself;


    // 이 오브젝트의 자체의 이름은, D4b 
    // 음 소리를 내거나, 사용자 입력과 비교하기 위해서, 파싱할 필요가 없을듯.. 

    void Awake()
    {
        // 어웨이크로 옮김. 

        //-------------------------------------------
        // 현재 브릭의 데이터 관리를 위한 준비. 
        // 왜 관리? 1) 매칭 불가한 연타 생성 방지. 
        // 23.08.21
        
        // 생성할 때, 싱글턴 리스트에 담아주고.. 나 스스로의 이름을.
        // 안쓰고 구현. GameManager.Instance.li_gmobj_CurrentlyExistingBricks.Add(this.transform.gameObject); 

        // 23. 08.21
        // 일단 지금 존재하는 브릭, 왼쪽으로 쌩 가서 아직 사라지기 전인 브릭 포함. 몇개임?
        // 안쓰고 구현. GameManager.Instance.ScoreSystem_Check_CurrentlyExistingBricks();


        this.bSetMeCorrectOnce = false;
        this.bSetMeWrongOnce = false; // 이건, "사용자 입력"에 의해 한번도 wrong 으로 세팅 되지 않았다는 의미. 
        
    }

    // Start is called before the first frame update
    void Start()
    {

        this.crRemoveMyself = null;

        if(Application.isEditor) Debug.Log("Quiz brick name: " + this.name); // 

        this.brickSpeaker = this.gameObject.AddComponent<AudioSource>();
        
        this.brickSpeaker.loop = false;


        this.vOrigianlSize = this.transform.localScale; 


        Check_WhoAmI_AndPlaySound();




    }

    /*
    void OnCollisionEnter(Collision objs)
    {
        if(Application.isEditor)
        {
            Debug.Log(objs.gameObject.name);
        }

        if( objs.gameObject.name == "Ground")
            this.bAmI_Landed = true;

    }
    */

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


    #endregion

    #region SoundRelated

    private void Check_WhoAmI_AndPlaySound()
    {



        this.brickSpeaker.clip = ContentsManager.Instance.Check_WhoAmI_Retrieve_myAudioClip_CodeOrScale( this.name );

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
/*
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

    }
*/


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
        //this.AmI_Ckey_thenShowScore(this.name);
        // 공용화. 23.08.07
        this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material 
                            = ContentsManager.Instance.Check_WhoAmI_Retrieve_myMusicalNotation_CodeOrScale(this.name);

        // 플레이 매니져의 업데이트 함수에서 확인하기 위함. 
        this.bSetMeCorrectOnce = true;

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

        // 플레이 매니져의 업데이트 함수에서 확인하기 위함. 
        // 어웨이크에서 하고, 상태는 비가역 적이므로 주석처리해도 무방. this.bSetMeCorrectOnce = false;

        // ScaleMode_Level_RecogKeys_PlayManager 클래스의, 플레이 매니져의 업데이트 함수에서 확인하기 위함. 
        this.bSetMeWrongOnce = true;

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

        float fDelay = 0.1f; // 이 딜레이, 좀 브릭이 왼쪽으로 쌩 날아가고 나서 없애줘야 하므로. 눈 앞에서 바로 사라지는 것보다..


        //=======================================================================================
        // Q.. GameManager.Instance.li_gmobj_CurrentlyExistingBricks 에 있는 게임오브젝트와, 
        //     이 스크립의의 실제부모인 게임오브젝트 인스턴스는, 실제로 메모리 상에서 같은 '것' 인가?.. 음..
        // 어쨌든 순서를 지켜보자: 이미 디스트로이된 것을 리스트에서 remove 하면 이상하니까..

        //=======================================================================================
        // 누군가가 생성한 "나" 를
        // 이제 사라질 것이므로, 리스트에서 날린다. 데이터로서만.. 데이터 리스트에서 사라진다고, 내가 없어진 것은 아님. 
        //GameManager.Instance.li_gmobj_CurrentlyExistingBricks.Remove(this.transform.gameObject); // 중복된 이름의 오브젝트가 있어도 잘 제거 되려나?... 잘됩니다, 감사합니다, 주님!!!
        this.RemoveMyData_afterThisDelay( fDelay );

        //=======================================================================================
        // 나 자신을 없애기..
        // 여기에 넣으면 내 자신이 파괴되어서, 코루틴이 실행 안돰. Destroy(this.transform.gameObject, fDelay);


    }

    private void RemoveMyData_afterThisDelay(float fAfterThisTime)
    {
        if(this.crRemoveMyself != null) StopCoroutine(crRemoveMyself);

        this.crRemoveMyself = StartCoroutine( crRemoveMyData_afterThisDelay(fAfterThisTime) );        
    }


    IEnumerator crRemoveMyData_afterThisDelay(float fDelayedTime)// (GameObject gmobjTarget, float fDelayedTime)
    {
        // 인자로 받은 시간 만큼 기다렸다가 받은 오브젝트를, 데이터 리스트에서 제거해 주는 함수. 
        yield return null;

        yield return new WaitForSeconds(fDelayedTime);

        // 안쓰고 구현. GameManager.Instance.li_gmobj_CurrentlyExistingBricks.Remove(this.transform.gameObject); // 중복된 이름의 오브젝트가 있어도 잘 제거 되려나?... 잘됩니다, 감사합니다, 주님!!!

        // 23. 08.21
        // 일단 지금 존재하는 브릭, 왼쪽으로 쌩 가서 아직 사라지기 전인 브릭 포함. 몇개임?
        // 안쓰고 구현. GameManager.Instance.ScoreSystem_Check_CurrentlyExistingBricks();

        //=======================================================================================
        // 나 자신을 없애기..
        // 여기에 "안" 넣으면 내 자신이 파괴되어서, 코루틴이 실행 안돰. 
        Destroy(this.transform.gameObject, 0f);

    }


#endregion

    private void OnMouseDown()
    {
        PopEffect();

        Check_WhoAmI_AndPlaySound();

    }


}
