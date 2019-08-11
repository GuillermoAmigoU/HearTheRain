

using Assets.Scripts.Auxiliar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HideJoystick : MonoBehaviour
{
    //In PC, we disable the Joystick
#if UNITY_STANDALONE_WIN
    void Start ()
    {
        //Hide joystick
        this.gameObject.SetActive(false);
	}
#endif

    //For mobile, we check  wheter we should disable the joystick or not.
#if UNITY_ANDROID
    // Update is called once per frame
    void Update()
    {
        //If we are on a puzzle we hide the Player Joystick. We enable it when returning to main map
        if(Constantes.IS_IN_PUZZLE)
        {
            //this.gameObject.SetActive(false);

        }
        else if (!Constantes.IS_IN_PUZZLE)
        {
            this.gameObject.SetActive(true);
        }
    }
#endif
}


