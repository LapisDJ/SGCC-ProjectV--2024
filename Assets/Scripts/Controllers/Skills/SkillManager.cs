using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public int maxActiveSkills = 4; //최대 액티브 개수
    public int maxPassiveSkills = 6; //최대 패시브 개수

    private List<Skill> availableActiveSkills = new List<Skill>(); // 모든 액티브 스킬 리스트
    private List<PassiveSkill> availablePassiveSkills = new List<PassiveSkill>(); // 모든 패시브 스킬 리스트

    private List<Skill> activeSkills = new List<Skill>(); // 현재 활성화된 액티브 스킬
    private List<PassiveSkill> passiveSkills = new List<PassiveSkill>(); // 현재 활성화된 패시브 스킬
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

    public void LevelUp()
    {
        List<Skill> skillChoices = GetRandomSkillChoices(2); // 레벨업 ui에 선택될 액티브 스킬들
        List<PassiveSkill> passiveChoices = GetRandomPassiveChoices(2); // 레벨업 ui에 선택될 패시브 스킬들
        ShowLevelUpUI(skillChoices,passiveChoices); // 로직에 따라 선택된 2개의 액티브, 2개의 패시브 스킬이 레벨업 가능 스킬로 표시됨
    }

    private List<Skill> GetRandomSkillChoices(int count) // 로직에 따라 액티브 스킬 2개 반환
    {
        var retList = new List<Skill>();
        if(activeSkills.Count >= maxActiveSkills)//액티브 스킬 가짓수를 모두 채우면 가지고 있는 액티브 중에서 가져옴
        {
            int rand1 = UnityEngine.Random.Range(0,maxActiveSkills);
            int rand2 = UnityEngine.Random.Range(0,maxActiveSkills);
            while(rand1 != rand2)
            {
                rand2 = UnityEngine.Random.Range(0,maxActiveSkills);
            }
            retList.Add(activeSkills[rand1]);
            retList.Add(activeSkills[rand2]);
        }
        else//액티브 스킬 가짓수를 모두 채우지 않았으면 모든 리스트에서 가져옴
        {
            int rand1 = UnityEngine.Random.Range(0,8);
            int rand2 = UnityEngine.Random.Range(0,8);
            while(rand1 != rand2)
            {
                rand2 = UnityEngine.Random.Range(0,8);
            }
            retList.Add(availableActiveSkills[rand1]);
            retList.Add(availableActiveSkills[rand2]);
        }
        return retList;
    }
    private List<PassiveSkill> GetRandomPassiveChoices(int count) // 로직에 따라 패시브 스킬 2개 반환
    {
        //액티브와 방식은 같다.
        var retList = new List<PassiveSkill>();
        if(activeSkills.Count >= maxPassiveSkills)
        {
            int rand1 = UnityEngine.Random.Range(0,maxPassiveSkills);
            int rand2 = UnityEngine.Random.Range(0,maxPassiveSkills);
            while(rand1 != rand2)
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
    public void AddActiveSkill(Skill skill)
    {
        activeSkills.Add(skill);
    }
    public void AddPassiveSkill(PassiveSkill skill)
    {
        passiveSkills.Add(skill);
    }
    public static int skillchoice;//ui 버튼으로 입력받아서 int형식으로 저장할 예정..
    public List<String> ShowLevelUpUI(List<Skill> skillChoices,List<PassiveSkill> passiveChoices)
    {
        var retlist = new List<String>();
        retlist.Add(skillChoices[0].name);
        retlist.Add(skillChoices[1].name);
        retlist.Add(passiveChoices[0].name);
        retlist.Add(passiveChoices[1].name);
        return retlist;
    }
    void SkillLevleUP(List<Skill> skillChoices,List<PassiveSkill> passiveChoices)
    {
        //0레벨이면 리스트에 등록, 1레벨 이상이면 레벨업만
        if(skillchoice < 2)
        {
            if (skillChoices[skillchoice].level == 0)
            {
                AddActiveSkill(skillChoices[skillchoice]);
                skillChoices[skillchoice].LevelUp();
            }
            else
            {
                skillChoices[skillchoice].LevelUp();
            }
        }
        else
        {
            if (passiveChoices[skillchoice-2].level == 0)
            {
                AddPassiveSkill(passiveChoices[skillchoice-2]);
                passiveChoices[skillchoice-2].LevelUp();
            }
            else
            {
                passiveChoices[skillchoice-2].LevelUp();
            }
        }
    }
}