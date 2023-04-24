using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    public GameObject[] currentWeapons = new GameObject[2];

    public int currentWeaponHeld = 0;

    public void PickUpWeapon(GameObject wep)
    {
        GameObject weapon = Instantiate(wep, transform.Find("Weapons"));
        weapon.transform.localPosition = new Vector2(1f, 0f);


        if (currentWeapons[0] == null)
        {
            currentWeapons[0] = weapon;
            if (currentWeaponHeld == 1) currentWeapons[0].SetActive(false);
        }
        else if (currentWeapons[1] == null)
        {
            currentWeapons[1] = weapon;
            if (currentWeaponHeld == 0) currentWeapons[1].SetActive(false);
        }
        else
        {
            GameObject.Instantiate(currentWeapons[currentWeaponHeld].GetComponent<Weapon>().collectableWeaponPrefab);
            currentWeapons[currentWeaponHeld] = weapon;
        }

        FindObjectOfType<PlayerCombat>().UpdateAmmoCount();
    }
}