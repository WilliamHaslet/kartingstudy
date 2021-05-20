using System.Collections.Generic;
using UnityEngine;

public class CartMovement : MonoBehaviour
{

    private class AdditionalVelocity
    {
        public Vector3 velocity;
        public Vector3 direction;

        public AdditionalVelocity(Vector3 startVelocity)
        {
            velocity = startVelocity;
            direction = velocity.normalized;
        }

        public void ClearVelocityY()
        {
            velocity.y = 0;
            direction.y = 0;
            direction = direction.normalized;
        }

        public void ApplyDrag(float drag)
        {
            velocity -= direction * drag;
        }
    }

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float reciprocalInputAcceleration;
    [SerializeField] private float reciprocalBrakeDeceleration;
    [SerializeField] private float gravity;
    [SerializeField] private float restGravity;
    [Header("Turning")]
    [SerializeField] private AnimationCurve turnSpeedCurve;
    [SerializeField, HideInInspector] private Rect turnSpeedCurveRange;
    [SerializeField] private AnimationCurve wheelTurnCurve;
    [SerializeField, HideInInspector] private Rect wheelTurnCurveRange;
    [SerializeField] private float wheelRadius;
    [SerializeField] private float minimumTurnSpeed;
    [SerializeField] private Transform cartCenter;
    [SerializeField] private Transform[] wheels;
    [Header("Drift")]
    [SerializeField] private Transform driftCenter;
    [SerializeField] private AnimationCurve outerDriftCurve;
    [SerializeField, HideInInspector] private Rect outerDriftCurveRange;
    [SerializeField] private AnimationCurve innerDriftCurve;
    [SerializeField, HideInInspector] private Rect innerDriftCurveRange;
    [SerializeField] private float driftWheelTurn;
    [SerializeField] private float driftWheelOffset;
    [SerializeField] private float driftEvaluationTime;
    [SerializeField] private float driftRecoveryTime;
    [SerializeField] private float driftTurnAngle;
    [SerializeField] private float driftTurnAngleSmoothness;
    [SerializeField] private ParticleSystem[] driftEffects;
    [Header("Collision")]
    [SerializeField] private float groundCastDistance;
    [SerializeField] private LayerMask groundLayers;
    [Header("Add Velocity")]
    [SerializeField] private float groundDrag;
    [SerializeField] private float airDrag;
    [Header("Body")]
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private float bodyAccelerationStength;
    [SerializeField] private float bodyAccelerationSmoothness;
    [SerializeField] private float bodyTurnStength;
    [SerializeField] private float bodyDriftTurnStength;
    [SerializeField] private float bodyTurnSmoothness;
    [Header("Debug")]
    [SerializeField] private bool debugDrift;

    private Rigidbody characterRigidbody;
    private BoostManager boostManager;
    private Vector3 gravityDownDirection;
    private Vector2 input;
    private Vector3 gravityVelocity;
    private Vector3 inputVelocity;
    private Vector3 targetBodyRotation;
    private Vector3 currentBodyRotation;
    private Vector3 bodyRotationRef;
    private Vector3 debugDriftStartPosition;
    private float debugDriftStartRotation;
    private float driftCenterRotationRef;
    private float driftCenterRotationTarget;
    private float currentDriftCenterRotation;
    private List<AdditionalVelocity> addVelocities = new List<AdditionalVelocity>();
    private float wheelRotation;
    private float driftTime;
    private float moveDirection = 1;
    private float cartCenterRotation;
    private int driftDirection;
    private float lastXInput;
    private bool grounded;
    private bool canMove = true;

    private void Start()
    {

        characterRigidbody = GetComponent<Rigidbody>();

        boostManager = GetComponent<BoostManager>();

    }

    private void Update()
    {

        RotateToGround();

        currentBodyRotation.x = Mathf.SmoothDampAngle(currentBodyRotation.x, targetBodyRotation.x, ref bodyRotationRef.x, bodyAccelerationSmoothness);

        currentBodyRotation.z = Mathf.SmoothDampAngle(currentBodyRotation.z, targetBodyRotation.z, ref bodyRotationRef.z, bodyTurnSmoothness);

        bodyTransform.localRotation = Quaternion.Euler(currentBodyRotation);

        float lastDriftCenterRotation = currentDriftCenterRotation;

        currentDriftCenterRotation = Mathf.SmoothDampAngle(currentDriftCenterRotation, driftCenterRotationTarget, ref driftCenterRotationRef, driftTurnAngleSmoothness);

        driftCenter.localRotation = Quaternion.Euler(0, currentDriftCenterRotation, 0);

        SpinWheels();

        if (driftDirection == 0)
        {

            cartCenter.rotation *= Quaternion.Euler(0, (cartCenterRotation * Time.deltaTime) + (lastDriftCenterRotation - currentDriftCenterRotation), 0);

        }
        else
        {

            cartCenter.rotation *= Quaternion.Euler(0, cartCenterRotation * Time.deltaTime, 0);

        }

        if (driftDirection != 0 && moveDirection == 1 && inputVelocity.magnitude >= minimumTurnSpeed)
        {

            float boostJuiceMultiplier;

            if (driftDirection == 1)
            {

                boostJuiceMultiplier = input.x;

            }
            else
            {

                boostJuiceMultiplier = -input.x;

            }

            boostManager.AddBoostJuice(boostJuiceMultiplier);

        }

        if (driftDirection != 0 && moveDirection == 1 && inputVelocity.magnitude >= minimumTurnSpeed && !driftEffects[0].isPlaying)
        {

            foreach (ParticleSystem driftEffect in driftEffects)
            {

                driftEffect.Play();

            }

        }
        else if ((driftDirection == 0 || moveDirection == -1 || inputVelocity.magnitude < minimumTurnSpeed) && driftEffects[0].isPlaying)
        {

            foreach (ParticleSystem driftEffect in driftEffects)
            {

                driftEffect.Stop();

            }

        }

        //AdjustBoost();

        if (debugDrift)
        {

            DrawDebugDriftLines();

        }

    }

    private void FixedUpdate()
    {

        if (canMove)
        {

            CalculateInputVelocity();

            CalculateTurn();

            ApplyGravity();

            characterRigidbody.velocity = inputVelocity + gravityVelocity + (cartCenter.forward * boostManager.GetCurrentBoost());

            ApplyAditionalVelocities();

        }

    }

    private void RotateToGround()
    {

        if (Physics.Raycast(transform.position, -cartCenter.up, out RaycastHit rayHit, groundCastDistance, groundLayers))
        {

            grounded = true;

            cartCenter.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(cartCenter.forward, rayHit.normal), rayHit.normal);

            gravityDownDirection = -cartCenter.up;

        }
        else
        {

            grounded = false;

            gravityDownDirection = Vector3.down;

            //cartCenter.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(cartCenter.forward, Vector3.up), Vector3.up);

        }

        inputVelocity = Vector3.ProjectOnPlane(inputVelocity, cartCenter.up);

    }

    private void CalculateInputVelocity()
    {

        Vector3 deltaVelocity = Vector3.zero;

        deltaVelocity.x = -Vector3.Dot(inputVelocity, cartCenter.right);

        deltaVelocity.z = (input.y * moveSpeed) - Vector3.Dot(inputVelocity, cartCenter.forward);

        if (deltaVelocity.magnitude < 0.1f)
        {

            deltaVelocity = Vector3.ClampMagnitude(deltaVelocity, Time.deltaTime / reciprocalInputAcceleration);

        }
        else
        {

            Vector3 localDeltaVelocity = Vector3.zero;

            localDeltaVelocity += deltaVelocity.z * cartCenter.forward;
            localDeltaVelocity += deltaVelocity.x * cartCenter.right;
            localDeltaVelocity += deltaVelocity.y * cartCenter.up;

            bool accelerating = Vector3.Dot(localDeltaVelocity, inputVelocity) > 0;

            if (input.y != 0 && !accelerating)
            {

                deltaVelocity = Vector3.ClampMagnitude(deltaVelocity, Time.deltaTime / reciprocalBrakeDeceleration);

            }
            else
            {

                deltaVelocity = Vector3.ClampMagnitude(deltaVelocity, Time.deltaTime / reciprocalInputAcceleration);

            }

        }

        Vector3 lastInputVelocity = inputVelocity;

        inputVelocity += (cartCenter.right * deltaVelocity.x) + (cartCenter.forward * deltaVelocity.z);

        moveDirection = Mathf.Sign(Vector3.Dot(inputVelocity.normalized, cartCenter.forward));

        targetBodyRotation.x = (lastInputVelocity.magnitude - inputVelocity.magnitude) * bodyAccelerationStength * moveDirection;

    }

    private void ApplyGravity()
    {

        if (grounded)
        {

            if (gravityVelocity.y < 0)
            {

                for (int i = 0; i < addVelocities.Count; i++)
                {

                    addVelocities[i].ClearVelocityY();

                }

            }

            if (input != Vector2.zero && grounded)
            {

                inputVelocity += -cartCenter.up * restGravity;

            }

            gravityVelocity = Vector3.zero;

        }
        else
        {

            //gravityVelocity += -cartCenter.up * gravity * Time.deltaTime;
            gravityVelocity += gravityDownDirection * gravity * Time.deltaTime;

        }

    }

    private void ApplyAditionalVelocities()
    {

        for (int i = addVelocities.Count - 1; i >= 0; i--)
        {

            characterRigidbody.velocity += addVelocities[i].velocity;

            if (grounded)
            {

                addVelocities[i].ApplyDrag(groundDrag * Time.deltaTime);

            }
            else
            {

                addVelocities[i].ApplyDrag(airDrag * Time.deltaTime);

            }

            if (Vector3.Dot(addVelocities[i].velocity.normalized, addVelocities[i].direction) < 0.99f)
            {

                addVelocities.RemoveAt(i);

            }

        }

    }

    private void SpinWheels()
    {

        float wheelCircumfrence = 2 * Mathf.PI * wheelRadius;

        float revolutions = (inputVelocity.magnitude * Time.deltaTime) / wheelCircumfrence;

        wheelRotation += revolutions * 360 * moveDirection;

        wheelRotation %= 360;

        float wheelTurn;

        if (driftDirection == 0 || moveDirection == -1)
        {

            wheelTurn = input.x * wheelTurnCurve.Evaluate(inputVelocity.magnitude / moveSpeed);

        }
        else
        {

            wheelTurn = (input.x * driftWheelTurn) + (driftWheelOffset * driftDirection);

        }

        for (int i = 0; i < wheels.Length; i++)
        {

            if (i < 2)
            {

                wheels[i].localRotation = Quaternion.Euler(wheelRotation, wheelTurn, 0);

            }
            else
            {

                wheels[i].localRotation = Quaternion.Euler(wheelRotation, 0, 0);

            }

        }

    }

    private void CalculateTurn()
    {

        if (inputVelocity.magnitude >= minimumTurnSpeed)
        {

            float turnAngle;

            if (driftDirection == 0 || moveDirection == -1)
            {

                driftCenterRotationTarget = 0;

                turnAngle = turnSpeedCurve.Evaluate(inputVelocity.magnitude / moveSpeed) * input.x;

                targetBodyRotation.z = turnAngle * bodyTurnStength;

                driftTime -= Time.deltaTime / driftRecoveryTime;

                if (driftTime < 0)
                {

                    driftTime = 0;

                }

            }
            else
            {

                float driftLerp = (input.x + 1) / 2;

                if (driftDirection == 1)
                {

                    driftLerp = 1 - driftLerp;

                }

                turnAngle = Mathf.Lerp(innerDriftCurve.Evaluate(driftTime), outerDriftCurve.Evaluate(driftTime), driftLerp) * driftDirection;

                driftTime += Time.deltaTime / driftEvaluationTime;

                if (driftTime > 1)
                {

                    driftTime = 1;

                }

                driftCenterRotationTarget = driftTurnAngle * driftDirection;

                targetBodyRotation.z = turnAngle * bodyDriftTurnStength;

            }

            //cartCenter.rotation *= Quaternion.Euler(0, turnAngle * Mathf.Sign(input.y) * Time.deltaTime, 0);
            cartCenterRotation = turnAngle * Mathf.Sign(moveDirection);

        }
        else
        {

            targetBodyRotation.z = 0;

            cartCenterRotation = 0;

            driftCenterRotationTarget = 0;

        }

        if (moveDirection == 1)
        {

            inputVelocity = Vector3.RotateTowards(inputVelocity, cartCenter.forward, 10, 0);

        }
        else
        {

            inputVelocity = Vector3.RotateTowards(inputVelocity, -cartCenter.forward, 10, 0);

        }

    }

    public void Move(Vector2 movementInput)
    {

        if (input.x == 0 && movementInput.x != 0)
        {

            lastXInput = movementInput.x;

        }

        input = movementInput;

    }

    public void Drift(bool drift)
    {

        if (drift && driftDirection == 0)
        {

            if (input.x == 0)
            {

                driftDirection = (int)Mathf.Sign(lastXInput);

            }
            else
            {

                driftDirection = (int)Mathf.Sign(input.x);

            }

            //driftTime = 0;

            debugDriftStartPosition = transform.position;

            debugDriftStartRotation = 90 - cartCenter.eulerAngles.y;

        }
        else if (!drift)
        {

            driftDirection = 0;

        }

    }

    public void AddVelocity(Vector3 velocity)
    {

        addVelocities.Add(new AdditionalVelocity(velocity));

    }

    public void SetCanMove(bool move)
    {

        canMove = move;

    }

    public void ClearVelocity()
    {

        characterRigidbody.velocity = Vector3.zero;

        inputVelocity = Vector3.zero;

        addVelocities.Clear();

    }
    
    public void SetCartRotation(Quaternion rotation)
    {

        cartCenter.localRotation = rotation;

        cartCenterRotation = 0;

    }

    private void DrawDebugDriftLines()
    {

        void DrawDriftCurve(float lerp, Color color, float debugLineRotation)
        {

            Vector3 debugLinePosition = debugDriftStartPosition;

            float deltaTime = 0.02f;

            for (float i = 0; i <= 1; i += deltaTime / driftEvaluationTime)
            {

                debugLineRotation -= Mathf.Lerp(innerDriftCurve.Evaluate(i), outerDriftCurve.Evaluate(i), lerp) * deltaTime;

                Vector3 debugLineDirection = new Vector3(Mathf.Cos(debugLineRotation * Mathf.Deg2Rad), 0, Mathf.Sin(debugLineRotation * Mathf.Deg2Rad));

                Vector3 newDebugLinePosition = debugLinePosition + (debugLineDirection * (moveSpeed * deltaTime));

                Debug.DrawLine(debugLinePosition, newDebugLinePosition, color);

                debugLinePosition = newDebugLinePosition;

            }

        }

        if (inputVelocity.magnitude < minimumTurnSpeed)
        {

            DrawDriftCurve(0, Color.blue, 90);

            DrawDriftCurve(1, Color.blue, 90);

            DrawDriftCurve(0.5f, Color.green, 90);

        }
        else
        {

            float driftLerp = (input.x + 1) / 2;

            if (driftDirection == 1)
            {

                driftLerp = 1 - driftLerp;

            }

            DrawDriftCurve(0, Color.blue, debugDriftStartRotation);

            DrawDriftCurve(1, Color.blue, debugDriftStartRotation);

            DrawDriftCurve(driftLerp, Color.green, debugDriftStartRotation);

        }

    }

}
