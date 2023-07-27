//====================================================================================
//
// 주님, 특히 이 스크립트를 찬찬히 잘 짤 수 있게 해 주십시요...
// 
// 플레이 전체를 주관하는 스크립트
// [주요 하는 일]
//   * 
//
//  2023.0 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class GameManager : MonoBehaviour
{


    private static GameManager instance = null;



    public eAVAILABLEKEYS eSelectedKey;

    public eMUSICMODE eSelectedMusicMode;

    #region Score System Related variable declaration.
    // 최대치 넘는지 체크 필요함. 
    public long nl_NoteScore, nl_CodeScore;
    #endregion

    void Awake()
    {

        if(instance == null)
        {

            // Ref. 1
            //이 클래스 인스턴스가 탄생했을 때 전역변수 instance에 게임매니저 인스턴스가 담겨있지 않다면, 자신을 넣어준다.
            instance = this;
            
            // Ref. 1
            //씬 전환이 되더라도 파괴되지 않게 한다.
            //gameObject만으로도 이 스크립트가 컴포넌트로서 붙어있는 Hierarchy상의 게임오브젝트라는 뜻이지만, 
            //나는 헷갈림 방지를 위해 this를 붙여주기도 한다.
            DontDestroyOnLoad(this.gameObject);

        }else
        {
            // Ref. 1
            //만약 씬 이동이 되었는데 그 씬에도 Hierarchy에 GameMgr이 존재할 수도 있다.
            //그럴 경우엔 이전 씬에서 사용하던 인스턴스를 계속 사용해주는 경우가 많은 것 같다.
            //그래서 이미 전역변수인 instance에 인스턴스가 존재한다면 자신(새로운 씬의 GameMgr)을 삭제해준다.
            Destroy(this.gameObject);

        }


    #region Score System Related variable initialization.
        // 최대치 넘는지 체크 필요함. 
        nl_NoteScore = 1234567;
        nl_CodeScore = 1234567;
        
    #endregion


    }




    // Start is called before the first frame update
    void Start()
    {
        this.eSelectedMusicMode = eMUSICMODE.Code; // 의미는 없지만 그냥 초기값으로.
        this.eSelectedKey = eAVAILABLEKEYS.NONE;        



    }


    // 컨텐츠 매니저 인스턴스에 접근할 수 있는 프로퍼티. static이므로 다른 클래스에서 맘껏 호출할 수 있다.
    public static GameManager Instance
    {
        get
        {
            if( instance == null )
            {
                return null;
            }
            return instance;
        }

    }


    void Update()
    {

        if(Application.platform == RuntimePlatform.Android)
        {

            if(Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

        }


    }



}
