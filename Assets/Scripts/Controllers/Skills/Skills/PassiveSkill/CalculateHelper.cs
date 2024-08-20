using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateHelper : PassiveSkill
{
    public CalculateHelper() : base("CalculateHelper", 0.2f, 0f) {}// 생성자 : 스킬명, 1랩 데미지, 1랩 쿨타임
    public override void LevelUp() // 연산 보조 장치 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.effect += 0.08f; // 1레벨을 제외한 모든 레벨에서 효과가 8%p씩 상승.
    }
}
