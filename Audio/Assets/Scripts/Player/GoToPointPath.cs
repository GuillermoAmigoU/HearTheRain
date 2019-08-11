using Assets.Scripts.Auxiliar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(FMODUnity.StudioEventEmitter))]
public class GoToPointPath : MonoBehaviour
{
    GameObject[] points = new GameObject[2];
    private int destPoint = 0;
    private NavMeshAgent agent;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        //The path we would want  it to follow
        points[0] = GameObject.Find("SnapPathPoint");
        points[1] = SelectObjective();

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;

        GotoNextPoint();
    }


    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].transform.position;

        //Choose the enxt or destroy itself if finished
            destPoint = (destPoint + 1);
    }


    void Update()
    {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            //reached last position
            if (destPoint == points.Length)
            {
                Destroy(this.gameObject);
            }
            else
                GotoNextPoint();

        }
    }

    public GameObject SelectObjective()
    {
        GameObject response = null;
        //Metodo antiguo, crear la colision circular
        //GameObject InstanceColision = Instantiate(colision, this.transform.position, this.transform.rotation);

        //New Method: Indicating the next objective
        //What puzzle are we actually going for

        if (!Constantes.IS_PUZZLE1_SOLVED)
        {
            response = GameObject.Find("PuzleEnter_Kitchen");
        }
        else if (!Constantes.IS_PUZZLE3_SOLVED)
        {
            response = GameObject.Find("PuzleEnter_LivingRoom_Window");
        }
        else if (!Constantes.IS_PUZZLE2_SOLVED)
        {
            response = GameObject.Find("PuzleEnter_LivingRoom_Radio");
        }
        else if (!Constantes.IS_PUZZLE4_SOLVED)
        {
            response = GameObject.Find("Puzle_Wall");
        }
        else if (!Constantes.IS_PUZZLE5_SOLVED)
        {
            response = GameObject.Find("PuzleEnter_Bedroom_TV");
        }
        else if (!Constantes.IS_PUZZLE6_SOLVED)
        {
            response = GameObject.Find("PuzleEnter_Bathroom");
        }
        else if (!Constantes.IS_PUZZLE7_SOLVED)
        {
            response = GameObject.Find("PuzleEnter_SecretRoom");
        }

        return response;
    }

}
