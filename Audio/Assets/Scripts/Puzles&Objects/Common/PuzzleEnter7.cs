using Assets.Scripts.Auxiliar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleEnter7 : PuzzleEnter
{

    //This puzzle will oclude other puzles in the area until it is solved. Once it is, we will enable the other sounds and puzzles.
    public GameObject[] ObjectsToActivate;
    public GameObject[] ObjectsToDeactivate;


    private void Update()
    {

        //If we solved this puzzle, we no longer need the script but the component it has.
        if (Constantes.IS_PUZZLE7_SOLVED)
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
