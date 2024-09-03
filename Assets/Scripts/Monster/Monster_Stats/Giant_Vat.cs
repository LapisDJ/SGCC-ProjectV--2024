using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giant_Vat : Monster
{
    protected override void Awake()
    {
        stats = new MonsterStats
        {
            initialHP = 200f,
            initialAttackDamage = 4f,
            initialSpeed = 3.5f
        };
        InitializeStats();
        weakness = WeaknessType.All;
        key = "Bat";
    }

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
