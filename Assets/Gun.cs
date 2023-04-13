using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float reloadSpeed;
    public float attackSpeed;
    public float damage;
    public int maxAmmo;
    public float bulletSize;
    private int ammo;

    public Transform bulletPrefab;
    public Transform bulletSpawnPoint;

    private float _timeUntilNextAttack;
    private bool _isReloading;

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !_isReloading && Time.time >= _timeUntilNextAttack)
        {
            Shoot();
            _timeUntilNextAttack = Time.time + 1f / attackSpeed;
        }

        if (Input.GetKeyDown(KeyCode.R) && !_isReloading && ammo < maxAmmo)
        {
            StartCoroutine(Reload());
        }
    }

    private void Shoot()
    {
        Transform bulletInstance = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        bulletInstance.localScale = new Vector3(bulletSize, bulletSize, bulletSize);
        Bullet bulletScript = bulletInstance.GetComponent<Bullet>();
        bulletScript.SetDamage(damage);
        ammo--;

        if (ammo == 0)
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        _isReloading = true;
        yield return new WaitForSeconds(reloadSpeed);
        ammo = maxAmmo;
        _isReloading = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerCombat>().PickUpGun(this);
            gameObject.SetActive(false);
        }
    }
}