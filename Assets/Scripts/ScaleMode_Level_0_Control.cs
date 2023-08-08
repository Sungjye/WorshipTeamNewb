//====================================================================================
// 
// 주님, 그냥 진행했다면 맞추기 레벨을 진행했을 것 같은데, 
// 기도 드렸을 떄 주시는 마음에 따라, 음 맞추기 0단계를 먼저 해 봅니다. 
// 주님, 무엇을 하든 어떤 상황이든 주 예수님만 의지하며 모든 시간을 살게하여 주십시요! 
// 주님의 사랑안에 거하며 주신 계명에 순종해서 열매 맺게 하여 주십시요!
// 예수 그리스도의 이름으로 기도드렸습니다, 아멘!
// 
// 음 연습을 하는 첫 레벨에서, 각 버튼 (피아노 건반) 브릭에 붙는 스크립트.
//
// 2023.07.11. sjjo. Initial. @정도
//
//
 //====================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class ScaleMode_Level_0_Control : MonoBehaviour
{
    public GameObject gmobjScorePanel; // 점수 표시 패널. 

    public GameObject gmobjScaleBrickPrefab;
    public GameObject gmobjFruitBrickPrefab;
    public GameObject gmobjErrorBrickPrefab;

    //Vector3 mousePosition;

    // 선택시 효과 관련. 
    private Vector3 vOrigianlSize;
    private Coroutine crPopEffect;



    // Start is called before the first frame update
    void Start()
    {

        vOrigianlSize = this.transform.localScale; 
        crPopEffect = null;

        if(Application.isEditor) Debug.Log("Scale input object: " + this.name + ": " + vOrigianlSize );

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

        GameObject instCodeBrick;

        //-------------------------------------------
        // 인스턴시에잇된 오브젝트 자식으로 붙어 있는 TMP의 텍스트 내용을 정하기:
        // Ref. https://mentum.tistory.com/333 , https://chashtag.tistory.com/50 
        // 탭된 (버튼 역할인) 3D 오브젝트의 이름은, 건반의 고유 번호. e.g. C4
        
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        // 첫번쨰 자식은, 납작한 큐브에 올라가 있는 악보 이미지.

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        // 두번쨰 자식은 TMP 텍스트. 

        //---------------------------------------------
        // 지금 탭된 피아노 키가 무엇인지 확인부터해서.. 
        // 어떤 것을 인스턴시에잇 할지 결정!
        string sParsedPinanoKey = ContentsManager.Instance.ParsingTheTappedPianoKey(this.name); // e.g. C4는 C로, D4b 은 Db로 변환해 준다. 
        // 피아노 키 값이 나중에 뭐가될지 C# or Db 뭐가 될지 모르므로.. 있는지 확인해야 함. 
        // ref. https://learn.microsoft.com/ko-kr/dotnet/api/system.enum.isdefined?view=net-7.0 
        if( System.Enum.IsDefined( typeof(ePIANOKEYS),  sParsedPinanoKey ) ) 
        {
            // 값이 enum 정의에 존재하면 
            //      지금 스케일에 해당하는 음인지 확인 해서 
            //          해당하면 해당 브릭을 인스턴시에잇.
            //          해당하지 않으면 간식을 인스턴시에잇. 
            
            ePIANOKEYS eTappedPianoKey = (ePIANOKEYS)System.Enum.Parse(typeof(ePIANOKEYS), sParsedPinanoKey);
        
            int nCorrespondentKeyValue = ContentsManager.Instance.dicScale_byKeyAndPianoKeys[GameManager.Instance.eSelectedKey][eTappedPianoKey];

            //-------------------------------------------
            // 스코어 증감 조건인지 체크: 선텍된 스케일의, 화음인지! 23.08.03
            //
            //GameManager.Instance.ScoreSystem_PleaseUpdateTheScore( this.gmobjScorePanel, eSCORING_CASEID.SM_ITR_1 );

            // 해당하는 키 스케일에 포함되는 음이면 1~7 일것이고, 아니면 0 일것임. 1은 해당 키 스케일의 1번음. 
            if( (nCorrespondentKeyValue >= 1) && (nCorrespondentKeyValue <= 7) )
            {  
                // 정상적인 스케일 음 오브젝트를 인스턴시에잇.        
                instCodeBrick = Instantiate(gmobjScaleBrickPrefab, new Vector3(0f, 6f, 0f), Quaternion.identity);

                //-------------------------------------------
                // 인스턴시에잇된 오브젝트 자체의 이름 정하기:
                // 인스턴시에잇된 (하늘에서 떨어지는) 스케일 브릭 + 현재선택된 키, 사용자가 누른 어떤 키인지를 나타내는 값.
                instCodeBrick.name = "instScaleBrick_" + GameManager.Instance.eSelectedKey.ToString()+ "_" + this.name;
        
                instCodeBrick.transform.GetChild(1).gameObject.GetComponent<TextMeshPro>().text = this.name; // 건반 이름을 그대로. 

                //-------------------------------------------
                // 임시로, 스코어 반영되는지 해보기. 
                //GameManager.Instance.ScoreSystem_PleaseUpdateTheScore( this.gmobjScorePanel, eSCORING_CASEID.SM_ITR_0 );



            }else
            {
                // 이 키 스케일에 해당하는 음이 아니면,
                // 간식을 인스턴 시에잇. 
                instCodeBrick = Instantiate(gmobjFruitBrickPrefab, new Vector3(0f, 6f, 0f), Quaternion.identity);

                //-------------------------------------------
                // 해당 스케일이 아니더라도, 인스턴시에잇된 오브젝트 자체의 이름을 정해야, 화음 확인에 대해서 일반적인 코드가 된다. 
                // 인스턴시에잇된 (하늘에서 떨어지는) 스케일 브릭 + 현재선택된 키, 사용자가 누른 어떤 키인지를 나타내는 값.
                instCodeBrick.name = "instScaleBrick_" + GameManager.Instance.eSelectedKey.ToString()+ "_" + this.name; // 디스.네임은, 건반의 고유이름.

                instCodeBrick.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = this.name;

            }
            
            //===========================================================
            // 정상적인 브릭이 인스턴시에잇 된 경우에만, 스코어시스템 함수 호출. 
            // 그런데, 이 (화음체크 등의 경우) 바로 생성된 직후에 하면 좀 이상하니까,
            // 그리고, 인스턴스 브릭의 스크립트에서, 싱글톤 리스트에 데이터 추가할 시간도 기다릴 겸...
            // 0.#초 기다렸다가 호출 및 체크. 
            // 끙.. 브릭도 자기 타이머로 사라져서.. 시점 차이로, 화음 체크 안되는 경우 많음. 다시 0초..
            Invoke("CheckIfScored", 0f);
            


        }else
        {
            // 값이 enum 정의에 없으면 에러 브릭을 인스턴시에잇.
            // 

            instCodeBrick = Instantiate(gmobjErrorBrickPrefab, new Vector3(0f, 6f, 0f), Quaternion.identity);

            instCodeBrick.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = "Err:" + this.name;
        }


        //instCodeBrick.transform.GetChild(1).gameObject.GetComponent<TextMeshPro>().text 
        //            = ContentsManager.Instance.dicScale_byKeyAndPianoKeys[GameManager.Instance.eSelectedKey][(ePIANOKEYS)System.Enum.Parse(typeof(ePIANOKEYS), sParsedPinanoKey)];




    }

    private void CheckIfScored()
    {

        GameManager.Instance.ScoreSystem_ScaleMode_Intro_CheckAndApplyScore__BasicHarmonies( this.gmobjScorePanel );

    }

}
