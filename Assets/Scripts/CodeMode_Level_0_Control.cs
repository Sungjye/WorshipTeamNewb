//====================================================================================
// 주님. 감사합니다. 지금은 마음이 조금 서두르지만.. 이것을 해 나가며.. 
// 마음 다른데 돌리지 않고, 이 과정에 함께 해 주셔서, 이 앱이, 
// 주님을 마음껏 찬양 드리기 위해 도울 수 있는 선한 도구가 되게 해 주십시요.
// 이것으로 일용할 양식도, 주님께서 허락하신다면 베풀어 주십시요.
// 세상적으로 보면 낮은 자리이지만, 주님께서 저의 과거의 죄를 모두 용서하여 주시고, 
// 새로운 사람으로, 주님의 말씀안에서 새로운 삶으로 살아가서
// 주 예수님께서 허락하시는 많은 열매를 맺는 삶을 살 수 있게 해 주십시요!
// 저의 주님 되어 주시는 예수 그리스도의 이름으로 기도드렸습니다, 아멘!
// 
//
// 코드 연습을 하는 첫 레벨에서, 각 버튼 브릭에 붙는 스크립트. 
//
// 2023.07.06. sjjo. Initial. @정약용 도서관
// 
// References
// Unity Drag and Drop 3D - Easy Tutorial (2023) https://www.youtube.com/watch?v=pFpK4-EqHXQ 
//
//
//====================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using System;

public class CodeMode_Level_0_Control : MonoBehaviour
{

    public GameObject gmobjCodeBrickPrefab;

    //Vector3 mousePosition;

    // 선택시 효과 관련. 
    private Vector3 vOrigianlSize;
    private Coroutine crPopEffect;

/* 이건 뭐 점수줄 상황이 없음.  23.08.15. 
    void Awake()
    {
        // 딱 한번만 하게.. 탭 브릭에 다 붙는 스크립트니.. 가 안되는 구나. 
        // 꼼수로.. 
        if( this.name == "_1do" ) GameManager.Instance.gmobjScorePanel = GameObject.Find("Panel_ScoreDisplay"); 

    }
*/

    // Start is called before the first frame update
    void Start()
    {

        vOrigianlSize = this.transform.localScale; 
        crPopEffect = null;

        if(Application.isEditor) Debug.Log("Code input object: " + this.name + ": " + vOrigianlSize );

    }


    void PopEffect_inTermsOf_Size()
    {
        // 자신이 선택되면 통 (크기가) 튀는 효과. (나중엔 위로 점프하는 효과? ^^; 아 쓸데없다 ㅎ)

        // 혹시, 기존에 돌던게 있었다면 정지해 주고, 
        if( crPopEffect != null ) StopCoroutine(crPopEffect); 

        // 효과 코루틴 시작.
        //crPopEffect = StartCoroutine( MakeMe_Pop_TypeA(0.1f, 0.02f) );
        crPopEffect = StartCoroutine( MakeMe_Pop_TypeA(0.15f, 0.01f) );
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
/*
    private Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }
*/



    private void OnMouseDown()
    {
        //mousePosition = Input.mousePosition - GetMousePos();
        if(Application.isEditor) Debug.Log("Mouse Down: " + this.name);

        PopEffect_inTermsOf_Size();


        GameObject instCodeBrick = Instantiate(gmobjCodeBrickPrefab, new Vector3(0f, 6f, 0f), Quaternion.identity);

        // 인스턴시에잇된 오브젝트 자체의 이름 정하기:
        // 인스턴시에잇된 (하늘에서 떨어지는) 코드 브릭 + 현재선택된 키, 사용자가 누른 몇번 화음인지를 나타내는 값.
        instCodeBrick.name = "instCodeBrick_" + GameManager.Instance.eSelectedKey.ToString()+ "_" + this.name;

        // F#m 이런 코드의 enum 타입은 Fsharpm 이다. 
        // 딕셔너리로 찾은, Fsharpm 과 같은 스트링을, F#m 이렇게 바꾸어 주어야, 피아노 건반 탭한 것과 비교 및 화면 표시등에 사용할 수 있다. 
        // ( F#m 인데. Fsharpm 이렇게 표시되면 어색함..)
        string sCodeName = ContentsManager.Instance.dicCode_byKeyAndDoNum[GameManager.Instance.eSelectedKey][(eDO_NUMBER)System.Enum.Parse(typeof(eDO_NUMBER), this.name)];

        sCodeName = ContentsManager.Instance.CheckAndReplace_sharpString_with_sharpMark( sCodeName );


        // 인스턴시에잇된 오브젝트 자식으로 붙어 있는 TMP의 텍스트 내용을 정하기:
        // Ref. https://mentum.tistory.com/333 , https://chashtag.tistory.com/50 
        // 탭된 (버튼 역할인) 3D 오브젝트의 이름 자체가, eDO_NUMBER 타입의 이름. 그래서 변환해서 바로 인덱싱 하면 됨. 
        instCodeBrick.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text 
                    = sCodeName;
                    //= ContentsManager.Instance.dicCode_byKeyAndDoNum[GameManager.Instance.eSelectedKey][(eDO_NUMBER)System.Enum.Parse(typeof(eDO_NUMBER), this.name)];
                    //= ContentsManager.Instance.dicCode_byKeyAndDoNum[eKEYCODES.C][eDO_NUMBER._1do];




    }


}
