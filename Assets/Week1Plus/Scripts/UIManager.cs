using System;
using System.Collections;
using TMPro;
using UnityEngine;
using EnumTypes;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameManager _gameManager;
    
    [Header("Start Panel")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private Button startBtn;
    [SerializeField] private Button exitBtn;
    
    [Header("Choose Panel")]
    [SerializeField] private GameObject choosePanel;
    [SerializeField] private Button dashBossBtn;
    [SerializeField] private Button laserBossBtn;
    [SerializeField] private Button blackHoleBossBtn;
    [SerializeField] private Button goToStartUIBtn;
    
    [Header("Play Panel")]
    [SerializeField] private GameObject playPanel;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private TMP_Text ultimateCount;
    [SerializeField] private Slider ultimateSlider;
    [SerializeField] private TMP_Text healthCount;
    [SerializeField] private Slider healthSlider;
    
    [Header("Score Panel")]
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private TMP_Text totalScoreText;
    [SerializeField] private Button goToChooseUIBtn;
    
    public void ChangeUI(GameState state)
    {
        ActivateAllPanel(false);
        
        switch (state)
        {
            case GameState.Start:
                startPanel.SetActive(true);
                break;
            case GameState.Choose:
                choosePanel.SetActive(true);
                break;
            case GameState.Play:
                playPanel.SetActive(true);
                break;
            case GameState.Score:
                scorePanel.SetActive(true);
                UpdateTotalScore(_gameManager.TotalScroeCount);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;

        AddListener();
    }

    public void ResetPlayerSettingUI()
    {
        timerSlider.maxValue = _gameManager.LeftTime;
        ultimateSlider.maxValue = 20;
        healthSlider.maxValue = 20;
        
        UpdateScore(_gameManager.TotalScroeCount);
        UpdateHealth(_gameManager.Player.HealthTotalCount);
        UpdateUltimate(_gameManager.Player.playerAttack.UltimateTotalCount);
    }
    
    private void ActivateAllPanel(bool isActive)
    {
        startPanel.SetActive(isActive);
        choosePanel.SetActive(isActive);
        playPanel.SetActive(isActive);
        scorePanel.SetActive(isActive);
    }

    private void AddListener()
    {
        startBtn.onClick.AddListener(OnClickStartBtn);
        
        dashBossBtn.onClick.AddListener(OnClickDashBossBtn);
        laserBossBtn.onClick.AddListener(OnClickLaserBossBtn);
        blackHoleBossBtn.onClick.AddListener(OnClickBlackHoleBossBtn);
        goToStartUIBtn.onClick.AddListener(OnClickGoToStartUIBtn);
        
        goToChooseUIBtn.onClick.AddListener(OnClickGoToChooseUIBtn);
    }

    #region StartPanel

    private void OnClickStartBtn()
    {
        _gameManager.ChangeGameState(GameState.Choose);
        _gameManager.ResetPlayerSetting();
    }

    #endregion
    
    #region ChoosePanel

    private void OnClickDashBossBtn()
    {
        _gameManager.StartPlayGame(0);
    }

    private void OnClickLaserBossBtn()
    {
        _gameManager.StartPlayGame(1);
    }
    
    private void OnClickBlackHoleBossBtn()
    {
        _gameManager.StartPlayGame(2);
    }
    
    private void OnClickGoToStartUIBtn()
    {
        _gameManager.ChangeGameState(GameState.Start);
    }
    
    #endregion

    #region PlayPanel
    public void UpdateScore(int score)
    {
        scoreText.text = $"{score}";
    }
    
    public void UpdateTimerText(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = time <= 0 ? "00:00" : string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateTimerSlider(float time)
    {
        timerSlider.value = time;
    }
    
    public void UpdateUltimate(int ultimate)
    {
        ultimateCount.text = $"Ult {ultimate}"; 
        ultimateSlider.value = ultimate;
    }

    public void UpdateHealth(int health)
    {
        healthCount.text = $"Hp {health}"; 
        healthSlider.value = health;
    }
    
    #endregion
    
    #region ScorePanel

    private void UpdateTotalScore(int totalScore)
    {
        StartCoroutine(AnimateScore(totalScore, 1));
    }
    
    private IEnumerator AnimateScore(int endScore, float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            int currentScore = Mathf.RoundToInt(Mathf.Lerp(0, endScore, elapsedTime / time));
            totalScoreText.text = currentScore.ToString();
            yield return null;
        }
        totalScoreText.text = endScore.ToString();
    }
    
    private void OnClickGoToChooseUIBtn()
    {
        _gameManager.ResetPlayerSetting();
        _gameManager.ChangeGameState(GameState.Choose);
    }
    
    
    
    #endregion
    
    
}
