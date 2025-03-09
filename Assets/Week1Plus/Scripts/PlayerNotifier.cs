using System;
using UnityEngine;

public class PlayerNotifier : MonoBehaviour
{
    [SerializeField] private GameObject bossNotifier;
    public GameObject _bossObject;
    
    private Transform player;

    public void SetBossObject(GameObject bossObject)
    {
        _bossObject = bossObject;
        bossNotifier.SetActive(true);
    }

    public void SetBossNotifier(bool status)
    {
        bossNotifier.SetActive(status);
    }

    private void Start()
    {
        player = GameManager.Instance.Player.transform;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_bossObject == null)
        {
            return;
        }

        var direction = (_bossObject.transform.position - player.position).normalized;
        bossNotifier.transform.position = player.position + direction * 1.5f;

        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bossNotifier.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }
}
