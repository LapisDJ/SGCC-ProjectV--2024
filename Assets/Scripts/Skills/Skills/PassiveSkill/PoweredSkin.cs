using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoweredSkin : PassiveSkill
{
    protected override void Awake()
    {
        skillName = "강화 피부";
        effect = 0.1f;
        cooldown = 0f;
        icon = Resources.Load<Sprite>("UI/Icon/8");
        this.levelupguide = "최대체력이 늘어납니다.";
    }
    public override void LevelUp() // 강화 피부 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.effect += 0.05f; // 1레벨을 제외한 모든 레벨에서 효과가 5%p씩 상승.
        this.levelupguide = "최대체력 증가 " + Convert.ToString(effect * 100) + "% -> " + Convert.ToString((effect + 0.05f) * 100) + "%";
    }
}
