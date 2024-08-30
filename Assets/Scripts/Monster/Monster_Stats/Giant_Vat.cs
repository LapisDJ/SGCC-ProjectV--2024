using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giant_Vat : Monster
{
    public Giant_Vat() : base(200f, 4f, 3.5f, WeaknessType.All, "Bat") { } // 생성자 : 최대 체력, 공격력, 이동 속도, 약점 타입

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
