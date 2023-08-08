//====================================================================================
//
// 주님, 맑은 머리를 주셔서 감사합니다!
// 
// 키 알아 맞추기, 스케일(단음) 모드를 하는 레벨에서,
//      위에서 내려온 사운드 브릭을 (탭해서) 듣고 맞추는데 사용되는
//      하단의 "건반" 버튼 브릭 각각에 붙는 스크립트. 
// 
// 이, 스트립트가 다른 레벨의 건반(또는 코드 브릭) 의 제어 스크립트와 공통의 스크립트로 사용할 수 없는 이유는, 
// 키 입력 체크하는 함수가 각자의 상황에 맞는 player script 내의 함수를 호출해야 하기 때문. 
//
// 2023.08.04. sjjo. Copied from ScaleMode_Level_PickPatNotes_Control
//====================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class ScaleMode_Level_RecogKeys_Control : MonoBehaviour
{

    // 나 (지금 이 스크립트)는 (스케일 모드의) 피아노 건반 키패드 브릭에 붙어 있는 스크립트 인데, 
    // 이 퀴즈 레벨의 전체를 주관하는 플레이어 매니저의 스크립트에게, 
    // 내가 탭 된것을 알리기 위해서 가지고 오는 오브젝트. 
    public GameObject gmobjPlayManager; 

    // 선택시 효과 관련. 
    private Vector3 vOrigianlSize;
    private Coroutine crPopEffect;

    // Start is called before the first frame update
    void Start()
    {
        vOrigianlSize = this.transform.localScale; 
        crPopEffect = null;

        // 내이름, 선택된 키에 따라, D4b 스타일로 할것인가, C4#으로 할것인가.. 
        // 이건 그냥 Db 스타일 표기 그대로. this.name = ContentsManager.Instance.ScaleMode_UserTapKeysName_CheckAnd_PresentAlternativeKeyName_accordingToTheSelectedKey(this.name);

        if(Application.isEditor) Debug.Log("User tapped object: " + this.name + ": " + vOrigianlSize );

        // 이 블럭의 텍스트 표기를 자신의 오브젝트 이름으로. (피아노 키 값)
        this.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = this.name;

    }

    void PopEffect_inTermsOf_Size()
    {
        // 자신이 선택되면 통 (크기가) 튀는 효과. (나중엔 위로 점프하는 효과? ^^; 아 쓸데없다 ㅎ)

        // 혹시, 기존에 돌던게 있었다면 정지해 주고, 
        if( crPopEffect != null ) StopCoroutine(crPopEffect); 

        // 효과 코루틴 시작.
        //crPopEffect = StartCoroutine( MakeMe_Pop_TypeA(0.1f, 0.02f) );
        crPopEffect = StartCoroutine( MakeMe_Pop_TypeA(0.15f, 0.04f) ); // 25fps
        //crPopEffect = StartCoroutine( MakeMe_Pop_TypeA_General(this, 0.1f, 0.02f) ); 안되네. 다른 방법으로.. 

    }

    IEnumerator MakeMe_Pop_TypeA(float fMaxIncSize, float fInterval) // 0.3f, 0.03f
    {
        // 자신(메인 번들 리스트 아이템 하나)을 살짝 크기를 키웠다가 원래 사이즈로 만드는 코루틴.

        // 다음 프레임까지 깔끔하게 기다렸다가!
        yield return null;

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
        //yield return new WaitForSeconds(0.3f);

         Vector3 vChangedSize = this.transform.localScale;

        //----------------------
         // 작아지는 단계 
         // fSizeSpan = 0.03f; // 작아지는 속도는 좀 빠르게?

        //for(float fSizeInc = fMaxIncSize; fSizeInc > 0f; fSizeInc -= fSizeSpan)
        for(float fSizeInc = 0f; fSizeInc < fMaxIncSize; fSizeInc += fSizeSpan)
        //for(float fSizeInc = 0f; fSizeInc < fMaxIncSize; fSizeInc += fSizeSpan)// 살짝 작아졌다가?
        {
            Vector3 vNewSize = new Vector3(fSizeInc, fSizeInc, fSizeInc);

            //this.transform.localScale = vOrigianlSize - vNewSize;
            this.transform.localScale = vChangedSize - vNewSize;

            yield return new WaitForSeconds(fInterval);
        }

        //----------------------
        // 안전하게, 원래 사이즈로. 
         this.transform.localScale = vOrigianlSize;

    }

    private void OnMouseDown()
    {
        //mousePosition = Input.mousePosition - GetMousePos();
        if(Application.isEditor) Debug.Log("Mouse Down: " + this.name);

        PopEffect_inTermsOf_Size();

       // sCodeMode_Tapped_Keypad_inTermsOfTheSelectedKey

        this.gmobjPlayManager.GetComponent<ScaleMode_Level_RecogKeys_PlayManager>().CheckIfInputIsCorrect(this.name);

        
/*
        if( ContentsManager.Instance.sCodeMode_Level_PickNumber_QuizBrickName 
                == sMyName_inTermsOfTheSelectedKey )
        {
            Debug.Log("Correct!");
        }else
        {
            Debug.Log("Wrong... Brick: " + ContentsManager.Instance.sCodeMode_Level_PickNumber_QuizBrickName 
                            + " Tapped: " + sMyName_inTermsOfTheSelectedKey);
        }
*/



/*
        GameObject instCodeBrick = Instantiate(gmobjCodeBrickPrefab, new Vector3(0f, 6f, 0f), Quaternion.identity);

        // 인스턴시에잇된 오브젝트 자체의 이름 정하기:
        // 인스턴시에잇된 (하늘에서 떨어지는) 코드 브릭 + 현재선택된 키, 사용자가 누른 몇번 화음인지를 나타내는 값.
        instCodeBrick.name = "instCodeBrick_" + GameManager.Instance.eSelectedKey.ToString()+ "_" + this.name;

        // 인스턴시에잇된 오브젝트 자식으로 붙어 있는 TMP의 텍스트 내용을 정하기:
        // Ref. https://mentum.tistory.com/333 , https://chashtag.tistory.com/50 
        // 탭된 (버튼 역할인) 3D 오브젝트의 이름 자체가, eDO_NUMBER 타입의 이름. 그래서 변환해서 바로 인덱싱 하면 됨. 
        instCodeBrick.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text 
                    = ContentsManager.Instance.dicCode_byKeyAndDoNum[GameManager.Instance.eSelectedKey][(eDO_NUMBER)System.Enum.Parse(typeof(eDO_NUMBER), this.name)];
                    //= ContentsManager.Instance.dicCode_byKeyAndDoNum[eKEYCODES.C][eDO_NUMBER._1do];
*/


    }

}
