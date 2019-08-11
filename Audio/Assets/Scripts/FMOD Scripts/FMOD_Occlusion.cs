using Assets.Scripts.Auxiliar;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Collider))]
[RequireComponent(typeof(Rigidbody))]

public class FMOD_Occlusion : MonoBehaviour {

    [Header("FMOD Event")]
    [FMODUnity.EventRef]
    public string SelectAudio;
    FMOD.Studio.EventInstance Audio;
    FMOD.Studio.ParameterInstance VolumeParameter;
    FMOD.Studio.ParameterInstance LPFParameter;

    Transform SlLocation;

    [Header("Occlusion Options")]
    [Range(0f, 1f)]
    public float VolumeValue = 0.5f;
    [Range(10f, 22000f)]
    public float LPFCutoff = 10000f;
    public LayerMask OcclusionLayer = 1;

    void Awake()
    {
        SlLocation = GameObject.FindObjectOfType<StudioListener>().transform;

        //Debug.Log(SlLocation.name);

        Audio = FMODUnity.RuntimeManager.CreateInstance(SelectAudio);
        Audio.getParameter("Volume", out VolumeParameter);
        Audio.getParameter("LPF", out LPFParameter);
        RuntimeManager.AttachInstanceToGameObject(Audio, transform, GetComponent<Rigidbody>());
        FMODUnity.RuntimeUtils.To3DAttributes(this.transform.position);

    }

    void Start()
    {
        FMOD.Studio.PLAYBACK_STATE PbState;
        Audio.getPlaybackState(out PbState);
        if (PbState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            Audio.start();
        }
        //Call every time (seconds)
        InvokeRepeating("CheckOclussion", 0.0f, 0.5f);
    }

    //This operation is expensive, so instead of executing it on Update(); we execute it with InvokeRepeating, every half a second.(2 per second instead of 60 times per second)
    void CheckOclussion()
    {

        try
        {
            RaycastHit hit;
            Physics.Linecast(transform.position, SlLocation.position, out hit, OcclusionLayer);
            //if the length of the distance is grater or equal than x (meters), we dont play the sound. 
           
            if (hit.collider.transform.name == "Player")
            {
                //Debug.Log ("not occluded");
                NotOccluded();
                //Debug.DrawLine(transform.position, SlLocation.position, Color.blue);
            }
            else
            {
                //Debug.Log ("occluded");
                Occluded();
                //Debug.DrawLine(transform.position, hit.point, Color.red);
            }
        }
        //The player is in the object origin so no linecast is beeing casted
        catch
        {
            NotOccluded();
        }

    }

    void Occluded()
    {
        VolumeParameter.setValue(VolumeValue);
        LPFParameter.setValue(LPFCutoff);
    }

    void NotOccluded()
    {
        VolumeParameter.setValue(1f);
        LPFParameter.setValue(22000f);
    }

    private void OnDisable()
    {
        Audio.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
