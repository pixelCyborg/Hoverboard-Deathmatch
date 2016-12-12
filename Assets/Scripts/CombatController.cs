using UnityEngine;
using System.Collections;

public class CombatController : MonoBehaviour {
    public Transform weaponSlot;
    public Weapon equippedWeapon;
    public float health;
    public Transform hitSlot;
    private Goal _goal;
    public Transform lastUsedWeapon;
    Animator anim;
    AudioSource source;

    private Goal goal()
    {
        if (_goal == null)
        {
            _goal = FindObjectOfType<Goal>();
        }
        return _goal;
    }

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        source = GetComponent<AudioSource>();
    }

    public void SetWeapon(Weapon weapon)
    {
        lastUsedWeapon = null;
        equippedWeapon = weapon;
    }

    public void Damage(float damage, CombatController lastUser)
    {
        health -= damage;
        if (health <= 0)
        {
            if(Goal.mode == Goal.GameMode.Deathmatch)
            {
                FindObjectOfType<Goal>().Score(lastUser);
            }
            Die();
        }
    }

    public void OnCollisionEnter(Collision col)
    {
        Weapon weapon = col.transform.GetComponent<Weapon>();
    }

    public void Die()
    {
        if (equippedWeapon != null) Drop();
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<MovementController>().enabled = false;
        GetComponentInChildren<RagdollController>().Ragdoll();
        transform.tag = "Untagged";
        StartCoroutine(WaitForRespawn());
    }

    IEnumerator WaitForRespawn()
    {
        yield return new WaitForSeconds(3.0f);
        Respawn();
    }

    void Respawn()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
        GetComponent<MovementController>().enabled = true;
        GetComponentInChildren<RagdollController>().UnRagdoll();
        transform.tag = "Player";
        health = 100;
        equippedWeapon = null;
        transform.position = MapController.RandomSpawnPosition();
        Weapon[] impales = GetComponentsInChildren<Weapon>();
        for(int i = 0; i < impales.Length; i++)
        {
            Destroy(impales[i].gameObject);
        }
    }

    public void Attack(Vector3 target)
    {
        if (equippedWeapon != null)
        {
            anim.SetTrigger("Throw");
            source.Play();
            StartCoroutine(_Attack(target));
        }
    }

    IEnumerator _Attack(Vector3 target)
    {
        yield return new WaitForSeconds(0.05f);
        if (equippedWeapon != null)
        {
            lastUsedWeapon = equippedWeapon.transform;
            equippedWeapon.Attack(target, this);
            equippedWeapon = null;
        }
    }

    public void Charge()
    {
        if(equippedWeapon != null)
            equippedWeapon.Charge();
    }

    public void Drop()
    {
        if (equippedWeapon != null)
        {
            equippedWeapon.Drop(this);
            equippedWeapon = null;
        }
    }
}
