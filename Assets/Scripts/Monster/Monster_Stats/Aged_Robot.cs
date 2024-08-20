using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aged_Robot : MonoBehaviour
{
    [SerializeField] float Attack_Damage = 8.0f;
    [SerializeField] float HP = 25000.0f;
    [SerializeField] float Speed = 3.5f;
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
