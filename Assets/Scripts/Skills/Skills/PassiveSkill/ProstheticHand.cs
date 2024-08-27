using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProstheticHand : PassiveSkill
{
    protected override void Awake()
    {
        skillName = "ProstheticHand";
        effect = 0.08f;
        cooldown = 0f;
        icon.sprite = Resources.Load<Sprite>("UI/Icon/3.png");
    }
    public override void LevelUp() // 업무용 의수 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.effect += 0.06f; // 1레벨을 제외한 모든 레벨에서 효과가 6%p씩 상승.
    }
}
