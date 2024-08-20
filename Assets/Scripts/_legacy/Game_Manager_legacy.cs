using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class game_manager : MonoBehaviour
{
    private float initial_time;
    public GameObject enemy;
    [SerializeField] GameObject[] spawnpoints;
    void Start()
    {
        initial_time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - initial_time >= 1)
        {
            initial_time = Time.time;
            for(int i = 0; i < 8; i++)
            {

                GameObject spawned_enemy = Instantiate(enemy);
                spawned_enemy.transform.position = spawnpoints[i].transform.position;
            }
        }
    }
}