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

using System.Linq; // 23.07.07 딕셔너리내의 셀렉트를 사용해보기 위해. 


//---------------------------------------
// [스케일, 코드 값 관련 공통]
public enum eMUSICMODE {Scale, Code}; // 음 연습인지, 코드 연습인지. 
public enum eAVAILABLEKEYS {C, G, D, A, E, F, Bb, NONE}; // 플레이 가능한 키 코드들. 즉, C key...
//---------------------------------------

//---------------------------------------
// [코드]
// 몇도를 나타내는 키패드 3D 오브젝트의 이름은, 아래의 enum 과 같아야 함! 왜? 상호간 형변환 해서 인덱싱 하기 떄문!
public enum eDO_NUMBER { _1do, _2do, _3do, _4do, _5do, _6do, _7do }; // 영어로 몰라서.. 1도.. 화음. 지금은, 1~7화음만 하지만, 나중에는 dim, sus4 등도 할수 있으므로.
public enum e_C_KEYCODES {C, Dm, Em, F, G, Am, Bb}; // 해당 키의 가족 코드들. 
//---------------------------------------

//---------------------------------------
// [단음] Scale
// 건반의 개수를 다 화면에 표시하고, 
// 특정 키에서 쓰이는 음에는 해당 음을, 
// 안 쓰이는 음은 x 표시를 하기. (밤송이 투하)

// 왜 14개 이냐면, C4 D E F G A B C5 (건반 모양상 C#)
// 옥타브 번호를 뺀, 그 키를 구성하는 음만.
//      e.g. C4 건반을 누른다 => 파싱해서 C만 때낸다.   => C키 + C건반 으로 딕셔너리를 인덱싱 한다 => O (해당키 음 맞음) 를 결과로.
//      e.g. D4b 건반을 누른다 => 파싱해서 Db만 떼낸다. => C키 + Db건반 으로 딕셔너리를 인덱싱 한다 => X (해당키 음 아님) 를 결과로.

// 스케일 연습 화면상에서 보이는 모든 피아노 키의 대표 이름. 
public enum ePIANOKEYS {C, Db, D, Eb, E, F, Fsharp, G, Ab, A, Bb, B};
// 이건 그냥 이넘이므로, 위의 것과 일치하지 않는다. 해당 키 스케일의 딕셔녀리 데이터를 완성하는 부분의 코드를 참조. 
// 0은 그 키 스케일에 해당음이 아닐때. 1 은 그 키 스케일의 1도 음, 2는 2도 음을 의미. 
//public enum e_KEYSCALES_NUMBER {_0, _1, _2, _3, _4, _5, _6, _7} // 음.. 이게 뭔 의미가 있나. 판별하는데서 정수로 사용할 건데. 

//public enum e7_PIANOKEYS {C, Db, D, Eb, E, F, Fsharp, G, Ab, A, Bb, B}

//---------------------------------------



public class GameManager : MonoBehaviour
{

    //public const string s3D_Keypads_7do_Name = ""; // 사용자가 입력하는 1~7도 화음 브릭의 오브젝트 이름. 탭 되면 코드에서 구분하기 위함. 나중에 이름이 어떻게 바뀔지 모르므로.. 아. 스위치 문에 안되어서 그냥 다시 주석. 

    private static GameManager instance = null;

    public AudioClip[] aryAudioClips_Ckey;
    public AudioClip AudioClip_Error;

    public Material[] matCkey_ScoreImage;

    public eAVAILABLEKEYS eSelectedKey;

    public eMUSICMODE eSelectedMusicMode;

    // 각 키 ~~코드~~ 애 따라, 1~7 화음의 코드가 뭔지 넣기 위해. 
    //public Dictionary<int, string> dicCodeNum_itsCode;
    public Dictionary<eAVAILABLEKEYS, Dictionary<eDO_NUMBER, string>> dicCode_byKeyAndDoNum; // 키와 몇도인지 값에 따라, 해당 코드를 알려주기 위해.  

    // 각 키에 따라, 구성하는 기본음이 뭔지 넣기 위해. 
    public Dictionary<eAVAILABLEKEYS, Dictionary<ePIANOKEYS, int>> dicScale_byKeyAndPianoKeys; // 키와 피아노 건반에 따라, 해당 키의 기본 스케일 음의 번호를 알려주기 위해.
    //public Dictionary<eAVAILABLEKEYS, Dictionary<ePIANOKEYS, string>> dicScale_byKeyAndPianoKeys; // 키와 피아노 건반에 따라, 해당 키의 기본 스케일 음을 알려주기 위해. 
    //public Dictionary<eAVAILABLEKEYS, Dictionary<e7_PIANOKEYS, string>> dicScale_byKeyAndPianoKeys; // 키와 피아노 건반에 따라, 해당 키의 기본 스케일 음을 알려주기 위해. 


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

        this.aryAudioClips_Ckey = new AudioClip[7];

        // 머티리얼 로드. 
        // Ref. https://bloodstrawberry.tistory.com/813
        //      https://m.blog.naver.com/PostView.naver?isHttpsRedirect=true&blogId=oklmg&logNo=221148770706 
        //      https://cagongman.tistory.com/64 
        // Resources.LoadAll<Material>("Material/BlockColor");
        this.matCkey_ScoreImage = Resources.LoadAll<Material>("Materials/Ckey_Scores");

    }




    // Start is called before the first frame update
    void Start()
    {
        this.eSelectedMusicMode = eMUSICMODE.Code; // 의미는 없지만 그냥 초기값으로.
        
        aryAudioClips_Ckey[0] = Resources.Load<AudioClip>("Audio/C_code");
        aryAudioClips_Ckey[1] = Resources.Load<AudioClip>("Audio/Dm_code");
        aryAudioClips_Ckey[2] = Resources.Load<AudioClip>("Audio/Em_code");
        aryAudioClips_Ckey[3] = Resources.Load<AudioClip>("Audio/F_code");
        aryAudioClips_Ckey[4] = Resources.Load<AudioClip>("Audio/G_code");
        aryAudioClips_Ckey[5] = Resources.Load<AudioClip>("Audio/Am_code");
        aryAudioClips_Ckey[6] = Resources.Load<AudioClip>("Audio/Bb_code");

        AudioClip_Error = Resources.Load<AudioClip>("Audio/KbdKeyTap");

        this.eSelectedKey = eAVAILABLEKEYS.NONE;


        this.dicCode_byKeyAndDoNum = new Dictionary<eAVAILABLEKEYS, Dictionary<eDO_NUMBER, string>>();

        //==============================================
        // 키 별 각 가족코드 데이터 넣기. 
        // 이 데이터는, 화면에 코드값을 표시하거나, 
        // 소리를 내 줄때, 파싱하는 데이터로도 쓰인다.       
        // Ref. https://afsdzvcx123.tistory.com/entry/C-%EB%AC%B8%EB%B2%95-Dictionarykey-DcitionaryTT-%EC%9D%B4%EC%A4%91-Dictionary-Linq-%EC%82%AC%EC%9A%A9-%EB%B0%A9%EB%B2%95 
        //==============================================
        //this.dicCode_byKeyAndDoNum.Add(eAVAILABLEKEYS.C, new Dictionary<eDO_NUMBER, string>( eDO_NUMBER._1do, e_C_KEYCODES.C.ToString() ) );
        Dictionary<eDO_NUMBER, string> dic_C_KeyFamily = new Dictionary<eDO_NUMBER, string>
                                                            {
                                                                {eDO_NUMBER._1do, e_C_KEYCODES.C.ToString()},
                                                                {eDO_NUMBER._2do, e_C_KEYCODES.Dm.ToString()},
                                                                {eDO_NUMBER._3do, e_C_KEYCODES.Em.ToString()},
                                                                {eDO_NUMBER._4do, e_C_KEYCODES.F.ToString()},
                                                                {eDO_NUMBER._5do, e_C_KEYCODES.G.ToString()},
                                                                {eDO_NUMBER._6do, e_C_KEYCODES.Am.ToString()},
                                                                {eDO_NUMBER._7do, e_C_KEYCODES.Bb.ToString()}
                                                            };

        this.dicCode_byKeyAndDoNum.Add(eAVAILABLEKEYS.C, dic_C_KeyFamily);

        if(Application.isEditor)
        {
            Debug.Log( dicCode_byKeyAndDoNum[eAVAILABLEKEYS.C][eDO_NUMBER._2do] );

            var result = dicCode_byKeyAndDoNum.SelectMany( s => s.Value )
                                                .Select( x => x.Value )
                                                .ToList();

            foreach(var item in result)
            {
                Debug.Log($"{item}");
            }

        }

        //==============================================
        // 키 별 각 단음 (스케일) 데이터 넣기. 
        // 이 데이터는, 화면에 단음 값을 표시하거나, 
        // 소리를 내 줄때, 파싱하는 데이터로도 쓰인다.     
        // treble clef, bass clef
        //==============================================
        // 0은 그 키 스케일에 해당음이 아닐때. 1 은 그 키 스케일의 1도 음, 2는 2도 음을 의미. 
        this.dicScale_byKeyAndPianoKeys = new Dictionary<eAVAILABLEKEYS, Dictionary<ePIANOKEYS, int>>();

        Dictionary<ePIANOKEYS, int> dic_C_KeyScales = new Dictionary<ePIANOKEYS, int>
                                                {
                                                    {ePIANOKEYS.C, 1},
                                                    {ePIANOKEYS.Db, 0},
                                                    {ePIANOKEYS.D, 2},
                                                    {ePIANOKEYS.Eb, 0},
                                                    {ePIANOKEYS.E, 3},
                                                    {ePIANOKEYS.F, 4},
                                                    {ePIANOKEYS.Fsharp, 0},
                                                    {ePIANOKEYS.G, 5},
                                                    {ePIANOKEYS.Ab, 0},
                                                    {ePIANOKEYS.A, 6},
                                                    {ePIANOKEYS.Bb, 0},
                                                    {ePIANOKEYS.B, 7}
                                                };


        this.dicScale_byKeyAndPianoKeys.Add(eAVAILABLEKEYS.C, dic_C_KeyScales);


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

    public string ParsingTheTappedPianoKey(string sPianoKeyNameWithPositionNumber)
    {
        string sScaleAlphabet_withoutPositionNumber;


        if( sPianoKeyNameWithPositionNumber.Length == 2 ) // C4, D4.. 
        {
            sScaleAlphabet_withoutPositionNumber = sPianoKeyNameWithPositionNumber.Substring(0, 1);

        }else if( sPianoKeyNameWithPositionNumber.Length == 3 ) // D4b .. 
        {
            sScaleAlphabet_withoutPositionNumber = 
                                      sPianoKeyNameWithPositionNumber.Substring(0, 1)
                                    + sPianoKeyNameWithPositionNumber.Substring(2, 1);
        }else
        {
            // 예외적인 것은 그대로 표시. 
            sScaleAlphabet_withoutPositionNumber = sPianoKeyNameWithPositionNumber;
        }

        return sScaleAlphabet_withoutPositionNumber;

    }
    

}
