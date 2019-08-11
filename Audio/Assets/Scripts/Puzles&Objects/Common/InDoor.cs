using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InDoor : MonoBehaviour
{


    [FMODUnity.EventRef]
    public string openSound;
    FMOD.Studio.EventInstance openSoundEvent;

    bool locked;

    // Start is called before the first frame update
    void Start()
    {
        openSoundEvent = FMODUnity.RuntimeManager.CreateInstance(openSound);
        locked = GetComponentInParent<Door>().getLocked();
        FMODUnity.RuntimeUtils.To3DAttributes(this.transform.position);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(openSoundEvent, transform, GetComponent<Rigidbody>());

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {

        locked = GetComponentInParent<Door>().getLocked();

        if (other.tag == "Player")
        {

            if (!locked)
            {
                //play open sound event
                OpenDoor();
            }
        }
    }
    void OpenDoor()
    {
        //Reproduce open sound
        openSoundEvent.start();
    }


}
