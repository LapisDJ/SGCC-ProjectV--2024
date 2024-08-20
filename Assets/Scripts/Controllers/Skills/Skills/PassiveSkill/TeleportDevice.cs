using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportDevice : PassiveSkill
{
    public TeleportDevice() : base("TeleportDevice", 3f, 30f) {}// 생성자 : 스킬명, 1랩 데미지, 1랩 쿨타임
    public override void LevelUp() // 연산 보조 장치 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.cooldown -= 2f; // 1레벨을 제외한 모든 레벨에서 쿨타임이 2초씩 하락.
    }
}
