///////////////////////////////////////////////////////////////////////////////////
//
// PickNumber : 화음번호 고르기 퀴즈 단계어서, (위에서 떨어지는) 문제 브릭의 제어를 위한 스크립트. 
// 
// 주님, 힘 주시고 용기 주시고 지혜와 집중력 주셔서, 이렇게 진행할 수 있게 해 주셔서 감사합니다!
// 
// 하는일
// : 상위 스크립트에서 이 브릭의 이름을 해당 키의 특정 코드 값이름으로 생성. 
// : 생성시 사운드 해당 사운드 출력. 
// : 그림 부분에는 스피커 모양, 글자는 ? 표. 
// : 탭하면 해당 사운드 출력. 
// : 이 브릭과 사용자가 D&D 한 브릭이 만나면, 글자 부분에 정답을 보여주고, 그림 부분에는 맞으면 O와 띵동, 틀리면 X와 부저음.
// 
// 
// 23.07.12. sjjo. 처음 만듭니다!
// 
///////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quiz_SoundBrick_typeA_Control : MonoBehaviour
{
    private AudioSource brickSpeaker;

    private Coroutine crVanishingEffect;
    
    private Vector3 vOrigianlSize;

    // Start is called before the first frame update
    void Start()
    {

        // 게임 매니져에게, 내가 누구인지 알려줘야. 
        // 그래야, 키패드 컨트롤 스크립트에서 맞게 눌렀는지 아닌지 판단하지. 
        GameManager.Instance.sCodeMode_Level_PickNumber_QuizBrickName = this.name;

        // sCodeMode_Tapped_Keypad_inTermsOfTheSelectedKey


        this.brickSpeaker = this.gameObject.AddComponent<AudioSource>();
        
        this.brickSpeaker.loop = false;


        this.vOrigianlSize = this.transform.localScale; 


        Check_WhoAmI_AndPlaySound();

        //Invoke("VanishingEffect", 1f);

    }

    void VanishingEffect()
    {

        if( crVanishingEffect != null ) StopCoroutine(crVanishingEffect); 

        // 효과 코루틴 시작.
        //crVanishingEffect = StartCoroutine( MakeMe_Disappear(0.15f, 0.01f) );
        //crVanishingEffect = StartCoroutine( MakeMe_Disappear_typeA(0.15f, Time.deltaTime) );
        //crVanishingEffect = StartCoroutine( MakeMe_Disappear_typeB(0.15f, Time.deltaTime) );

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

        // this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = GameManager.Instance.matCkey_ScoreImage[0];

//        if(Application.isEditor)

        switch( this.name )
        {
            case "instCodeBrick_C__1do": // C
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey_Code[0];
                break;
            case "instCodeBrick_C__2do":
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey_Code[1];
                break;
            case "instCodeBrick_C__3do":
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey_Code[2];
                break;
            case "instCodeBrick_C__4do":
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey_Code[3];
                break;
            case "instCodeBrick_C__5do":
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey_Code[4];
                break;
            case "instCodeBrick_C__6do":
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey_Code[5];
                break;
            case "instCodeBrick_C__7do":
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey_Code[6];
                break;
            default:
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey_Code[6];
                break;
        }

        brickSpeaker.Play();
    }

    private void OnMouseDown()
    {
        Check_WhoAmI_AndPlaySound();


    }

}
