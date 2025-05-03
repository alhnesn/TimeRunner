using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform followTarget; // Usually the player or camera
    public float parallaxEffect = 0.1f; // How much the background moves relative to the camera (0 = still, 1 = moves with camera)
    public bool infiniteHorizontal = true; // Whether to repeat the background horizontally
    
    private float startPosX;
    private float spriteWidth;
    private Transform cameraTransform;
    private float lastCameraX;
    
    void Start()
    {
        if (followTarget == null)
            followTarget = Camera.main.transform;
            
        cameraTransform = Camera.main.transform;
        startPosX = transform.position.x;
        lastCameraX = cameraTransform.position.x;
        
        // Get sprite width if there's a sprite renderer
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteWidth = spriteRenderer.bounds.size.x;
        else
            spriteWidth = 0f;
    }
    
    void LateUpdate()
    {
        // How much the camera has moved
        float cameraDeltaX = cameraTransform.position.x - lastCameraX;
        
        // Current target X position with parallax effect
        float parallaxX = transform.position.x + (cameraDeltaX * parallaxEffect);
        
        // Apply the new position
        transform.position = new Vector3(
            parallaxX,
            transform.position.y,
            transform.position.z
        );
        
        // Update the last camera position
        lastCameraX = cameraTransform.position.x;
        
        // Infinite scrolling - if enabled and we have a valid sprite width
        if (infiniteHorizontal && spriteWidth > 0)
        {
            float relativeCameraDistance = cameraTransform.position.x * (1 - parallaxEffect);
            
            // If the camera has moved beyond the sprite's bounds
            if (relativeCameraDistance > startPosX + spriteWidth)
            {
                startPosX += spriteWidth;
                transform.position = new Vector3(startPosX, transform.position.y, transform.position.z);
            }
            else if (relativeCameraDistance < startPosX - spriteWidth)
            {
                startPosX -= spriteWidth;
                transform.position = new Vector3(startPosX, transform.position.y, transform.position.z);
            }
        }
    }
}