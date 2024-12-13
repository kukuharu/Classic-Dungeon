using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; // Import this namespace

public class Weapon : Collidable
{
    // Damage structure
    public int[] damagePoint = { 1, 2, 3, 4, 5, 6, 7 };
    public float[] pushForce = { 2.0f, 2.2f, 2.5f, 3f, 3.2f, 3.6f, 4f };

    public AudioClip weaponSwingClip;

    // Upgrade
    public int weaponLevel = 0;
    public SpriteRenderer spriteRenderer;

    // Swing
    private Animator anim;
    public float cooldown = 0.4f;
    private float lastSwing;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
    }

    public void PlaySwingSound()
    {
        AudioSource.PlayClipAtPoint(weaponSwingClip, transform.position, 0.5f);
    }

    protected override void Update()
    {
        base.Update();

        // Ignore input if the game is paused, in Main Menu, dialogue is active, or UI is interacted with
        if (PauseMenu.GameIsPaused || DialogueManager.isDialogueActive ||
            IsPointerOverButton())
            return;

        if (SceneManager.GetActiveScene().name == "Menu")
            return;

        // Handle weapon swing
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (Time.time - lastSwing > cooldown)
            {
                lastSwing = Time.time;
                Swing();
            }
        }
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.CompareTag("Fighter") || coll.CompareTag("Enemy"))
        {
            if (coll.name == "Player")
                return;

            // Create a new damage object and send it to the fighter we've hit
            Damage dmg = new Damage
            {
                damageAmount = damagePoint[weaponLevel],
                origin = transform.position,
                pushForce = pushForce[weaponLevel]
            };

            coll.SendMessage("ReceiveDamage", dmg);
        }
    }

    private void Swing()
    {
        anim.SetTrigger("Swing");
    }

    public void UpgradeWeapon()
    {
        weaponLevel++;
        spriteRenderer.sprite = GameManager.instance.weaponSprites[weaponLevel];
    }

    public void SetWeaponLevel(int level)
    {
        weaponLevel = level;
        spriteRenderer.sprite = GameManager.instance.weaponSprites[weaponLevel];
    }

    // Check if the pointer is over a UI button
    private bool IsPointerOverButton()
    {
        // Check if the pointer is over a UI element
        if (!EventSystem.current.IsPointerOverGameObject())
            return false;

        // Perform a raycast to detect the UI element
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        // Check if any of the hit UI elements match the target tags
        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.CompareTag("SuppressWeapon"))
            {
                return true; // Suppress input
            }
        }

        return false; // Allow input
    }
}
