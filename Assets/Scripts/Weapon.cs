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
    float origColRadius;

    public float maxCharge = 50.0f;
    private float chargeStart;
    private float charge;
    public Vector3 lastTarget;
    public bool active = false;
    public Vector3 lastLocation;
    private Transform assignedSlot;
    Collider[] ignoringColliders;
    bool weaponUsed = false;
    public CombatController lastUser;
    TrailRenderer trail;
    private float origColHeight;

    Vector3 oldVelocity;
    public GameObject explosion;

    void Start()
    {
        //myCollider = GetComponent<Collider>();
        body = GetComponent<Rigidbody>();
        trail = GetComponentInChildren<TrailRenderer>();
        origColRadius = ((CapsuleCollider)myCollider).radius;
        origColHeight = ((CapsuleCollider)myCollider).height;
    }

    void Update()
    {
        if(assignedSlot != null)
        {
            transform.position = assignedSlot.position;
            transform.rotation = assignedSlot.rotation;
        }
    }

    void FixedUpdate()
    {
        if (active)
        {
            RaycastHit hit;
            if(Physics.SphereCast(transform.position, 3.0f, transform.forward, out hit, 10.0f))
            {
                if (hit.transform.tag == "Player")
                {
                    if (hit.transform.GetComponentInParent<CombatController>() != lastUser)
                    {
                        CombatController combat = hit.transform.GetComponentInParent<CombatController>();
                        GetDamage(combat, hit);
                    }
                }
            }
           
        }

        lastLocation = transform.position;
        oldVelocity = body.velocity;
    }

    void OnCollisionEnter(Collision col)
    {
        if(active && col.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            Debug.Log("Setting inactive");
            StartCoroutine(SetInactive());
        }

        if (col.transform.tag == "Player" && active)
        {
            //Debug.Log("Collided with player");
            active = false;
            CombatController combat = col.transform.GetComponentInParent<CombatController>();
            GetDamage(combat, col);
        }
    }

    IEnumerator SetInactive()
    {
        yield return new WaitForSeconds(1.0f);
        active = false;
        if (trail)
        {
            trail.enabled = false;
        }
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
            if(trail == null)
            {
                trail = GetComponentInChildren<TrailRenderer>();
            }
            if (trail)
            {
                trail.enabled = true;
            }
            transform.SetParent(null);
            body.useGravity = false;
            myCollider.isTrigger = true;
            body.constraints = RigidbodyConstraints.FreezeAll;
            assignedSlot = controller.weaponSlot;
            controller.SetWeapon(this);
            lastUser = controller;
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
            GetComponent<CapsuleCollider>().radius = 2.0f;
            GetComponent<CapsuleCollider>().height = 10.0f;
        }
    }

    public void GetDamage(CombatController controller, Collision col)
    {
        Debug.Log(col.gameObject.name);
        controller.hitSlot = col.contacts[0].otherCollider.transform;
        StartCoroutine(_GetDamage(controller, col.relativeVelocity));
        assignedSlot = null;
        //return damage;aw
    }

    public void GetDamage(CombatController controller, RaycastHit hit)
    {
        Debug.Log(hit.collider.gameObject.name);
        controller.hitSlot = hit.transform;   //col.contacts[0].otherCollider.transform;
        StartCoroutine(_GetDamage(controller, body.velocity * hit.distance));
        assignedSlot = null;
        //return damage;
    }

    IEnumerator _GetDamage(CombatController controller, Vector3 velocity)
    {
        GetComponent<AudioSource>().PlayDelayed(0.25f);
        if (type == WeaponType.Thrown)
        {
            body.useGravity = false;
            myCollider.enabled = false;
            body.velocity = Vector3.zero;
            body.isKinematic = true;
            yield return new WaitForEndOfFrame();
            //transform.position = transform.position + transform.forward * 0.5f;
            transform.SetParent(controller.hitSlot);
            transform.localPosition = -Vector3.forward * 1.5f;
        }
        if (trail)
        {
            trail.enabled = false;
        }
        controller.Damage(damage, lastUser);
        yield return new WaitForSeconds(1.0f);
        GameObject explode = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
        explosion.transform.position = transform.position;
        yield return new WaitForEndOfFrame();

        /*Rigidbody[] hitBodies = controller.GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < hitBodies.Length; i++)
        {
           hitBodies[i].AddExplosionForce(50, transform.position, 25, 0, ForceMode.Impulse);
        }*/

        if (type == WeaponType.Thrown)
        {
            weaponUsed = true;
            active = false;
            MapController.SpawnSpear();
            Destroy(gameObject);
        }
        //yield return new WaitForSeconds(5);
        //Destroy(explosion);
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
            GetComponent<CapsuleCollider>().radius = origColRadius;
            GetComponent<CapsuleCollider>().height = origColHeight;
        }
        if (myCollider != null && ignoringColliders != null)
        {
            for (int i = 0; i < ignoringColliders.Length; i++)
            {
                Physics.IgnoreCollision(myCollider, ignoringColliders[i], shouldIgnore);
            }
        }
    }
}
