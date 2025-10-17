using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotDogController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float walkCycleSpeed = 1f;
    public float legSwingAngle = 45f;
    public float legLiftHeight = 0.3f;

    [Header("Joint References")]
    public LegController frontLeftLeg;
    public LegController frontRightLeg;
    public LegController backLeftLeg;
    public LegController backRightLeg;

    private Rigidbody bodyRigidbody;
    private bool isWalking = true;
    private float walkCycleTime = 0f;

    void Start()
    {
        bodyRigidbody = GetComponent<Rigidbody>();
        InitializeLegs();
        StartWalking();
    }

    void InitializeLegs()
    {
        frontLeftLeg.Initialize(this);
        frontRightLeg.Initialize(this);
        backLeftLeg.Initialize(this);
        backRightLeg.Initialize(this);
    }

    void FixedUpdate()
    {
        if (isWalking)
        {
            UpdateWalkCycle();
            ApplyForwardMovement();
        }
    }

    void UpdateWalkCycle()
    {
        walkCycleTime += Time.fixedDeltaTime * walkCycleSpeed;
        
        // Диагональная походка: передняя левая + задняя правая синхронизированы
        float phase1 = Mathf.Sin(walkCycleTime * Mathf.PI * 2f);
        float phase2 = Mathf.Sin((walkCycleTime + 0.5f) * Mathf.PI * 2f);

        frontLeftLeg.UpdateLegMovement(phase1);
        backRightLeg.UpdateLegMovement(phase1);
        
        frontRightLeg.UpdateLegMovement(phase2);
        backLeftLeg.UpdateLegMovement(phase2);
    }

    void ApplyForwardMovement()
    {
        Vector3 forwardForce = transform.forward * moveSpeed;
        bodyRigidbody.AddForce(forwardForce, ForceMode.VelocityChange);
        
        // Стабилизация поворота
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward);
        bodyRigidbody.MoveRotation(Quaternion.Slerp(bodyRigidbody.rotation, targetRotation, Time.fixedDeltaTime * 2f));
    }

    public void StartWalking()
    {
        isWalking = true;
    }

    public void StopWalking()
    {
        isWalking = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Автоматическое восстановление при падении
        if (collision.gameObject.CompareTag("Ground") && Vector3.Dot(transform.up, Vector3.up) < 0.5f)
        {
            StartCoroutine(RecoverFromFall());
        }
    }

    IEnumerator RecoverFromFall()
    {
        StopWalking();
        yield return new WaitForSeconds(1f);
        
        // Медленное выравнивание
        float timer = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
        
        while (timer < 2f)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timer / 2f);
            timer += Time.deltaTime;
            yield return null;
        }
        
        StartWalking();
    }
}