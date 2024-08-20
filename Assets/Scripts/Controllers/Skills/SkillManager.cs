using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    //스킬레벨업, 스킬 리스트 초기화
    List <Skill> activeskills = new List<Skill>();
    List <PassiveSkill> passiveskills = new List< PassiveSkill>();
    void Start()
    {
        //액티브 스킬 리스트 초기화
        BaseballBat baseballbat = gameObject.AddComponent<BaseballBat>();
        activeskills.Add(baseballbat);

        Bazooka bazooka = gameObject.AddComponent<Bazooka>();
        activeskills.Add(bazooka);

        CircleSword circlesword = gameObject.AddComponent<CircleSword>();
        activeskills.Add(circlesword);

        ElectronicField electronicfield = gameObject.AddComponent<ElectronicField>();
        activeskills.Add(electronicfield);

        Pistol pistol = gameObject.AddComponent<Pistol>();
        activeskills.Add(pistol);

        RotateKnifeDrone rotateknifedrone = gameObject.AddComponent<RotateKnifeDrone>();
        activeskills.Add(rotateknifedrone);

        ShotGun shotgun = gameObject.AddComponent<ShotGun>();
        activeskills.Add(shotgun);
        
        //패시브 스킬 리스트 초기화
        Adrenaline adrenaline = gameObject.AddComponent<Adrenaline>();
        passiveskills.Add(adrenaline);

        CalculateHelper calculatehelper = gameObject.AddComponent<CalculateHelper>();
        passiveskills.Add(calculatehelper);

        Coin coin = gameObject.AddComponent<Coin>();
        passiveskills.Add(coin);

        FatalVirus fatalvirus = gameObject.AddComponent<FatalVirus>();
        passiveskills.Add(fatalvirus);

        LearnPill learnpill = gameObject.AddComponent<LearnPill>();
        passiveskills.Add(learnpill);

        PoweredSkin poweredskin = gameObject.AddComponent<PoweredSkin>();
        passiveskills.Add(poweredskin);

        ProstheticHand prosthetichand = gameObject.AddComponent<ProstheticHand>();
        passiveskills.Add(prosthetichand);

        RocketBoots rocketboots = gameObject.AddComponent<RocketBoots>();
        passiveskills.Add(rocketboots);

        TeleportDevice teleportdevice = gameObject.AddComponent<TeleportDevice>();
        passiveskills.Add(teleportdevice);
    }

    public void LevelUpSkill(string skillName)
    {
        // 스킬 리스트에서 스킬 이름으로 스킬을 찾아 레벨업
        Skill skill = activeskills.Find(s => s.skillName == skillName);
        if (skill != null)
        {
            skill.LevelUp();
        }
        else
        {
            Debug.LogWarning("Skill not found: " + skillName);
        }
    }

    void Update()
    {
        
    }
}
