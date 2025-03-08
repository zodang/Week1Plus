using System;
using UnityEngine;
using EnumTypes;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public GameState currentGameState;
    public static GameManager Instance;
    public PlayerController Player;
    public CameraController Camera;
    public UIManager UIManager;
    public SpawnManager SpawnManager;

    

    public int totalScoreCount = 0;
    
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        currentGameState = GameState.Play;
        UIManager.ChangeUI(currentGameState);
    }
    
    public void IncreaseScore(int score)
    {
        totalScoreCount += score;
        UIManager.UpdateScore(totalScoreCount);
    }
    
    
}
