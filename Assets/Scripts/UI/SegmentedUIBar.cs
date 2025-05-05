using UnityEngine;
using UnityEngine.UI;

public class SegmentedRewindBar : MonoBehaviour
{
    [Tooltip("Drag your segment Images here in order (0 → N-1).")]
    public Image[] segments;

    [Tooltip("Sprite when the segment is full (lit).")]
    public Sprite fullSprite;

    [Tooltip("Sprite when the segment is empty (unlit).")]
    public Sprite emptySprite;

    void Update()
    {
        // fraction of bar filled
        float chargeFrac = RewindManager.I.RewindCharge / RewindManager.I.RewindLimit;
        int litCount = Mathf.RoundToInt(chargeFrac * segments.Length);

        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].sprite = (i < litCount) ? fullSprite : emptySprite;
        }
    }
}
