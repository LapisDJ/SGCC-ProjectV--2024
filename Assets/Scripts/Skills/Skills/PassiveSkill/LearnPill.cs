using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearnPill : PassiveSkill
{
    protected override void Awake()
    {
        skillName = "LearnPill";
        effect = 0.3f;
        cooldown = 0f;
    }
    public override void LevelUp() // 학습 증진 알약 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.effect += 0.1f; // 1레벨을 제외한 모든 레벨에서 효과가 10%p씩 상승.
    }
}
