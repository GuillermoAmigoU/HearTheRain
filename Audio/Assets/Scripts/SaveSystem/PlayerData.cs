using Assets.Scripts.Auxiliar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    //Variables we need to save or load.
    public float[] position;
    public float[] rotation;
    public bool[] puzlesSolved;
    public int saved_Used;
    public int audios_Played;

    //Constructor for the player
    public PlayerData(MovingUnit player)
    {
        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

        rotation = new float[3];
        rotation[0] = player.transform.rotation.x;
        rotation[1] = player.transform.rotation.y;
        rotation[2] = player.transform.rotation.z;

        puzlesSolved = new bool[7];
        puzlesSolved[0] = Constantes.IS_PUZZLE1_SOLVED;
        puzlesSolved[1] = Constantes.IS_PUZZLE2_SOLVED;
        puzlesSolved[2] = Constantes.IS_PUZZLE3_SOLVED;
        puzlesSolved[3] = Constantes.IS_PUZZLE4_SOLVED;
        puzlesSolved[4] = Constantes.IS_PUZZLE5_SOLVED;
        puzlesSolved[5] = Constantes.IS_PUZZLE6_SOLVED;
        puzlesSolved[6] = Constantes.IS_PUZZLE7_SOLVED;

        audios_Played = Constantes.AUDIOS_PLAYED;
        saved_Used = Constantes.SAVED_USED;
    }

}
