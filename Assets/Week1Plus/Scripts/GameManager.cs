using System;
using UnityEngine;
using UnityEngine.Serialization;



public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerController Player;
    public CameraController Camera;

    public bool isBossState;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
