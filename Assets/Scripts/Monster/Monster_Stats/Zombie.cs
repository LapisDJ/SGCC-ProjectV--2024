using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Monster
{
    public Zombie() : base(5000f, 5f, 2.5f, WeaknessType.All) { } // 생성자 : 최대 체력, 공격력, 이동 속도, 약점 타입

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
