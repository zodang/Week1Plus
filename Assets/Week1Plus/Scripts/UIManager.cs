using System;
using TMPro;
using UnityEngine;
using EnumTypes;

public class UIManager : MonoBehaviour
{
    private GameManager _gameManager;
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
                break;
            case GameState.ChooseBoss:
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
        
        UpdateScore(_gameManager.totalScoreCount);
        UpdateHealth(_gameManager.Player.HealthTotalCount);
        UpdateUltimate(_gameManager.Player.playerAttack.UltimateTotalCount);
    }

    private void ActivateAllPanel(bool isActive)
    {
        playPanel.SetActive(isActive);
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
