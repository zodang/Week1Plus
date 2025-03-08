using UnityEngine;

public class BossZone : MonoBehaviour
{
    [SerializeField] private BossEnemy boss;
    
    private bool isPlayerInBossZone;
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
        StartCoroutine(boss.MoveRoutine());
    }
}
