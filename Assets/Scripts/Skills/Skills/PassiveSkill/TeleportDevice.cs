using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportDevice : PassiveSkill
{
    protected override void Awake()
    {
        skillName = "아공간 전송 장치";
        effect = 3f;
        cooldown = 30f;
        icon = Resources.Load<Sprite>("UI/Icon/0");
    }
    public override void LevelUp() // 아공간 전송 장치 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.cooldown -= 2f; // 1레벨을 제외한 모든 레벨에서 쿨타임이 2초씩 하락.
    }
}