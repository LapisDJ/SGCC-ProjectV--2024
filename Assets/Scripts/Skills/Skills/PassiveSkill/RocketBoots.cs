using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBoots : PassiveSkill
{
    protected override void Awake()
    {
        skillName = "로켓 장화";
        effect = 0.1f;
        cooldown = 0f;
        icon = Resources.Load<Sprite>("UI/Icon/7");
        this.levelupguide = "이동속도가 증가합니다";
    }
    public override void LevelUp() // 로켓 장화 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.effect += 0.05f; // 1레벨을 제외한 모든 레벨에서 효과가 5%p씩 상승.
        Player_Stat.instance.speedbypassive = effect;
        this.levelupguide = "이동속도 증가 " + Convert.ToString(effect * 100) + "% -> " + Convert.ToString((effect + 0.05f) * 100) + "%";
    }
}
