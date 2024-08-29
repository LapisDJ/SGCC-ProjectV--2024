using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
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
}
