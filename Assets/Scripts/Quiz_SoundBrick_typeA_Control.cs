///////////////////////////////////////////////////////////////////////////////////
//
// PickNumber : 화음번호 맞추기 퀴즈 단계어서, (위에서 떨어지는) 문제 브릭 프리팹에 컴포넌트로 붙는 스크립트. (제어)
// 
// 주님, 힘 주시고 용기 주시고 지혜와 집중력 주셔서, 이렇게 진행할 수 있게 해 주셔서 감사합니다!
// 
// 하는일
// : 상위 스크립트에서 이 브릭의 이름을 해당 키의 특정 코드 값이름으로 생성. 
// : 생성시 해당 사운드 출력. 
// : 그림 부분에는 탭 손 모양 (사용이 직관적이도록), 글자는 ? 표. 
// : 탭하면 해당 사운드 출력. 
// : 이 브릭과 사용자가 탭한 브릭의 이름이 같으면, 
//   글자 부분에 정답 코드를 보여주고, 그림 부분에는 맞으면 O와 띵동, 틀리면 X와 부저음. (23.07.18 TBD.)
// 
// 
// 23.07.12. sjjo. 처음 만듭니다!
// 
///////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;



public class Quiz_SoundBrick_typeA_Control : MonoBehaviour
{
    private AudioSource brickSpeaker;

    private Coroutine crPopEffect, crVanishingEffect;
    
    private Vector3 vOrigianlSize;

    // 이 오브젝트의 자체의 이름은, instCodeBrick_C__2do 
    // 이런 식일 텐데, 이 값에는 이것으로 파싱한, 이를테면 Dm 이 들어가 있다. 
    public string sMyDictionariedCodeName; 


    // Start is called before the first frame update
    void Start()
    {

        // 게임 매니져에게, 내가 누구인지 알려줘야. 
        // 그래야, 키패드 컨트롤 스크립트에서 맞게 눌렀는지 아닌지 판단하지. 
        // Nope, as it is.. ContentsManager.Instance.sCodeMode_Level_PickNumber_QuizBrickName = this.name;

        // 여기에는 몇도가 아니라, 구체적인 코드 이름이 들어간다. 
        string sWhatDoNumber = ContentsManager.Instance.ParsingTheQuiz_SoundBrickName(this.name);
        //ContentsManager.Instance.sCodeMode_Level_PickNumber_QuizBrickName
        this.sMyDictionariedCodeName
            = ContentsManager.Instance.dicCode_byKeyAndDoNum[GameManager.Instance.eSelectedKey][(eDO_NUMBER)System.Enum.Parse(typeof(eDO_NUMBER), sWhatDoNumber)];

        if(Application.isEditor) Debug.Log("Quiz brick name. Original: " + this.name + ". Parsed: " + this.sMyDictionariedCodeName);

        // sCodeMode_Tapped_Keypad_inTermsOfTheSelectedKey


        this.brickSpeaker = this.gameObject.AddComponent<AudioSource>();
        
        this.brickSpeaker.loop = false;


        this.vOrigianlSize = this.transform.localScale; 


        Check_WhoAmI_AndPlaySound();


        //Invoke("MovingAway_type1", 2f);

    }


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



    private void Check_WhoAmI_AndPlaySound()
    {

//        if(Application.isEditor)

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
        
        
        //AmI_Dkey_thenPlaySound(this.name);
        //AmI_Akey_thenPlaySound(this.name);
        //AmI_Ekey_thenPlaySound(this.name);

        //AmI_Fkey_thenPlaySound(this.name);
        // .. 


    }

    private void AmI_Ckey_thenPlaySound(string sMyName)
    {
        // 한꺼번에 해도 되지만, 보기 산만하니까, 키별로 나누어서..

        //-----------
        // C 키
        switch( sMyName )
        {
            case "instCodeBrick_C__1do": // C
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Code[0];
                break;
            case "instCodeBrick_C__2do":
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Code[1];
                break;
            case "instCodeBrick_C__3do":
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Code[2];
                break;
            case "instCodeBrick_C__4do":
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Code[3];
                break;
            case "instCodeBrick_C__5do":
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Code[4];
                break;
            case "instCodeBrick_C__6do":
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Code[5];
                break;
            case "instCodeBrick_C__7do":
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Code[6];
                break;
            default:
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Code[6];
                break;
        }

        brickSpeaker.Play();

    }


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
                = this.sMyDictionariedCodeName;
                //= ContentsManager.Instance.sCodeMode_Level_PickNumber_QuizBrickName;

        this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matQuiz_O_Mark_Image;

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
