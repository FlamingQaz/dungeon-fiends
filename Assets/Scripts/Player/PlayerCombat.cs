using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    public LayerMask enemyLayer;
    public LayerMask friendlyLayer;

    private PlayerInventory playerInventory;

    public GameObject ammoCountUI;
    public GameObject reloadBar;

    private void Awake()
    {
        playerInventory = GetComponent<PlayerInventory>();
    }



    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (playerInventory.currentWeapons[playerInventory.currentWeaponHeld] != null)
                playerInventory.currentWeapons[playerInventory.currentWeaponHeld].GetComponent<Weapon>().Attack();
        }
        if (Input.GetButtonDown("Reload"))
        {
            if (playerInventory.currentWeapons[playerInventory.currentWeaponHeld] != null)
                playerInventory.currentWeapons[playerInventory.currentWeaponHeld].GetComponent<Weapon>().Reload();
        }
        if (Input.GetButtonDown("Swap Weapons"))
        {
            if (playerInventory.currentWeapons[0] != null && playerInventory.currentWeapons[1] != null)
            {
                SwapWeapons();
            }
        }
    }

    private void SwapWeapons()
    {
        if (playerInventory.currentWeapons[playerInventory.currentWeaponHeld].GetComponent<Weapon>().GetReloadStatus())
            playerInventory.currentWeapons[playerInventory.currentWeaponHeld].GetComponent<Weapon>().StopReload();

        playerInventory.currentWeapons[playerInventory.currentWeaponHeld].SetActive(false);
        playerInventory.currentWeaponHeld = (playerInventory.currentWeaponHeld + 1) % playerInventory.currentWeapons.Length;
        playerInventory.currentWeapons[playerInventory.currentWeaponHeld].SetActive(true);

        UpdateAmmoCount();
    }

    public void UpdateAmmoCount()
    {
        if (playerInventory.currentWeapons[playerInventory.currentWeaponHeld].GetComponent<Weapon>().WeaponType == Weapon.WeaponCategory.Ranged)
        {
            if (ammoCountUI.activeSelf == false)
            {
                ammoCountUI.SetActive(true);
                reloadBar.SetActive(false);
            }

            Text ammoText = GameObject.Find(ammoCountUI.name + "/Ammo Count").GetComponent<Text>();

            ammoText.text =
                playerInventory.currentWeapons[playerInventory.currentWeaponHeld].GetComponent<Weapon>().AmmoUpdate();
        }
    }

    public void UpdateReloadBar(float percent)
    {
        if (reloadBar.activeInHierarchy == false)
            reloadBar.SetActive(true);

        reloadBar.GetComponent<Slider>().value = percent;

        if (percent >= 1)
            reloadBar.SetActive(false);
    }
}

