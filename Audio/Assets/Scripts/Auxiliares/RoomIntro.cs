using Assets.Scripts.Auxiliar;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider))]
public class RoomIntro : MonoBehaviour
{
    //Sound
    [FMODUnity.EventRef]
    public string audioString;
    FMOD.Studio.EventInstance audioEvent;

    private void Start()
    {
        audioEvent = RuntimeManager.CreateInstance(audioString);
        RuntimeManager.AttachInstanceToGameObject(audioEvent, Constantes.MAIN_PLAYER.transform, Constantes.MAIN_PLAYER.GetComponent<Rigidbody>());
    }

    //Play when we enter the room
    private void OnTriggerEnter(Collider other)
    {
        audioEvent.start();
    }

    private void OnTriggerExit(Collider other)
    {

    }

}
