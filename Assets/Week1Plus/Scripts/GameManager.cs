using System.Collections;
using UnityEngine;
using EnumTypes;

public class GameManager : MonoBehaviour
{
    public GameState currentGameState;
    public static GameManager Instance;
    public PlayerController Player;
    public CameraController Camera;
    public UIManager UIManager;
    public SpawnManager SpawnManager;

    public int Stage = 0;

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

    private void Start()
    {
        currentGameState = GameState.Play;
        UIManager.ChangeUI(currentGameState);
        StartCoroutine(StartCount(1));
        StartCoroutine(SpawnEnemy(10));
    }
    
    public void IncreaseScore(int score)
    {
        totalScoreCount += score;
        UIManager.UpdateScore(totalScoreCount);
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

    private IEnumerator StartCount(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        SpawnManager.SpawnBossEnemy(0);
        for (var i = SpawnManager.SpawnedEnemyList.Count - 1; i >= 0; i--)
        {
            SpawnManager.SpawnedEnemyList[i].KillEnemy(false);
        }
    }
    
}
