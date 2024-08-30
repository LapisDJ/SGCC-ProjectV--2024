using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RealtimeManager : MonoBehaviour
{
    public static RealtimeManager instance;

    [SerializeField] Player_Stat playerstat;
    public int monsterkill; // 처치한 몬스터의 수

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

        if (monsterkill / 10 >= 1)
        {
            playerstat.LevelUp();
            monsterkill %= 10;
        }
        
    }
}
