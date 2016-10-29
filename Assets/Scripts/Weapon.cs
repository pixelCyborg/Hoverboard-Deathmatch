using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
    public enum WeaponType
    {
        Melee, Thrown
    }

    public WeaponType type;
    public float damage;
    public float speed;
    private Collider myCollider;
    private Rigidbody body;

    public float maxCharge = 50.0f;
    private float chargeStart;
    private float charge;

    private Transform assignedSlot;

    void Start()
    {
        myCollider = GetComponent<Collider>();
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(assignedSlot != null)
        {
            transform.position = assignedSlot.position;
            transform.rotation = assignedSlot.rotation;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            CombatController controller = col.GetComponent<CombatController>();
            if (controller != null && controller.equippedWeapon == null)
            {
                PickUp(controller);
            }
        }
    }

    public void PickUp(CombatController controller)
    {
        body.useGravity = false;
        myCollider.isTrigger = true;
        body.constraints = RigidbodyConstraints.FreezeAll;
        assignedSlot = controller.weaponSlot;
        controller.SetWeapon(this);
    }

    public void Drop(CombatController controller)
    {
        body.useGravity = true;
        body.constraints = RigidbodyConstraints.None;
        assignedSlot = null;
        controller.equippedWeapon = null;
        myCollider.isTrigger = false;
    }

    public void Charge()
    {
        chargeStart = Time.time;
    }

    public void Attack(Vector3 target, CombatController controller)
    {
        if(type == WeaponType.Thrown)
        {
            charge = Time.time - chargeStart;
            if (charge > maxCharge) charge = maxCharge;

            Drop(controller);

            StartCoroutine(_Attack(target));
        }
    }

    IEnumerator _Attack(Vector3 target)
    {
        yield return new WaitForEndOfFrame();
        transform.LookAt(target);
        body.AddForce(transform.forward * charge * speed, ForceMode.Impulse);
    }
}
