using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade_Zombie : Monster
{
    protected override void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        stats = new MonsterStats
        {
            initialHP = 30000f,
            initialAttackDamage = 10f,
            initialSpeed = 4f
        };
        InitializeStats();
        weakness = WeaknessType.Slash;
        key = "Elite1";
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
