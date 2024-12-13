using UnityEngine;
using TMPro;

public class FloatingText
{
    public bool active;
    public GameObject go;
    public TextMeshProUGUI txt;
    public Vector3 motion;
    public float duration;
    public float lastShown;
    public bool isWorldSpace;

    // New properties for arc and size adjustment
    public float arcFactor = 0f;
    public float sizeChangeRate = 0f;

    public void Show()
    {
        active = true;
        lastShown = Time.time;
        go.SetActive(active);
    }

    public void Hide()
    {
        active = false;
        go.SetActive(active);
    }

    public void UpdateFloatingText()
    {
        if (!active)
            return;

        float elapsed = Time.time - lastShown;

        if (elapsed > duration)
        {
            Hide();
            return;
        }

        // Apply motion and arc effect
        if (isWorldSpace)
        {
            go.transform.position += motion * Time.deltaTime;
            go.transform.position += new Vector3(Mathf.Sin(elapsed * Mathf.PI / duration) * arcFactor, 0, 0) * Time.deltaTime;
        }
        else
        {
            go.transform.position += motion * Time.deltaTime;
            go.transform.position += new Vector3(Mathf.Sin(elapsed * Mathf.PI / duration) * arcFactor, 0, 0) * Time.deltaTime;
        }

        // Apply size adjustment
        if (sizeChangeRate != 0)
        {
            float newScale = Mathf.Lerp(1f, 0f, elapsed / duration) * sizeChangeRate;
            go.transform.localScale = Vector3.one * newScale;
        }
    }
}