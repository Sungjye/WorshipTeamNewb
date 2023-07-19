//=====================================================================================
//
// 주님, 감사합니다, 차분히 할 수 있게 인도해 주십시요!
// 
// 현재 이 레벨의 플레이를 주관하는 스크립트.
// 해당 레벨: PickNumber : 브릭의 소리를 듣고 맞는 화음 '번호' 를 맞추는 레벨. 
//
// 하는일
// > 시작하면 랜덤하게 브릭 1개를 생성.
// > 사용자가 맞추면 이 브릭을 왼쪽으로 ‘쌩’ 이동하고
// > 사용자가 틀리면 계속 두기. 
// > 사용자가 맞추고 이 브릭이 왼쪽으로 이동하여 카메라 시야에서 사라지면, 새로운 브릭을 생성
// > (TBD) Score 와 Combo 를 표시해 주기. 
//
// 2023.07.14. sjjo. Initial.
//
//=====================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class CodeMode_Level_PickNumber_PlayManager : MonoBehaviour
{
    // 사운드 브릭이 될 수도 있고, 글자 브릭이 될 수도 있고 그림 브릭이 될 수도 있고..
    public GameObject gmobjQuizBrickPrefab; // 지금 이 레벨(PickNumber)의 경우는 사운드 브릭

    //public int nQuizTotalCount; // 있다 사용하겠지만..  어찌면, 전체를 주관하는 GameManager에 두어야 할지도?

    private GameObject gmobjCurrentBrick; // 현재 생성되어 있는 브릭

    // Start is called before the first frame update
    //void Start()
    // There is a self-checking of a instace object name 
    // in the prefab script Start function
    // to play its sound. 
    // Therefore it should be instantiated before the run of the prefab script's Start function.
    void Awake()
    {
        this.SpawnNewBrick();
    }

    private void SpawnNewBrick()
    {
        this.gmobjCurrentBrick = Instantiate( this.gmobjQuizBrickPrefab, new Vector3(0f, 6f, 0f), Quaternion.identity );

        //--------------------------------------------------
        // 오브젝트 자체의 이름을 정하기
        // 이걸, 몇도 이렇게 해야.. 다른 레벨에서의 생성 규칙도 일관성 있어지고.. 
        //And the code name is just a name in the system of harmonies (화성) 
        // 
        // Refer to. CodeMode_Level_0_Control.cs > OnMouseDown()

        // Tentative. Randomly chosen code number
        //eDO_NUMBER eQuizCodeNumber = eDO_NUMBER._2do; // Randomly or a certain pattern.
        eDO_NUMBER eQuizCodeNumber = GetOneBrickName_ineDO_NUMBER__Randomly();
        
        this.gmobjCurrentBrick.name = "instCodeBrick_" + ContentsManager.Instance.eSelectedKey.ToString()+ "_" + eQuizCodeNumber.ToString();

        //--------------------------------------------------
        // 오브젝트 자식중, 이미지 브릭의 머티리얼(이미지) 을 정하기.

        //--------------------------------------------------
        // Set the text of the child object for this instantiated object 
        // with searching the Dictionary according to the selected code-key. (e.g. C key)
        /*
        this.gmobjCurrentBrick.transform.GetChild(1).gameObject.GetComponent<TextMeshPro>().text 
                    = ContentsManager.Instance.dicCode_byKeyAndDoNum[ContentsManager.Instance.eSelectedKey][eQuizCodeNumber] ;
        */
        // Nope. use the method in the spawned object. 
        this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeA_Control>().SetMe_asQuestion();


    }

    private eDO_NUMBER GetOneBrickName_ineDO_NUMBER__Randomly()
    {
        // Ref. 
        // https://www.reddit.com/r/Unity3D/comments/ax1tqf/unity_tip_random_item_from_enum/
        // https://afsdzvcx123.tistory.com/entry/C-%EB%AC%B8%EB%B2%95-C-Enum-Count-%EA%B0%80%EC%A0%B8%EC%98%A4%EB%8A%94-%EB%B0%A9%EB%B2%95 


        //int nUpperBound = (int)(eDO_NUMBER._7do);
        int nUpperBound = System.Enum.GetValues(typeof(eDO_NUMBER)).Length;

        // random function for integer has the upper bound value which is not include itself.
        //nUpperBound++;

        eDO_NUMBER eRandomDo = (eDO_NUMBER)( Random.Range(0, nUpperBound) );


        return eRandomDo;
    }


    public void CheckIfInputIsCorrect(string sTappedKeyObjectName)
    {
        // This function is called from the each key tap of a user.
        //
        // Functions
        // > Compare the tapped key object name with the quiz brick name
        // > Set the quiz brick components according to the result.

        // 탭된 (버튼 역할인) 3D 오브젝트의 이름 자체가, eDO_NUMBER 타입의 이름. 
        // 그래서 변환해서 바로 인덱싱 하면 됨.
        string sTappedCodeName_inTermsOfTheSelectedKey = ContentsManager.Instance.dicCode_byKeyAndDoNum[ContentsManager.Instance.eSelectedKey][(eDO_NUMBER)System.Enum.Parse(typeof(eDO_NUMBER), sTappedKeyObjectName)];



        //if( ContentsManager.Instance.sCodeMode_Level_PickNumber_QuizBrickName 
        if( this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeA_Control>().sMyDictionariedCodeName
                == sTappedCodeName_inTermsOfTheSelectedKey )
        {
            Debug.Log("Correct!");

            // 여기서 이 맞는 브릭은 사라지게 함. 
            this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeA_Control>().SetMe_asCorrect(); 

            // 맞으면 다음 브릭 생성!
            //this.SpawnNewBrick();
            Invoke("SpawnNewBrick", 0.7f);

        }else
        {
            Debug.Log("Wrong... Brick: " + this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeA_Control>().sMyDictionariedCodeName // + ContentsManager.Instance.sCodeMode_Level_PickNumber_QuizBrickName 
                            + " Tapped: " + sTappedCodeName_inTermsOfTheSelectedKey);

            this.gmobjCurrentBrick.GetComponent<Quiz_SoundBrick_typeA_Control>().SetMe_asWrong();

        }

    }


}
