using Assets.Scripts.Auxiliar;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [SerializeField]
    GameObject Player = null;


    //SavePoint only appears when completed certain puzzle.
    [SerializeField]
    bool locked = false;

    [SerializeField]
    [Range(1, 7)]
    int Key = 0;

    //Sound
    [FMODUnity.EventRef]
    public string audioToUse;
    FMOD.Studio.EventInstance audioToUseEvent;

    bool once = false;

    private void Start()
    {

        audioToUseEvent = RuntimeManager.CreateInstance(audioToUse);
        RuntimeManager.AttachInstanceToGameObject(audioToUseEvent, transform, GetComponent<Rigidbody>());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!locked && this.enabled)
            {
                StartCoroutine(save());
            }
        }
    }

    private IEnumerator save()
    {
        yield return new WaitForSeconds(1.0f);
        this.enabled = false;
        //Save the state of the player on the savedata and the constant class
        Constantes.SAVED_USED++;
        SaveSystem.SavePlayer(Player.GetComponent<MovingUnit>());
        Constantes.MAIN_PLAYER = Player;
        audioToUseEvent.start();
        //Debug.Log("Tu progreso ha sido guardado, posición " + Player.transform.position);
        //Debug.Log("Audios escuchados: " + Constantes.AUDIOS_PLAYED + " , guardados usados: " + Constantes.SAVED_USED);

        //After saving, destroy this.
        Destroy(this.gameObject);
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
                if (Key > 1)
                    destruir();
            }
            if (!locked && Constantes.IS_PUZZLE2_SOLVED)
            {
                if (Key > 2)
                    destruir();
            }
            if (!locked && Constantes.IS_PUZZLE3_SOLVED)
            {
                if (Key > 3)
                    destruir();
            }
            if (!locked && Constantes.IS_PUZZLE4_SOLVED)
            {
                if (Key > 4)
                    destruir();
            }
            if (!locked && Constantes.IS_PUZZLE5_SOLVED)
            {
                if (Key > 5)
                    destruir();
            }
            if (!locked && Constantes.IS_PUZZLE6_SOLVED)
            {
                if (Key > 6)
                    destruir();
            }
        }
        */
    }

    private void destruir()
    {
        Destroy(this.gameObject);
    }
}
