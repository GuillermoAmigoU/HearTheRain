using Assets.Scripts.Auxiliar;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle5 : MonoBehaviour
{
    float elapsed = 0f;
    bool win = false;


    [SerializeField]
    float TimeToPlayHint = 15.0f;

    [SerializeField]
    [FMODUnity.EventRef]
    public string Pista = "event:/Puzzle/Radio/RadioPuzzle";
    EventInstance Pista_Event;

    [FMODUnity.EventRef]
    public string CassetteRecordSound = "event:/Puzzle/Living Room/WindowPuzzle";
    EventInstance CassetteRecordEvent;

    [FMODUnity.EventRef]
    public string CassetteButtonSound = "event:/Puzzle/Living Room/WindowPuzzle";
    EventInstance CassetteButtondEvent;

    //1-2 Play, 2.1-3-Rewind, 3.1-4 Fast Forward
    ParameterInstance CassetteState;
    //0-1 Play, 1-2 Rewind, 2-3 Fast Forward, 3-4 Stop, 4-5 Release button, 5-6 Already pressed button
    ParameterInstance CassetteButton;

    //The player
    public GameObject Player;

    //Button states
    bool playPressed;
    bool stopPressed;
    bool rewindPressed;
    bool forwardPressed;
    bool playing = false;

    //Puzzle State, 0 is the answer.
    int puzzleState = 0;

    //To trigger the exit just once.
    bool once;

    //Time to call Exitpuzzle (wait for VO to exit puzzle)
    public float WaitForVO = 17.0f;

    //Time to Release the button
    public float time = 0.25f;

    // Use this for initialization
    void Start()
    {
        CassetteRecordEvent = RuntimeManager.CreateInstance(CassetteRecordSound);
        CassetteButtondEvent = RuntimeManager.CreateInstance(CassetteButtonSound);
        Pista_Event = RuntimeManager.CreateInstance(Pista);

        //we get the parameters from fmod to use them here
        CassetteRecordEvent.getParameter("CassetteState", out CassetteState);
        CassetteButtondEvent.getParameter("CassetteButton", out CassetteButton);

        RuntimeManager.AttachInstanceToGameObject(CassetteRecordEvent, Player.transform, Player.transform.GetComponent<Rigidbody>());
        RuntimeManager.AttachInstanceToGameObject(CassetteButtondEvent, Player.transform, Player.transform.GetComponent<Rigidbody>());
        RuntimeManager.AttachInstanceToGameObject(Pista_Event, Player.transform, Player.transform.GetComponent<Rigidbody>());


        selectRandom();
        Debug.Log(puzzleState);

        Constantes.CAN_MOVE = true;
        //First time hint
        StartCoroutine(PlayPista(0.0f));
    }
    void Update()
    {
        //First clue
        if (!win && !playing)
        {
            elapsed += Time.deltaTime;
            //Every X seconds
            if (elapsed >= TimeToPlayHint)
            {
                elapsed = elapsed % 1f;
                StartCoroutine(PlayPista(0.0f));
            }

        }
    }

    //Select a Random solution
    void selectRandom()
    {
        while (puzzleState == 0)
        {
            puzzleState = (int)Random.Range(-1.0f, 1.0f);
        }
    }

    //if the button is already pressed, we play certain sound
    private void AlreadyPressed()
    {
        CassetteButton.setValue(5.5f);
        CassetteButtondEvent.start();
    }

    //When released, we wait a moment to release the other button
    private IEnumerator OnRelease()
    {
        yield return new WaitForSeconds(time);
        playing = false;
        CassetteButton.setValue(4.5f);
        CassetteButtondEvent.start();
    }

    //When winning
    IEnumerator WinPuzzleCassette()
    {
        win = true;
        Pista_Event.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        Constantes.CAN_MOVE = false;
        yield return new WaitForSeconds(WaitForVO);
        Player.GetComponent<PuzzleController_Puzzle5>().WinPuzzle();
        once = true;
    }

    IEnumerator PlayPista(float time)
    {
        yield return new WaitForSeconds(time);
        Pista_Event.start();
    }

    //Play button
    public void OnPlay()
    {
        //If the button has already been pressed.
        if (playPressed)
        {
            AlreadyPressed();
            Vibration.CreateOneShot(25);
            playing = false;

        }
        else
        {
            //If some button was pressed before, we release that button.
            if (rewindPressed || forwardPressed || stopPressed)
                StartCoroutine(OnRelease());

            //We set all buttons to released
            rewindPressed = false;
            forwardPressed = false;
            stopPressed = false;

            //We press the Play button
            CassetteButton.setValue(0.5f);
            CassetteButtondEvent.start();
            playPressed = true;
            Vibration.CreateOneShot(50);
            playing = true;


            //Play the record
            //Play the record if correct
            if (puzzleState == 0 && !once)
            {
                CassetteRecordEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                CassetteState.setValue(1.5f);
                CassetteRecordEvent.start();
                Vibration.CreateOneShot(75);

                StartCoroutine(WinPuzzleCassette());
            }
            else
            {
                //Sound not working
                CassetteRecordEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                CassetteState.setValue(4.5f);
                CassetteRecordEvent.start();
            }
        }


    }

    //Rewind button
    public void onRewind()
    {
        if (rewindPressed)
        {
            AlreadyPressed();
            Vibration.CreateOneShot(25);
            playing = false;

        }
        else
        {
            //If some button was pressed before, we release that button.
            if (playPressed || forwardPressed || stopPressed)
                StartCoroutine(OnRelease());

            //We set all buttons to released
            playPressed = false;
            forwardPressed = false;
            stopPressed = false;

            //We press the button
            CassetteButton.setValue(1.5f);
            CassetteButtondEvent.start();
            rewindPressed = true;
            Vibration.CreateOneShot(50);
            playing = true;

            CassetteRecordEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

            //If we can rewind
            if (puzzleState != -1)
            {
                puzzleState--;
                //Play the record
                CassetteState.setValue(2.5f);
                CassetteRecordEvent.start();

            }

        }
    }

    //Forward button
    public void OnForward()
    {
        if (forwardPressed)
        {
            AlreadyPressed();
            Vibration.CreateOneShot(25);
            playing = false;

        }
        else
        {
            //If some button was pressed before, we release that button.
            if (playPressed || rewindPressed || stopPressed)
                StartCoroutine(OnRelease());
            //We set all buttons to released
            rewindPressed = false;
            playPressed = false;
            stopPressed = false;

            //We press the button
            CassetteButton.setValue(2.5f);
            CassetteButtondEvent.start();
            forwardPressed = true;
            Vibration.CreateOneShot(50);
            playing = true;


            CassetteRecordEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

            //If the cassette isn't forwarded
            if (puzzleState != 1)
            {
                puzzleState++;
                CassetteState.setValue(3.5f);
                CassetteRecordEvent.start();
            }


        }
    }

    //Stop button
    public void OnStop()
    {
        if (stopPressed)
        {
            AlreadyPressed();
            Vibration.CreateOneShot(25);
            playing = false;

        }
        else
        {
            //If some button was pressed before, we release that button.
            if (playPressed || rewindPressed || forwardPressed)
                StartCoroutine(OnRelease());
            //We set all buttons to released
            rewindPressed = false;
            playPressed = false;
            forwardPressed = false;

            //We press the button
            CassetteButton.setValue(3.5f);
            CassetteButtondEvent.start();
            stopPressed = true;
            Vibration.CreateOneShot(50);
            playing = true;


            //Stop the record
            CassetteRecordEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }
}
