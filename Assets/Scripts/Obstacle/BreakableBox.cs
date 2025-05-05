using UnityEngine;

public class BreakableBox : MonoBehaviour
{
    public GameObject floatingTextPrefab; // assign in Inspector

public void Break()
{
    // Your existing break logic...

    ScoreManagerTMP.I.AddScore(10);

    if (floatingTextPrefab != null)
    {
        Instantiate(floatingTextPrefab, transform.position + new Vector3(0, 1f, -0.1f), Quaternion.identity);
    }

    Destroy(gameObject);
}

}
