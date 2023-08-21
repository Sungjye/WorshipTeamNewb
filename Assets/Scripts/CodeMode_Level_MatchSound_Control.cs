//====================================================================================
// 
// 주님, 찬찬히 잘 짤 수 있게 도와 주십시요!
//
// 코드 텍스트가 퀴즈로 나오는 모드. 이 레벨에서 사용자가 탭하는 물음표 비밀 키패드 브릭 각각에 붙는 스크립트.
//
// : 이번 레벨에서 이 control 스크립트는 다른 레벨의 (사용자 탭하는 키패드 브릭에 붙는) control 스크립트와는 
// : 상당히 다른 역할을 하는데.. : 텍스트 히든 사운드 브릭
// 
// 하는일
//  1 단계
//   > V 이 레벨의 플레이어 스크립트를 통해, 자신의 히든 (뮤지컬) 코드가 무엇인지 받아온다. 
//   > V 그것으로 자신의 오브젝트 이름을 정한다. 표시되는 이름은 ? 표로 한다. 
//   > V 탭 되면, 컨텐츠매니저 퍼블릭 함수로, 자신의 이름에 해당하는 음을 플레이 한다. 
//  2 단계
//   > drag 를 시작하면, 손 위치에 따라 움직인다. 
//   > drop 을 퀴즈 텍스트 브릭위치에 놓고 맞으면, 플레이어 매니져에 맞다고 알리고, 자신의 원래 위치로 순간이동한다. (퀴즈브릭은 사라지고)
//   > drop 이 그외의 경우라면, 이동경로가 보이게, 자신의 원래 위치로 이동한다. 
// 
// 2023.07.25. sjjo. Initial.
// 2023.07.26. sjjo. 이게.. 애드포스가 아니라, 위치이동을 바로 하면서 온_콜리전_엔터 를 체크하니.. 안되는 경우가 있다... 아예 안부딧히게 하고, 처음으로 레이캐스트 사용해 봅니다. 
// 
//====================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class CodeMode_Level_MatchSound_Control : MonoBehaviour
{
    public GameObject gmobjPlayManager;


    private AudioSource brickSpeaker;

    
    private Vector3 vOrigianlPosition; 
    private Vector3 mousePosition; // This variable is used for a drag and drop fuction. 

    //private RaycastHit hitObject; // I use Raycast for the first time.
    private RaycastHit[] hitObject; // I use Raycast for the first time.

    private bool bCorrectAnswer_whileDragging; // 이게.. (맞췄을 경우) 새로운 퀴즈 브릭 스파운 시점이, 레이캐스트가 달린 이 키패드 브릭을 drag&drop 에서 사용자가 놓고, 원위치 되었을 때 스파운해야 해서..

    void Awake()
    {
        this.vOrigianlPosition = this.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        // int.Parse(this.name.Substring(1,1))
        string sMyNewName = this.gmobjPlayManager.GetComponent<CodeMode_Level_MatchSound_PlayManager>().sCode_doList_scrambled[ (int.Parse(this.name.Substring(1,1)) -1) ]; // 이름은 1부터, 인덱스는 0부터라 1을 빼줌. 

        this.name = sMyNewName;

        // Audio related settings.
        this.brickSpeaker = this.gameObject.AddComponent<AudioSource>();        
        this.brickSpeaker.loop = false;        


        // tentative. To check if it works.
        //this.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = sMyNewName;

        // My code name should be hidden to a player.
        this.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = "?";

        this.bCorrectAnswer_whileDragging = false;

    }


    // 음.. 레이케스트 사용.. 
    // D&D 를 할 때 키패드 브릭이 퀴즈 브릭을 밀기도 하고.. 그래서.. 충돌감지도 잘 안되고.. 
    void FixedUpdate()
    {
        if(Application.isEditor) 
        {
            Debug.DrawRay( this.transform.position, this.transform.forward * 10f, Color.red);
        }

        #if FALSE
        // 이렇게 하나만 하면, 퀴즈 브릭의 자식들이 먼저 걸려서 정작 _#do 는 인식 안되는 경우가 있음. 
        // 어쩔수 없이 레이캐스트올! 로.. 
        if( Physics.Raycast(this.transform.position, this.transform.forward, out hitObject) )        
        {
            if(Application.isEditor)
            {
                Debug.Log(hitObject.collider.name);
            }


            if( this.name == hitObject.collider.name ) // e.g. _3do == _3do
            {
                this.gmobjPlayManager.GetComponent<CodeMode_Level_MatchSound_PlayManager>().TheInputIsCorrect();

                this.bCorrectAnswer_whileDragging = true;

            }else
            {
                this.gmobjPlayManager.GetComponent<CodeMode_Level_MatchSound_PlayManager>().TheInputIsWrong();

                this.bCorrectAnswer_whileDragging = false;
            }

        }
        #endif
        //if( Physics.Raycast(this.transform.position, this.transform.forward, out hitObject) )        
        this.hitObject = Physics.RaycastAll( this.transform.position, transform.forward );

        if( this.hitObject.Length > 0)
        {
            // 뭔가 부딪힌게 있으면.

            for(int i=0; i<this.hitObject.Length; i++)
            {
                // 같은 이름인게 있는지 확인. 
                if( this.name == hitObject[i].collider.name ) // e.g. _3do == _3do
                {
                    this.gmobjPlayManager.GetComponent<CodeMode_Level_MatchSound_PlayManager>().TheInputIsCorrect();

                    this.bCorrectAnswer_whileDragging = true;

                    break; // 확인했으므로 루프 더 돌 필요 없음. 
                }
            }

            // 여기까지 오면, 틀린 브릭을 드래그했다는 뜻. 음.. 다른 오브젝트도 부딪히잖아.. 

        }

    }

    #region Private methods

    private void ReAssignMyName_afterReScrambleFromThePlayManager()
    {
        // 이게 위치가 매번 같으면 음 익히는데.. 방해가 되서..?
        // 일단 주시는 마음에 따라 보류. 23.07.26
        
    }

    #endregion


    #region MonoBehavior inherited methods
    private void OnMouseDown()
    {
        //mousePosition = Input.mousePosition - GetMousePos();
        if(Application.isEditor) Debug.Log("Mouse Down: " + this.name);

        //PopEffect_inTermsOf_Size();

       // sCodeMode_Tapped_Keypad_inTermsOfTheSelectedKey

        // this.gmobjPlayManager.GetComponent<CodeMode_Level_PickPatNumber_PlayManager>().CheckIfInputIsCorrect(this.name);

        this.brickSpeaker.clip = ContentsManager.Instance.Check_WhoAmI_Retrieve_myAudioClip_CodeOrScale(this.name);


        this.brickSpeaker.Play();

        this.SetCurrent_UserTappedPosition_forDragAndDrop();

    }

    private void OnMouseDrag()
    {
        this.transform.position = Camera.main.ScreenToWorldPoint( Input.mousePosition - this.mousePosition );
    }

    private void OnMouseUp()
    {
        // tentative. Place me in the initial postion

        this.transform.position = this.vOrigianlPosition;


        if( this.bCorrectAnswer_whileDragging == true)
        {
            this.gmobjPlayManager.GetComponent<CodeMode_Level_MatchSound_PlayManager>().GiveMeNewQuizBrick();
            this.bCorrectAnswer_whileDragging = false;
        }
        
        // 이거하면, 소리들으려고 탭만 해도 wrong 뜨고 감점..
        /*
        else 
        {
            // 여기가 틀린 브릭을 드래그 했다가 놓은 경우. 
            this.gmobjPlayManager.GetComponent<CodeMode_Level_MatchSound_PlayManager>().TheInputIsWrong();
        }
        */

    }


    /* 레이 캐스트 사용으로 대체.
    private void OnCollisionEnter(Collision collision)
    {
        // 내가 (움직이며) 부딪힌 물체를 확인하고, 그것이 정답브릭인지 아닌지를 플레이어 매니저에게 알리기. 
        this.CheckAndHandle_theObject_I_Hit(collision); 
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        this.CheckAndHandle_theObject_I_Hit(collision);
    }
    */

    #endregion

    /* 레이 캐스트 사용으로 대체.
    #region Object check which is hit as a result of a drag-and-drop
    private void CheckAndHandle_theObject_I_Hit(Collision colTheObject)
    {
        // 뭐하는 함수?
        // 내가 (움직이며) 부딪힌 물체를 확인하고, 
        // 그것이 정답브릭인지 아닌지를 플레이어 매니저에게 알리는 함수. 

        if(Application.isEditor) Debug.Log( $"HIT!: MyName={this.name}. I Hit={colTheObject.transform.gameObject.name}" );
    }
    #endregion
    */

    #region Object Drag and Drop related methods
    // Ref. https://www.youtube.com/watch?v=pFpK4-EqHXQ 

    private Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(this.transform.position);
    }

    private void SetCurrent_UserTappedPosition_forDragAndDrop()
    {
        this.mousePosition = Input.mousePosition - this.GetMousePos();
    }


    #endregion


}
