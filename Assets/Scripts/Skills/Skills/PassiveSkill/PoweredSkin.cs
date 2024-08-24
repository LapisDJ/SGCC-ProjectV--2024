using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoweredSkin : PassiveSkill
{
    protected override void Awake()
    {
        skillName = "PoweredSkin";
        effect = 0.1f;
        cooldown = 0f;
    }
    public override void LevelUp() // 강화 피부 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.effect += 0.05f; // 1레벨을 제외한 모든 레벨에서 효과가 5%p씩 상승.
    }
}
