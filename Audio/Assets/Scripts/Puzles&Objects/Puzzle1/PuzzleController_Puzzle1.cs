using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMOD.Studio;
using FMODUnity;
using Assets.Scripts.Auxiliar;

public class PuzzleController_Puzzle1 : PuzzleController {

	// Use this for initialization
	void Start () {
        mapToUnload = "FirstPuzzle";
	}

    public override void WinPuzzle()
    {
        base.WinPuzzle();
        Constantes.IS_PUZZLE1_SOLVED = true;
    }
}
