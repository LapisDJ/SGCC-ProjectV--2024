using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] GameObject LayoverUI;
    [SerializeField] GameObject SkillChoose;
    [SerializeField] GameObject MissionUI;
    [SerializeField] GameObject ExitSceneUI;
    [SerializeField] GameObject pausemenu;
    [SerializeField] SkillManager skillmanager;
    [SerializeField] GameObject StartUI;
    private bool ispause = false;
    public static bool isskillchoose = false;
    void Awake()
    {
        Time.timeScale = 0f;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        //레이오버를 제외한 모든 UI씬 비활성화
        LayoverUI.SetActive(true);
        SkillChoose.SetActive(false);
        MissionUI.SetActive(false);
        ExitSceneUI.SetActive(false);
        pausemenu.SetActive(false);
        StartUI.SetActive(true);
        isskillchoose = false;
        ispause = false;
        //스킬쿨
        foreach (Image weapon in weaponImages)
        {
            weapon.enabled = false;
        }
        foreach (Image hide in hideWeaponImages)
        {
            hide.enabled = false;
        }
    }
    public void SkillChooseStart()//진행중인 게임을 일시정지하고 레벨업할 스킬을 선택하는 창으로 진입.
    {
        Time.timeScale = 0;
        skillmanager.LevelUp();
        SkillChoose.SetActive(true);
    }
    public int tempskillchoice = 9;
    //몇번째 버튼인지에 따라 임시값을 설정하고 확인버튼을 눌렀을 때 skillchoice변수를 변형한다.
    public void Firstskillchooosebutton()
    {
        tempskillchoice = 0;
    }
    public void Secondskillchooosebutton()
    {
        tempskillchoice = 1;
    }
    public void Thirdskillchooosebutton()
    {
        tempskillchoice = 2;
    }
    public void Fourthskillchooosebutton()
    {
        tempskillchoice = 3;
    }
    public void Confirmskillchoosebutton()
    {
        SkillManager.skillchoice = tempskillchoice;
        SkillChoose.SetActive(false);
        Time.timeScale = 1.0f;
        SkillManager.SkillLevelUP();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ispause = !ispause;
            if(ispause)
            {
                Callmenu();
            }
            else
            {
                Closemenu();
            }
        }
        if(isskillchoose)
        {
            skillchoiceimages[0].sprite = SkillManager.skillchoices[0].icon;
            skillchoiceimages[1].sprite = SkillManager.skillchoices[0].icon;
            skillchoiceimages[2].sprite = SkillManager.passivechoices[0].icon;
            skillchoiceimages[3].sprite = SkillManager.passivechoices[0].icon;
            SkillChooseStart();
            isskillchoose = false;
        }
        //스킬쿨
        WeaponCoolChk();
        UpdateWeaponImages();
    }
    public void Callmenu()
    {
        Time.timeScale = 0f;
        pausemenu.SetActive(true);
    }
    public void Closemenu()
    {
        Time.timeScale = 1f;
        pausemenu.SetActive(false);
    }
    public void PauseResume()
    {
        ispause = false;
        Closemenu();
    }
    public void PauseMainmenu()
    {
        //게임 완성 후 게임 요소 destroy하는 함수 추가 필요
        LoadingSceneController.Loadscene("Main Menu");
    }
    public void PauseQuit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    public void StartConfirmButton()
    {
        Time.timeScale = 1f;
        StartUI.SetActive(false);
    }
    //스킬쿨 UI
    public Image[] weaponImages;
    public Image[] hideWeaponImages;
    public static bool[] isCool = { false, false, false, false };
    public static float[] weaponTimes = { 3, 6, 9, 12 }; // 추후에 바뀌어야 함(쿨타임 받아오기)
    public static float[] getWeaponTimes = { 0, 0, 0, 0 };

    private Coroutine[] coolTimeCoroutines = new Coroutine[4];

    private void UpdateWeaponImages()
    {
        for (int i = 0; i < weaponImages.Length; i++)
        {
            bool isActive = i < SkillManager.activeSkills.Count;
            weaponImages[i].enabled = isActive;
            weaponImages[i].sprite = SkillManager.activeSkills[i].icon;
            hideWeaponImages[i].enabled = isActive;
            hideWeaponImages[i].sprite = SkillManager.activeSkills[i].icon;
        }
    }

    void WeaponCoolChk()
    {
        for (int i = 0; i < isCool.Length; i++)
        {
            if (isCool[i] && coolTimeCoroutines[i] == null)
            {
                coolTimeCoroutines[i] = StartCoroutine(CoolTimeChk(i));
            }
        }
    }

    public static void WeaponCoolSetting(int weaponNum)
    {
        getWeaponTimes[weaponNum] = weaponTimes[weaponNum];
        isCool[weaponNum] = true;
    }

    IEnumerator CoolTimeChk(int weaponNum)
    {
        while (getWeaponTimes[weaponNum] > 0)
        {
            getWeaponTimes[weaponNum] -= Time.deltaTime;
            float time = Mathf.Clamp01(getWeaponTimes[weaponNum] / weaponTimes[weaponNum]);
            hideWeaponImages[weaponNum].fillAmount = time;
            yield return null;
        }

        isCool[weaponNum] = false;
        hideWeaponImages[weaponNum].fillAmount = 0f;
        coolTimeCoroutines[weaponNum] = null;
    }
    //스킬레벨업 UI
    public Image[] skillchoiceimages;
}
