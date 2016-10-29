using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour {
    public Transform board;
    public float turnSpeed = 1.0f;
    public float turnMax = 5.0f;
    public float acceleration = 5.0f;
    public float topSpeed = 50.0f;

    public float boostedFactor = 1.5f;

    public float decellerationFactor = 0.98f;
    public float turnDeceleration = 0.98f;

    private Rigidbody body;
    private float velocity = 0;
    private float turnVel = 0;
    private Vector3 velDirection;

    public GameObject[] physicsPoints;
    public float distanceToGround = 0.5f;

    Vector3 moveDirection = Vector3.zero;
    public bool drifting = false;

    public float turn = 0;
    public float thrust = 0;

    LayerMask groundmask;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody>();
        groundmask = 1 << LayerMask.NameToLayer("Terrain");
        moveDirection = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (drifting && Grounded())
        {
            Drift();
            Turn();
        }
        else
        {
            Move();
            Turn();
        }
    }
    void Drift()
    {
        body.MovePosition(transform.position + (velDirection.normalized * velocity) * 0.01f);
    }

    void Turn()
    {
        turnVel *= turnDeceleration;
        turnVel += turn * turnSpeed;
        if (turnVel > turnMax) turnVel = turnMax;
        else if (turnVel < -turnMax) turnVel = -turnMax;
        body.MoveRotation(transform.rotation * Quaternion.Euler(new Vector3(0, turnVel * 0.03f, 0)));
        if (Grounded())
        {
            TurnBoard(turn);
        }
    }

    void Move()
    {
        velocity *= decellerationFactor;

        if (thrust < 0)
        {
            thrust = 0;
        }

        //Calculate velocity
        velocity += (thrust * acceleration);
        if (velocity > topSpeed) velocity = topSpeed;
        else if (velocity < -topSpeed) velocity = -topSpeed;

        if (thrust > 0 || thrust < 0)
        {
            velDirection = ((transform.forward * thrust) + velDirection) /2;
        }

        Vector3 targetPosition = transform.position + (velDirection.normalized * velocity) * 0.01f;

        body.MovePosition(targetPosition);

        moveDirection = moveDirection - transform.position;
    }

    public float angularStability = 0.3f;
    public float correctionSpeed = 2.0f;
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 predictedUp = Quaternion.AngleAxis(
            body.angularVelocity.magnitude * Mathf.Rad2Deg * angularStability / correctionSpeed,
            body.angularVelocity
        ) * transform.up;
        Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
        body.AddTorque(torqueVector * correctionSpeed * correctionSpeed);
    }

    bool Grounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1f, groundmask);
    }

    void TurnBoard(float lean)
    {
        Vector3 boardRot = board.localRotation.eulerAngles;
        boardRot.z = -lean * 30;
        board.localRotation = Quaternion.Lerp(board.localRotation, Quaternion.Euler(boardRot), Time.deltaTime *2);
    }




}
