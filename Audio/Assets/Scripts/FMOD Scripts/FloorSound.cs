using Assets.Scripts.Auxiliar;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(BoxCollider))]
public class FloorSound : MonoBehaviour {


    [Header("Floor Material/s")]
    public float S_Pavement =0.0f;
    public float S_Wood = 0.0f;
    public float S_Tile= 0.0f;
    public float S_ShatteredGlass = 0.0f;
    public float S_Water = 0.0f;
    public float S_Wood2 = 0.0f;
    public float S_Stairs= 0.0f;


    //When we enter the Trigger we change the values of the floor sound (its material)
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getFootsteps().setParameterValue(Constantes.MAT_S_PAVEMENT, S_Pavement);
            Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getFootsteps().setParameterValue(Constantes.MAT_S_WOOD, S_Wood );
            Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getFootsteps().setParameterValue(Constantes.MAT_S_TILE, S_Tile);
            Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getFootsteps().setParameterValue(Constantes.MAT_S_SHATTEREDGLASS, S_ShatteredGlass);
            Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getFootsteps().setParameterValue(Constantes.MAT_S_WATER, S_Water);
            Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getFootsteps().setParameterValue(Constantes.MAT_S_WOOD2, S_Wood2);
            Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getFootsteps().setParameterValue(Constantes.MAT_S_STAIRS, S_Stairs);
        }
    }
}
