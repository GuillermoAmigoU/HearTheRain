using Assets.Scripts.Auxiliar;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle3 : MonoBehaviour
{

    //Fmod Sound
    [SerializeField]
    [FMODUnity.EventRef]
    public string WindowAmbient = "event:/Puzzle/Living Room_Window/Window_PuzzleAmbient";
    EventInstance WindowAmbient_Event;

    [SerializeField]
    [FMODUnity.EventRef]
    public string WindowHitCloseSound = "event:/Puzzle/Living Room_Window/Window_HitClose";
    EventInstance Window_Hit_Close_Event;

    [SerializeField]
    [FMODUnity.EventRef]
    public string WindowHitting = "event:/Puzzle/Living Room_Window/Window_Opened";
    EventInstance WindowHitting_Event;


    //parameters to use
    ParameterInstance open;
    ParameterInstance close;
    ParameterInstance sol;
    ParameterInstance interact;
    ParameterInstance LPF;
    ParameterInstance panner;

    //For closing a window frame
    ParameterInstance hit;

    Slider sli;

    [SerializeField]
    public GameObject Player;
    public GameObject OtherSlider;


    //If we are using the right or left slider
    public bool Right = true;
    float SliderValue = 0.0f;

    //Store old value
    float oldValue = 0.0f;

    //to finish the puzle
    bool once = false;

    public float waitTime = 0.1f;
    WaitForSeconds wait;

    // Use this for initialization
    void Start()
    {

        sli = GetComponent<Slider>();
        //WindowWin_Event = RuntimeManager.CreateInstance(WindowWinSound);
        WindowAmbient_Event = RuntimeManager.CreateInstance(WindowAmbient);
        Window_Hit_Close_Event = RuntimeManager.CreateInstance(WindowHitCloseSound);
        WindowHitting_Event = RuntimeManager.CreateInstance(WindowHitting);

        wait = new WaitForSeconds(waitTime);


        WindowAmbient_Event.getParameter("LPF", out LPF);
        Window_Hit_Close_Event.getParameter("Hit", out hit);
        WindowHitting_Event.getParameter("Panner", out panner);

        //Set attributes
        WindowAmbient_Event.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.transform.position));
        Window_Hit_Close_Event.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.transform.position));
        WindowHitting_Event.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.transform.position));


        //We atach and play
        RuntimeManager.AttachInstanceToGameObject(WindowAmbient_Event, Player.transform, Player.transform.GetComponent<Rigidbody>());
        WindowAmbient_Event.start();

        RuntimeManager.AttachInstanceToGameObject(WindowHitting_Event, Player.transform, Player.transform.GetComponent<Rigidbody>());
        WindowHitting_Event.start();

        //WindowAmbient_Event.setPaused(true);

        //Hitting door sound position
        if (Right)
            panner.setValue(1.0f);
        else
            panner.setValue(-1.0f);

    }

    public void OnValueChanged(Single f)
    {

        interact.setValue(1.0f);
        SliderValue = f;


        //Do we have a window closed?
        if (SliderValue == 0.0f)
        {
            if (Right)
            {
                panner.setValue(1.0f);
                WindowHitting_Event.start();
            }
            else
            {
                panner.setValue(-1.0f);
                WindowHitting_Event.start();

            }

        }


        if (SliderValue != 0)
        {
            WindowHitting_Event.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }


        //Closing
        if (oldValue < f)
        {
            open.setValue(0.0f);
            close.setValue(1.0f);
            Debug.Log("Estoy cerrando la ventana");
            RuntimeManager.PlayOneShot("event:/Puzzle/Living Room_Window/Window_Closing");
            Vibration.CreateOneShot(10);

        }

        //Opening
        else if (oldValue > f)
        {
            close.setValue(0.0f);
            open.setValue(1.0f);

            Debug.Log("Estoy abriendo la ventana");
            RuntimeManager.PlayOneShot("event:/Puzzle/Living Room_Window/Window_Opening");

            Vibration.CreateOneShot(10);
        }


        //Check if closed the door
        if (f == 1.0f)
        {
            //We are closing it, not closed from before (to avoid the repetition of the closure sound)
            if (oldValue != f)
            {
                //If we have one opened we play other hit sound
                if (OtherSlider.GetComponent<Puzzle3>().SliderValue == 1.0f)
                {
                    //Occlude the window cause we closed it.
                    LPF.setValue(4200.0f);

                    hit.setValue(1.0f);
                    Debug.Log("Segundo sonido de puerta cerrada");
                    Vibration.CreateOneShot(100);


                    //Windows closed, so no hitting sound.
                    WindowHitting_Event.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                }
                else
                {
                    Debug.Log("Primer sonido de puerta cerrada");
                    hit.setValue(0.0f);
                    LPF.setValue(8200.0f);
                }


                OtherSlider.GetComponent<Puzzle3>().LPF.setValue(0.0f);
                RuntimeManager.AttachInstanceToGameObject(Window_Hit_Close_Event, Player.transform, Player.transform.GetComponent<Rigidbody>());
                Window_Hit_Close_Event.start();
                Vibration.CreateOneShot(100);
            }

        }
        if (f != 1.0f && OtherSlider.GetComponent<Puzzle3>().SliderValue != 1.0f)
            LPF.setValue(22000.0f);

        //Check if we won
        checkWin();
        oldValue = SliderValue;

        //Reset Values
        StartCoroutine(ResetValues());

    }

    private IEnumerator ResetValues()
    {
        yield return wait;
        close.setValue(0.0f);
        open.setValue(0.0f);

    }

    //Wait for the solution sound to be reproduced to solve the puzzle
    private IEnumerator WaitToEnd()
    {
        yield return new WaitForSeconds(2.0f);

        //Back to the menu
        WindowAmbient_Event.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        OtherSlider.GetComponent<Puzzle3>().WindowAmbient_Event.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        Player.GetComponent<PuzzleController_Puzzle3>().WinPuzzle();
    }

    //At the end, we must hear first the door closing, then the solution sound
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.2f);
        sol.setValue(1.0f);

    }

    void checkWin()
    {
        //only if its the right slider
        if (Right)
        {
            //If the right one is at 1, and the left one is at 1, the puzzle is solved.
            if (sli.value == 1.0f && OtherSlider.GetComponent<Puzzle3>().SliderValue == 1.0f && !once)
            {
                once = true;
                //Puzzle solved
                Debug.Log("I win!");


                Vibration.CreateOneShot(150);

                //disable UI elements to avoid extra interaction after winning.
                this.GetComponent<Slider>().interactable = false;
                OtherSlider.GetComponent<Slider>().interactable = false;

                //Reproduce last sound and solution sound
                StartCoroutine(Wait());

                //Wait the solution sound to end, then we can move on
                StartCoroutine(WaitToEnd());
            }
        }
    }

    private void Update()
    {
        if (Right)
        {
            checkWin();
        }
    }


}
