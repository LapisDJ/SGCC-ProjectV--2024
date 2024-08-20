using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade_Zombie : Monster
{
    public Upgrade_Zombie() : base(30000f, 10f, 4f, WeaknessType.Slash) { } // 생성자 : 최대 체력, 공격력, 이동 속도, 약점 타입

    public float getAttackDamage()
    {
        return this.attackDamage;
    }
    public float getHP()
    {
        return this.currentHP;
    }
    public float getSpeed()
    {
        return this.speed;
    }
}
