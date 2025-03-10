using UnityEngine;

public class BossZone : MonoBehaviour
{
    public bool isPlayerInBossZone;
    [SerializeField] private BossEnemy boss;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }
        
        if (isPlayerInBossZone)
        {
            return;
        }
        
        isPlayerInBossZone = true;
        GameManager.Instance.Camera.SetCameraTarget(boss.gameObject);
        
    }
}
