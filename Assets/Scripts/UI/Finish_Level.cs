using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Finish_Level : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI level;
    void Start()
    {
        level.text = "클리어 레벨 : " + Convert.ToString(Player_Stat.instance.player_level);
    }
}
