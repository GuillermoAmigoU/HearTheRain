using Assets.Scripts.Auxiliar;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof (BoxCollider))]

public class WallSound : MonoBehaviour {


    [Header("Wall Material/s")]
    public float P_Pavement =0.0f;
    public float P_Tile = 0.0f;
    public float P_Wood = 0.0f;
    public float P_Plaster= 0.0f;
    public float P_Brick = 0.0f;
    public float P_Metal = 0.0f;
    public float P_LockedDoor = 0.0f;
    public float P_MainDoor = 0.0f;
    public float P_MainDoor2 = 0.0f;

    public void SetMaterials()
    {
        Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getHitWall().setParameterValue(Constantes.MAT_P_PAVEMENT, P_Pavement);
        Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getHitWall().setParameterValue(Constantes.MAT_P_TILE, P_Tile);
        Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getHitWall().setParameterValue(Constantes.MAT_P_WOOD, P_Wood);
        Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getHitWall().setParameterValue(Constantes.MAT_P_PLASTER, P_Plaster);
        Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getHitWall().setParameterValue(Constantes.MAT_P_BRICK, P_Brick);
        Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getHitWall().setParameterValue(Constantes.MAT_P_METAL, P_Metal);
        Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getHitWall().setParameterValue(Constantes.MAT_P_LockedDoor, P_LockedDoor);
        Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getHitWall().setParameterValue(Constantes.MAT_P_MainDoor, P_MainDoor);
        Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getHitWall().setParameterValue(Constantes.MAT_P_MainDoor2, P_MainDoor2);

    }
}
