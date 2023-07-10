using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeBrick_typeA_Control : MonoBehaviour
{
    private AudioSource brickSpeaker;
    //public AudioClip c_I, c_ii, c_iii, c_IV, c_V, c_vi, C_VII,

    
    private Coroutine crVanishingEffect;


     private Vector3 vOrigianlSize;

    void Awake()
    {
        


    }

    // Start is called before the first frame update
    void Start()
    {
        this.brickSpeaker = this.gameObject.AddComponent<AudioSource>();
        
        this.brickSpeaker.loop = false;


        this.vOrigianlSize = this.transform.localScale; 


        Check_WhoAmI_AndPlaySound();

        Invoke("VanishingEffect", 1f);
        

        // 각도 변환이 어렵지는 않습니다. 도를 라디안으로 바꿀 때는 π/180 (= 0.0174533)를 곱하면 되고, 라디안을 도로 바꿀 때는 180/π (= 57.2958)를 곱하면 되죠.
        float fAngle_inDegree = 45f;
        float fAngle_inRadian = fAngle_inDegree * 0.0174533f; // * pMathf.PI/180
        float fTfuncReuslt = Mathf.Sin(fAngle_inRadian);
        Debug.Log(vOrigianlSize + "  |  "+ fTfuncReuslt);

    }

    void VanishingEffect()
    {

        if( crVanishingEffect != null ) StopCoroutine(crVanishingEffect); 

        // 효과 코루틴 시작.
        //crVanishingEffect = StartCoroutine( MakeMe_Disappear(0.15f, 0.01f) );
        crVanishingEffect = StartCoroutine( MakeMe_Disappear_typeA(0.15f, Time.deltaTime) );
        //crVanishingEffect = StartCoroutine( MakeMe_Disappear_typeB(0.15f, Time.deltaTime) );

    }

/*

    IEnumerator MakeMe_Disappear_typeB(float fMaxIncSize, float fInterval)
    {
        // 사인 곡선에 의해서 커지고, 작아지게. 
        // 
        // 원본 크기의 스케일은 1f 이라는 가정 하에서. 
        // 
        // fMaxIncSize 이 크기 (비율) 까지 커지게. 
        // fMinDecSize 이 크기 (비율) 까지 작아지고, 이것 이하로 작아지면 사라지게(destroy)
        // fPopTime 크기가 커지는 시간. 즉, 커지는 것이 이 시간내로 되게
        // fDisappearTime 크기가 작아지는 시간. 즉, 작아지는 것이 이 시간 내로 되게. 




        yield return null; // 다음 프레임까지 깔끔하게 기다림. 

        // 삼각함수로 증분, 감분 구하기. 

        //    <증분>
        //    목표 초 : 90도 = 델타타임 : x도   (왜 90도? 사인함수 반주기에서 상승분이 90도, 하강분이 90도라서.)
        //    x도 = (90도 * 델타타임) / 목표 초
        //    sin 함수가 라디안을 쓰니까... 

        //    x 증분 in radian = (90도 * (PI/180) * 델타타임) / 목표 초
        //                     = ((PI/2) * 델타타임) / 목표 초

        //    x 감분 inradian = ((PI/2) * 델타타임) / 목표 초

        //    뭐, 크기니까, 크게 정밀도 중요하지 않으니.. PI/2 를 그냥 상수로 사용해서.. 

        //    PI/2 = 1.57563f

        
        float fPIdiv2 = 1.57563f; // Mathf.PI / 2f = 1.57563f

        float fIncrease_sizeDelta = (fPIdiv2 * fInterval) / fPopTime; // 넘어온 것이 델타타임 이므로. 
        float fDecrease_sizeDelta = (fPIdiv2 * fInterval) / fDisappearTime; 

        //float fSizeSpan = 0.03f;

        //Vector3 vNewSize = new Vector3(1.1f, 1.1f, 1.1f);

        //----------------------
        // 커지는 단계
        for(float fSizeInc = 0f; fSizeInc < fMaxIncSize; fSizeInc += fIncrease_sizeDelta)
        {
            Vector3 vNewSize = new Vector3(fSizeInc, fSizeInc, fSizeInc);

            this.transform.localScale = vOrigianlSize + vNewSize;

            yield return new WaitForSeconds(fInterval);
        }

        //----------------------
        // 잠시 멈추는 단계
        yield return new WaitForSeconds(0.7f);

        Vector3 vChangedSize = this.transform.localScale;

        //----------------------
        // 작아지는 단계 
        // : 거의 원래대로 만드는 함수. 
        // : 비선형적으로 작아지며, 끝에는 사라지기. 
        // Ref. https://docs.unity3d.com/ScriptReference/Mathf.Sin.html 
        //
        // fSizeSpan = 0.03f; // 작아지는 속도는 좀 빠르게?
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


    }
    */

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
        //yield return new WaitForSeconds(0.7f);

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

        Destroy(this.transform.gameObject, 0.1f);


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
        yield return new WaitForSeconds(0.7f);

        Vector3 vChangedSize = this.transform.localScale;

        //----------------------
        // 작아지는 단계 
        // : 거의 원래대로 만드는 함수. 
        //
        // fSizeSpan = 0.03f; // 작아지는 속도는 좀 빠르게?
        //

        //for(float fSizeInc = fMaxIncSize; fSizeInc > 0f; fSizeInc -= fSizeSpan)
        for(float fSizeInc = 0f; fSizeInc < fMaxIncSize; fSizeInc += fSizeSpan)
        //for(float fSizeInc = 0f; fSizeInc < fMaxIncSize; fSizeInc += fSizeSpan)// 살짝 작아졌다가?
        {
            Vector3 vNewSize = new Vector3(fSizeInc, fSizeInc, fSizeInc);

            //this.transform.localScale = vOrigianlSize - vNewSize;
            this.transform.localScale = vChangedSize - vNewSize;

            yield return new WaitForSeconds(fInterval);
        }


    }


    private void Check_WhoAmI_AndPlaySound()
    {
        switch( this.name )
        {
            case "instCodeBrick_C__1do": // C
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey[0];
                break;
            case "instCodeBrick_C__2do":
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey[1];
                break;
            case "instCodeBrick_C__3do":
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey[2];
                break;
            case "instCodeBrick_C__4do":
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey[3];
                break;
            case "instCodeBrick_C__5do":
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey[4];
                break;
            case "instCodeBrick_C__6do":
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey[5];
                break;
            case "instCodeBrick_C__7do":
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey[6];
                break;
            default:
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey[6];
                break;
        }

        brickSpeaker.Play();
    }

}
