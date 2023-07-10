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
        
    }

    void VanishingEffect()
    {

        if( crVanishingEffect != null ) StopCoroutine(crVanishingEffect); 

        // 효과 코루틴 시작.
        //crVanishingEffect = StartCoroutine( MakeMe_Disappear(0.15f, 0.01f) );
        crVanishingEffect = StartCoroutine( MakeMe_Disappear(0.15f, Time.deltaTime) );

    }

    IEnumerator MakeMe_Disappear(float fMaxIncSize, float fInterval)
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
