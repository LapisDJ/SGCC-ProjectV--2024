using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timer;
    private int wholetime;
    private int minute;
    private int second;
    private float initialtime;
    void Start()
    {
        initialtime = Time.time;
    }

    void FixedUpdate()
    {
        wholetime = Convert.ToInt16(Time.time - initialtime);
        minute = wholetime/60;
        second = wholetime%60;
        if(second < 10)
        {
            if(minute < 10)
            {
                timer.text = '0' + Convert.ToString(minute) + " : " + '0' + Convert.ToString(second);
            }
            else
            {
                timer.text = Convert.ToString(minute) + " : " + '0' + Convert.ToString(second);
            }
        }
        else
        {
            if(minute < 10)
            {
                timer.text = '0' + Convert.ToString(minute) + " : " + Convert.ToString(second);
            }
            else
            {
                timer.text = Convert.ToString(minute) + " : " + Convert.ToString(second);
            }
        }
    }
}
