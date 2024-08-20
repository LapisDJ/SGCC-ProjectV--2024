using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatalVirus : PassiveSkill
{
    public FatalVirus() : base("FatalVirus", 0.01f, 0f) {}// 생성자 : 스킬명, 1랩 데미지, 1랩 쿨타임
    public override void LevelUp() // 치명적 바이러스 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.effect += 0.01f; // 1레벨을 제외한 모든 레벨에서 효과가 1%p씩 상승.
    }
}
