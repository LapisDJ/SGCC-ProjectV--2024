using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    public string skillName;
    public int level;
    public float baseDamage;
    public float baseCooldown;

    public Skill(string name, float baseDamage, float baseCooldown)
    {
        this.skillName = name;
        this.baseDamage = baseDamage;
        this.baseCooldown = baseCooldown;
        this.level = 0;
    }

    public virtual void LevelUp()
    {
        level++;
    }

    public virtual float GetDamage()
    {
        return baseDamage * (1 + 0.1f * (level - 1));
    }

    public virtual float GetCooldown()
    {
        return baseCooldown / (1 + 0.05f * (level - 1));
    }

    // 몬스터와 상호작용하는 추상 메서드
    public abstract void Activate(GameObject target);
}
