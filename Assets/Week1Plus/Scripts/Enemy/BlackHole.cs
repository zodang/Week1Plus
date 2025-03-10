using System.Collections;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField] private EnemyMovement movement;
    [SerializeField] PointEffector2D pointEffector;
    [SerializeField] GameObject particle;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("@@@");
            // ASdf();
        }
    }

    
    
}
