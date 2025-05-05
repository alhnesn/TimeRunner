using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StoryManager : MonoBehaviour
{
    public Image imageDisplay;
    public TextMeshProUGUI textDisplay;
    public Sprite[] images;
    [TextArea(2, 5)]
    public string[] texts;

    public float fadeDuration = 0.5f;
    public float typeSpeed = 0.03f;

    private int currentIndex = -1;
    private bool isTransitioning = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private void Start()
    {
        imageDisplay.color = new Color(1, 1, 1, 0);
        ShowNext();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTransitioning)
        {
            if (isTyping)
            {
                // Eğer yazı yazılıyorsa, hemen tamamla
                CompleteTyping();
            }
            else
            {
                ShowNext();
            }
        }
    }

    void ShowNext()
    {
        currentIndex++;

        if (currentIndex >= images.Length)
        {
            textDisplay.text = "";
            StartCoroutine(FadeOutImage());
            return;
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        StartCoroutine(TransitionImage(images[currentIndex]));
        typingCoroutine = StartCoroutine(TypeText(texts[currentIndex]));
    }

    IEnumerator TransitionImage(Sprite newSprite)
    {
        isTransitioning = true;

        yield return StartCoroutine(FadeOutImage());

        imageDisplay.sprite = newSprite;

        yield return StartCoroutine(FadeInImage());

        isTransitioning = false;
    }

    IEnumerator FadeOutImage()
    {
        float elapsed = 0f;
        Color c = imageDisplay.color;
        while (elapsed < fadeDuration)
        {
            c.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            imageDisplay.color = c;
            elapsed += Time.deltaTime;
            yield return null;
        }
        c.a = 0f;
        imageDisplay.color = c;
    }

    IEnumerator FadeInImage()
    {
        float elapsed = 0f;
        Color c = imageDisplay.color;
        while (elapsed < fadeDuration)
        {
            c.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            imageDisplay.color = c;
            elapsed += Time.deltaTime;
            yield return null;
        }
        c.a = 1f;
        imageDisplay.color = c;
    }

    IEnumerator TypeText(string fullText)
    {
        isTyping = true;
        textDisplay.text = "";
        foreach (char letter in fullText)
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
    }

    void CompleteTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        textDisplay.text = texts[currentIndex];
        isTyping = false;
    }
}
