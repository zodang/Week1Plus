using System;
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
    [SerializeField] private Button LaserBossBtn;
    [SerializeField] private Button BlackHoleBossBtn;
    
    [Header("Play Panel")]
    [SerializeField] private GameObject playPanel;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text ultimateCount;
    [SerializeField] private TMP_Text healthCount;
    
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
            case GameState.End:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public void Start()
    {
        _gameManager = GameManager.Instance;

        AddListener();
        
        UpdateScore(_gameManager.totalScoreCount);
        UpdateHealth(_gameManager.Player.HealthTotalCount);
        UpdateUltimate(_gameManager.Player.playerAttack.UltimateTotalCount);
    }

    private void ActivateAllPanel(bool isActive)
    {
        startPanel.SetActive(isActive);
        choosePanel.SetActive(isActive);
        playPanel.SetActive(isActive);
    }

    private void AddListener()
    {
        startBtn.onClick.AddListener(OnClickStartBtn);
        
        dashBossBtn.onClick.AddListener(OnClickDashBossBtn);
    }

    private void OnClickStartBtn()
    {
        _gameManager.ChangeGameState(GameState.Choose);
    }

    private void OnClickDashBossBtn()
    {
        _gameManager.StartPlayGame(0);
    }




    public void UpdateScore(int score)
    {
        scoreText.text = $"{score}";
    }
    
    public void UpdateUltimate(int ultimate)
    {
        ultimateCount.text = $"Ultimate: {ultimate}"; 
    }

    public void UpdateHealth(int health)
    {
        healthCount.text = $"Health: {health}"; 
    }
    
    
}
