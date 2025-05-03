using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class SectionSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Theme
    {
        public string themeName;
        public GameObject[] sectionPrefabs;
        public GameObject backgroundPrefab; // Background prefab for this theme
        [Header("Background Settings")]
        public float parallaxFactor = 0.1f; // How much the background moves (0 = still)
        [Header("Theme Transition")]
        public Color themeOverlayColor = Color.black; // Color to use during transition
        
        [HideInInspector]
        public GameObject backgroundInstance; // Reference to instantiated background
    }

    [Header("Theme Settings")]
    public Theme[] themes;
    public int sectionsPerTheme = 5;

    [Header("Spawn Settings")]
    public Transform player;
    public float spawnDistanceAhead = 30f;
    public float deleteDistanceBehind = 40f;

    [Header("Transition Settings")]
    public float transitionDuration = 1.5f;
    public GameObject transitionOverlay; // Assign a UI Image that covers the screen
    
    private float nextSpawnX = 0f;
    private List<GameObject> spawnedSections = new List<GameObject>();
    private int currentThemeIndex = 0;
    private int previousThemeIndex = -1;
    private int sectionsSpawnedInTheme = 0;
    private bool isTransitioning = false;

    void Start()
    {
        nextSpawnX = player.position.x;
        
        // Validate themes
        if (themes.Length == 0)
        {
            Debug.LogError("No themes added to SectionSpawner! Please add at least one theme with section prefabs.");
            enabled = false;
            return;
        }

        // Setup transition overlay
        if (transitionOverlay != null)
        {
            // Make sure it starts invisible
            CanvasGroup overlayCanvasGroup = transitionOverlay.GetComponent<CanvasGroup>();
            if (overlayCanvasGroup == null)
            {
                overlayCanvasGroup = transitionOverlay.AddComponent<CanvasGroup>();
            }
            overlayCanvasGroup.alpha = 0;
            transitionOverlay.SetActive(true);
        }

        // Create all backgrounds up front
        CreateAllBackgrounds();

        // Select a random starting theme
        currentThemeIndex = Random.Range(0, themes.Length);
        ActivateThemeBackground(currentThemeIndex);
    }

    void CreateAllBackgrounds()
    {
        Debug.Log("Creating all theme backgrounds in advance");
        
        for (int i = 0; i < themes.Length; i++)
        {
            Theme theme = themes[i];
            
            if (theme.backgroundPrefab != null)
            {
                // Position it at camera location but behind everything
                Vector3 spawnPos = new Vector3(
                    Camera.main.transform.position.x,
                    Camera.main.transform.position.y,
                    1f // Ensure this is in front of the default far clipping plane
                );
                
                // Create background
                GameObject backgroundInstance = Instantiate(theme.backgroundPrefab, spawnPos, Quaternion.identity);
                backgroundInstance.name = theme.themeName + "_Background";
                
                // Set up renderers
                SpriteRenderer[] renderers = backgroundInstance.GetComponentsInChildren<SpriteRenderer>();
                if (renderers.Length > 0)
                {
                    Debug.Log($"Found {renderers.Length} renderers in background for {theme.themeName}");
                    foreach (SpriteRenderer renderer in renderers)
                    {
                        // Ensure the sprite is visible in the scene
                        renderer.sortingOrder = -100; // Put it behind everything
                    }
                }
                else
                {
                    Debug.LogWarning($"No renderers found in background prefab for {theme.themeName}!");
                }
                
                // Set up parallax effect if desired
                if (theme.parallaxFactor > 0)
                {
                    ParallaxBackground parallax = backgroundInstance.AddComponent<ParallaxBackground>();
                    if (parallax != null)
                    {
                        parallax.followTarget = Camera.main.transform;
                        parallax.parallaxEffect = theme.parallaxFactor;
                        Debug.Log($"Added parallax effect with factor {theme.parallaxFactor} to {theme.themeName}");
                    }
                }
                else
                {
                    // For a completely static background, parent to camera
                    backgroundInstance.transform.SetParent(Camera.main.transform);
                    // Reset local position to ensure it's centered on camera
                    backgroundInstance.transform.localPosition = new Vector3(0, 0, 1f);
                    Debug.Log($"Static background {theme.themeName} parented to camera");
                }
                
                // Store reference and deactivate initially
                theme.backgroundInstance = backgroundInstance;
                backgroundInstance.SetActive(false);
                
                Debug.Log($"Created background for theme: {theme.themeName}");
            }
            else
            {
                Debug.LogWarning($"No background prefab assigned for theme: {theme.themeName}");
            }
        }
    }

    void Update()
    {
        // Don't spawn during transitions
        if (isTransitioning)
            return;
            
        // Spawn new section if needed
        if (player.position.x + spawnDistanceAhead >= nextSpawnX)
        {
            GameObject section = SpawnSectionFromCurrentTheme();
            float width = GetSectionWidth(section);
            nextSpawnX += width;
            spawnedSections.Add(section);
            
            // Check if we need to switch themes
            sectionsSpawnedInTheme++;
            if (sectionsSpawnedInTheme >= sectionsPerTheme)
            {
                StartCoroutine(TransitionToNewTheme());
            }
        }

        // Delete passed sections
        for (int i = spawnedSections.Count - 1; i >= 0; i--)
        {
            if (player.position.x - spawnedSections[i].transform.position.x > deleteDistanceBehind)
            {
                Destroy(spawnedSections[i]);
                spawnedSections.RemoveAt(i);
            }
        }
    }

    GameObject SpawnSectionFromCurrentTheme()
    {
        // Make sure current theme exists and has prefabs
        if (currentThemeIndex >= themes.Length || themes[currentThemeIndex].sectionPrefabs.Length == 0)
        {
            Debug.LogWarning("Current theme has no prefabs. Selecting a new theme.");
            SwitchToRandomTheme();
        }

        Theme currentTheme = themes[currentThemeIndex];
        int prefabIndex = Random.Range(0, currentTheme.sectionPrefabs.Length);
        
        Vector3 spawnPosition = new Vector3(nextSpawnX, 0f, 0f);
        GameObject sectionInstance = Instantiate(currentTheme.sectionPrefabs[prefabIndex], spawnPosition, Quaternion.identity);
        
        // Ensure section and all its children are active
        sectionInstance.SetActive(true);
        
        // Force activate all child GameObjects if they exist
        foreach (Transform child in sectionInstance.transform)
        {
            child.gameObject.SetActive(true);
        }
        
        // Name the instance with theme info for debugging
        sectionInstance.name = $"{currentTheme.themeName}_Section_{sectionsSpawnedInTheme}";
        
        return sectionInstance;
    }

    IEnumerator TransitionToNewTheme()
    {
        isTransitioning = true;
        
        // Store the previous theme index
        previousThemeIndex = currentThemeIndex;
        
        // Find a different theme than the current one
        SwitchToRandomTheme();
        
        // Set the transition overlay color to match theme
        if (transitionOverlay != null)
        {
            UnityEngine.UI.Image overlayImage = transitionOverlay.GetComponent<UnityEngine.UI.Image>();
            if (overlayImage != null)
            {
                overlayImage.color = themes[currentThemeIndex].themeOverlayColor;
            }
            
            CanvasGroup overlayCanvasGroup = transitionOverlay.GetComponent<CanvasGroup>();
            
            // Fade in
            float elapsedTime = 0;
            while (elapsedTime < transitionDuration / 2)
            {
                overlayCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / (transitionDuration / 2));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            overlayCanvasGroup.alpha = 1;
            
            // Switch background objects at the peak of the transition
            ActivateThemeBackground(currentThemeIndex);
            
            // Fade out
            elapsedTime = 0;
            while (elapsedTime < transitionDuration / 2)
            {
                overlayCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / (transitionDuration / 2));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            overlayCanvasGroup.alpha = 0;
        }
        else
        {
            // No transition overlay - just wait a bit and change background
            yield return new WaitForSeconds(0.5f);
            ActivateThemeBackground(currentThemeIndex);
        }
        
        sectionsSpawnedInTheme = 0;
        Debug.Log($"Switched to theme: {themes[currentThemeIndex].themeName}");
        
        isTransitioning = false;
    }

    void SwitchToRandomTheme()
    {
        // Find a different theme than the current one
        if (themes.Length > 1)
        {
            int newThemeIndex;
            do
            {
                newThemeIndex = Random.Range(0, themes.Length);
            } while (newThemeIndex == currentThemeIndex);
            
            currentThemeIndex = newThemeIndex;
        }
    }
    
    void ActivateThemeBackground(int themeIndex)
    {
        if (themeIndex < 0 || themeIndex >= themes.Length)
        {
            Debug.LogError("Invalid theme index: " + themeIndex);
            return;
        }
            
        Theme theme = themes[themeIndex];
        Debug.Log($"Activating background for theme: {theme.themeName}");
        
        // First deactivate previous background if it exists
        if (previousThemeIndex >= 0 && previousThemeIndex < themes.Length)
        {
            Theme prevTheme = themes[previousThemeIndex];
            if (prevTheme.backgroundInstance != null)
            {
                prevTheme.backgroundInstance.SetActive(false);
                Debug.Log($"Deactivated previous background: {prevTheme.themeName}");
            }
        }
        
        // Now activate the current background
        if (theme.backgroundInstance != null)
        {
            // Update position to match camera
            if (theme.parallaxFactor <= 0)
            {
                // For static backgrounds, they're already parented to camera so no need to update position
            }
            else
            {
                // For parallax backgrounds, make sure they're positioned correctly
                Vector3 newPos = new Vector3(
                    Camera.main.transform.position.x,
                    Camera.main.transform.position.y,
                    theme.backgroundInstance.transform.position.z
                );
                theme.backgroundInstance.transform.position = newPos;
            }
            
            // Activate the background
            theme.backgroundInstance.SetActive(true);
            Debug.Log($"Activated background for theme: {theme.themeName}");
        }
        else
        {
            Debug.LogWarning($"No background instance found for theme: {theme.themeName}");
        }
    }

    float GetSectionWidth(GameObject section)
    {
        Tilemap tilemap = section.GetComponentInChildren<Tilemap>();
        if (tilemap != null)
        {
            Bounds bounds = tilemap.localBounds;
            return bounds.size.x;
        }
        else
        {
            Debug.LogWarning("No Tilemap found in section: " + section.name);
            return 20f; // Fallback width
        }
    }
}