using Assets.Scripts.Auxiliar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignUI : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

        //WE assign the UI to the main static UI 
        Constantes.MAIN_UI = this.gameObject;

    }
}
