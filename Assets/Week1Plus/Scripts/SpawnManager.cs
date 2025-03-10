using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject EnemyGroup;
    public GameObject ItemGroup;

    [Header("Item")]
    [SerializeField] private Item[] Items;
    public List<Item> SpawnedItemList;
    
    [Header("Enemy")]
    [SerializeField] private EnemyController[] Enemies;
    public List<EnemyController> SpawnedEnemyList;
    [SerializeField] private Vector2 enemySpawnRange = new Vector2(7, 12); 
    
    [Header("Boss Enemy")]
    [SerializeField] private BossEnemy[] bossEnemies;
    public BossEnemy SpawnedBossEnemy;

    public void SpawnItem(Transform spawnPoint)
    {
        if (Random.value < 0.3f)
        {
            return;
        }
        
        var newItem = Instantiate(Items[Random.Range(0, Items.Length)], spawnPoint.position, Quaternion.identity, ItemGroup.transform);
        SpawnedItemList.Add(newItem);
    }

    public void RemoveSpawnedItem(Item item)
    {
        SpawnedItemList.Remove(item);
        Destroy(item.gameObject);
    }

    public void SpawnRandomEnemy()
    {
        var randomIndex = Random.Range(0, 2);
        if (randomIndex != 0)
        {
            randomIndex = GameManager.Instance.CurrentBossIndex + 1;
        }
        var randomPos = SetRandomPosition(enemySpawnRange.x, enemySpawnRange.y);
        var newEnemy = Instantiate(Enemies[randomIndex], randomPos, Quaternion.identity, EnemyGroup.transform);
        SpawnedEnemyList.Add(newEnemy);
    }

    public void SpawnEnemy(int index, Vector3 spawnPoint)
    {
        var newEnemy = Instantiate(Enemies[index], spawnPoint, Quaternion.identity, EnemyGroup.transform);
        SpawnedEnemyList.Add(newEnemy);
    }

    public void RemoveSpawnedEnemy(EnemyController enemy)
    {
        SpawnedEnemyList.Remove(enemy);
    }
    
    public void SpawnBossEnemy(int index)
    {
        var randomPos = SetRandomPosition(enemySpawnRange.x, enemySpawnRange.y);
        SpawnedBossEnemy = Instantiate(bossEnemies[index], randomPos, Quaternion.identity, EnemyGroup.transform);
        GameManager.Instance.isBossSpawned = true;
    }
    
    public Vector3 SetRandomPosition(float minRadius, float maxRadius)
    {
        var randomDirection = Random.insideUnitCircle.normalized;
        var randomDistance = Random.Range(minRadius, maxRadius);
        var spawnPosition = GameManager.Instance.Player.transform.position + new Vector3(randomDirection.x, randomDirection.y, 0) * randomDistance;
        
        return spawnPosition;
    }

    public void DespawnAllEnemies()
    {
        for (var i = SpawnedEnemyList.Count - 1; i >= 0; i--)
        {
            SpawnedEnemyList[i].KillEnemy(false, false);
        }
    }
    
    public void DespawnAllItems()
    {
        foreach (var item in SpawnedItemList)
        {
            Destroy(item.GameObject());
        }
        SpawnedItemList.Clear();
    }

    public void DespawnBossEnemy()
    {
        if (SpawnedBossEnemy != null)
        {
            Destroy(SpawnedBossEnemy.GameObject());
        }
    }

}
