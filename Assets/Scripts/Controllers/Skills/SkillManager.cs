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
        List<Skill> skillChoices = GetRandomSkillChoices(4); // 레벨업 ui에 선택될 스킬들

        ShowLevelUpUI(skillChoices); // 로직에 따라 선택된 2개의 액티브, 2개의 패시브 스킬이 레벨업 가능 스킬로 표시됨
    }

    private List<Skill> GetRandomSkillChoices(int count) // 로직에 따라 스킬 4개(액티브2, 패시브2) 반환
    {
 
    }

    public void AssignActiveSkill(Skill skill, KeyCode key)
    {
 
    }

    public void AddPassiveSkill(PassiveSkill skill)
    {

    }

    void ShowLevelUpUI(List<Skill> skillChoices)
    {

    }
}
