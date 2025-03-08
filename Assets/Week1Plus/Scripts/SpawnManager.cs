using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] public Item[] Items;
    public List<Item> SpawnedItemList;

    public void SpawnItem(Transform spawnPoint)
    {
        var newItem = Instantiate(Items[Random.Range(0, Items.Length)], spawnPoint.position, Quaternion.identity);
        SpawnedItemList.Add(newItem);
    }

    public void RemoveSpawnedItem(Item item)
    {
        SpawnedItemList.Remove(item);
        Destroy(item.gameObject);
    }
}
