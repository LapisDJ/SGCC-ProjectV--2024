using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Slider slider;
    [SerializeField] GameObject player;
    [SerializeField] Player_Stat playerstat;
    void Start()
    {
        playerstat = GetComponent<Player_Stat>();
        Vector3 hpbarpos = player.transform.position;
        hpbarpos.y += 1;
        transform.position = hpbarpos;
    }


    void Update()
    {
        float currenthp = playerstat.HPcurrent;
        float maxhp = playerstat.HPmax;
        //슬라이더로 체력바 표시
        slider.value = currenthp / maxhp;
        //체력바 위치 조정
        Vector3 hpbarpos = player.transform.position;
        hpbarpos.y += 1;
        transform.position = hpbarpos;
    }
}
