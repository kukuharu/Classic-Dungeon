using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // For TextMeshPro

public class KeyGoldUI : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI keysText;
    public GameObject goldObject; // Reference to the parent Gold GameObject
    public GameObject keysObject; // Reference to the parent Keys GameObject

    private void Update()
    {
        // Update the gold UI
        if (GameManager.instance.gold > 0)
        {
            goldObject.SetActive(true);
            goldText.text = GameManager.instance.gold.ToString();
        }
        else
        {
            goldObject.SetActive(false);
        }

        // Update the keys UI
        if (GameManager.instance.keys > 0)
        {
            keysObject.SetActive(true);
            keysText.text = GameManager.instance.keys.ToString();
        }
        else
        {
            keysObject.SetActive(false);
        }
    }
}
