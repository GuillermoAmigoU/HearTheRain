using Assets.Scripts.Auxiliar;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class DialogVolume : MonoBehaviour
{

    public Button boton;

    //Sound
    [FMODUnity.EventRef]
    public string audiosound;
    FMOD.Studio.EventInstance audioEvent;

    [SerializeField]
    bool locked = false;

    [SerializeField]
    [Range(1, 7)]
    int Key = 0;

    [SerializeField]
    bool repeat = false;

    [SerializeField]
    float timeToWaitBeforeStarting = 0.0f;

    bool once = false;
    bool destruironce = false;


    private void Start()
    {
        audioEvent = RuntimeManager.CreateInstance(audiosound);
        audioEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        RuntimeManager.AttachInstanceToGameObject(audioEvent, transform, GetComponent<Rigidbody>());
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !locked)
        {
            Constantes.CAN_MOVE = false;
            Constantes.DONT_SNAP = true;

            if (!destruironce)
            {
                destruironce = true;
                StartCoroutine(StartSound());
            }
        }
    }

    private IEnumerator StartSound()
    {
        yield return new WaitForSeconds(timeToWaitBeforeStarting);
        audioEvent.start();
        FuncAuxiliares.Instance.DisableInputUntilAudioFinished(audioEvent);

        if (!repeat)
        {
            Destruir();
        }
    }

    public void Destruir()
    {
        Destroy(this.gameObject);
        Constantes.AUDIOS_PLAYED++;
        //Debug.Log(Constantes.AUDIOS_PLAYED);
    }

    void Update()
    {


        if (!once)
        {
            if (locked)
            {
                switch (Key)
                {
                    case 1:
                        if (Constantes.IS_PUZZLE1_SOLVED)
                        {
                            locked = false;
                            once = true;
                        }
                        break;
                    case 2:
                        if (Constantes.IS_PUZZLE2_SOLVED)
                        {
                            locked = false;
                            once = true;

                        }
                        break;
                    case 3:
                        if (Constantes.IS_PUZZLE3_SOLVED)
                        {
                            locked = false;
                            once = true;

                        }
                        break;
                    case 4:
                        if (Constantes.IS_PUZZLE4_SOLVED)
                        {
                            locked = false;
                            once = true;

                        }
                        break;
                    case 5:
                        if (Constantes.IS_PUZZLE5_SOLVED)
                        {
                            locked = false;
                            once = true;

                        }
                        break;
                    case 6:
                        if (Constantes.IS_PUZZLE6_SOLVED)
                        {
                            locked = false;
                            once = true;

                        }
                        break;
                    case 7:
                        if (Constantes.IS_PUZZLE7_SOLVED)
                        {
                            locked = false;
                            once = true;

                        }
                        break;

                }
            }
        }
        /*
        if (Constantes.IS_PLAYER_LOADED)
        {

            //When we laod the game, we erase the volumes whoch were reproduced before
            if (!locked && Constantes.IS_PUZZLE1_SOLVED)
            {
                if (Key != 1)
                    destruir();
            }
            if (!locked && Constantes.IS_PUZZLE2_SOLVED)
            {
                if (Key != 2)
                    destruir();
            }
            if (!locked && Constantes.IS_PUZZLE3_SOLVED)
            {
                if (Key != 3)
                    destruir();
            }
            if (!locked && Constantes.IS_PUZZLE4_SOLVED)
            {
                if (Key != 4)
                    destruir();
            }
            if (!locked && Constantes.IS_PUZZLE5_SOLVED)
            {
                if (Key != 5)
                    destruir();
            }
            if (!locked && Constantes.IS_PUZZLE6_SOLVED)
            {
                if (Key != 6)
                    destruir();
            }
        }
        */

    }
}
