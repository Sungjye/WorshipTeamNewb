///////////////////////////////////////////////////////////////////////////////////
//
// 단음 (scale) 브릭 프리팹의 제어를 위한 스크립트. 
// 
// 
// 주님, 안정적으로 동작할 수 있게, 차분히 만들게 해 주십시오. 
//
// 23.07.11. sjjo. 큐브 기반으로, 하위의 악보 큐브, 글자 TMP로 구성한 프리팹 대상. 
// 
///////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleBrick_typeA_Control : MonoBehaviour
{
    private AudioSource brickSpeaker;

    private Coroutine crVanishingEffect;
    
    private Vector3 vOrigianlSize;



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

//        if(Application.isEditor)

        switch( this.name )
        {
            case "instScaleBrick_C_C4": // C4 음. 
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey[0];
                this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = GameManager.Instance.matCkey_ScoreImage[0];
                break;
            case "instScaleBrick_C_D4":
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey[1];
                this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = GameManager.Instance.matCkey_ScoreImage[1];
                break;
            case "instScaleBrick_C_E4":
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey[2];
                this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = GameManager.Instance.matCkey_ScoreImage[2];
                break;
            case "instScaleBrick_C_F4":
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey[3];
                this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = GameManager.Instance.matCkey_ScoreImage[3];
                break;
            case "instScaleBrick_C_G4":
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey[4];
                this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = GameManager.Instance.matCkey_ScoreImage[4];
                break;
            case "instScaleBrick_C_A4":
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey[5];
                this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = GameManager.Instance.matCkey_ScoreImage[5];
                break;
            case "instScaleBrick_C_B4":
                this.brickSpeaker.clip = GameManager.Instance.aryAudioClips_Ckey[6];
                this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = GameManager.Instance.matCkey_ScoreImage[6];
                break;
            default:
                this.brickSpeaker.clip = GameManager.Instance.AudioClip_Error;
                break;
        }

        brickSpeaker.Play();
    }

}
