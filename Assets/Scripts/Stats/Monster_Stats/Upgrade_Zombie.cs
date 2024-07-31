using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade_Zombie : MonoBehaviour
{
    [SerializeField] float Attack_Damage = 20.0f;
    [SerializeField] float HP = 1000000.0f;
    [SerializeField] float Speed = 4.0f;
    public float GetAD()
    {
        return this.Attack_Damage;
    }
    public float GetHP()
    {
        return this.HP;
    }
    public float GetSpeed()
    {
        return this.Speed;
    }
}
