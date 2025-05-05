using UnityEngine;

public class UIMain : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void SwitchMenuTo(GameObject uiMenu)
    {
        for (int i = 0; i< transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        uiMenu.SetActive(true);
    }
}
