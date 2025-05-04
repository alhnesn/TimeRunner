using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    private ThemeSectionSpawner themeSectionSpawner;
    
    void Start()
    {
        // Find the ThemeSectionSpawner in the scene
        themeSectionSpawner = FindObjectOfType<ThemeSectionSpawner>();
        
        // Ensure this has a Collider2D component with isTrigger = true
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        else
        {
            Debug.LogError("Portal prefab needs a Collider2D component with isTrigger enabled!");
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            if (themeSectionSpawner != null)
            {
                themeSectionSpawner.OnPortalTriggered();
            }
        }
    }
}