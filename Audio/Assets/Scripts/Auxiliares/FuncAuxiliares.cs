using Assets.Scripts.Auxiliar;
using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuncAuxiliares : MonoBehaviour
{

    private static FuncAuxiliares instance;

    public static FuncAuxiliares Instance { get { return instance; } set { instance = value; } }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this);
    }

    //Do a function delayed, for certain sound functions
    public void StartFunctionDelayed(float time, Delegate function, params object[] parameters)
    {
        StartCoroutine(StartFunctionDel(time, function, parameters));
    }

    IEnumerator StartFunctionDel(float time, Delegate function, params object[] parameters)
    {
        yield return new WaitForSeconds(time);
        function.DynamicInvoke(parameters);
    }

    public void DisableInputUntilAudioFinished(EventInstance audio)
    {
        Constantes.CAN_MOVE = false;
        StartCoroutine(DisableInputUntilFinished(audio));
    }

    IEnumerator DisableInputUntilFinished(EventInstance audio)
    {
        //We check if the audio has finished
        PLAYBACK_STATE pbState;
        audio.getPlaybackState(out pbState);

        while (!pbState.Equals(PLAYBACK_STATE.STOPPED))
        {
            //Wait for it to end
            audio.getPlaybackState(out pbState);
            yield return null;
        }

        Constantes.CAN_MOVE = true;

    }

    public IEnumerator DisableInputUntilFinished(EventInstance audio, Func<bool> onSuccess)
    {
        //We check if the audio has finished
        PLAYBACK_STATE pbState;
        audio.getPlaybackState(out pbState);

        while (!pbState.Equals(PLAYBACK_STATE.STOPPED))
        {
            //Wait for it to end
            audio.getPlaybackState(out pbState);
            yield return null;
        }
        Constantes.CAN_MOVE = true;

    }
}
