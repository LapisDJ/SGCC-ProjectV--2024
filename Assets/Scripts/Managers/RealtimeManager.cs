using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RealtimeManager : MonoBehaviour
{
    public static RealtimeManager instance;

    [SerializeField] static Player_Stat playerstat;
    public static int monsterkill; // 처치한 몬스터의 수

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void Monsterkill()
    //몬스터가 destroy처리될 때 불러올 함수. 처치한 몬스터 개수를 구하고, 그에따라 플레이어를 레벨업함.
    {
        monsterkill++;
        //처치한 몬스터 수에 따라 레벨업. 레벨업 기준에 따라 작성할 것(현재는 레벨에 해당하는 숫자만큼 넣어둠)
        if (monsterkill == 10)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 20)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 30)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 40)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 50)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 60)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 70)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 80)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 90)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 100)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 110)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 120)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 130)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 140)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 150)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 160)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 170)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 180)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 190)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 200)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 210)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 220)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 230)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 240)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 250)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 260)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 270)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 280)
        {
            playerstat.LevelUp();
        }
        else if (monsterkill == 290)
        {
            playerstat.LevelUp();
        }
    }
}
