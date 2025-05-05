using UnityEngine;
using UnityEngine.Video;

public class VideoCarousel : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public VideoClip[] videoClips;

    private int currentIndex = 0;

    private void Start()
    {
        if (videoClips.Length == 0 || videoPlayer == null)
        {
            Debug.LogWarning("No videos or video player assigned!");
            return;
        }

        videoPlayer.isLooping = true;
        PlayVideo(currentIndex);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextVideo();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousVideo();
        }
    }

    void NextVideo()
    {
        currentIndex = (currentIndex + 1) % videoClips.Length;
        PlayVideo(currentIndex);
    }

    void PreviousVideo()
    {
        currentIndex = (currentIndex - 1 + videoClips.Length) % videoClips.Length;
        PlayVideo(currentIndex);
    }

    void PlayVideo(int index)
    {
        videoPlayer.clip = videoClips[index];
        videoPlayer.Play();
    }
}
