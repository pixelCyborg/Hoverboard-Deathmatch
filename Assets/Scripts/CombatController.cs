using UnityEngine;
using System.Collections;

public class CombatController : MonoBehaviour {
    public Transform weaponSlot;
    public Weapon equippedWeapon;

    public void SetWeapon(Weapon weapon)
    {
        equippedWeapon = weapon;
    }

    public void Attack(Vector3 target)
    {
        if (equippedWeapon != null)
            equippedWeapon.Attack(target, this);
    }

    public void Charge()
    {
        if(equippedWeapon != null)
            equippedWeapon.Charge();
    }

    public void Drop()
    {
        if (equippedWeapon != null)
            equippedWeapon.Drop(this);
    }
}
