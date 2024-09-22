using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearnPill : PassiveSkill
{
    protected override void Awake()
    {
        skillName = "학습 증진 알약";
        effect = 0.3f;
        cooldown = 0f;
        icon = Resources.Load<Sprite>("UI/Icon/4");
        this.levelupguide = "경험치 획득량이 증가합니다";
    }
    public override void LevelUp() // 학습 증진 알약 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.effect += 0.1f; // 1레벨을 제외한 모든 레벨에서 효과가 10%p씩 상승.
        Player_Stat.instance.expgainratebypassive = effect;
        this.levelupguide = "경헙치 획득량 증가 " + Convert.ToString(effect * 100) + "% -> " + Convert.ToString((effect + 0.1f) * 100) + "%";
    }
}
