using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearnPill : PassiveSkill
{
    public LearnPill() : base("LearnPill", 0.3f, 0f) {}// 생성자 : 스킬명, 1랩 데미지, 1랩 쿨타임
    public override void LevelUp() // 연산 보조 장치 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.effect += 0.1f; // 1레벨을 제외한 모든 레벨에서 효과가 10%p씩 상승.
    }
}
