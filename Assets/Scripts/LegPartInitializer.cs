using UnityEngine;

public class LegPartInitializer : MonoBehaviour
{
    public enum LegPartType { Thigh, Shin, Foot }

    [Header("Part Settings")]
    public LegPartType partType;
    public float mass = 1f;
    public float drag = 0.5f;
    public float angularDrag = 0.5f;

    public void InitializePart(PhysicMaterial highFriction, PhysicMaterial lowFriction)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        rb.mass = mass;
        rb.drag = drag;
        rb.angularDrag = angularDrag;
        rb.interpolation = RigidbodyInterpolation.None;

        // Настройка коллайдеров в зависимости от типа части
        switch (partType)
        {
            case LegPartType.Thigh:
                SetupThighCollider(rb, highFriction);
                break;
            case LegPartType.Shin:
                SetupShinCollider(rb, highFriction);
                break;
            case LegPartType.Foot:
                SetupFootCollider(rb, highFriction);
                break;
        }
    }

    void SetupThighCollider(Rigidbody rb, PhysicMaterial material)
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider == null) collider = gameObject.AddComponent<BoxCollider>();
        
        collider.size = new Vector3(0.1f, 0.3f, 0.1f);
        collider.material = material;
    }

    void SetupShinCollider(Rigidbody rb, PhysicMaterial material)
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider == null) collider = gameObject.AddComponent<BoxCollider>();
        
        collider.size = new Vector3(0.08f, 0.25f, 0.08f);
        collider.material = material;
    }

    void SetupFootCollider(Rigidbody rb, PhysicMaterial material)
    {
        SphereCollider collider = GetComponent<SphereCollider>();
        if (collider == null) collider = gameObject.AddComponent<SphereCollider>();
        
        collider.radius = 0.05f;
        collider.material = material;
    }
}