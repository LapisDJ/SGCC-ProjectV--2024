using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giant_Spider : Monster
{
    protected override void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        stats = new MonsterStats
        {
            initialHP = 80f,
            initialAttackDamage = 1f,
            initialSpeed = 1f
        };
        InitializeStats();
        weakness = WeaknessType.All;
        key = "Spider";
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
