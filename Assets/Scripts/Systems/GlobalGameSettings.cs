using UnityEngine;

[CreateAssetMenu(fileName = "GlobalGameSettings", menuName = "Game/Global Settings")]
public class GlobalGameSettings : ScriptableObject
{
    [Header("Rewind Settings")]
    [Tooltip("Maximum seconds of rewind allowed when the bar is full.")]
    public float rewindLimit = 2f;
    [Tooltip("How many seconds of history to keep for all rewindables.")]
    public float bufferDuration = 5f;
    [Tooltip("Speed multiplier when rewinding (e.g. 2 = twice as fast).")]
    public float rewindSpeed = 2f;
    [Tooltip("How many seconds it takes to fully refill the rewind bar.")]
    public float rechargeTime = 20f;

    [Header("Other Global Variables")]
    public float initialMaxPlayerSpeed = 8f;
    // Add as many global variables as you need!
}