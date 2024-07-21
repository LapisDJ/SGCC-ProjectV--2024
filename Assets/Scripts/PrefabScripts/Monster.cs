using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    MonsterStat monsterStat;
    MonsterController monsterController;
    void Awake()
    {
        monsterStat = GetComponent<MonsterStat>();
        monsterController = GetComponent<MonsterController>();
    }


    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        if (monsterStat != null)
        {
            monsterStat.MosnterCurrentHP -= damage;
            if (monsterStat.MosnterCurrentHP <= 0)
            {
                Die();
            }
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
