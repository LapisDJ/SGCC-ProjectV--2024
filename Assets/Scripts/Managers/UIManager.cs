using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] GameObject LayoverUI;
    [SerializeField] GameObject SkillChoose;
    [SerializeField] GameObject MissionUI;
    [SerializeField] GameObject ExitSceneUI;
    [SerializeField] GameObject pausemenu;
    [SerializeField] SkillManager skillmanager;
    [SerializeField] GameObject StartUI;
    [SerializeField] TextMeshProUGUI[] skillnames;
    [SerializeField] Player_Stat playerstat;
    [SerializeField] Slider hpbar;

    private bool ispause = false;
    public bool isskillchoose = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Time.timeScale = 0f;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
        recheckwindow.SetActive(false);
        isskillchoose = false;
        ispause = false;
        initialtime = Time.time;
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
    //메인메뉴
    public void OnClickNewgame()
    {
        SceneManager.LoadScene("Character choose");
    }
    public void OnClickBringup()
    {
        SceneManager.LoadScene("?");
    }
    public void OnClickSettings()
    {
        SceneManager.LoadScene("Settings");
    }
    public void OnClickQuit()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    //캐릭터 선택창
    int characternum;
    public void FirstCharacterButton()
    {
        characternum = 1;
    }
    public void Confirmbutton()
    {
        LoadingSceneController.Loadscene("Map1");
    }
    public void Previousbutton()
    {
        SceneManager.LoadScene("Main Menu");
    }
    public void SkillChooseStart()//진행중인 게임을 일시정지하고 레벨업 가능한 스킬 리스트 가져옴. 레벨업할 스킬을 선택하는 창으로 진입.
    {
        Time.timeScale = 0;
        SkillManager.instance.LevelUp();
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
        SkillManager.instance.skillchoice = tempskillchoice;
        SkillChoose.SetActive(false);
        Time.timeScale = 1.0f;
        SkillManager.instance.SkillLevelUP();
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
            SkillChooseStart();//timescale 0으로, skillmanager에서 레벨업 가능한 리스트 뽑아오기, 캔버스 활성화.
            skillchoiceimages[0].sprite = SkillManager.instance.skillchoices[0].icon;
            skillnames[0].text = SkillManager.instance.skillchoices[0].skillName;
            skillchoiceimages[1].sprite = SkillManager.instance.skillchoices[1].icon;
            skillnames[1].text = SkillManager.instance.skillchoices[1].skillName;
            skillchoiceimages[2].sprite = SkillManager.instance.passivechoices[0].icon;
            skillnames[2].text = SkillManager.instance.passivechoices[0].skillName;
            skillchoiceimages[3].sprite = SkillManager.instance.passivechoices[1].icon;
            skillnames[3].text = SkillManager.instance.passivechoices[1].skillName;
            isskillchoose = false;
        }
        hpbar.value = playerstat.HPcurrent / playerstat.HPmax;//HP바 표시
        //스킬쿨
        WeaponCoolChk();
        UpdateWeaponImages();
        //능력쿨
        if (Input.GetKeyDown(KeyCode.Space)) // 테스트용 키코드. 스페이스를 누르면 능력쿨타임이 돌아간다.
        {
            AbilityCoolSetting();
        }
        if(isAbilityCool)
        {
            AbilityCoolChk();
        }
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
    public void StartConfirmButton()//게임시작시 확인버튼 누르면 시작하도록.
    {
        Time.timeScale = 1f;
        StartUI.SetActive(false);
    }
    //스킬쿨 UI
    public Image[] weaponImages;
    public Image[] hideWeaponImages;
    public bool[] isCool = { false, false, false, false };
    public float[] weaponTimes = { 3, 6, 9, 12 }; // 추후에 바뀌어야 함(쿨타임 받아오기)
    public float[] getWeaponTimes = { 0, 0, 0, 0 };

    private Coroutine[] coolTimeCoroutines = new Coroutine[4];

    private void UpdateWeaponImages()
    {
        for (int i = 0; i < 4; i++)
        {
            bool isActive = i < SkillManager.instance.activeSkills.Count;
            if(isActive)
            {
                weaponImages[i].sprite = SkillManager.instance.activeSkills[i].icon;
                weaponImages[i].enabled = isActive;
                hideWeaponImages[i].sprite = SkillManager.instance.activeSkills[i].icon;
                hideWeaponImages[i].enabled = isActive;
                weaponTimes[i] = SkillManager.instance.activeSkills[i].cooldown;
            }
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

    public void WeaponCoolSetting(int weaponNum)
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

    //정산창
    [SerializeField] GameObject recheckwindow;
    public void GotoMainmenuButton()
    {
        recheckwindow.SetActive(true);
    }
    public void Recheckyes()
    {
        LoadingSceneController.Loadscene("Main Menu");
    }
    public void Recheckno()
    {
        recheckwindow.SetActive(false);
    }
    public void Retry()
    {
        QuestManager.instance.currentQuest--;
        switch(QuestManager.instance.currentQuest)
        {
            case 1 : 
                LoadingSceneController.Loadscene("Map1");
                break;
            case 2 : 
                LoadingSceneController.Loadscene("Map1");
                break;
            case 3 : 
                LoadingSceneController.Loadscene("Map1");
                break;
            default:
                break;
        }
    }
    public void Nextchapter()
    {
        switch(QuestManager.instance.currentQuest)
        {
            case 1 : 
                LoadingSceneController.Loadscene("Map1");
                break;
            case 2 : 
                LoadingSceneController.Loadscene("Map1");
                break;
            case 3 : 
                LoadingSceneController.Loadscene("Map1");
                break;
            default:
                break;
        }
    }
    //타이머
    [SerializeField] TextMeshProUGUI timer;
    public static string time;
    private int wholetime;
    private int minute;
    private int second;
    private float initialtime;

    void FixedUpdate()
    {
        wholetime = Convert.ToInt16(Time.time - initialtime);
        minute = wholetime/60;
        second = wholetime%60;
        if(second < 10)
        {
            if(minute < 10)
            {
                time = '0' + Convert.ToString(minute) + " : " + '0' + Convert.ToString(second);
            }
            else
            {
                time = Convert.ToString(minute) + " : " + '0' + Convert.ToString(second);
            }
        }
        else
        {
            if(minute < 10)
            {
                time = '0' + Convert.ToString(minute) + " : " + Convert.ToString(second);
            }
            else
            {
                time = Convert.ToString(minute) + " : " + Convert.ToString(second);
            }
        }
        timer.text = time;
    }
    //
    public Image abilityImage;
    public Image hideAbilityImage;
    public static bool isAbilityCool = false; // 능력 쿨타임 여부
    public static float abilityTime = 5f; // 능력 쿨타임
    public static float getAbilityTime = 0f; // 능력 사용 후 시간 체크

    private Coroutine coolTimeCoroutine;

    private void AbilityCoolChk()
    {
        if (isAbilityCool && coolTimeCoroutine == null)
        {
            coolTimeCoroutine = StartCoroutine(CoolTimeChk());
        }
    }

    public static void AbilityCoolSetting()
    {
        getAbilityTime = abilityTime;
        isAbilityCool = true;
    }

    IEnumerator CoolTimeChk() // 능력 쿨타임 잔여량만큼 불투명한 상자로 가리는 함수. 쿨타임이 0 이하일 시 isAbilityCool도 false로 바꿔줌.
    {
        while (getAbilityTime > 0)
        {
            getAbilityTime -= Time.deltaTime;
            float time = Mathf.Clamp01(getAbilityTime / abilityTime);
            hideAbilityImage.fillAmount = time;
            yield return null;
        }

        // 쿨타임 종료 처리
        isAbilityCool = false;
        hideAbilityImage.fillAmount = 0f;
        coolTimeCoroutine = null;
    }
}
