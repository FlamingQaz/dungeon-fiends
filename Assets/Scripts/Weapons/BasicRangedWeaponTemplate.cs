using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRangedWeaponTemplate : Weapon
{


    //internal weapon stats these are the values that will be used for attacks and such
    [Header("Stats")]
    [SerializeField] private float _damage;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private int _maxAmmo;
    [SerializeField] private int _ammo;
    [SerializeField] private float _reloadTime;


    [SerializeField] private BasicProjectile basicProjectileScript;
    private float timeSince;

    // Start is called before the first frame update
    void Awake()
    {
        _damage = BaseDamage;
        _attackSpeed = BaseAttackSpeed;
        _maxAmmo = BaseMaxAmmo;
        _reloadTime = BaseReloadTime;

        timeSince = 0f;

        _ammo = _maxAmmo;
    }

    public override void Attack()
    {
        if (_ammo > 0 && !isReloading)
            if (_attackSpeed <= Time.fixedTime - timeSince)
            {
                timeSince = Time.fixedTime;
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                basicProjectileScript.Shoot(transform.parent, mousePos, TargetLayer, FriendlyLayer, _damage);
                _ammo--;
                FindObjectOfType<PlayerCombat>().UpdateAmmoCount();
            }
    }

    float reloadPercent;
    bool isReloading = false;
    public override void Reload()
    {
        if (!isReloading && _ammo < _maxAmmo)
        {
            isReloading = true;

            reloadPercent = 0;
            StartCoroutine(ReloadAmmo());
        }
    }

    public override void StopReload()
    {
        StopCoroutine(ReloadAmmo());
        reloadPercent = 1;
        isReloading = false;
        FindObjectOfType<PlayerCombat>().UpdateReloadBar(reloadPercent);
    }

    public override bool GetReloadStatus()
    {
        return isReloading;
    }

    IEnumerator ReloadAmmo()
    { 

        while (reloadPercent < 1)
        {
            reloadPercent += 1 / _reloadTime * Time.deltaTime;
            FindObjectOfType<PlayerCombat>().UpdateReloadBar(reloadPercent);
            yield return null;
        }

        _ammo = _maxAmmo;
        isReloading = false;

        FindObjectOfType<PlayerCombat>().UpdateReloadBar(reloadPercent);
        FindObjectOfType<PlayerCombat>().UpdateAmmoCount();
        yield return null;
    }

    public override string AmmoUpdate()
    {
        return _ammo + "/" + _maxAmmo;
    }
}
