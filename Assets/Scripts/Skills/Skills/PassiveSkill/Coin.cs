using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : PassiveSkill
{
    protected override void Awake()
    {
        skillName = "동전";
        effect = 0.05f;
        cooldown = 0f;
        icon = Resources.Load<Sprite>("UI/Icon/2");
        this.levelupguide = "행운이 증가합니다";
    }
    public override void LevelUp() // 동전 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.effect += 0.01f; // 1레벨을 제외한 모든 레벨에서 효과가 1%p씩 상승.
        this.levelupguide = "행운 증가 " + Convert.ToString(effect * 100) + "% -> " + Convert.ToString((effect + 0.01f) * 100) + "%";
    }
}
