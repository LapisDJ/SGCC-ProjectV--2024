using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giant_Rat : Monster
{
    protected override void Awake()
    {
        stats = new MonsterStats
        {
            initialHP = 100f,
            initialAttackDamage = 2f,
            initialSpeed = 2f
        };
        InitializeStats();
        weakness = WeaknessType.All;
        key = "Rat";
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
