using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Slider slider;
    public PlayerStat playerStat;


    void Update()
    {
        slider.value = playerStat.CurrentHP / playerStat.MaxHP;
    }
}
