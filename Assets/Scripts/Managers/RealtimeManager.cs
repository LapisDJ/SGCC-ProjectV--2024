using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RealtimeManager : MonoBehaviour
{
    public static RealtimeManager instance;

    public int monsterkill; // 처치한 몬스터의 수
    public int needKillsToLevelUp = 1;

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

    public void Monsterkill()
    //몬스터가 destroy처리될 때 불러올 함수. 처치한 몬스터 개수를 구하고, 그에따라 플레이어를 레벨업함.
    {
        monsterkill++;
        //처치한 몬스터 수에 따라 레벨업. 레벨업 기준에 따라 작성할 것(현재는 레벨에 해당하는 숫자만큼 넣어둠)

        if (monsterkill >= needKillsToLevelUp)
        {
            Player_Stat.instance.LevelUp();
            monsterkill -= needKillsToLevelUp;
            updateNeedKillsToLevelUp();
        }
        
    }

    private void updateNeedKillsToLevelUp()
    {
        if(needKillsToLevelUp == 1)
        {
            needKillsToLevelUp = 3;
        }
        else if(needKillsToLevelUp ==3)
        {
            needKillsToLevelUp = 5;
        }
        else if(needKillsToLevelUp ==5)
        {
            needKillsToLevelUp = 10;
        }
        else if(needKillsToLevelUp ==10)
        {
            needKillsToLevelUp = 15;
        }
        else if(needKillsToLevelUp ==15)
        {
            needKillsToLevelUp = 20;
        }
        else{
            needKillsToLevelUp += 10;
        }
    }
}
