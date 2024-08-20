using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateKnifeDrone : Skill
{
    public RotateKnifeDrone() : base("RotateKnifeDrone", 10f, 5f) { }

     public override void Activate(GameObject target)
    {
        // 몬스터에게 화염구를 발사하는 로직 구현
        MonsterHealth monsterHealth = target.GetComponent<MonsterHealth>();
        if (monsterHealth != null)
        {
            float damage = GetDamage();
            monsterHealth.TakeDamage(damage);
            Debug.Log($"{skillName} dealt {damage} damage to {target.name}");
        }
    }

    public override void LevelUp()
    {
        base.LevelUp();
        baseDamage += 2f; // 레벨업마다 추가 데미지 증가
    }

    public override float GetDamage()
    {
        return baseDamage * (1 + 0.1f * (level - 1));
    }

    public override float GetCooldown()
    {
        return baseCooldown / (1 + 0.05f * (level - 1));
    }
}
