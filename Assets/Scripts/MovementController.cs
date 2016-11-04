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
    public GameObject[] physicsPoints;
    public float distanceToGround = 0.5f;

    Vector3 oldPosition;

    public bool drifting = false;
    public bool boosting = false;

    public float turn = 0;
    public float thrust = 0;

    private float origTurnSpeed;
    private float origTurnMax;
    private float origAcceleration;
    private float origTopSpeed;

    public Vector3 velDirection;
    private Vector3 oldVelDirection;
    private Vector3 oldVelocity;
    public PlayerController.ControlType controlType = PlayerController.ControlType.Keyboard;

    public Vector3 moveDirection = Vector3.zero;

    LayerMask groundmask;
    LayerMask obstacleMask;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody>();
        groundmask = 1 << LayerMask.NameToLayer("Terrain");
        obstacleMask = 1 << LayerMask.NameToLayer("Obstacle");
        oldPosition = transform.position;
        origTurnMax = turnMax;
        origTurnSpeed = turnSpeed;
        origAcceleration = acceleration;
        origTopSpeed = topSpeed;
	}

    // Update is called once per frame
    void Update() {
        moveDirection.x = turn;
        moveDirection.z = thrust;

        if (boosting)
        {
            topSpeed = origTopSpeed * 1.5f;
        }
        else
        {
            topSpeed = origTopSpeed;
        }

        if (/*controlType == PlayerController.ControlType.Keyboard*/true)
        {
            if (drifting && Grounded())
            {
                turnSpeed = origTurnSpeed * 2;
                turnMax = origTurnMax * 3;

                Drift();
                Turn();
            }
            else
            {
                turnSpeed = origTurnSpeed;
                turnMax = origTurnMax;

                Move();
                Turn();
            }
        }
        else
        {
            if (drifting && Grounded())
            {
                turnSpeed = origTurnSpeed * 2;
                turnMax = origTurnMax * 3;

                MoveInDirection();
            }
            else
            {
                turnSpeed = origTurnSpeed;
                turnMax = origTurnMax;

                MoveInDirection();
            }
        }
    }

    void Drift()
    {
        velocity *= decellerationFactor + (decellerationFactor / 100.0f);
        body.MovePosition(transform.position + (velDirection.normalized * velocity) * Time.deltaTime * 0.66f);
    }

    void MoveInDirection()
    {
        thrust *= decellerationFactor;
        thrust += moveDirection.magnitude * acceleration;

        if (moveDirection.magnitude > 0)
        {
            Vector3 targetDir = Vector3.RotateTowards(transform.forward, moveDirection, turnSpeed * 0.01f, turnMax * 0.1f);
            transform.rotation = Quaternion.LookRotation(targetDir);
        }
        body.MovePosition(transform.position + transform.forward * thrust * 0.2f);

        /*        velocity *= decellerationFactor;
                if (thrust < 0)
                {
                    thrust = 0;
                }

                //Calculate velocity
                velocity += (thrust * acceleration);
                //Limit the velocity
                if (velocity > topSpeed) velocity = topSpeed;
                else if (velocity < -topSpeed) velocity = -topSpeed;
                //If were moving forward calculate the direction to go in
                velDirection = (transform.forward * velocity + velDirection) / 2;

                Vector3 targetPosition = transform.position + (velDirection.normalized * velocity * Time.deltaTime * 0.66f);
                body.MovePosition(targetPosition);
                oldPosition = transform.position;
                oldVelocity = oldPosition - transform.position;
        */
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
        //Limit the velocity
        if (velocity > topSpeed) velocity = topSpeed;
        else if (velocity < -topSpeed) velocity = -topSpeed;
        Vector3 flatForward = transform.forward;
        flatForward.y *= 0.33f;
        //If were moving forward calculate the direction to go in
        velDirection = (flatForward * velocity + velDirection) / 2;

        Vector3 targetPosition = transform.position + (velDirection.normalized * velocity * Time.deltaTime * 0.66f);
        if (!Physics.Raycast(transform.position, transform.position - targetPosition, 2, obstacleMask)) {
            body.MovePosition(targetPosition);
        }
        oldPosition = transform.position;
        oldVelocity = oldPosition - transform.position;
    }

    void Turn()
    {
        turnVel *= turnDeceleration;
        if (!boosting)
        {
            turnVel += turn * turnSpeed;
            if (turnVel > turnMax) turnVel = turnMax;
            else if (turnVel < -turnMax) turnVel = -turnMax;
        }
        else
        {
            turnVel += turn * turnSpeed * 0.2f;
            if (turnVel > turnMax) turnVel = turnMax * 0.2f;
            else if (turnVel < -turnMax) turnVel = -turnMax * 0.2f;
        }
        body.MoveRotation(transform.rotation * Quaternion.Euler(new Vector3(0, turnVel * 0.03f, 0)));
        if (Grounded())
        {
            TurnBoard(turn);
        }
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

    void ResetMovement()
    {
        turn = 0;
        thrust = 0;
        velDirection = Vector3.zero;
        oldPosition = transform.position;
        oldVelocity = Vector3.zero;
    }


}
