using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hacked_Android : MonoBehaviour
{
    [SerializeField] float Attack_Damage = 12.0f;
    [SerializeField] float HP = 250000.0f;
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
