using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Manager : MonoBehaviour
{
    [SerializeField] float timeforspawn;
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject[] spawnpoints;
    void Start()
    {
        timeforspawn = Time.time;
    }

    void FixedUpdate()
    {
        if(Time.time - timeforspawn >= 4)
        {
            timeforspawn = Time.time;
            for(int i = 0; i < 8; i++)
            {
                GameObject spawned_enemy = Instantiate(enemy);
                spawned_enemy.transform.position = spawnpoints[i].transform.position;
            }
        }
    }
}
