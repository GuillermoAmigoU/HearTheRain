using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLoader : MonoBehaviour
{

    //This volume will enable or disable sound when entered.
    public GameObject[] ObjectsToActivate;
    public GameObject[] ObjectsToDeactivate;


    private void OnTriggerEnter(Collider other)
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
