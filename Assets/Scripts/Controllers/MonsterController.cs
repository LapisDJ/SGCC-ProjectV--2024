using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{

    [SerializeField]
    GameObject player;

    [SerializeField]
    float __MonsterSpeed = 3.0f;

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 PlayerPosition = player.transform.position;
        Vector3 MonsterPosition = transform.position;

        Vector3 dirVector = (PlayerPosition - MonsterPosition).normalized;

        transform.position = Vector3.MoveTowards(MonsterPosition, PlayerPosition, __MonsterSpeed * Time.deltaTime);
    }
}
