using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateHelper : PassiveSkill
{
    protected override void Awake()
    {
        skillName = "연산 보조 장치";
        effect = 0.2f;
        cooldown = 0f;
        icon = Resources.Load<Sprite>("UI/Icon/1");
        this.levelupguide = "치명타 확률이 증가합니다.";
    }
    public override void LevelUp() // 연산 보조 장치 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.effect += 0.08f; // 1레벨을 제외한 모든 레벨에서 효과가 8%p씩 상승.
        Player_Stat.instance.CriticalChancebypassive = effect;
        this.levelupguide = "치명타 확률 " + Convert.ToString(effect * 100) + "% -> " + Convert.ToString((effect + 0.08f) * 100) + "%";
    }
}
