using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMoveToWaypoint : BossMove
{
    private GameObject[] waypointObjects;
    private GameObject moveToWaypoint;
    private GameObject currentWaypoint;
    private NavMeshPath path;

    public BossMoveToWaypoint(Boss boss) : base(boss)
    {
        path = new NavMeshPath();
    }

    public override void OnStateEnter()
    {
        GetValidWaypointToMoveTo();

        Vector3 moveToPos = Vector3.zero;

        if (currentWaypoint != null)
        {
            moveToPos = currentWaypoint.transform.position;
        }
        else
        {
            moveToPos = boss.transform.position;
        }

        boss.bossNavMeshAgent.SetDestination(moveToPos);

        base.OnStateEnter();
    }

    public override void OnStateTransition()
    {
        base.OnStateTransition();
    }

    public override void OnStateUpdate()
    {
        ProcessMoveSpeedAndPlayMoveAnim();

        if (boss.bossNavMeshAgent.remainingDistance <= 0.45f)
        {
            SecondaryBoss secBoss = (SecondaryBoss)boss;
            secBoss.ResetPosChangeTime();
        }
    }

    private void GetValidWaypointToMoveTo()
    {
        waypointObjects = GameObject.FindGameObjectsWithTag("Waypoint");

        if (waypointObjects.Length > 0)
        {
            if (waypointObjects.Length == 1)
            {
                moveToWaypoint = waypointObjects[0];
                boss.bossNavMeshAgent.CalculatePath(waypointObjects[0].transform.position, path);
                if (path.status != NavMeshPathStatus.PathComplete) moveToWaypoint = null;
            }
            else
            {
                List<GameObject> waypointsWithValidPath = new List<GameObject>();

                for (int i = 0; i < waypointObjects.Length; i++)
                {
                    boss.bossNavMeshAgent.CalculatePath(waypointObjects[i].transform.position, path);
                    if (path.status != NavMeshPathStatus.PathComplete) continue;
                    waypointsWithValidPath.Add(waypointObjects[i]);
                }

                if (waypointsWithValidPath.Count == 0) moveToWaypoint = null;

                else if (waypointsWithValidPath.Count == 1) moveToWaypoint = waypointsWithValidPath[0];

                else
                {
                    moveToWaypoint = waypointsWithValidPath[Random.Range(0, waypointsWithValidPath.Count)];

                    while (currentWaypoint == moveToWaypoint)
                    {
                        moveToWaypoint = waypointsWithValidPath[Random.Range(0, waypointsWithValidPath.Count)];
                    }
                }
            }
        }
        else moveToWaypoint = null;

        currentWaypoint = moveToWaypoint;
    }
}
