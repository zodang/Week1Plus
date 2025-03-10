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

    [Header("Boss")]
    public int CurrentBossIndex = -1;
    public bool isBossSpawned = false;

    public int TotalScroeCount = 0; 
    private int _TotalScoreCount => TotalScroeCount;
    
    [SerializeField] private int initialHealthCount = 5;
    [SerializeField] private int initialUltimateCount = 5;

    private bool _isCheckLeftTime = false;
    private bool _isCheckElapsedTime = false;
    
    public float LeftTime = 15;
    private float _currentLeftTime;
    private float _elapsedTime;
    [SerializeField] private int basicEnemyCount = 20;
   
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
        StartGame();
    }

    private void Update()
    {
        if (_isCheckLeftTime)
        {
            StartCoroutine(StartLeftTimerCo());
            _isCheckLeftTime = false;
        }

        if (_isCheckElapsedTime)
        {
            StartCoroutine(StartElapsedTimerCo());
            _isCheckElapsedTime = false;
        }
    }

    public void StartLeftTimer()
    {
        _currentLeftTime = LeftTime + 1;
        _isCheckLeftTime = true;
    }

    public void StartElapsedTimer()
    {
        _elapsedTime = 0;
        _isCheckElapsedTime = true;
    }

    private IEnumerator StartLeftTimerCo()
    {
        while (_currentLeftTime > 0)
        {
            _currentLeftTime -= Time.deltaTime;
            UIManager.UpdateTimerText(_currentLeftTime);
            UIManager.UpdateTimerSlider(_currentLeftTime);
            yield return null;
        }
    }
    
    private IEnumerator StartElapsedTimerCo()
    {
        while (Player.IsBossState)
        {
            _elapsedTime += Time.deltaTime;
            UIManager.UpdateTimerText(_elapsedTime);
            yield return null;
        }
    }

    private void StartGame()
    {
        ResetPlayerSetting();
        ChangeGameState(GameState.Start);
    }

    public void IncreaseScore(int score)
    {
        TotalScroeCount += score;
        UIManager.UpdateScore(_TotalScoreCount);
    }

    public void ChangeGameState(GameState newGameState)
    {
        currentGameState = newGameState;

        switch (currentGameState)
        {
            case GameState.None:
                break;
            case GameState.Start:
                Player.StartPlayerMove(true);
                Player.playerAttack.StartPlayerShoot(true);
                break;
            case GameState.Choose:
                Player.StartPlayerMove(false);
                Player.playerAttack.StartPlayerShoot(false);
                break;
            case GameState.Play:
                Player.StartPlayerMove(true);
                Player.playerAttack.StartPlayerShoot(true);
                break;
            case GameState.Score:
                Player.StartPlayerMove(false);
                Player.playerAttack.StartPlayerShoot(false);
                if (SpawnManager.SpawnedBossEnemy)
                {
                    SpawnManager.SpawnedBossEnemy.StartDamage(false);
                }

                break;  
        }
        
        UIManager.ChangeUI(currentGameState);
    }
    public void StartPlayGame(int bossIndex)
    {
        CurrentBossIndex = bossIndex;
        ChangeGameState(GameState.Play);
        
        StartLeftTimer();
        StartCoroutine(SpawnBossEnemyCo(LeftTime, bossIndex));
        StartCoroutine(SpawnBasicEnemyCo());

        for (var i = 0; i < basicEnemyCount; i++)
        {
            SpawnManager.SpawnRandomEnemy();
        }
    }

    private IEnumerator SpawnBasicEnemyCo()
    {
        while (_currentLeftTime > 0)
        {
            SpawnManager.SpawnRandomEnemy();
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator SpawnBossEnemyCo(float seconds, int bossIndex)
    {
        yield return new WaitForSeconds(seconds);
        
        SpawnManager.SpawnBossEnemy(bossIndex);
        SpawnManager.DespawnAllEnemies();
    }

    public void ResetPlayerSetting()
    {
        SpawnManager.DespawnAllEnemies();
        SpawnManager.DespawnAllItems();
        SpawnManager.DespawnBossEnemy();
        
        TotalScroeCount = 0;
        Player.HealthTotalCount = initialHealthCount;
        Player.playerAttack.UltimateTotalCount = initialUltimateCount;

        Player.StartPlayerDamage(true);
        _currentLeftTime = LeftTime;
        _elapsedTime = 0;
        
        Camera.ChangeTarget(Player.gameObject);
        Player.PlayerNotifier.SetBossNotifier(false);
        
        UIManager.ResetPlayerSettingUI();
    }
    
}
