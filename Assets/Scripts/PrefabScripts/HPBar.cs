using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Slider slider;
    [SerializeField] GameObject player;
    void Start()
    {
        Vector3 hpbarpos = player.transform.position;
        hpbarpos.y += 1;
        transform.position = hpbarpos;
    }


    void Update()
    {
        float currenthp = Player_Stat.instance.HPcurrent;
        float maxhp = Player_Stat.instance.HPmax;
        //슬라이더로 체력바 표시
        slider.value = currenthp / maxhp;
        //체력바 위치 조정
        Vector3 hpbarpos = player.transform.position;
        hpbarpos.y += 1;
        transform.position = hpbarpos;
    }
}
