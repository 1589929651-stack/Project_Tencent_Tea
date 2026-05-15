using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI hintText;

    void Awake()
    {
        Instance = this;
    }

    public void ShowText(string msg)
    {
        StopAllCoroutines();
        StartCoroutine(ShowRoutine(msg));
    }

    System.Collections.IEnumerator ShowRoutine(string msg)
    {
        hintText.text = msg;
        hintText.enabled = true;

        yield return new WaitForSeconds(1.5f);

        hintText.enabled = false;
    }
}