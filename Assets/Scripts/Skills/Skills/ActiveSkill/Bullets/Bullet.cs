using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public bool canPenetrate;

    void Start()
    {
        Destroy(gameObject, 3f); // 3초 후 자동으로 총알을 파괴
    }
}
