//using UnityEngine;

//public class BreakableBox : MonoBehaviour
//{
//    private bool isTouchingPlayer = false;

//    // Player kutuya çarptığında, kutu yok olacak
//    void OnCollisionEnter2D(Collision2D collision)
//    {
//        if (collision.collider.CompareTag("Player"))
//        {
//            isTouchingPlayer = true;
//        }
//        else if (collision.collider.CompareTag("Chaser"))
//        {
//            Destroy(gameObject);
//        }
//    }

//    void Update()
//    {
//        // D tuşuna basıldığında kutu yok olacak
//        if (isTouchingPlayer && Input.GetKeyDown(KeyCode.E))
//        {
//            Destroy(gameObject);
//        }
//    }

//    // Player kutudan ayrıldığında, yok olma işlemi duracak
//    void OnCollisionExit2D(Collision2D collision)
//    {
//        if (collision.collider.CompareTag("Player"))
//        {
//            isTouchingPlayer = false;
//        }
//    }
//}

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class BreakableBox : MonoBehaviour, IRewindable
{
    // components we’ll toggle
    SpriteRenderer sprite;
    Collider2D col;

    // our “broken” state
    bool isBroken = false;

    struct State
    {
        public float time;
        public bool broken;
    }
    List<State> history = new List<State>();

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void OnEnable() => RewindManager.I.Register(this);
    void OnDisable() => RewindManager.I.Unregister(this);

    /// <summary>
    /// Called by PlayerBoxBreaker when the player presses E in range.
    /// </summary>
    public void Break()
    {
        if (isBroken) return;

        isBroken = true;
        sprite.enabled = false;
        col.enabled = false;

        // spawn your VFX / play sound here
    }

    // — IRewindable —

    public void RecordState(float t)
    {
        history.Add(new State { time = t, broken = isBroken });

        // drop history older than rewindBuffer
        float buf = RewindManager.I.bufferDuration;
        while (history.Count > 0 && t - history[0].time > buf)
            history.RemoveAt(0);
    }

    public void RestoreState(float t)
    {
        // find the most recent state <= t
        for (int i = history.Count - 1; i >= 0; i--)
        {
            if (history[i].time <= t)
            {
                isBroken = history[i].broken;
                sprite.enabled = !isBroken;
                col.enabled = !isBroken;
                return;
            }
        }

        // if no history (t=0), assume unbroken
        isBroken = false;
        sprite.enabled = true;
        col.enabled = true;
    }
}


// TODO: We might destroy the box if 5 secs (max rewind time) has passed
