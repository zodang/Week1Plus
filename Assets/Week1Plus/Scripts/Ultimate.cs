using System.Collections;
using UnityEngine;

public class Ultimate : Projectile
{
    private GameObject _player;
    private PlayerController _playerController;
    
    protected override void Start()
    {
        base.Start();
        _player = GameObject.Find("Player");
        _playerController = _player.GetComponent<PlayerController>();
    }
    
    private void Update()
    {
        if (_player == null)
        {
            return;
        }

        if (!_playerController.playerAttack.CheckUltimateDistance())
        {
            return;
        }
        
        Vector2 direction = - _player.transform.position + transform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    public IEnumerator DestroyUltimate()
    {
        yield return new WaitForSeconds(1.0f);
        StartParticle();
        Destroy(gameObject);
    }
}
