using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    // Start is called before the first frame update

    public float radius;
    [Range(0,360)]
    public float angle;

    public float backRadius=2f;
    public float backAngle=360f;

    public GameObject player;
    public LayerMask targetMask;
    public LayerMask groundMask;

    public bool seesPlayer;
    public bool seesPlayerClose;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        float delay = 0.2f;
        WaitForSeconds wait = new WaitForSeconds(delay);

        while(true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionTarget) < angle / 2)
            {
                float distanceTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionTarget, distanceTarget, groundMask))
                {
                    seesPlayer = true;
                }
                else
                {
                    seesPlayer = false;
                }
            }
            else { seesPlayer = false; }
        }
        else if (seesPlayer == true)
        {
            seesPlayer = false;
        }



        Collider[] rangeBackChecks = Physics.OverlapSphere(transform.position, backRadius, targetMask);
        if (rangeBackChecks.Length != 0)
        {
            Transform backTarget = rangeBackChecks[0].transform;
            Vector3 backDirectionTarget = (backTarget.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, backDirectionTarget) < backAngle / 2)
            {
                float backDistanceTarget = Vector3.Distance(transform.position, backTarget.position);

                if (!Physics.Raycast(transform.position, backDirectionTarget, backDistanceTarget, groundMask))
                {
                    seesPlayerClose = true;
                }
                else
                {
                    seesPlayerClose = false;
                }
            }
            else { seesPlayerClose = false; }
        }
        else if (seesPlayerClose == true)
        {
            seesPlayerClose = false;
        }
    }


}
