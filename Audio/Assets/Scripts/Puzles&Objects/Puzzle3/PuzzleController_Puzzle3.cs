using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMOD.Studio;
using FMODUnity;
using UnityEngine.SceneManagement;
using Assets.Scripts.Auxiliar;

public class PuzzleController_Puzzle3 : PuzzleController {


    // Use this for initialization
    void Start () {
        mapToUnload = "ThirdPuzzle";

	}

    public override void WinPuzzle()
    {
        base.WinPuzzle();
        Constantes.IS_PUZZLE3_SOLVED = true;
    }
}
