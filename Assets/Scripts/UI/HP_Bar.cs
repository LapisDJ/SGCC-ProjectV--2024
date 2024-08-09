using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP_Bar : MonoBehaviour
{
    [SerializeField] Player_Stat playerstat;
    [SerializeField] Slider hpbar;
    void Start()
    {
        
    }

    void Update()
    {
        hpbar.value = playerstat.HPcurrent / playerstat.HPmax;
    }
}
