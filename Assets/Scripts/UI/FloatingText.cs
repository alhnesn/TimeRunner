using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float lifeTime = 1.5f;

    private Vector3 moveDirection = Vector3.up;

    void Start()
{
    // Optional: set sorting layer and order through code
    var renderer = GetComponent<MeshRenderer>();
    renderer.sortingLayerName = "Default"; // Or use your custom layer
    renderer.sortingOrder = 10;

    Destroy(gameObject, lifeTime);
}


    void Update()
    {
        transform.position += moveDirection * floatSpeed * Time.deltaTime;
    }
}
