using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public int maxActiveSkills = 4; //최대 액티브 개수
    public int maxPassiveSkills = 6; //최대 패시브 개수
    public static bool trigger = false;

    private List<Skill> availableActiveSkills = new List<Skill>(); // 모든 액티브 스킬 리스트
    private List<PassiveSkill> availablePassiveSkills = new List<PassiveSkill>(); // 모든 패시브 스킬 리스트

    public static List<Skill> activeSkills = new List<Skill>(); // 현재 활성화된 액티브 스킬
    public static List<PassiveSkill> passiveSkills = new List<PassiveSkill>(); // 현재 활성화된 패시브 스킬
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start() // 모든 액티브, 패시브 스킬 초기화 후 담기
    {
        // 액티브 스킬 초기화
        availableActiveSkills.Add(new BaseballBat());
        availableActiveSkills.Add(new Bazooka());
        availableActiveSkills.Add(new CircleSword());
        availableActiveSkills.Add(new ElectronicField());
        availableActiveSkills.Add(new Pistol());
        availableActiveSkills.Add(new RotateKnifeDrone());
        availableActiveSkills.Add(new ShotGun());

        // 패시브 스킬 초기화
        availablePassiveSkills.Add(new Adrenaline());
        availablePassiveSkills.Add(new CalculateHelper());
        availablePassiveSkills.Add(new Coin());
        availablePassiveSkills.Add(new FatalVirus());
        availablePassiveSkills.Add(new LearnPill());
        availablePassiveSkills.Add(new PoweredSkin());
        availablePassiveSkills.Add(new ProstheticHand());
        availablePassiveSkills.Add(new RocketBoots());
        availablePassiveSkills.Add(new TeleportDevice());
    }
    public static List<Skill> skillchoices;
    public static List<PassiveSkill> passivechoices;
    public void LevelUp()
    {
        skillchoices = GetRandomSkillChoices(); // 레벨업 ui에 선택될 액티브 스킬들
        passivechoices = GetRandomPassiveChoices(); // 레벨업 ui에 선택될 패시브 스킬들
    }
    void Update()
    {
        if(trigger)
        {
            SkillLevelUP();
            trigger = false;
        }
    }

    private List<Skill> GetRandomSkillChoices() // 로직에 따라 액티브 스킬 2개 반환
    {
        var retList = new List<Skill>();
        if(activeSkills.Count >= maxActiveSkills)//액티브 스킬 가짓수를 모두 채우면 가지고 있는 액티브 중에서 가져옴
        {
            int rand1 = UnityEngine.Random.Range(0,maxActiveSkills);
            int rand2 = UnityEngine.Random.Range(0,maxActiveSkills);
            while(rand1 == rand2)
            {
                rand2 = UnityEngine.Random.Range(0,maxActiveSkills);
            }
            retList.Add(activeSkills[rand1]);
            retList.Add(activeSkills[rand2]);
        }
        else//액티브 스킬 가짓수를 모두 채우지 않았으면 모든 리스트에서 가져옴
        {
            int rand1 = UnityEngine.Random.Range(0,7);
            int rand2 = UnityEngine.Random.Range(0,7);
            while(rand1 == rand2)
            {
                rand2 = UnityEngine.Random.Range(0,7);
            }
            retList.Add(availableActiveSkills[rand1]);
            retList.Add(availableActiveSkills[rand2]);
        }
        return retList;
    }
    private List<PassiveSkill> GetRandomPassiveChoices() // 로직에 따라 패시브 스킬 2개 반환
    {
        //액티브와 방식은 같다.
        var retList = new List<PassiveSkill>();
        if(passiveSkills.Count >= maxPassiveSkills)
        {
            int rand1 = UnityEngine.Random.Range(0,maxPassiveSkills);
            int rand2 = UnityEngine.Random.Range(0,maxPassiveSkills);
            while(rand1 == rand2)
            {
                rand2 = UnityEngine.Random.Range(0,maxPassiveSkills);
            }
            retList.Add(passiveSkills[rand1]);
            retList.Add(passiveSkills[rand2]);
        }
        else
        {
            int rand1 = UnityEngine.Random.Range(0,9);
            int rand2 = UnityEngine.Random.Range(0,9);
            while(rand1 != rand2)
            {
                rand2 = UnityEngine.Random.Range(0,9);
            }
            retList.Add(availablePassiveSkills[rand1]);
            retList.Add(availablePassiveSkills[rand2]);
        }
        return retList;
    }
    public static int skillchoice;//ui 버튼으로 입력받아서 int형식으로 저장
    public static void SkillLevelUP()
    {
        //0레벨이면 리스트에 등록, 1레벨 이상이면 레벨업만
        if(skillchoice < 2)
        {
            if (skillchoices[skillchoice].level == 0)
            {
                activeSkills.Add(skillchoices[skillchoice]);
                skillchoices[skillchoice].LevelUp();
            }
            else
            {
                skillchoices[skillchoice].LevelUp();
            }
        }
        else
        {
            if (passivechoices[skillchoice-2].level == 0)
            {
                passiveSkills.Add(passivechoices[skillchoice-2]);
                passivechoices[skillchoice-2].LevelUp();
            }
            else
            {
                passivechoices[skillchoice-2].LevelUp();
            }
        }
    }
}