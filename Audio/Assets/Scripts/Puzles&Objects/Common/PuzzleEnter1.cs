using Assets.Scripts.Auxiliar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleEnter1 : PuzzleEnter
{


    //This puzzle will oclude other puzles in the area until it is solved. Once it is, we will enable the other sounds and puzzles.
    public GameObject[] ObjectsToActivate;
    public GameObject[] ObjectsToDeactivate;

    private IEnumerator Wait(Collider other)
    {
        yield return new WaitForSeconds(1.5f);
        base.OnTriggerStay(other);

    }

    //every frame
    private void Update()
    {


        //If we solved this puzzle, we no longer need the script but the component it has.
        if (Constantes.IS_PUZZLE1_SOLVED)
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
