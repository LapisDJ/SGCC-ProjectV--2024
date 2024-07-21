using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    GameObject player;

    [SerializeField]
    float __zPos = -10.0f;

    void LateUpdate()
    {
       Vector3 newPosition = player.transform.position;
       newPosition.z = __zPos;
       this.transform.position = newPosition;
    }
}
