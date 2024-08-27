using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

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
    void FixedUpdate()
    {
        if(isskillchoose)
        {
            SkillChooseStart();
            isskillchoose = false;
        }
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
}
