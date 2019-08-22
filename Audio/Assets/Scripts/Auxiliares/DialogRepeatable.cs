using Assets.Scripts.Auxiliar;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogrepeatAudioable : DialogVolume
{

    //Sound
    [FMODUnity.EventRef]
    public string audioToUse;
    FMOD.Studio.EventInstance audioEvent;

    [SerializeField]
    bool locked = false;


    [SerializeField]
    [Range(1, 7)]
    int Key = 0;

    bool once = false;

    private void Start()
    {
        audioEvent = RuntimeManager.CreateInstance(audioToUse);
        audioEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        RuntimeManager.AttachInstanceToGameObject(audioEvent, transform, GetComponent<Rigidbody>());

    }

    private void OnTriggerEnter(Collider other)
    {
        //The sound has a delay in FMOD to play with a cooldown

        audioEvent.start();
        FuncAuxiliares.Instance.DisableInputUntilAudioFinished(audioEvent);

    }

    private void destruir()
    {
        Destroy(this.gameObject);
    }

    void Update()
    {
        if (!once)
        {
            if (!locked)
            {
                switch (Key)
                {
                    case 1:
                        if (Constantes.IS_PUZZLE1_SOLVED)
                        {
                            locked = true;
                            once = true;
                        }
                        break;
                    case 2:
                        if (Constantes.IS_PUZZLE2_SOLVED)
                        {
                            locked = true;
                            once = true;

                        }
                        break;
                    case 3:
                        if (Constantes.IS_PUZZLE3_SOLVED)
                        {
                            locked = true;
                            once = true;

                        }
                        break;
                    case 4:
                        if (Constantes.IS_PUZZLE4_SOLVED)
                        {
                            locked = true;
                            once = true;

                        }
                        break;
                    case 5:
                        if (Constantes.IS_PUZZLE5_SOLVED)
                        {
                            locked = true;
                            once = true;

                        }
                        break;
                    case 6:
                        if (Constantes.IS_PUZZLE6_SOLVED)
                        {
                            locked = true;
                            once = true;

                        }
                        break;
                    case 7:
                        if (Constantes.IS_PUZZLE7_SOLVED)
                        {
                            locked = true;
                            once = true;

                        }
                        break;

                }
            }
        }


    }
}
