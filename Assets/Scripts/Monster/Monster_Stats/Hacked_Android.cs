using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hacked_Android : Monster
{
     protected override void Awake()
    {
        stats = new MonsterStats
        {
            initialHP = 250000f,
            initialAttackDamage = 12f,
            initialSpeed = 5f
        };
        InitializeStats();
        weakness = WeaknessType.Blow;
        key = "Elite2";
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
