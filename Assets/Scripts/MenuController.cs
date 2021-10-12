using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    public int depth = 0;
    public Button play, rules, quit, easy, medium, hard, hidePlay, hideRules;
    public GameObject playPanel, rulesPanel;
    public static MenuController instance;
    public Slider debugSlider;
    public bool debugMode = false;

    public void AssignUI()
    {
        if (SceneManager.GetActiveScene().name != "menu")
            return;
        play = GameObject.Find("PlayButton").GetComponent<Button>();
        rules = GameObject.Find("RulesButton").GetComponent<Button>();
        quit = GameObject.Find("QuitButton").GetComponent<Button>();
        easy = GameObject.Find("EasyButton").GetComponent<Button>();
        medium = GameObject.Find("MediumButton").GetComponent<Button>();
        hard = GameObject.Find("HardButton").GetComponent<Button>();
        hidePlay = GameObject.Find("HidePlay").GetComponent<Button>();
        hideRules = GameObject.Find("HideRules").GetComponent<Button>();
        playPanel = GameObject.Find("PlayPanel");
        rulesPanel = GameObject.Find("RulesPanel");

        debugSlider = GameObject.Find("DebugModeSlider").GetComponent<Slider>();

        play.onClick.AddListener(() => Play());
        rules.onClick.AddListener(()=>Rules());
        quit.onClick.AddListener(()=>Quit());

        easy.onClick.AddListener(() => Easy());
        medium.onClick.AddListener(()=>Medium());
        hard.onClick.AddListener(()=>Hard());

        hidePlay.onClick.AddListener(()=>HidePlayPanel());
        hideRules.onClick.AddListener(()=>HideRulesPanel());

        playPanel.SetActive(false);
        rulesPanel.SetActive(false);
    }

    void Awake()
    {
        
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        AssignUI();
    }

    void Update()
    {
        if (Input.GetKey("escape") && SceneManager.GetActiveScene().name =="menu")
        {
            if (playPanel.activeSelf)
                playPanel.SetActive(false);

            else if (rulesPanel.activeSelf)
                rulesPanel.SetActive(false);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignUI();
    }

    public void Play()
    {
        if (debugSlider.value == 1)
            debugMode = true;
        else debugMode = false;
        playPanel.SetActive(true);

    }

    public void Rules()
    {
        rulesPanel.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Easy()
    {
        depth = 1;
        SceneManager.LoadScene("test");
        this.gameObject.SetActive(true);
    }

    public void Medium()
    {
        depth = 2;
        SceneManager.LoadScene("test");
        this.gameObject.SetActive(true);
    }

    public void Hard()
    {
        depth = 3;
        SceneManager.LoadScene("test");
        this.gameObject.SetActive(true);
    }
    public void HidePlayPanel()
    {
        playPanel.SetActive(false);
    }

    public void HideRulesPanel()
    {
        rulesPanel.SetActive(false);
    }

    public void Disable()
    {
        this.gameObject.SetActive(false);
    }

    public void Enable()
    {
        this.gameObject.SetActive(true);
    }
}
