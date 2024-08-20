using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Finish_Level : MonoBehaviour
{
    [SerializeField] Player_Stat playerstat;
    [SerializeField] TextMeshProUGUI level;
    void Start()
    {
        playerstat = GetComponent<Player_Stat>();
        level.text = "클리어 레벨 : " + Convert.ToString(playerstat.player_level);
    }
}
