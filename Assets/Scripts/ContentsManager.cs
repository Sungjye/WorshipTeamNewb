//====================================================================================
// 주님, 주님께서 기뻐하시는 컨텐츠만을 담게 하여 주십시오!!!
// 
// 코드, 음계, 코드 패턴, 등의 컨텐츠 관련된 내용을 담는 스크립트. 
// 왜? 
// 결국 기초적인 연습이 끝나고 나면, 자주쓰는 패턴 연습과, 전조, (어쩌면) 곡 연습 등이 중요해 질 수 있으므로.. 
// 
// 
// 2023.07.19. sjjo. Initial
// 
//====================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq; // 23.07.07 딕셔너리내의 셀렉트를 사용해보기 위해. 

//---------------------------------------
// [스케일, 코드 값 관련 공통]
public enum eMUSICMODE {Scale, Code}; // 음 연습인지, 코드 연습인지. 
//public enum eAVAILABLEKEYS {C, G, D, A, E, F, Bb}; // 플레이 가능한 키 코드들. 즉, C key...
public enum eAVAILABLEKEYS {C, G}; // 플레이 가능한 키 코드들. 즉, C key...
//---------------------------------------

//---------------------------------------
// [코드]
// 몇도를 나타내는 키패드 3D 오브젝트의 이름은, 아래의 enum 과 같아야 함! 왜? 상호간 형변환 해서 인덱싱 하기 떄문!
public enum eDO_NUMBER { _1do, _2do, _3do, _4do, _5do, _6do, _7do }; // 영어로 몰라서.. 1도.. 화음. 지금은, 1~7화음만 하지만, 나중에는 dim, sus4 등도 할수 있으므로.
public enum e_C_KEYCODES {C, Dm, Em, F, G, Am, Bb}; // 해당 키의 가족 코드들. 
public enum e_G_KEYCODES {G, Am, Bm, C, D, Em, F}; // 해당 키의 가족 코드들. 
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

// 스케일 모드의 퀴즈는, 위치 번호까지 존재하므로, (적어도 Pick Note 퀴즈는)
// 별도의 enum type을 사용함!
public enum e_C_SCALENOTES {C4, D4, E4, F4, G4, A4, B4}; // 나중에 한 옥타브 넘어서 퀴즈내고 싶으면, 여기에 추가.. 
public enum e_G_SCALENOTES {G4, A4, B4, C4, D4, E4, F4sharp};
public enum e_F_SCALENOTES {F4, G4, A4, B4b, C4, D4, E4};



public class ContentsManager : MonoBehaviour
{
    private static ContentsManager instance = null;

    public AudioClip[] aryAudioClips_Ckey_Code;
    public AudioClip[] aryAudioClips_Gkey_Code;

    public AudioClip[] aryAudioClips_Ckey_Scale;

    public AudioClip AudioClip_Error;

    public Material[] matCkey_ScoreImage;
    public Material[] matGkey_ScoreImage;

    public Material matQuiz_Tap_Mark_Image, matQuiz_O_Mark_Image, matQuiz_X_Mark_Image;

    // 각 키 ~~코드~~ 애 따라, 1~7 화음의 코드가 뭔지 넣기 위해. 
    //public Dictionary<int, string> dicCodeNum_itsCode;
    public Dictionary<eAVAILABLEKEYS, Dictionary<eDO_NUMBER, string>> dicCode_byKeyAndDoNum; // 키와 몇도인지 값에 따라, 해당 코드를 알려주기 위해.  

    // 각 키에 따라, 구성하는 기본음이 뭔지 넣기 위해. 
    public Dictionary<eAVAILABLEKEYS, Dictionary<ePIANOKEYS, int>> dicScale_byKeyAndPianoKeys; // 키와 피아노 건반에 따라, 해당 키의 기본 스케일 음의 번호를 알려주기 위해.
    //public Dictionary<eAVAILABLEKEYS, Dictionary<ePIANOKEYS, string>> dicScale_byKeyAndPianoKeys; // 키와 피아노 건반에 따라, 해당 키의 기본 스케일 음을 알려주기 위해. 
    //public Dictionary<eAVAILABLEKEYS, Dictionary<e7_PIANOKEYS, string>> dicScale_byKeyAndPianoKeys; // 키와 피아노 건반에 따라, 해당 키의 기본 스케일 음을 알려주기 위해. 


    //--------------------------------------
    // public string sCodeMode_Level_PickNumber_QuizBrickName;
    //public string sCodeMode_Tapped_Keypad_inTermsOfTheSelectedKey; 

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

        this.aryAudioClips_Ckey_Code = new AudioClip[7];
        this.aryAudioClips_Gkey_Code = new AudioClip[7];

        this.aryAudioClips_Ckey_Scale = new AudioClip[12]; // 23.07.12

        // 머티리얼 로드. 
        // Ref. https://bloodstrawberry.tistory.com/813
        //      https://m.blog.naver.com/PostView.naver?isHttpsRedirect=true&blogId=oklmg&logNo=221148770706 
        //      https://cagongman.tistory.com/64 
        // Resources.LoadAll<Material>("Material/BlockColor");
        this.matCkey_ScoreImage = Resources.LoadAll<Material>("Materials/Ckey_Scores");
        this.matGkey_ScoreImage = Resources.LoadAll<Material>("Materials/Gkey_Scores"); // 23.08.07


        this.matQuiz_Tap_Mark_Image = Resources.Load<Material>("Materials/Mark_Tap");
        this.matQuiz_O_Mark_Image = Resources.Load<Material>("Materials/Mark_O");
        this.matQuiz_X_Mark_Image = Resources.Load<Material>("Materials/Mark_X");

    }

    // Start is called before the first frame update
    void Start()
    {
        //= Code: C Major ====================================================================================
        aryAudioClips_Ckey_Code[0] = Resources.Load<AudioClip>("Audio/Code_C_Codes/C_code");
        aryAudioClips_Ckey_Code[1] = Resources.Load<AudioClip>("Audio/Code_C_Codes/Dm_code");
        aryAudioClips_Ckey_Code[2] = Resources.Load<AudioClip>("Audio/Code_C_Codes/Em_code");
        aryAudioClips_Ckey_Code[3] = Resources.Load<AudioClip>("Audio/Code_C_Codes/F_code");
        aryAudioClips_Ckey_Code[4] = Resources.Load<AudioClip>("Audio/Code_C_Codes/G_code");
        aryAudioClips_Ckey_Code[5] = Resources.Load<AudioClip>("Audio/Code_C_Codes/Am_code");
        aryAudioClips_Ckey_Code[6] = Resources.Load<AudioClip>("Audio/Code_C_Codes/Bb_code");

        //= Code: G Major ====================================================================================
        aryAudioClips_Gkey_Code[0] = Resources.Load<AudioClip>("Audio/Code_G_Codes/G_code_guitar");
        aryAudioClips_Gkey_Code[1] = Resources.Load<AudioClip>("Audio/Code_G_Codes/Am_code_guitar");
        aryAudioClips_Gkey_Code[2] = Resources.Load<AudioClip>("Audio/Code_G_Codes/Bm_code_guitar");
        aryAudioClips_Gkey_Code[3] = Resources.Load<AudioClip>("Audio/Code_G_Codes/C_code_guitar");
        aryAudioClips_Gkey_Code[4] = Resources.Load<AudioClip>("Audio/Code_G_Codes/D_code_guitar");
        aryAudioClips_Gkey_Code[5] = Resources.Load<AudioClip>("Audio/Code_G_Codes/Em_code_guitar");
        aryAudioClips_Gkey_Code[6] = Resources.Load<AudioClip>("Audio/Code_G_Codes/F_code_guitar");


        /////////////////////////////////////////////////////////////////////////////////////////
        // 현재는 C4~B4 한 옥타브만 하는데, 나중에는 더 생길수도.. 그러면, 머티리얼 리소스 로드하는 것처럼 해야할지도..
        // 23.07.12
        aryAudioClips_Ckey_Scale[0] = Resources.Load<AudioClip>("Audio/Scale_C_SingleNote/C4");
        aryAudioClips_Ckey_Scale[1] = Resources.Load<AudioClip>("Audio/Scale_C_SingleNote/D4b");
        aryAudioClips_Ckey_Scale[2] = Resources.Load<AudioClip>("Audio/Scale_C_SingleNote/D4");
        aryAudioClips_Ckey_Scale[3] = Resources.Load<AudioClip>("Audio/Scale_C_SingleNote/E4b");
        aryAudioClips_Ckey_Scale[4] = Resources.Load<AudioClip>("Audio/Scale_C_SingleNote/E4");

        aryAudioClips_Ckey_Scale[5] = Resources.Load<AudioClip>("Audio/Scale_C_SingleNote/F4");
        aryAudioClips_Ckey_Scale[6] = Resources.Load<AudioClip>("Audio/Scale_C_SingleNote/F4#");
        aryAudioClips_Ckey_Scale[7] = Resources.Load<AudioClip>("Audio/Scale_C_SingleNote/G4");
        aryAudioClips_Ckey_Scale[8] = Resources.Load<AudioClip>("Audio/Scale_C_SingleNote/A4b");
        aryAudioClips_Ckey_Scale[9] = Resources.Load<AudioClip>("Audio/Scale_C_SingleNote/A4");

        aryAudioClips_Ckey_Scale[10] = Resources.Load<AudioClip>("Audio/Scale_C_SingleNote/B4b");
        aryAudioClips_Ckey_Scale[11] = Resources.Load<AudioClip>("Audio/Scale_C_SingleNote/B4");

        AudioClip_Error = Resources.Load<AudioClip>("Audio/KbdKeyTap");



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

        Dictionary<eDO_NUMBER, string> dic_G_KeyFamily = new Dictionary<eDO_NUMBER, string>
                                                            {
                                                                {eDO_NUMBER._1do, e_G_KEYCODES.G.ToString()},
                                                                {eDO_NUMBER._2do, e_G_KEYCODES.Am.ToString()},
                                                                {eDO_NUMBER._3do, e_G_KEYCODES.Bm.ToString()},
                                                                {eDO_NUMBER._4do, e_G_KEYCODES.C.ToString()},
                                                                {eDO_NUMBER._5do, e_G_KEYCODES.D.ToString()},
                                                                {eDO_NUMBER._6do, e_G_KEYCODES.Em.ToString()},
                                                                {eDO_NUMBER._7do, e_G_KEYCODES.F.ToString()}
                                                            };
        this.dicCode_byKeyAndDoNum.Add(eAVAILABLEKEYS.G, dic_G_KeyFamily);

/*
        Dictionary<eDO_NUMBER, string> dic_%_KeyFamily = new Dictionary<eDO_NUMBER, string>
                                                            {
                                                                {eDO_NUMBER._1do, e_%_KEYCODES.%.ToString()},
                                                                {eDO_NUMBER._2do, e_%_KEYCODES.%.ToString()},
                                                                {eDO_NUMBER._3do, e_%_KEYCODES.%.ToString()},
                                                                {eDO_NUMBER._4do, e_%_KEYCODES.%.ToString()},
                                                                {eDO_NUMBER._5do, e_%_KEYCODES.%.ToString()},
                                                                {eDO_NUMBER._6do, e_%_KEYCODES.%.ToString()},
                                                                {eDO_NUMBER._7do, e_%_KEYCODES.%.ToString()}
                                                            };
        this.dicCode_byKeyAndDoNum.Add(eAVAILABLEKEYS.%, dic_%_KeyFamily);
*/

        ///////////////////////////////////////////////////////////////////////////////
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
        ///////////////////////////////////////////////////////////////////////////////

        //==============================================
        // 키 별 각 단음 (스케일) 데이터 넣기. 
        // 이 데이터는, 화면에 단음 값을 표시하거나, 
        // 소리를 내 줄때, 파싱하는 데이터로도 쓰인다.     
        // treble clef, bass clef
        //==============================================
        // 0은 그 키 스케일에 해당음이 아닐때. 1 은 그 키 스케일의 1도 음, 2는 2도 음을 의미. 
        this.dicScale_byKeyAndPianoKeys = new Dictionary<eAVAILABLEKEYS, Dictionary<ePIANOKEYS, int>>();

        // 이 데이터는, 사용자가 친 음이, 해당 스케일의 음인지 아닌지를 판정하는데만 쓴다!
        // 스케일 모드의 경우, 탭된 건반에 해당하는 유일한 음을 내주고, 또 그 음 (e.g. C4 숫자 포함)으로 퀴즈모드에서 비교해야. 
        Dictionary<ePIANOKEYS, int> dic_C_KeyScales = new Dictionary<ePIANOKEYS, int> // C major
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

        Dictionary<ePIANOKEYS, int> dic_G_KeyScales = new Dictionary<ePIANOKEYS, int> // G major
                                                {
                                                    {ePIANOKEYS.C, 4},
                                                    {ePIANOKEYS.Db, 0},
                                                    {ePIANOKEYS.D, 5},
                                                    {ePIANOKEYS.Eb, 0},
                                                    {ePIANOKEYS.E, 6},
                                                    {ePIANOKEYS.F, 0},
                                                    {ePIANOKEYS.Fsharp, 7},
                                                    {ePIANOKEYS.G, 1},
                                                    {ePIANOKEYS.Ab, 0},
                                                    {ePIANOKEYS.A, 2},
                                                    {ePIANOKEYS.Bb, 0},
                                                    {ePIANOKEYS.B, 3}
                                                };
        this.dicScale_byKeyAndPianoKeys.Add(eAVAILABLEKEYS.G, dic_G_KeyScales);

/*
        Dictionary<ePIANOKEYS, int> dic_%_KeyScales = new Dictionary<ePIANOKEYS, int> // % major
                                                {
                                                    {ePIANOKEYS.C, 0},
                                                    {ePIANOKEYS.Db, 0},
                                                    {ePIANOKEYS.D, 0},
                                                    {ePIANOKEYS.Eb, 0},
                                                    {ePIANOKEYS.E, 0},
                                                    {ePIANOKEYS.F, 0},
                                                    {ePIANOKEYS.Fsharp, 0},
                                                    {ePIANOKEYS.G, 0},
                                                    {ePIANOKEYS.Ab, 0},
                                                    {ePIANOKEYS.A, 0},
                                                    {ePIANOKEYS.Bb, 0},
                                                    {ePIANOKEYS.B, 0}
                                                };
        this.dicScale_byKeyAndPianoKeys.Add(eAVAILABLEKEYS.%, dic_%_KeyScales);
*/


    }


    // 컨텐츠 매니저 인스턴스에 접근할 수 있는 프로퍼티. static이므로 다른 클래스에서 맘껏 호출할 수 있다.
    public static ContentsManager Instance
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

#region Private Methods for the Audio clips
    //========================================
    // 각 키별, 음원 오디오 클립 

    //= 코드 모드 ==============================
    private AudioClip AmI_Ckey_CodeMode_Retrieve_AudioClip(string sMyName)
    {
        // 한꺼번에 해도 되지만, 보기 산만하니까, 키별로 나누어서..
        AudioClip acItsClip = null;
        //-----------
        // C 키
        switch( sMyName )
        {
            case "_1do": // C
                acItsClip = this.aryAudioClips_Ckey_Code[0];
                break;
            case "_2do":
                acItsClip = this.aryAudioClips_Ckey_Code[1];
                break;
            case "_3do":
                acItsClip = this.aryAudioClips_Ckey_Code[2];
                break;
            case "_4do":
                acItsClip = this.aryAudioClips_Ckey_Code[3];
                break;
            case "_5do":
                acItsClip = this.aryAudioClips_Ckey_Code[4];
                break;
            case "_6do":
                acItsClip = this.aryAudioClips_Ckey_Code[5];
                break;
            case "_7do":
                acItsClip = this.aryAudioClips_Ckey_Code[6];
                break;
            default:
                acItsClip = this.AudioClip_Error;
                break;
        }

        return acItsClip;

    }

    private AudioClip AmI_Gkey_CodeMode_Retrieve_AudioClip(string sMyName)
    {
        // 한꺼번에 해도 되지만, 보기 산만하니까, 키별로 나누어서..
        AudioClip acItsClip = null;
        //-----------
        // G 키
        switch( sMyName )
        {
            case "_1do": acItsClip = this.aryAudioClips_Gkey_Code[0]; break;
            case "_2do": acItsClip = this.aryAudioClips_Gkey_Code[1]; break;
            case "_3do": acItsClip = this.aryAudioClips_Gkey_Code[2]; break;
            case "_4do": acItsClip = this.aryAudioClips_Gkey_Code[3]; break;
            case "_5do": acItsClip = this.aryAudioClips_Gkey_Code[4]; break;
            case "_6do": acItsClip = this.aryAudioClips_Gkey_Code[5]; break;
            case "_7do": acItsClip = this.aryAudioClips_Gkey_Code[6]; break;
            default: acItsClip = this.AudioClip_Error; break;
        }
        return acItsClip;
    }    
    //==================================================
    //= 단음 (스케일) 모드 ==============================
    private AudioClip AmI_Ckey_ScaleMode_Retrieve_AudioClip(string sMyName)
    {
        AudioClip acItsClip = null;
        //-----------
        // C 키: 단음
        //-----------
        switch( sMyName )
        {
            case "C4": acItsClip = this.aryAudioClips_Ckey_Scale[0]; break;
            case "D4": acItsClip = this.aryAudioClips_Ckey_Scale[2]; break; // 이게 이렇게 건너 뛰는 이유는, 12음 전체 구성으로 음원이 되어 있기 떄문. 
            case "E4": acItsClip = this.aryAudioClips_Ckey_Scale[4]; break;
            case "F4": acItsClip = this.aryAudioClips_Ckey_Scale[5]; break;
            case "G4": acItsClip = this.aryAudioClips_Ckey_Scale[7]; break;
            case "A4": acItsClip = this.aryAudioClips_Ckey_Scale[9]; break;
            case "B4": acItsClip = this.aryAudioClips_Ckey_Scale[11]; break;
            default: acItsClip = this.AudioClip_Error; break;
        }
        return acItsClip;
    }

    private AudioClip AmI_Gkey_ScaleMode_Retrieve_AudioClip(string sMyName)
    {
        AudioClip acItsClip = null;
        //-----------
        // G 키: 단음
        //-----------
        switch( sMyName )
        {
            case "C4": acItsClip = this.aryAudioClips_Ckey_Scale[0]; break;
            case "D4": acItsClip = this.aryAudioClips_Ckey_Scale[2]; break; // 이게 이렇게 건너 뛰는 이유는, 12음 전체 구성으로 음원이 되어 있기 떄문. 
            case "E4": acItsClip = this.aryAudioClips_Ckey_Scale[4]; break;
            case "F4#": acItsClip = this.aryAudioClips_Ckey_Scale[6]; break; 
            case "G4": acItsClip = this.aryAudioClips_Ckey_Scale[7]; break;
            case "A4": acItsClip = this.aryAudioClips_Ckey_Scale[9]; break;
            case "B4": acItsClip = this.aryAudioClips_Ckey_Scale[11]; break;
            default: acItsClip = this.AudioClip_Error; break;
        }
        return acItsClip;
    }

#endregion

#region Private Methods for the Musical notation image materials
    //========================================
    // 각 키별, 악보이미지 머티리얼 
    private Material AmI_Ckey_ScaleMode_Retrieve_MusicalNotation(string sMyName)
    {
        Material maItsMaterial = null;
        //-----------
        // C 키: 단음
        //-----------
        switch( sMyName )
        {
            case "C4": maItsMaterial = this.matCkey_ScoreImage[0]; break;
            case "D4": maItsMaterial = this.matCkey_ScoreImage[1]; break;
            case "E4": maItsMaterial = this.matCkey_ScoreImage[2]; break;
            case "F4": maItsMaterial = this.matCkey_ScoreImage[3]; break;
            case "G4": maItsMaterial = this.matCkey_ScoreImage[4]; break;
            case "A4": maItsMaterial = this.matCkey_ScoreImage[5]; break;
            case "B4": maItsMaterial = this.matCkey_ScoreImage[6]; break;
            default: maItsMaterial = null; break;
        }
        return maItsMaterial;
    }

    private Material AmI_Gkey_ScaleMode_Retrieve_MusicalNotation(string sMyName)
    {
        Material maItsMaterial = null;
        //-----------
        // G 키: 단음
        //-----------
        switch( sMyName )
        {
            case "G4": maItsMaterial = this.matGkey_ScoreImage[0]; break;
            case "A4": maItsMaterial = this.matGkey_ScoreImage[1]; break;
            case "B4": maItsMaterial = this.matGkey_ScoreImage[2]; break;
            case "C4": maItsMaterial = this.matGkey_ScoreImage[3]; break;
            case "D4": maItsMaterial = this.matGkey_ScoreImage[4]; break;
            case "E4": maItsMaterial = this.matGkey_ScoreImage[5]; break;
            case "F4#": maItsMaterial = this.matGkey_ScoreImage[6]; break;
            default: maItsMaterial = null; break;
        }
        return maItsMaterial;
    }

#endregion

#region Private Methods for etc functions

#endregion

#region Popular methods to retirve content related information.

    public string ParsingTheTappedPianoKey(string sPianoKeyNameWithPositionNumber)
    {
        // 뭐하는 함수?
        // 스케일 모드의 피아노 키(오브젝트)값을 받아서, 
        // 키의 위치 값만 떼고 키 값을 리턴해 주는 함수. 
        // 왜? 이 리턴값이 enum 인덱싱 하는데 쓰이므로...
        //
        // e.g. C4 => C, D4b => Db, F4# => Fsharp
        string sScaleAlphabet_withoutPositionNumber;


        if( sPianoKeyNameWithPositionNumber.Length == 2 ) // C4, D4.. 
        {

            sScaleAlphabet_withoutPositionNumber = sPianoKeyNameWithPositionNumber.Substring(0, 1);

        }else if( sPianoKeyNameWithPositionNumber.Length == 3 ) // D4b .. 
        {
            sScaleAlphabet_withoutPositionNumber = 
                                      sPianoKeyNameWithPositionNumber.Substring(0, 1)
                                    + sPianoKeyNameWithPositionNumber.Substring(2, 1);

            // 23.07.12. 주님, 감사합니다! 고민했었는데 이렇게 해결할 지혜를 주셔서 감사합니다!
            // # 이 붙은 것들은 enum 타입에 선언이 안되어서 sharp으로 되어 있다. 
            // 그래서 enum을 사용한 딕셔너리 인덱싱할 때만
            // (탭된 오브젝트의 이름에 포함 되었을) #을 sharp로 치환해서 사용!

            if( sScaleAlphabet_withoutPositionNumber.Substring(1, 1) == "#")
            {
                //Debug.Log("BANG! I detect #!");
                // #이 있으므로 다시 재구성. 
                sScaleAlphabet_withoutPositionNumber = 
                                    sPianoKeyNameWithPositionNumber.Substring(0, 1)
                                    + "sharp";
            }

        }else
        {
            // 예외적인 것은 그대로 표시. 
            sScaleAlphabet_withoutPositionNumber = sPianoKeyNameWithPositionNumber;
        }

        return sScaleAlphabet_withoutPositionNumber;

    }
    
    public string ParsingTheQuiz_SoundBrickName(string sQuizBrickName)
    {
        // 뭐하는 함수?
        // 코드모드의 퀴즈 레벨에서..
        // 문제로 나오는 인스턴시에엣 된 (사운드) 브릭의 (긴) 이름을 파싱해서, 
        // 아니 피싱일것도 없고, 몇도인지만 스트링을 떼서 리턴해 줌. 
        // 왜? 
        // 인스턴시에잇 되면서 만들어진 descriptive 한 오브젝트의 이름에서, 
        // 딕셔너리를 인덱싱하기 위한 '몇도' 인지만 알아내기 위해. 
        // e.g. 
        // instCodeBrick_C__1do => _1do

        int nLengthOfName = sQuizBrickName.Length;

        string sDoNumberString = sQuizBrickName.Substring((nLengthOfName-4), 4);

        return sDoNumberString;
    }

    public string GetOneBrickName_inePIANOKEYS__Randomly()
    {
        // 뭐하는 함수?
        // 스케일 모드 퀴즈 또는 스케일 패턴 모드 퀴즈 에서. 
        // 현재 선택된 키에 해당하는 노트 중에서, 랜덤으로 하나 골라 그것을 스트링으로 리턴해 주는 함수. 
        //
        // 
        // Ref. 
        // https://www.reddit.com/r/Unity3D/comments/ax1tqf/unity_tip_random_item_from_enum/
        // https://afsdzvcx123.tistory.com/entry/C-%EB%AC%B8%EB%B2%95-C-Enum-Count-%EA%B0%80%EC%A0%B8%EC%98%A4%EB%8A%94-%EB%B0%A9%EB%B2%95 

        string sRandomNote;

        switch( GameManager.Instance.eSelectedKey )
        {
            case eAVAILABLEKEYS.C:
                sRandomNote  = ((e_C_SCALENOTES)( Random.Range(0, System.Enum.GetValues(typeof(e_C_SCALENOTES)).Length) )).ToString();
                // 예는 샵이 없어서 안해도 될듯?
                //sRandomNote = this.CheckAndReplace_sharpString_with_sharpMark(sRandomNote);
                break;
            case eAVAILABLEKEYS.G:
                //sRandomNote = "F4#"; // tentative.
                sRandomNote  = ((e_G_SCALENOTES)( Random.Range(0, System.Enum.GetValues(typeof(e_G_SCALENOTES)).Length) )).ToString();
                sRandomNote = this.CheckAndReplace_sharpString_with_sharpMark(sRandomNote);
                break;
            default:
                sRandomNote = "Err:Rnd"; // 랜덤 브릭 생성 오류.
                break;
        }

        // random function for integer has the upper bound value which is not include itself.
        //nUpperBound++;



        return sRandomNote;
    }

    public string CheckAndReplace_sharpString_with_sharpMark(string sScaleBrickName)
    {
        // 뭐하는 함수?
        // 스케일모드, 퀴즈 브릭 1개 모드 등에서, 
        // F4sharp 이렇게 (정의된 enum 값의 스트링변환에 따라) 생성된 퀴즈 브릭의 이름을, 
        // F4# 이렇게 바꾸어 주는 함수. 
        // 왜 필요함? 이게, enum 값만 # 이 안되고 문제일 뿐이지, 코드 내부는 다 # (e.g. F4# ) 포함한 걸로 돌아서..

        // 입력 sScaleBrickName 예시: F4sharp (G major 키 선택시.)
        // 출력 sResult_scaleBrickName 예시: F4# 

        string sResult_scaleBrickName = null;

        if( sScaleBrickName.Length > 3) // 글자수가 3개 이상이라면 sharp 이 붙은거다. 샵이 없다면, D4b 이렇게가 최대 길이.
        {
            string sIsSharpStr = sScaleBrickName.Substring( sScaleBrickName.Length - 5, 5 ); 
            if( sIsSharpStr == "sharp" ) sResult_scaleBrickName = ( sScaleBrickName.Substring( 0, 2 ) + "#" ); 
            else sResult_scaleBrickName = sScaleBrickName;

        }else
        {
            sResult_scaleBrickName = sScaleBrickName;
        }

        return sResult_scaleBrickName;

    }



    public AudioClip Check_WhoAmI_Retrieve_myAudioClip_CodeOrScale(string sUserTappedObjectName)
    {
        // 뭐하는 함수?
        // 코드인지 스케일 모드인지 상관없이 (이름으로 구별..) 
        // 넘어온 게임오브젝트의 이름을 확인해서 
        // 키를 확인하고, 해당하는 소리의 클립 데이터를 리턴해 주는 함수. 

        AudioClip acResult_AudioClipData = null;

        if( sUserTappedObjectName.Substring(0,1) == "_" ) // _1do, _3do
        {
            // 코드 모드인 경우. 탭한 오브젝트의 키보드 브릭이.
            switch( GameManager.Instance.eSelectedKey )
            {
                case eAVAILABLEKEYS.C:
                    Debug.Log("Retireve AudioClip: C key. TabppedObj: " + sUserTappedObjectName);
                    acResult_AudioClipData = AmI_Ckey_CodeMode_Retrieve_AudioClip(sUserTappedObjectName);
                    break;
                case eAVAILABLEKEYS.G:
                    Debug.Log("Retireve AudioClip: G key. TabppedObj: " + sUserTappedObjectName);
                    acResult_AudioClipData = AmI_Gkey_CodeMode_Retrieve_AudioClip(sUserTappedObjectName);
                    break;
                default:
                    break;
            }
        }else
        {
            // 스케일 모드 인경우. 
            switch( GameManager.Instance.eSelectedKey )
            {
                case eAVAILABLEKEYS.C:
                    acResult_AudioClipData = AmI_Ckey_ScaleMode_Retrieve_AudioClip(sUserTappedObjectName);
                    break;
                //------
                // 음.. 스케일 모드는, 키 상관없이, 그냥 음 이름을 리턴해주면 될듯? 23.08.07
                // 아니, 인트로 모드 떄문에 안될듯...? 왜? 해당 스케일이 아닌 음은 '띡' 소리 나야 해서. 
                case eAVAILABLEKEYS.G:
                    acResult_AudioClipData = AmI_Gkey_ScaleMode_Retrieve_AudioClip(sUserTappedObjectName);
                    break;
                default:
                    break;
            }
        }

        return acResult_AudioClipData;
    }

    public Material Check_WhoAmI_Retrieve_myMusicalNotation_Scale(string sUserTappedObjectName)
    {
        // 뭐하는 함수?
        // 스케일 모드인 경우, 
        // 넘어온 게임오브젝트의 이름을 확인해서
        // 키를 확인하고, 해당하는 음의 악보 이미지 머티리얼을 리턴해 주는 함수. 

        Material maResult_NotationMaterialData = null;

        switch( GameManager.Instance.eSelectedKey )
        {
            case eAVAILABLEKEYS.C:
                maResult_NotationMaterialData = AmI_Ckey_ScaleMode_Retrieve_MusicalNotation(sUserTappedObjectName);
                break;
            case eAVAILABLEKEYS.G:
                maResult_NotationMaterialData = AmI_Gkey_ScaleMode_Retrieve_MusicalNotation(sUserTappedObjectName);
                break;
            default:
                break;
        }

        return maResult_NotationMaterialData;
    }

#endregion



}
