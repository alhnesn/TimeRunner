using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    public Transform target;           // Takip edilecek hedef (Player)
    public Vector3 offset = new Vector3(5f, 2f, -10f); // Konum farkı
    public float smoothTime = 0.3f;    // Takip yumuşaklığı (daha yüksek = daha yavaş, yumuşak)

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;

        // SmoothDamp ile kamerayı hedef pozisyona yumuşakça yaklaştır
        Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        transform.position = new Vector3(smoothPosition.x, smoothPosition.y, offset.z); // Z sabitlenir (genellikle -10)
    }
}
