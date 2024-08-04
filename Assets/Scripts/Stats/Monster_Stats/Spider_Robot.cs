using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider_Robot : MonoBehaviour
{
    [SerializeField] float Attack_Damage = 4.0f;
    [SerializeField] float HP = 100.0f;
    [SerializeField] float Speed = 6.5f;
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
