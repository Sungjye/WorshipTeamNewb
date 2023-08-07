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

        //-------------------------------------------
        // 스코어 증감 조건인지 확인 및 반영을 위한 데이터 준비. 
        // 해당 키에서의 도. 즉, 무슨 키이건, 1,3,5 이런식으로 리스트에 저장된다. 0 이 들어가면, 해당 스케일의 음이 아니라는 뜻. 
        // 이 리스트의 내용을 확인해서, 화음 인스턴시에잇 한것에 대해서 점수를 줄 수 있다!
        // 주님, 지혜를 주셔서 감사합니다!!! 2023.08.03
        
        // 생성할 때, 싱글턴 리스트에 담아주고.. 나 스스로의 이름을.
        GameManager.Instance.li_gmobj_CurrentlyExistingBricks.Add(this.transform.gameObject); 

        // Tentative.
        //GameManager.Instance.ScoreSystem_Check_CurrentlyExistingBricks();
        //GameManager.Instance.ScoreSystem_CheckAndApplyScore_ScaleMode_BasicHarmonies();


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


        //---------------------
        // 건반의 각 키에 붙어 있는, ScaleMode_Level_0_Control.cs 스크립트에서, 생성한 "나"를, 
        // 이제 사라질 것이므로, 리스트에서 날린다. 
        GameManager.Instance.li_gmobj_CurrentlyExistingBricks.Remove(this.transform.gameObject); // 중복된 이름의 오브젝트가 있어도 잘 제거 되려나?... 잘됩니다, 감사합니다, 주님!!!
        // Tentative.
        //GameManager.Instance.ScoreSystem_Check_CurrentlyExistingBricks();
        //GameManager.Instance.ScoreSystem_CheckAndApplyScore_ScaleMode_BasicHarmonies();

    }


    private void Check_WhoAmI_AndPlaySound()
    {

        string sMyName = this.name; // 나의 오브젝트 네임. 

        // e.g. 
        // instScaleBrick_C_F4#
        // 012345678901234567
        this.brickSpeaker.clip = ContentsManager.Instance.Check_WhoAmI_Retrieve_myAudioClip_CodeOrScale( sMyName.Substring(17) );

        // 공용화. 23.08.07
        this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material 
                            = ContentsManager.Instance.Check_WhoAmI_Retrieve_myMusicalNotation_Scale( sMyName.Substring(17) );

        brickSpeaker.Play();

        /*
//        if(Application.isEditor)

        switch( this.name )
        {
            case "instScaleBrick_C_C4": // C4 음. 
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[0];
                this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[0];
                break;
            case "instScaleBrick_C_D4":
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[2];
                this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[1];
                break;
            case "instScaleBrick_C_E4":
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[4];
                this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[2];
                break;
            case "instScaleBrick_C_F4":
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[5];
                this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[3];
                break;
            case "instScaleBrick_C_G4":
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[7];
                this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[4];
                break;
            case "instScaleBrick_C_A4":
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[9];
                this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[5];
                break;
            case "instScaleBrick_C_B4":
                this.brickSpeaker.clip = ContentsManager.Instance.aryAudioClips_Ckey_Scale[11];
                this.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = ContentsManager.Instance.matCkey_ScoreImage[6];
                break;
            default:
                this.brickSpeaker.clip = ContentsManager.Instance.AudioClip_Error;
                break;
        }

        brickSpeaker.Play();
        */
    }

}
