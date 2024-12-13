using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Add TextMeshPro namespace


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        if (GameManager.instance != null)
        {
            Destroy(gameObject);
            Destroy(player.gameObject);
            Destroy(floatingTextManager.gameObject);
            Destroy(hud);
            Destroy(menu);
            Destroy(music);        // Music object
            Destroy(pause);
            

            return;
        }

        instance = this;
        SceneManager.sceneLoaded += LoadState;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Resources
    public List<Sprite> playerSprites;
    public List<Sprite> weaponSprites;
    public List<int> weaponPrices;
    public List<int> xpTable;

    // References (player script, weapon script, etc.)
    public Player player;
    public Weapon weapon;
    public FloatingTextManager floatingTextManager;
    public RectTransform hitpointBar;
    public Animator deathMenuAnimator;
    public GameObject hud;
    public GameObject menu;
    public GameObject music;   // Music to Menu
    public GameObject pause;
    

    // Logic
    public int gold;
    public int experience;
    public int keys; // New Key variable

    // Floating Text
    public void ShowText(
        string msg,
        int fontSize,
        Color color,
        Vector3 position,
        Vector3 motion,
        float duration,
        bool isWorldSpace = true,
        float scale = 1.0f,
        float arcFactor = 0f,       // New default parameter
        float sizeChangeRate = 0f)  // New default parameter
    {
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration, isWorldSpace, scale, arcFactor, sizeChangeRate);
    }


    // Upgrade Weapon
    public bool TryUpgradeWeapon()
    {
        // is the weapon max level?
        if (weaponPrices.Count <= weapon.weaponLevel)
            return false;

        if (gold >= weaponPrices[weapon.weaponLevel])
        {
            gold -= weaponPrices[weapon.weaponLevel];
            weapon.UpgradeWeapon();
            return true;
        }
        return false;
    }

    // Health Bar
    public void OnHitpointChange()
    {
        float ratio = (float)player.hitpoint / (float)player.maxHitpoint;
        hitpointBar.localScale = new Vector3(1, ratio, 1);
    }

    // Character Swap
    public void ChangeCharacter(int characterIndex)
    {
        if (characterIndex < 0 || characterIndex >= playerSprites.Count)
        {
            Debug.LogWarning("Invalid character index.");
            return;
        }

        AnimationSwapper.instance.SwapAnimation(characterIndex);
    }

    private void Update()
    {
        //Debug.Log(GetCurrentLevel());
    }

    // Experience System
    public int GetCurrentLevel()
    {
        int r = 0;
        int add = 0;

        while (experience >= add)
        {
            add += xpTable[r];
            r++;

            if (r == xpTable.Count) // Max level
                return r;
        }
        return r;
    }
    public int GetXpToLevel(int level)
    {
        int r = 0;
        int xp = 0;
        while (r < level)
        {
            xp += xpTable[r];
            r++;
        }
        return xp;
    }
    public void GrantXp(int xp)
    {
        int currLevel = GetCurrentLevel();
        experience += xp;
        if (currLevel < GetCurrentLevel())
            OnLevelUp();
    }
    public void OnLevelUp()
    {
        Debug.Log("Level up!");
        player.OnLevelUp();
        AudioSource.PlayClipAtPoint(player.levelSound, transform.position, 1f);
        GameManager.instance.ShowText("Level up!", 30, Color.magenta, transform.position, Vector3.up * 0.3f, 0.8f, true, 0.0032f);
        //GameManager.instance.ShowText("Level up!", 30, Color.magenta, playerTransform.position, Vector3.up * 0.3f, 0.8f, true, 0.0032f);



        OnHitpointChange();
    }

    // New Key Stuff

    public void AddKey(int amount)
    {
        keys += amount;
        if (keys < 0)
            keys = 0; // Ensure keys don't go below zero
    }

    // On Scene Loaded
    public void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        // Assuming non-game scenes are in lower indexes, e.g., main menu at index 0
        if (s.buildIndex > 0) // Only runs for scenes after the main menu
        {
            GameObject spawnPoint = GameObject.Find("SpawnPoint");
            if (spawnPoint != null && player != null)
            {
                player.transform.position = spawnPoint.transform.position;
            }
        }
        DialogueManager.isDialogueActive = false;
    }

    // Death Menu and Respawn
    public void Respawn()
    {
        deathMenuAnimator.SetTrigger("Hide");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        player.Respawn();
        Debug.Log("Respawn");
    }

    // Save State
    public void SaveState()
    {
        string s = "";

        s += "0" + "|";                        // preferredSkin
        s += gold.ToString() + "|";            // gold
        s += experience.ToString() + "|";      // experience
        s += weapon.weaponLevel.ToString() + "|"; // weapon level
        s += player.maxHitpoint.ToString() + "|"; // maxHitpoint
        s += player.hitpoint.ToString() + "|"; // current hitpoint
     //   s += keys.ToString();                  // keys

        PlayerPrefs.SetString("SaveState", s);
    }

    public void LoadState(Scene s, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= LoadState;

        if (!PlayerPrefs.HasKey("SaveState"))
            return;

        string[] data = PlayerPrefs.GetString("SaveState").Split('|');

        if (data.Length < 7) // Updated to match the additional field for keys
        {
            Debug.LogWarning("Save data is incomplete.");
            return;
        }

        gold = int.Parse(data[1]);
        experience = int.Parse(data[2]);
        if (GetCurrentLevel() != 1)
            player.SetLevel(GetCurrentLevel());
        weapon.SetWeaponLevel(int.Parse(data[3]));
        player.maxHitpoint = int.Parse(data[4]);
        player.hitpoint = int.Parse(data[5]);
        //keys = int.Parse(data[6]); // Load keys

        OnHitpointChange();
    }


    public void ResetCharacter()
    {
        // Set default values
        gold = 0;
        experience = 0;
        weapon.SetWeaponLevel(0); // Assuming 0 is the starting level for the weapon
        player.maxHitpoint = 5; // Set a default max health value
        player.hitpoint = player.maxHitpoint; // Reset current health to max health
        Debug.Log("Game Reset");
        // Update HUD or health bar if needed
        OnHitpointChange();
        // Reset dialogue state
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null)
        {
            dialogueManager.ResetDialogue();
        }
    }

}



