using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Modifiers;

public abstract class Weapon : MonoBehaviour
{
    public enum WeaponCategory
    {
        Melee,
        Ranged,
        Spell
    }

    public enum DamageTypes
    {
        Fire = 1,
        Cold = 2,
        Lightning = 3,
        Earth = 4,
        Necrotic = 5,
        Divine = 6
    }

    public enum Rarity
    {
        Common = 0,
        Uncommon = 1,
        Magic = 2,
        Rare = 3,
        Mythic = 4,
    }

    public GameObject collectableWeaponPrefab;

    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask friendlyLayer;

    [SerializeField] private WeaponCategory weaponType;

    [Header("Base Stats")]
    [SerializeField] private DamageTypes damageType;
    [SerializeField] private Rarity weaponRarity = Rarity.Common;
    [SerializeField] private float baseAttackSpeed;
    [SerializeField] private float baseDamage;
    [SerializeField] private int baseMaxAmmo;
    [SerializeField] private int baseReloadTime;

    [Header("Weapon Modifiers")]
    [SerializeField] protected WeaponModifiers[] mods;

    public abstract void Attack();

    public virtual void Reload()
    { return; }

    public virtual void StopReload()
    { return; }

    public virtual bool GetReloadStatus()
    { return false; }

    public virtual string AmmoUpdate()
    { return null; }

    public WeaponCategory WeaponType { get => weaponType; set => weaponType = value; }
    public DamageTypes DamageType { get => damageType; set => damageType = value; }
    public Rarity WeaponRarity { get => weaponRarity; set => weaponRarity = value; }
    protected LayerMask TargetLayer { get => targetLayer; set => targetLayer = value; }
    protected LayerMask FriendlyLayer { get => friendlyLayer; set => friendlyLayer = value; }
    
    
    protected float BaseAttackSpeed { get => baseAttackSpeed; set => baseAttackSpeed = value; }
    protected float BaseDamage { get => baseDamage; set => baseDamage = value; }
    protected int BaseMaxAmmo { get => baseMaxAmmo; set => baseMaxAmmo = value; }
    protected int BaseReloadTime { get => baseReloadTime; set => baseReloadTime = value; }
}
