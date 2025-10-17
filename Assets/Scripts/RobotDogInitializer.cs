using UnityEngine;

public class RobotDogInitializer : MonoBehaviour
{
    [Header("Physical Settings")]
    public PhysicMaterial highFrictionMaterial;
    public PhysicMaterial lowFrictionMaterial;

    void Start()
    {
        InitializePhysics();
    }

    void InitializePhysics()
    {
        // Настройка Rigidbody для тела
        Rigidbody bodyRb = GetComponent<Rigidbody>();
        bodyRb.mass = 10f;
        bodyRb.drag = 0.5f;
        bodyRb.angularDrag = 2f;
        bodyRb.interpolation = RigidbodyInterpolation.Interpolate;
        bodyRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // Настройка коллайдера тела
        BoxCollider bodyCollider = GetComponent<BoxCollider>();
        if (bodyCollider != null)
        {
            bodyCollider.material = highFrictionMaterial;
        }

        // Инициализация всех ног
        InitializeAllLegs();
    }

    void InitializeAllLegs()
    {
        LegPartInitializer[] legParts = GetComponentsInChildren<LegPartInitializer>();
        foreach (var part in legParts)
        {
            part.InitializePart(highFrictionMaterial, lowFrictionMaterial);
        }
    }
}  
