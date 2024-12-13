using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingTextManager : MonoBehaviour
{
    public GameObject screenSpaceTextContainer;
    public GameObject worldSpaceTextContainer;

    public GameObject textPrefab;

    private List<FloatingText> floatingTexts = new List<FloatingText>();

    private void Update()
    {
        foreach (FloatingText txt in floatingTexts)
        {
            txt.UpdateFloatingText();
        }
    }

    public void Show(
        string msg,
        int fontSize,
        Color color,
        Vector3 position,
        Vector3 motion,
        float duration,
        bool isWorldSpace,
        float scale = 1.0f,
        float arcFactor = 0f,       // New default parameter
        float sizeChangeRate = 0f)  // New default parameter
    {
        FloatingText floatingText = GetFloatingText(isWorldSpace);

        floatingText.txt.text = msg;
        floatingText.txt.fontSize = fontSize;
        floatingText.txt.color = color;

        if (isWorldSpace)
        {
            floatingText.go.transform.position = position;
        }
        else
        {
            floatingText.go.transform.position = Camera.main.WorldToScreenPoint(position);
        }

        floatingText.go.transform.localScale = Vector3.one * scale;
        floatingText.motion = motion;
        floatingText.duration = duration;

        // Assign new effect variables
        floatingText.arcFactor = arcFactor;
        floatingText.sizeChangeRate = sizeChangeRate;

        floatingText.Show();
    }

    private FloatingText GetFloatingText(bool isWorldSpace)
    {
        FloatingText txt = floatingTexts.Find(t => !t.active);

        if (txt == null)
        {
            txt = new FloatingText();
            txt.go = Instantiate(textPrefab);

            if (isWorldSpace)
            {
                txt.go.transform.SetParent(worldSpaceTextContainer.transform, false);
            }
            else
            {
                txt.go.transform.SetParent(screenSpaceTextContainer.transform, false);
            }

            txt.txt = txt.go.GetComponent<TextMeshProUGUI>();
            floatingTexts.Add(txt);
        }

        return txt;
    }
}