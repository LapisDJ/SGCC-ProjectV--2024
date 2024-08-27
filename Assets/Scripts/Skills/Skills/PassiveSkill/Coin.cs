using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : PassiveSkill
{
    protected override void Awake()
    {
        skillName = "Coin";
        effect = 0.05f;
        cooldown = 0f;
        icon.sprite = Resources.Load<Sprite>("Assets/TeamAssets/skill_icon/2.png");
    }
    public override void LevelUp() // 동전 레벨업 로직
    {
        base.LevelUp(); // 스킬 레벨업
        this.effect += 0.01f; // 1레벨을 제외한 모든 레벨에서 효과가 1%p씩 상승.
    }
}
