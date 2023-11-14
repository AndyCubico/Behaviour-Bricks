using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;

public class movement : MonoBehaviour
{

    private NavMeshAgent agent;
    private Vector3 worldTarget = Vector3.zero;
    private float freqWander = 0f;
    private float freqChase = 0f;
    private Animator animator;
    public Vector3 hideValue = Vector3.zero;
    public Collider floor;

    //wander
    [Header ("Settings Wander")]
    [Range(0.0f, 10.0f)]
    public float freqWanderMax = 2.5f;
    [Range(0.0f, 10.0f)]
    public float radius = 10.0f;
    [Range(0.0f, 10.0f)]
    public float offset = 2.0f;

    //seek
    [Header("Settings Seek")]
    [Range(0.0f, 10.0f)]
    public float freqSeekMax = 0.0f;

    //hide
    public GameObject[] hidingSpots;
    public GameObject cop;

    // Start is called before the first frame update
    void Start()
    {
        hidingSpots = GameObject.FindGameObjectsWithTag("hide");
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.SetBool("walk", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void seek(Vector3 target)
    {
        freqChase += Time.deltaTime;

        if (freqChase > freqSeekMax)
        {
            freqChase -= freqSeekMax;
            agent.SetDestination(target);
        }
    }

    public void wander()
    {
        freqWander += Time.deltaTime;

        if (freqWander > freqWanderMax)
        {
            freqWander -= freqWanderMax;
            Vector3 localTarget = UnityEngine.Random.insideUnitCircle * radius;
            localTarget += new Vector3(0, 0, offset);
            worldTarget = transform.TransformPoint(localTarget);
            worldTarget.y = 0f;
        }

        if (!floor.bounds.Contains(worldTarget))
        {
            worldTarget = -transform.position * 0.1f;

        };

        agent.destination = worldTarget;
    }

    public  void hide()
    {
        float dist = Mathf.Infinity;
        Vector3 chosenSpot = Vector3.zero;
        Vector3 chosenDir = Vector3.zero;
        GameObject chosenGO = hidingSpots[0];

        for (int i = 0; i < hidingSpots.Length; i++)
        {
            Vector3 hideDir = hidingSpots[i].transform.position - cop.transform.position;
            Vector3 hidePos = hidingSpots[i].transform.position + hideDir.normalized * 100;

            if (Vector3.Distance(transform.position, hidePos) < dist)
            {
                chosenSpot = hidePos;
                chosenDir = hideDir;
                chosenGO = hidingSpots[i];
                dist = Vector3.Distance(transform.position, hidePos);
            }
        }

        Collider hideCol = chosenGO.GetComponent<Collider>();
        Ray backRay = new Ray(chosenSpot, -chosenDir.normalized);
        RaycastHit info;
        float distance = 250.0f;
        hideCol.Raycast(backRay, out info, distance);

        hideValue = info.point + chosenDir.normalized;

        seek(hideValue);
    }
}
