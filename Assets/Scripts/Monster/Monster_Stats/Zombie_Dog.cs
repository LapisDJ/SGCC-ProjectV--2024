using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_Dog : Monster
{
    public Zombie_Dog() : base(200f, 6f, 5f, WeaknessType.Slash, "ZombieDog") { } // 생성자 : 최대 체력, 공격력, 이동 속도, 약점 타입

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
