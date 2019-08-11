using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMOD.Studio;
using FMODUnity;
using UnityEngine.SceneManagement;
using Assets.Scripts.Auxiliar;

public class PuzzleController_Puzzle5 : PuzzleController {

   
    // Use this for initialization
    void Start () {
        mapToUnload = "FifthPuzzle";
	}


    public override void WinPuzzle()
    {
        base.WinPuzzle();

        //First puzzle completed
        Constantes.IS_PUZZLE5_SOLVED = true;
        StartCoroutine(this.exitPuzzle(mapToUnload));

    }
}
