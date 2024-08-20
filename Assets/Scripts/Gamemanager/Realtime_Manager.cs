using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Realtime_Manager : MonoBehaviour
{
    [SerializeField] static Player_Stat playerstat;
    public static int monsterkill; // 처치한 몬스터의 수
    
    public static void Monsterkill()
    //몬스터가 destroy처리될 때 불러올 함수. 처치한 몬스터 개수를 구하고, 그에따라 플레이어를 레벨업함.
    {
        monsterkill++;
        //처치한 몬스터 수에 따라 레벨업. 레벨업 기준에 따라 작성할 것(현재는 레벨에 해당하는 숫자만큼 넣어둠)
        if(monsterkill == 2)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 3)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 4)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 5)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 6)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 7)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 8)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 9)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 10)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 11)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 12)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 13)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 14)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 15)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 16)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 17)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 18)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 19)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 20)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 21)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 22)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 23)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 24)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 25)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 26)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 27)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 28)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 29)
        {
            playerstat.LevelUp();
        }
        else if(monsterkill == 30)
        {
            playerstat.LevelUp();
        }
    }
}
