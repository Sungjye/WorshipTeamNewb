//=====================================================================================
//
// 주님, 이렇게 스케일 모드도 퀴즈 레벨을 시작할 수 있게 해 주셔서 감사합니다!!!
//
// 현재 이 레벨의 플레이를 주관하는 스크립트.
// 해당 레벨: PickNote : 건반의 음 브릭의 소리를 듣고 맞는 그 음의 건반을 '구체적으로'  탭해서 맞추는 레벨. 
//                    (구체적 이란, C 가 아니라, C4)
//
// 하는일
// > 시작하면 랜덤하게 브릭 1개를 생성.
// > 사용자가 맞추면 이 브릭을 왼쪽으로 ‘쌩’ 이동하고
// > 사용자가 틀리면 계속 두기. 
// > 사용자가 맞추고 이 브릭이 왼쪽으로 이동하여 카메라 시야에서 사라지면, 새로운 브릭을 생성
// > (TBD) Score 와 Combo 를 표시해 주기. 
//
// 2023.07.18. sjjo. Initial.
// 
//=====================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class ScaleMode_Level_PickNote_PlayManager : MonoBehaviour
{
    // 사운드 브릭이 될 수도 있고, 글자 브릭이 될 수도 있고 그림 브릭이 될 수도 있고..
    public GameObject gmobjQuizBrickPrefab; // 지금 이 레벨(PickNote)의 경우는 사운드 브릭

    //public int nQuizTotalCount; // 있다 사용하겠지만..  어찌면, 전체를 주관하는 GameManager에 두어야 할지도?

    private GameObject gmobjCurrentBrick; // 현재 생성되어 있는 브릭

    void Awake()
    {
        this.SpawnNewBrick();
    }

    private void SpawnNewBrick()
    {
        this.gmobjCurrentBrick = Instantiate( this.gmobjQuizBrickPrefab, new Vector3(0f, 6f, 0f), Quaternion.identity );

        //--------------------------------------------------
        // 오브젝트 자체의 이름을 정하기
        // 이걸, 구제적인 노트번호 D4b 이렇게 해야.. 다른 레벨에서의 생성 규칙도 일관성 있어지고.. 
        //And the note name should be a absolute name among the piano keyboard. 
        // 
        // Refer to. CodeMode_Level_0_Control.cs > OnMouseDown()

        // Tentative. Randomly chosen note among the available keys in the piano keyboard which are defined in GameManager.
        //ePIANOKEYS eQuizScaleNote = ePIANOKEYS.Db; // Randomly or a certain pattern.
        // ePIANOKEYS eQuizScaleNote = GetOneBrickName_inePIANOKEYS__Randomly();
        string sQuizScaleNote = GetOneBrickName_inePIANOKEYS__Randomly();

        //-------------------------------------------
        // 인스턴시에잇된 오브젝트 자체의 이름 정하기:
        // 인스턴시에잇된 (하늘에서 떨어지는) 스케일 브릭 + 현재선택된 키, 사용자가 누른 어떤 키인지를 나타내는 값.        
        //this.gmobjCurrentBrick.name = "instScaleBrick_" + GameManager.Instance.eSelectedKey.ToString()+ "_" + eQuizScaleNote.ToString();
        // 스케일 모드는, 악보를 표시할 때 빼고는 그냥 이름 inst 이런거 붙이지 말고 이름그대로 사용하자! 
        // 스케일을 섞어서, 브릭을 생성해 낼 일도 없을듯.. 
        // 나중에 악보 표시나, 소리 매칭할 때, 컨트롤 스크립트에서 현재 선택된 key (조) 를 확인해서 서브루틴 돌리기로.. 
        //this.gmobjCurrentBrick.name = "instScaleBrick_" + GameManager.Instance.eSelectedKey.ToString()+ "_" + eQuizScaleNote.ToString();
        this.gmobjCurrentBrick.name = sQuizScaleNote;

        //--------------------------------------------------
        // 오브젝트 자식중, 이미지 브릭의 머티리얼(이미지) 을 정하기.

        //--------------------------------------------------
        // Set the text of the child object for this instantiated object 
        // with searching the Dictionary according to the selected code-key. (e.g. C key)
        /*
        this.gmobjCurrentBrick.transform.GetChild(1).gameObject.GetComponent<TextMeshPro>().text 
                    = GameManager.Instance.dicCode_byKeyAndDoNum[GameManager.Instance.eSelectedKey][eQuizCodeNumber] ;
        */
        // Nope. use the method in the spawned object. 
        // Type B sound birck is for a scale mode. 23.07.18
        this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeB_Control>().SetMe_asQuestion();


    }

    private string GetOneBrickName_inePIANOKEYS__Randomly()
    {
        // Ref. 
        // https://www.reddit.com/r/Unity3D/comments/ax1tqf/unity_tip_random_item_from_enum/
        // https://afsdzvcx123.tistory.com/entry/C-%EB%AC%B8%EB%B2%95-C-Enum-Count-%EA%B0%80%EC%A0%B8%EC%98%A4%EB%8A%94-%EB%B0%A9%EB%B2%95 

        string sRandomNote;

        switch( GameManager.Instance.eSelectedKey )
        {
            case eAVAILABLEKEYS.C:
                //e_C_SCALENOTES eRandomNote = (e_C_SCALENOTES)( Random.Range(0, System.Enum.GetValues(typeof(e_C_SCALENOTES)).Length) );
                //string sRandomNote = (e_C_SCALENOTES)( Random.Range(0, System.Enum.GetValues(typeof(e_C_SCALENOTES)).Length) ).ToString();
                sRandomNote  = ((e_C_SCALENOTES)( Random.Range(0, System.Enum.GetValues(typeof(e_C_SCALENOTES)).Length) )).ToString();
                break;
            case eAVAILABLEKEYS.G:
                sRandomNote = "F4#"; // tentative.
                break;
            default:
                sRandomNote = "Err:Rnd"; // 랜덤 브릭 생성 오류.
                break;
        }

        // random function for integer has the upper bound value which is not include itself.
        //nUpperBound++;



        return sRandomNote;
    }


    public void CheckIfInputIsCorrect(string sTappedKeyObjectName)
    {
        // This function is called from the each key tap of a user.
        //
        // Functions
        // > Compare the tapped key object name with the quiz brick name
        // > Set the quiz brick components according to the result.

        // 탭된 (버튼 역할인) 3D 오브젝트의 이름 자체가, ePIANOKEYS 타입의 이름. 
        // 그래서 변환해서 바로 인덱싱 하면 됨.. 이라고 생각할 수 있지만, 

        // 피아노 키 값이 나중에 뭐가될지 C# or Db 뭐가 될지 모르므로.. 있는지 확인해야 함. 
        // ref. https://learn.microsoft.com/ko-kr/dotnet/api/system.enum.isdefined?view=net-7.0 
        //---------------------------------------------
        // 지금 탭된 피아노 키가 무엇인지 확인부터해서.. 
        // 어떤 것을 인스턴시에잇 할지 결정!
        string sParsedPinanoKey = GameManager.Instance.ParsingTheTappedPianoKey(sTappedKeyObjectName); // e.g. C4는 C로, D4b 은 Db로 변환해 준다. 
        
        //string sTappedNoteName_inTermsOfTheSelectedKey;


        if( System.Enum.IsDefined( typeof(ePIANOKEYS),  sParsedPinanoKey ) ) 
        {
            // 값이 enum 정의에 존재하면 
            // 일단, 체크를 해볼 수 있는 정상적인 (피아노) 키패드 입력값이라는 뜻!
            // 정상적이지 않은 것은? 예를 들어, D4b 대신 C4# 뭐 이렇게 피아노 건반 이름을 개발자가 실수로 잘못 만들고 사용자가 탭한 경우. 

            // 자, 정상적인 키 입력이었다면, 넘어온 오브젝트의 이름이 C4 나 D4b 이런식일 테므로, 
            // 1. 이 탭된 오브젝트의 값과, 방금 이 스크립트가 인스턴시에잇 한 오브젝트의 이름이 같은지 확인. 
            // 2. 같다면 맞다고 처리.
            // 3. 다르다면, 틀렸다고 처리. 

            //sTappedNoteName_inTermsOfTheSelectedKey = GameManager.Instance.dicScale_byKeyAndPianoKeys[GameManager.Instance.eSelectedKey][(ePIANOKEYS)System.Enum.Parse(typeof(ePIANOKEYS), sTappedKeyObjectName)];

            // 스케일 모드는, 악보를 표시할 때 빼고는 그냥 이름 inst 이런거 붙이지 말고 이름그대로 사용하자! 

            if( this.gmobjCurrentBrick.name
                    == sTappedKeyObjectName )
            {
                if(Application.isEditor) Debug.Log("Correct!");

                // 여기서 이 맞는 브릭은 사라지게 함. 
                this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeB_Control>().SetMe_asCorrect(); 

                // 맞으면 다음 브릭 생성!
                //this.SpawnNewBrick();
                Invoke("SpawnNewBrick", 0.7f);

            }else
            {
                // if(Application.isEditor) Debug.Log("Wrong..");
                //Debug.Log("Wrong... Brick: " + this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeB_Control>().sMyDictionariedCodeName // + GameManager.Instance.sCodeMode_Level_PickNumber_QuizBrickName 
                //                + " Tapped: " + sTappedNoteName_inTermsOfTheSelectedKey);
                if(Application.isEditor)
                {
                    Debug.Log("Wrong... Brick: " + this.gmobjCurrentBrick.name
                                + " Tapped: " + sTappedKeyObjectName);
                }

                this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeB_Control>().SetMe_asWrong();

            }


        }else
        {
            // 정상적이지 않은 것은? 예를 들어, D4b 이 아니고, C4# 뭐 이렇게 피아노 건반 이름을 개발자가 실수로 잘못 만들고 사용자가 탭한 경우. 

            if(Application.isEditor)
            {
                Debug.Log("Error! The Tapped key does not exist in the piano key enumerete type!!!");
            }
        }



    }

}
