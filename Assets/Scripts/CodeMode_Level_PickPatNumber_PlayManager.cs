//=====================================================================================
//
// 주님..
// 
// 현재 이 레벨의 플레이를 주관하는 스크립트.
// 해당 레벨: PickPatNumber : 패턴 세트로 내려온 브릭들의 소리를 듣고 맞는 화음 '번호' 를 순서대로 맞추는 레벨. 
//
// 하는일
// > 시작하면 랜덤하게 브릭 3~4개를 생성.
// > 사용자가 1개를 맞추면 해당 브릭의 코드를 표시. 
// > 사용자가 (쌓여 있는) 모든 브릭을 다 맞추면 전체 브릭들의 크기를 살짝 줄인 후 왼쪽으로 ‘쌩’ 이동.
// > 사용자가 도중에 틀리면, (쌓여 있는) 모든 브릭이 다 빠르게 쪼그라들면서 사라짐.
// > 사용자가 맞추고 브릭들이 왼쪽으로 이동하여 카메라 시야에서 사라지면, 새로운 브릭을 생성.
// > (TBD) Score 와 Combo 를 표시해 주기. 
//
// 2023.07.19. sjjo. Initial.
//
//=====================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeMode_Level_PickPatNumber_PlayManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
