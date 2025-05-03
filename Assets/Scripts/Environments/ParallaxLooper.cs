using UnityEngine;

public class ParallaxLooper : MonoBehaviour
{
    public float parallaxFactor = 0.5f;
    private float textureUnitSizeX;
    private Transform cam;
    private Vector3 lastCamPos;

    void Start()
    {
        cam = Camera.main.transform;
        lastCamPos = cam.position;

        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;

        textureUnitSizeX = texture.width / sprite.pixelsPerUnit * transform.localScale.x;
    }

    void LateUpdate()
    {
        Vector3 delta = cam.position - lastCamPos;
        transform.position += new Vector3(delta.x * parallaxFactor, delta.y * parallaxFactor, 0f);
        lastCamPos = cam.position;

        float distanceFromCamera = cam.position.x - transform.position.x;
        if (Mathf.Abs(distanceFromCamera) >= textureUnitSizeX)
        {
            float offset = (distanceFromCamera % textureUnitSizeX);
            transform.position += new Vector3(offset, 0f, 0f);
        }
    }
}