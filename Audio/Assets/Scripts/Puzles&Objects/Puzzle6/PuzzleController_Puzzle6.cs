using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMOD.Studio;
using FMODUnity;
using UnityEngine.SceneManagement;
using Assets.Scripts.Auxiliar;

public class PuzzleController_Puzzle6 : PuzzleController {

   
    // Use this for initialization
    void Start () {
        mapToUnload = "SixthPuzzle";
	}


    public override void WinPuzzle()
    {
        base.WinPuzzle();

        //First puzzle completed
        Constantes.IS_PUZZLE6_SOLVED = true;
        StartCoroutine(this.exitPuzzle(mapToUnload));

    }
}
