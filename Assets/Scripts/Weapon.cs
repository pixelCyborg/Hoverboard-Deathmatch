using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
    public enum WeaponType
    {
        Melee, Thrown, Ball
    }

    public WeaponType type;
    public float damage;
    public float speed;
    public Collider myCollider;
    private Rigidbody body;

    public float maxCharge = 50.0f;
    private float chargeStart;
    private float charge;
    public Vector3 lastTarget;
    public bool active = false;
    public Vector3 lastLocation;
    private Transform assignedSlot;
    Collider[] ignoringColliders;
    bool weaponUsed = false;

    Vector3 oldVelocity;

    void Start()
    {
        //myCollider = GetComponent<Collider>();
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(assignedSlot != null)
        {
            transform.position = assignedSlot.position;
            transform.rotation = assignedSlot.rotation;
        }

        lastLocation = transform.position;
        oldVelocity = body.velocity;
    }

    void OnCollisionEnter(Collision col)
    {
        if(active && col.transform.tag == "Ground")
        {
            StartCoroutine(SetInactive());
        }

        if (col.transform.tag == "Player" && active)
        {
            CombatController combat = col.transform.GetComponentInParent<CombatController>();
            combat.Damage(GetDamage(combat, col));
        }
    }

    IEnumerator SetInactive()
    {
        yield return new WaitForSeconds(1.0f);
        active = false;
        IgnoreCollisionWithUser(false);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag == "Player")
        {
            CombatController controller = col.transform.GetComponent<CombatController>();
            if (controller != null && controller.equippedWeapon == null)
            {
                PickUp(controller);
            }
        }
    }

    public void PickUp(CombatController controller)
    {
        if (!active && assignedSlot == null && !weaponUsed)
        {
            transform.SetParent(null);
            body.useGravity = false;
            myCollider.isTrigger = true;
            body.constraints = RigidbodyConstraints.FreezeAll;
            assignedSlot = controller.weaponSlot;
            controller.SetWeapon(this);
        }
    }

    public void Drop(CombatController controller)
    {
        body.useGravity = true;
        body.constraints = RigidbodyConstraints.None;
        assignedSlot = null;
        myCollider.isTrigger = false;
        active = false;
        IgnoreCollisionWithUser(false);
    }

    public void Charge()
    {
        chargeStart = Time.time;
    }

    public void Attack(Vector3 target, CombatController controller)
    {
        if(type == WeaponType.Thrown || type == WeaponType.Ball)
        {
            charge = (Time.time - chargeStart) * 3;
            if (charge > maxCharge) charge = maxCharge;

            Drop(controller);
            StartCoroutine(_Attack(target, controller));
        }
    }

    IEnumerator _Attack(Vector3 target, CombatController controller)
    {
        yield return new WaitForEndOfFrame();
        //target = target.normalized * 20;
        transform.LookAt(target);
        body.AddForce(transform.forward * charge * speed, ForceMode.Impulse);
        lastTarget = target;
        active = true;
        IgnoreCollisionWithUser(controller, true);
        if (GetComponent<CapsuleCollider>())
        {
            GetComponent<CapsuleCollider>().radius = 2f;
        }
    }

    public float GetDamage(CombatController controller, Collision col)
    {
        controller.hitSlot = col.contacts[0].otherCollider.transform;
        assignedSlot = null;
        StartCoroutine(_GetDamage(controller, col.relativeVelocity));
        return damage;
    }

    IEnumerator _GetDamage(CombatController controller, Vector3 velocity)
    {
        if (type == WeaponType.Thrown)
        {
            transform.SetParent(controller.hitSlot);
            body.useGravity = false;
            myCollider.enabled = false;
            body.velocity = oldVelocity;
            yield return new WaitForSeconds(0.05f);
            body.velocity = Vector3.zero;
            body.isKinematic = true;
        }
        yield return new WaitForEndOfFrame();

        Rigidbody[] hitBodies = controller.GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < hitBodies.Length; i++)
        {
           hitBodies[i].AddExplosionForce(50, transform.position, 25, 0, ForceMode.Impulse);
        }

        if (type == WeaponType.Thrown)
        {
            yield return new WaitForSeconds(0.9f);
            weaponUsed = true;
            active = false;
            MapController.SpawnSpear();
        }
    }

    void IgnoreCollisionWithUser(CombatController controller, bool shouldIgnore)
    {
        ignoringColliders = controller.GetComponentsInChildren<Collider>();
        IgnoreCollisionWithUser(shouldIgnore);
    }
    void IgnoreCollisionWithUser(bool shouldIgnore)
    {
        if (GetComponent<CapsuleCollider>())
        {
            GetComponent<CapsuleCollider>().radius = 0.5f;
        }
        if (ignoringColliders != null)
        {
            for (int i = 0; i < ignoringColliders.Length; i++)
            {
                Physics.IgnoreCollision(myCollider, ignoringColliders[i], shouldIgnore);
            }
        }
    }
}
