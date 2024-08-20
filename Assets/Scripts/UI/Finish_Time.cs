using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Finish_Time : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI finaltime;
    void Start()
    {
        finaltime.text = "클리어 시간 : " + Timer.time;
    }

    
}
