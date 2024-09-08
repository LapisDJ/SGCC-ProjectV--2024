using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    GameObject player;

    [SerializeField]
    float __zPos = -10.0f;


    private void Awake()
    {
        if (gameObject.CompareTag("MainCamera"))
        {
            DontDestroyOnLoad(gameObject); // Player 오브젝트가 파괴되지 않도록 설정
        }
    }
    void LateUpdate()
    {
        Vector3 newPosition = player.transform.position;
        newPosition.z = __zPos;
        transform.position = newPosition;
    }
}
