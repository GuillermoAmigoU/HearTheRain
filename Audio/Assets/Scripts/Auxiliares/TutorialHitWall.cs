using Assets.Scripts.Auxiliar;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHitWall : MonoBehaviour
{

    //Sound
    [FMODUnity.EventRef]
    public string audiosound;
    FMOD.Studio.EventInstance audioEvent;

    bool firstTime = true;

    private void Start()
    {
        audioEvent = RuntimeManager.CreateInstance(audiosound);
        audioEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        RuntimeManager.AttachInstanceToGameObject(audioEvent, transform, GetComponent<Rigidbody>());
    }

    private void OnTriggerStay(Collider other)
    {

        //If player moves forward on this trigger (steps into a wall)
        if (other.gameObject.CompareTag("Player") && firstTime)
        {
            //1st Phrase of the tutorial
            Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getHitWall().setParameterValue(Constantes.WALL_TUTORIAL1, 1.0f);

        }

        if (other.gameObject.CompareTag("Player") && !firstTime)
        {
            //If the player keeps colliding with the wall
            Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getHitWall().setParameterValue(Constantes.WALL_TUTORIAL2, 1.0f);
        }

    }

    private IEnumerator NotFirstTime()
    {
        yield return new WaitForSeconds(6.2f);
        firstTime = false;
    }

    private void OnTriggerExit(Collider other)
    {
        Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getHitWall().setParameterValue(Constantes.WALL_TUTORIAL1, 0.0f);
        Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().getHitWall().setParameterValue(Constantes.WALL_TUTORIAL2, 0.0f);
        Constantes.AUDIOS_PLAYED++;
        Destruir();
        //Debug.Log(Constantes.AUDIOS_PLAYED);

    }

    private void Destruir()
    {
        Destroy(this.gameObject);
    }


    void Update()
    {
        if (Constantes.IS_HITTING_WALL && firstTime)
        {
            StartCoroutine(NotFirstTime());

        }

        /*
        if (Constantes.IS_PLAYER_LOADED)
        {

            //When we laod the game, we erase the volumes whoch were reproduced before
            if (Constantes.IS_PUZZLE1_SOLVED)
            {
                    Destruir();
            }
        }
        */

    }
}