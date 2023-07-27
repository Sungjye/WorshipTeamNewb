//===================================================================================
// 주님, 시간이 지날수록 마음이 조급해 지고, 코딩도 한글자 칠 때마다 마음이 바빠집니다. 
// 주님, 찬찬히, 주님의 인도하심을 구하며 할 수 있도록 도와 주십시요!
// 만민의 구원자 되신 예수님의 이름으로 기도드렸습니다, 아멘!
// 
// 뭐하는 스크립트?
// : 스코어 2가지를 표시하는 최상위 패널에 붙어서, GameManager 에서 스코어 값을 받아와 그대로 표시만 하는 함수. 
// : 시작할 때 표시하고, 스코어 업데이트 함수가 불리면 표시함. 
// 
// 2023.07.27. sjjo. Initial.
// 
//===================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class Score_Panel_DisplayManager : MonoBehaviour
{

    public TextMeshProUGUI gmobjNoteScoreText;
    public TextMeshProUGUI gmobjCodeScoreText;

    private string sDISPLAYFORMAT;

    // Start is called before the first frame update
    void Start()
    {
        this.sDISPLAYFORMAT = "N0"; // "#,#";

        this.gmobjNoteScoreText = this.transform.GetChild(0).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        this.gmobjCodeScoreText = this.transform.GetChild(1).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

        this.gmobjNoteScoreText.text = GameManager.Instance.nl_NoteScore.ToString(this.sDISPLAYFORMAT);
        this.gmobjCodeScoreText.text = GameManager.Instance.nl_CodeScore.ToString(this.sDISPLAYFORMAT);

    }

    public void RefreshScores()
    {
        this.gmobjNoteScoreText.text = GameManager.Instance.nl_NoteScore.ToString(this.sDISPLAYFORMAT);
        this.gmobjCodeScoreText.text = GameManager.Instance.nl_CodeScore.ToString(this.sDISPLAYFORMAT);
    }



}
