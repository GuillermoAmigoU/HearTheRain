using Assets.Scripts.Auxiliar;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(BoxCollider))]

public class Puzzle4 : MonoBehaviour
{

    [FMODUnity.EventRef]
    public string BrokenWallSound = "event:/Puzzle/Living Room/WindowPuzzle";
    EventInstance BrokenWallEvent;

    [FMODUnity.EventRef]
    public string WinSound = "event:/Puzzle/Common/PuzzleSolved";
    EventInstance WinEvent;


    ParameterInstance par;

    public float timeToWait = 10.0f;

    float timesHit = 0.5f;
    bool stopSum = false;
    protected bool VOreproducing = true;

    private void Start()
    {
        BrokenWallEvent = RuntimeManager.CreateInstance(BrokenWallSound);
        WinEvent = RuntimeManager.CreateInstance(WinSound);

        RuntimeManager.AttachInstanceToGameObject(BrokenWallEvent, Constantes.MAIN_PLAYER.transform, Constantes.MAIN_PLAYER.transform.GetComponent<Rigidbody>());
        BrokenWallEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));

        RuntimeManager.AttachInstanceToGameObject(WinEvent, Constantes.MAIN_PLAYER.transform, Constantes.MAIN_PLAYER.transform.GetComponent<Rigidbody>());
        WinEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));

        BrokenWallEvent.getParameter("Breaking", out par);
    }

    //When we enter the Trigger we change the values of the Wall sound (its material)
    public void HitWall()
    {

            if (Constantes.IS_HITTING_WALL)
            {

                //Solo suena nuestra pared.
                Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getHitWall().setParameterValue(Constantes.MAT_P_PAVEMENT, 0.0f);
                Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getHitWall().setParameterValue(Constantes.MAT_P_TILE, 0.0f);
                Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getHitWall().setParameterValue(Constantes.MAT_P_WOOD, 0.0f);
                Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getHitWall().setParameterValue(Constantes.MAT_P_PLASTER, 0.0f);
                Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getHitWall().setParameterValue(Constantes.MAT_P_BRICK, 0.0f);
                Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getHitWall().setParameterValue(Constantes.MAT_P_METAL, 0.0f);
                Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getHitWall().setParameterValue(Constantes.MAT_P_LockedDoor, 0.0f);

                //Colisiona con esta pared concreta.
                if (!stopSum)
                {
                    //breaking sounds

                    Vibration.CreateOneShot(100);

                    //Change hit sound
                    timesHit += 1.0f;
                    par.setValue(timesHit);
                    //Hitwall comprobation
                    Constantes.IS_HITTING_WALL = false;

                    BrokenWallEvent.start();

                    //All Broken sound and VO
                    if (timesHit == 4.5f)
                    {
                    Constantes.CAN_MOVE = false;
                        stopSum = true;
                    Vibration.CreateOneShot(100);
                    StartCoroutine(waitToFinishVO());

                }
            }
                //When VO has finished, we put a default broken sound.
                else
                {
                par.setValue(6.0f);

                Vibration.CreateOneShot(100);

                    //Hitwall comprobation
                    Constantes.IS_HITTING_WALL = false;

                    BrokenWallEvent.start();
                }

        }
    }

    IEnumerator waitToFinishVO()
    {
        yield return new WaitForSeconds(timeToWait);
        VOreproducing = false;      
        Win();
        Constantes.CAN_MOVE = true;
    }

    public void Win()
    {

        Constantes.IS_PUZZLE4_SOLVED = true;
        Vibration.CreateOneShot(150);
        this.GetComponent<StudioEventEmitter>().enabled = false;

    }
}
