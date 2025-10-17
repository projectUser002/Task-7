using UnityEngine;

[System.Serializable]
public class LegController
{
    [Header("Leg Parts")]
    public Transform thigh;
    public Transform shin;
    public Transform foot;

    [Header("Joints")]
    public HingeJoint thighHinge;
    public HingeJoint shinHinge;
    public ConfigurableJoint thighConfigurable;

    private RobotDogController dogController;
    private Rigidbody thighRigidbody;
    private Rigidbody shinRigidbody;
    private Rigidbody footRigidbody;
    private Vector3 thighRestPosition;
    private Vector3 footRestPosition;

    public void Initialize(RobotDogController controller)
    {
        dogController = controller;
        
        thighRigidbody = thigh.GetComponent<Rigidbody>();
        shinRigidbody = shin.GetComponent<Rigidbody>();
        footRigidbody = foot.GetComponent<Rigidbody>();

        thighRestPosition = thigh.localPosition;
        footRestPosition = foot.localPosition;

        SetupJoints();
    }

    void SetupJoints()
    {
        // Настройка HingeJoint для бедра (плечо/бедро)
        if (thighHinge != null)
        {
            JointSpring spring = new JointSpring
            {
                spring = 1000f,
                damper = 100f,
                targetPosition = 0f
            };
            thighHinge.spring = spring;
            thighHinge.useSpring = true;
        }

        // Настройка HingeJoint для голени (колено/локоть)
        if (shinHinge != null)
        {
            JointSpring spring = new JointSpring
            {
                spring = 800f,
                damper = 80f,
                targetPosition = 30f // Слегка согнутое положение
            };
            shinHinge.spring = spring;
            shinHinge.useSpring = true;
        }

        // Настройка ConfigurableJoint для дополнительной стабилизации
        if (thighConfigurable != null)
        {
            // Ограничение движения для стабильности
            ConfigurableJointMotion motion = ConfigurableJointMotion.Limited;
            thighConfigurable.xMotion = motion;
            thighConfigurable.yMotion = motion;
            thighConfigurable.zMotion = motion;
        }
    }

    public void UpdateLegMovement(float phase)
    {
        if (thighHinge == null) return;

        // Параметры движения ноги
        float swing = Mathf.Sin(phase * Mathf.PI) * dogController.legSwingAngle;
        float lift = Mathf.Max(0, Mathf.Sin(phase * Mathf.PI)) * dogController.legLiftHeight;

        // Применение движения к бедру
        JointSpring thighSpring = thighHinge.spring;
        thighSpring.targetPosition = swing;
        thighHinge.spring = thighSpring;

        // Применение подъема ноги через силу
        Vector3 liftForce = thigh.up * lift * 50f;
        thighRigidbody.AddForce(liftForce);

        // Корректировка колена в зависимости от фазы
        if (shinHinge != null)
        {
            JointSpring shinSpring = shinHinge.spring;
            shinSpring.targetPosition = 30f + Mathf.Abs(phase) * 20f;
            shinHinge.spring = shinSpring;
        }
    }

    public void StabilizeLeg()
    {
        // Возвращение ноги в нейтральное положение
        if (thighHinge != null)
        {
            JointSpring spring = thighHinge.spring;
            spring.targetPosition = 0f;
            thighHinge.spring = spring;
        }
    }
}