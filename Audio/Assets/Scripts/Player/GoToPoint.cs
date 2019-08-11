using Assets.Scripts.Auxiliar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(FMODUnity.StudioEventEmitter))]
public class GoToPoint : MonoBehaviour
{
    [SerializeField]
    GameObject goal = null;
    public float distanceThreshold = 0.3f;

    private NavMeshAgent agente;
    private bool inpath = false;
    private int ActualPuzzle = 0;

    //[SerializeField]
    //GameObject[] puzzlesEnter = null;

    // Start is called before the first frame update
    void Start()
    {
        agente = this.GetComponent<NavMeshAgent>();
        goal = SelectObjective();
    }

    // Update is called once per frame
    void Update()
    {

        //When snapped
        if (!inpath)
        {
            if (goal != null)
            {
                agente.SetDestination(goal.transform.position);
                inpath = true;

            }
            //This should not happen
            else
                Destroy(this);
        }

        // Check if we've reached the destination and destroy when completed
        else if (!agente.pathPending && inpath)
        {
            if (agente.remainingDistance <= agente.stoppingDistance)
            {
                if (!agente.hasPath || agente.velocity.sqrMagnitude == 0f)
                {
                    Debug.Log(agente.remainingDistance + " " + agente.pathEndPosition);
                    Destroy(this.gameObject);
                }
            }
        }
    }

    /*
    public GameObject getGoal()
    {
        return goal;
    }
    public void setGoal(GameObject g)
    {
        goal = g;
    }
    */

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
