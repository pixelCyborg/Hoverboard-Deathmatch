using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    MovementController movement;
    CombatController combat;

	// Use this for initialization
	void Start () {
        movement = GetComponent<MovementController>();
        combat = GetComponent<CombatController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            movement.drifting = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            movement.drifting = false;
        }

        if(Input.GetMouseButtonDown(0))
        {
            combat.Charge();
        }
        if(Input.GetMouseButtonUp(0))
        {
            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.y = transform.position.y;
            combat.Attack(target);
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            combat.Drop();
        }

        movement.turn = Input.GetAxis("Horizontal");
        movement.thrust = Input.GetAxis("Vertical");

    }
}
