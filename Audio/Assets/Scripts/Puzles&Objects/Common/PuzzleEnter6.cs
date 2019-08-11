using Assets.Scripts.Auxiliar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleEnter6 : PuzzleEnter
{

    //This puzzle will be a open window we will need to close in order to hear the rest of the objects on the room.
    public GameObject[] ObjectsToActivate;
    public GameObject[] ObjectsToDeactivate;


    private void Update()
    {

        //If we solved this puzzle, we no longer need the script but the component it has.
        if (Constantes.IS_PUZZLE6_SOLVED)
        {
            //Enable the ocludded objects
            foreach (GameObject g in ObjectsToActivate)
            {
                g.SetActive(true);
            }

            foreach (GameObject g in ObjectsToDeactivate)
            {
                g.SetActive(false);
            }

            this.gameObject.SetActive(false);
        }
    }
}
