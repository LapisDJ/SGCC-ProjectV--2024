using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class Monster_Manager_legacy : MonoBehaviour
{
    [SerializeField] float initial_time;
    [SerializeField] float gametime;


    [SerializeField] Slime slime;
    [SerializeField] Giant_Rat giantrat;
    [SerializeField] Giant_Vat giantvat;
    [SerializeField] Zombie_Dog zomdog;
    [SerializeField] Zombie zombie;
    [SerializeField] Aged_Robot agedrobot;
    [SerializeField] float HP = 0.0f;
    [SerializeField] float AD = 0.0f;
    [SerializeField] float Speed = 0.0f;
    void Start()
    {
        slime = GetComponent<Slime>();
        giantrat = GetComponent<Giant_Rat>();
        giantvat = GetComponent<Giant_Vat>();
        zomdog = GetComponent<Zombie_Dog>();
        zombie = GetComponent<Zombie>();
        agedrobot = GetComponent<Aged_Robot>();
        initial_time = Time.time;
    }
    void FixedUpdate()
    {
        gametime = Time.time - initial_time;
        if(gametime <= 60)
        {
            HP = slime.GetHP();
            AD = slime.GetAD();
            Speed = slime.GetAD();
        }
        else if(gametime <= 180)
        {
            HP = giantrat.GetHP();
            AD = giantrat.GetAD();
            Speed = giantrat.GetAD();
        }
        else if(gametime <= 240)
        {
            HP = zomdog.GetHP();
            AD = zomdog.GetAD();
            Speed = zomdog.GetAD();
        }
    }
    public float GetAD()
    {
        return this.AD;
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
