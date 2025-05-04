using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;

public class ThemeSectionSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Theme
    {
        public string planetName;
        public GameObject[] sectionPrefabs;
        public GameObject planetObject; // Planet objesi sahnede
    }
        
    public Theme[] themes;
    public Transform sectionParent;

    [Header("UI & Glitch")]
    public TextMeshProUGUI planetNameText;
    public Image glitchOverlay;
    public float glitchDuration = 1.5f;
    public Color[] glitchColors;

    [Header("Player & Reset")]
    public Transform player;
    public Vector2 playerRespawnPosition = new Vector2(-1.37f, -0.46f);
    public float playerViewDistance = 30f; // Oyuncunun önünde section spawn edilecek mesafe
    public float cleanupDistance = 40f; // Oyuncudan bu kadar geride kalan sectionlar temizlenir

    private int currentThemeIndex = 0;
    private List<GameObject> currentSections = new List<GameObject>();
    private Vector3 lastSectionEndPosition = Vector3.zero;
    private bool isTransitioning = false;
    private Camera mainCamera;
    private PlayerFollow cameraFollow; // PlayerFollow referansı

    void Start()
    {
        mainCamera = Camera.main;
        // PlayerFollow componentini bul
        cameraFollow = mainCamera.GetComponent<PlayerFollow>();

        // Başlangıçta tüm planet objelerini devre dışı bırak
        foreach (var theme in themes)
        {
            if (theme.planetObject != null)
                theme.planetObject.SetActive(false);
        }

        // Glitch overlay'i başlangıçta gizle
        if (glitchOverlay != null)
        {
            glitchOverlay.gameObject.SetActive(true);
            glitchOverlay.color = new Color(0, 0, 0, 0); // Tamamen saydam başlasın
        }

        // Planet text'i başlangıçta gizle
        if (planetNameText != null)
        {
            planetNameText.gameObject.SetActive(false);
        }

        SpawnInitialSections();
    }

    void Update()
    {
        // Tab tuşu ile theme değişimi
        if (Input.GetKeyDown(KeyCode.Tab) && !isTransitioning)
        {
            // Tab'a basıldığında hemen kamerayı ortala
            if (mainCamera != null)
            {
                mainCamera.transform.position = new Vector3(0f, 0f, -10f);
                // PlayerFollow'u devre dışı bırak
                if (cameraFollow != null)
                {
                    cameraFollow.SetFollowing(false);
                }
            }
            StartCoroutine(ThemeTransition());
        }

        // Oyuncuya göre yeni sectionların spawn edilmesi
        if (player != null && !isTransitioning)
        {
            CheckAndSpawnNewSections();
            CleanupOldSections();
        }
    }

    void CheckAndSpawnNewSections()
    {
        // Oyuncunun pozisyonundan belli bir mesafe ilerisine kadar section olup olmadığını kontrol et
        if ((lastSectionEndPosition.x - player.position.x) < playerViewDistance)
        {
            SpawnNextSection();
        }
    }

    void CleanupOldSections()
    {
        // Oyuncunun arkasında kalan ve belli bir mesafeden uzak olan sectionları temizle
        List<GameObject> sectionsToRemove = new List<GameObject>();
        
        foreach (var section in currentSections)
        {
            if (section != null)
            {
                float sectionRightEdge = section.transform.position.x + GetSectionWidthFromTilemap(section);
                
                if (sectionRightEdge < (player.position.x - cleanupDistance))
                {
                    sectionsToRemove.Add(section);
                }
            }
        }
        
        foreach (var section in sectionsToRemove)
        {
            currentSections.Remove(section);
            Destroy(section);
        }
    }

    void SpawnInitialSections()
    {
        // Başlangıçta ekranı dolduracak kadar section oluştur
        // Başlangıçta en az 5 section oluşturalım
        for (int i = 0; i < 5; i++)
        {
            SpawnNextSection();
        }
    }

    void SpawnNextSection()
    {
        Theme currentTheme = themes[currentThemeIndex];
        
        // Rastgele bir section prefab seç
        GameObject prefab = currentTheme.sectionPrefabs[Random.Range(0, currentTheme.sectionPrefabs.Length)];
        
        // Section'ı oluştur
        GameObject section = Instantiate(prefab, lastSectionEndPosition, Quaternion.identity, sectionParent);
        section.SetActive(true);
        currentSections.Add(section);
        
        // Bir sonraki section için pozisyonu güncelle
        float sectionWidth = GetSectionWidthFromTilemap(section);
        lastSectionEndPosition += new Vector3(sectionWidth, 0f, 0f);
    }

    float GetSectionWidthFromTilemap(GameObject section)
    {
        Tilemap tilemap = section.GetComponentInChildren<Tilemap>();
        if (tilemap != null)
        {
            Bounds bounds = tilemap.localBounds;
            return bounds.size.x;
        }
        else
        {
            Debug.LogWarning("Tilemap bulunamadı: " + section.name);
            return 20f; // fallback
        }
    }

    IEnumerator ThemeTransition()
    {
        isTransitioning = true;
        
        // Sonraki theme'i belirle
        int nextThemeIndex = (currentThemeIndex + 1) % themes.Length;
        Theme nextTheme = themes[nextThemeIndex];
        
        // UI elementlerini glitch sırasında gizle
        if (planetNameText != null)
        {
            planetNameText.gameObject.SetActive(false);
            planetNameText.text = "Arriving at: " + nextTheme.planetName;
        }
        
        // Oyuncuyu ve sectionları hemen gizle
        if (player != null)
            player.gameObject.SetActive(false);
            
        foreach (var sec in currentSections)
        {
            if (sec != null)
                sec.SetActive(false);
        }
        
        // Glitch efektini başlat
        yield return StartCoroutine(GlitchEffect());
        
        // Kamerayı gezegen görünümü için merkeze al
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(0f, 0f, -10f);
        }
        
        // Glitch efekti bittikten sonra UI'ı göster
        if (planetNameText != null)
        {
            planetNameText.gameObject.SetActive(true);
        }
        
        // Tüm planet objelerini devre dışı bırak
        foreach (var theme in themes)
        {
            if (theme.planetObject != null)
                theme.planetObject.SetActive(false);
        }
        
        // Yeni gezegen objesini göster
        if (nextTheme.planetObject != null)
            nextTheme.planetObject.SetActive(true);
        
        // Sectionları temizle
        foreach (var sec in currentSections)
        {
            if (sec != null)
                Destroy(sec);
        }
        currentSections.Clear();
        
        // 5 saniye gezegen görünümünü bekle
        yield return new WaitForSeconds(5f);
        
        // Gezegen objesini gizle
        if (nextTheme.planetObject != null)
            nextTheme.planetObject.SetActive(false);
        
        // Yeni theme geçişi
        currentThemeIndex = nextThemeIndex;
        
        // Section spawn pozisyonunu sıfırla
        lastSectionEndPosition = new Vector3(playerRespawnPosition.x, 0f, 0f);
        
        // Yeni theme'in sectionlarını spawn et
        SpawnInitialSections();
        
        // Kamerayı player pozisyonuna geri getir
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(playerRespawnPosition.x, playerRespawnPosition.y, -10f);
            // PlayerFollow'u tekrar aktif et
            if (cameraFollow != null)
            {
                cameraFollow.SetFollowing(true);
            }
        }
        
        // Player'ı yeni konumda göster
        if (player != null)
        {
            player.position = playerRespawnPosition;
            player.gameObject.SetActive(true);
        }
        
        // Geçiş tamamlandı
        isTransitioning = false;
    }

    IEnumerator GlitchEffect()
    {
        if (glitchOverlay == null || glitchColors.Length == 0)
            yield break;
        
        glitchOverlay.gameObject.SetActive(true);
        float timeElapsed = 0f;
        
        while (timeElapsed < glitchDuration)
        {
            // Renk seçerken alfa değerini zorla yüksek yap
            Color randomColor = glitchColors[Random.Range(0, glitchColors.Length)];
            randomColor.a = 0.8f; // %80 opaklık
            glitchOverlay.color = randomColor;
            
            yield return new WaitForSeconds(0.05f);
            timeElapsed += 0.05f;
        }
        
        glitchOverlay.color = new Color(0, 0, 0, 0); // Tam şeffaf yap
        glitchOverlay.gameObject.SetActive(false);
    }
}