using System.Collections;
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

    public int CurrentBossIndex = -1;

    public int totalScoreCount = 0;
    
    public bool isBossSpawned = false;
    
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void IncreaseScore(int score)
    {
        totalScoreCount += score;
        UIManager.UpdateScore(totalScoreCount);
    }

    public void ChangeGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        UIManager.ChangeUI(currentGameState);
    }
    public void StartPlayGame(int bossIndex)
    {
        CurrentBossIndex = bossIndex;
        currentGameState = GameState.Play;
        UIManager.ChangeUI(currentGameState);
        StartCoroutine(StartCount(5, bossIndex));
        StartCoroutine(SpawnEnemy(10));
    }

    private IEnumerator SpawnEnemy(int count)
    {
        var spawndCount = 0;
        while (spawndCount <= count && !isBossSpawned)
        {
            yield return new WaitForSeconds(0.5f);
            SpawnManager.SpawnEnemy();
            spawndCount++;
        }
    }

    private IEnumerator StartCount(int seconds, int bossIndex)
    {
        yield return new WaitForSeconds(seconds);
        
        SpawnManager.SpawnBossEnemy(bossIndex);
        for (var i = SpawnManager.SpawnedEnemyList.Count - 1; i >= 0; i--)
        {
            SpawnManager.SpawnedEnemyList[i].KillEnemy(false);
        }
    }
    
}
