using System;
using System.Collections;
using EnumTypes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BossEnemy : EnemyController
{
    [Header("Boss Enemy")]
    [SerializeField] private Slider healthSlider;
    private Vector2 _randomDirection;

    protected override void Start()
    {
        base.Start();

        healthSlider.maxValue = healthMaxCount;
        healthSlider.value = healthMaxCount;
        
        gameManager.Player.PlayerNotifier.SetBossObject(gameObject);
        StartDamage(false);
    }

    public void UpdateHealthBar()
    {
        healthSlider.value = HealthTotalCount;
    }
}
