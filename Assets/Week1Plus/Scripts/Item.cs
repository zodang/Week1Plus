using System;
using UnityEngine;
using EnumTypes;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemType itemType;

    [SerializeField] private int healthAmount;
    [SerializeField] private int ultimateAmount;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UseItem();
        }
    }

    private void UseItem()
    {
        switch (itemType)
        {
            case ItemType.None:
                break;
            case ItemType.Heal:
                GameManager.Instance.Player.ChangePlayerHealth(healthAmount);
                break;
            case ItemType.Ultimate:
                GameManager.Instance.Player.playerAttack.ChangePlayerUltimateCount(ultimateAmount);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


}
