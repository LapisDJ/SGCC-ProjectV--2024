using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_Dog : MonoBehaviour
{
    [SerializeField] float Attack_Damage = 6.0f;
    [SerializeField] float HP = 200.0f;
    [SerializeField] float Speed = 5.0f;
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
