using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum UnitType
{
    GRUNT,
    ARCHER,
    ASSASIN,
}

public enum UnitColor
{
    RED,
    BROWN,
    BLUE,
    GREEN,
    YELLOW,
}

public class UnitController : MonoBehaviour
{
    public enum CombatState
    {
        IDLE,
        MOVE_TO_TARGET,
        ATTACK,
    }

    [Header("Class Properties ----------------------------")]
    public UnitType unitType;
    public UnitColor unitColor;

    [Header("Unit Properties ----------------------------")]
    public float HP;
    public LayerMask TargetableLayers;
    public int AttackPower;
    public float AttackDistance;
    public float Speed;

    private CombatState State;
    private UnitController CurrentTarget;
    private Vector3 LastTargetPosition;
    private NavMeshAgent Agent;
    private float WaitTimer;
    private float FindNewTargetTimer;

    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Agent.speed = Speed;

        UIController.UpdateSpawnCount(unitColor, false);

        // Update your properties based on the time of day
        switch (DayCycleController.Instance.CurrentTimeOfDay)
        {
            case TIMEOFDAY.NONE:
                break;

            case TIMEOFDAY.MORNING:
                break;

            case TIMEOFDAY.AFTERNOON:
                if (unitType == UnitType.GRUNT) AttackPower++;
                break;

            case TIMEOFDAY.EVENING:
                if (unitType == UnitType.ASSASIN) Speed += Random.Range(0f,2f);
                break;
        }
    }

    void Update()
    {
        switch (State)
        {
            case CombatState.IDLE:
                FindTarget();
                break;

            case CombatState.MOVE_TO_TARGET:
                MoveToTarget();
                break;

            case CombatState.ATTACK:
                Attack();
                break;
        }
    }

    void FindTarget()
    {
        // Randomly pick an enemy unit to attack
        if (!CurrentTarget)
        {
            Collider[] Hits = Physics.OverlapSphere(transform.position, 150f, TargetableLayers);
            if (Hits.Length > 0)
            {
                CurrentTarget = Hits[Random.Range(0, Hits.Length)].GetComponent<UnitController>();
            }
        }

        //begin chasing your target
        if(CurrentTarget)
        {
            State = CombatState.MOVE_TO_TARGET;
            LastTargetPosition = CurrentTarget.transform.position;
            Agent.SetDestination(LastTargetPosition);
        }
    }

    void MoveToTarget()
    {
        // Reset and look for a new target if your target is killed
        if (!CurrentTarget)
        {
            State = CombatState.IDLE;
        }

        // If your close enough to your target, attack
        else if (Vector3.Distance(transform.position, CurrentTarget.transform.position) < AttackDistance)
        {
            State = CombatState.ATTACK;
        }

        else
        {
            // Only re-path if your target moves too far
            if (Vector3.Distance(CurrentTarget.transform.position, LastTargetPosition ) > 1f)
            {
                LastTargetPosition = CurrentTarget.transform.position;
                Agent.SetDestination(LastTargetPosition);
            }

            // If you take wy to long to reach your target, look for someone else,keep the battle moving
            FindNewTargetTimer += Time.deltaTime;
            if(FindNewTargetTimer > 5f)
            {
                FindNewTargetTimer = 0f;
                State = CombatState.IDLE;

            }
        }
    }

    void Attack()
    {
        // Deal damage to your target every 0.5 seconds
        if (CurrentTarget)
        {
            if(WaitTimer <= 0)
            {
                CurrentTarget.ApplyDamage(AttackPower);
                WaitTimer = 0.5f;
                State = CombatState.MOVE_TO_TARGET;
            }
            else
            {
                WaitTimer -= Time.deltaTime;
            }
        }

        // Reset and look for a new target if your target is killed
        else
        {
            State = CombatState.IDLE;
        }
    }

    // Get damaged by another unit
    public void ApplyDamage(float Damage)
    {
        HP -= Damage;

        if(HP <= 0 )
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if(UIController.Instance != null)
            UIController.UpdateSpawnCount(unitColor, true);
    }
}
