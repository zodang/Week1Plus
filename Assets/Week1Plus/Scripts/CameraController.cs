using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    
    void Start()
    {
        transform.position = new Vector3(player.position.x, player.position.y, -10);
    }
    
    void Update()
    {
        transform.position = new Vector3(player.position.x, player.position.y, -10);
    }
}
