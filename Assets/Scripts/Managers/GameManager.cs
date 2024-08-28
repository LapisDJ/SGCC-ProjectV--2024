using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    void Start()
    {
        QuestManager questManager = QuestManager.instance;
        RealtimeManager realtimeManager = RealtimeManager.instance;
        SkillManager skillManager = SkillManager.instance;
        SpawnManager spawnManager = SpawnManager.instance;
    }

    void Update()
    {

    }
}
