using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyMove : MonoBehaviour
{
    NavMeshAgent myNavMeshAgent;
    private Animator anim;
   private Rigidbody rb;

    public Vector3 v,dv;

    [SerializeField] private Transform target;                                                   // target to aim for
    public GameObject[] playerList;


    [SerializeField] private float minimumDist; //edit this field to mkae the zombie change target
    [SerializeField] private float dist;

    void Start()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetDestinationToMousePosition();
        }

        //settarget();

        if (Mathf.Abs(myNavMeshAgent.desiredVelocity.x) > Mathf.Abs(myNavMeshAgent.desiredVelocity.z))
        {
            anim.SetFloat("Velocity X", Mathf.Abs(myNavMeshAgent.velocity.x));
        }
        else
        {
            anim.SetFloat("Velocity X", Mathf.Abs(myNavMeshAgent.velocity.z));
        }

        dv = myNavMeshAgent.desiredVelocity;
        v = myNavMeshAgent.velocity;

        //boost
       // if(anim.GetFloat("Velocity X") > 0.1)
       // {
       //     rb.AddForce(myNavMeshAgent.desiredVelocity * 3, ForceMode.Impulse);
       // }
       //else if (anim.GetFloat("Velocity X") > 0.45)
       //{
       //    rb.AddForce(myNavMeshAgent.desiredVelocity * 5, ForceMode.Impulse);
       //}
    


    }


    void settarget()
    {
        target = GameObject.Find("1001").transform;
        myNavMeshAgent.SetDestination(target.position);
    }


    void SetDestinationToMousePosition()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            myNavMeshAgent.SetDestination(hit.point);
        }

    }

}
