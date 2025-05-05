using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class RewindBarUI : MonoBehaviour
{
    Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
        // we don't want user dragging it
        slider.interactable = false;
        // setup slider range
        slider.minValue = 0f;
        slider.maxValue = RewindManager.I.RewindLimit;
    }

    void Update()
    {
        // every frame, sync slider to current charge
        slider.value = RewindManager.I.RewindCharge;
    }
}
