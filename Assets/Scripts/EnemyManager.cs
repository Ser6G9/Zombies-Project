using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public float damage = 20.0f;

    private void OnTriggerEnter(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();

        if (player != null)
        {
            player.Hit(damage);
        }
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
