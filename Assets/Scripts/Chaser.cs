using UnityEngine;

public class Chaser : MonoBehaviour
{
    public Transform player;
    public float speed = 4f;

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }
}
