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

    //To trigger the exit just once.
    bool once;

    //Time to call Exitpuzzle (wait for VO to exit puzzle)
    public float WaitForVO = 18.0f;

    //Time to Release the button
    public float time = 0.3f;

    [SerializeField]
    float TimeToPlayHint = 120.0f;

    int puzzleState = 0;

    [SerializeField]
    [FMODUnity.EventRef]
    public string Pista = "event:/VO/VO_Hint_Puzzle5";
    EventInstance Pista_Event;

    [FMODUnity.EventRef]
    public string CassetteRecordSound = "event:/Puzzle/Bedroom_Cassette/Cassette try";
    EventInstance CassetteRecordEvent;

    [FMODUnity.EventRef]
    public string CassetteButtonSound = "event:/Puzzle/Bedroom_Cassette/Cassette_Buttons";
    EventInstance CassetteButtondEvent;

    //0-Rewind, 1 Fast Forward
    ParameterInstance rewindOrForward;
    //0-1 Play, 1-2 Rewind, 2-3 Fast Forward, 3-4 Stop, 4-5 Release button, 5-6 Already pressed button
    ParameterInstance CassetteButton;
    //When pressed play
    ParameterInstance Solution;
    //When repeated
    ParameterInstance Repeated;
    //The player
    public GameObject Player;

    //Button states
    bool playPressed;
    bool stopPressed;
    bool rewindPressed;
    bool forwardPressed;
    bool playing = false;

    // Use this for initialization
    void Start()
    {
        CassetteRecordEvent = RuntimeManager.CreateInstance(CassetteRecordSound);
        CassetteButtondEvent = RuntimeManager.CreateInstance(CassetteButtonSound);
        Pista_Event = RuntimeManager.CreateInstance(Pista);

        //we get the parameters from fmod to use them here
        CassetteRecordEvent.getParameter("Button", out rewindOrForward);
        CassetteButtondEvent.getParameter("CassetteButton", out CassetteButton);
        CassetteRecordEvent.getParameter("Solution", out Solution);
        CassetteRecordEvent.getParameter("Repeat", out Repeated);

        RuntimeManager.AttachInstanceToGameObject(CassetteRecordEvent, Player.transform, Player.transform.GetComponent<Rigidbody>());
        RuntimeManager.AttachInstanceToGameObject(CassetteButtondEvent, Player.transform, Player.transform.GetComponent<Rigidbody>());
        RuntimeManager.AttachInstanceToGameObject(Pista_Event, Player.transform, Player.transform.GetComponent<Rigidbody>());


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

    IEnumerator PlayPista(float time)
    {
        yield return new WaitForSeconds(time);
        Pista_Event.start();
    }




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
            playPressed = true;

            CassetteButton.setValue(0.5f);
            CassetteButtondEvent.start();
            Solution.setValue(1.0f);

            //Get info: Is it paused, did it start the first time?
            bool paused;
            CassetteRecordEvent.getPaused(out paused);
            PLAYBACK_STATE state;
            CassetteRecordEvent.getPlaybackState(out state);

            //If paused, unpause, if didnt start, start.
            if (state != PLAYBACK_STATE.PLAYING)
                CassetteRecordEvent.start();

            //Seconds range to win
            int pos;
            CassetteRecordEvent.getTimelinePosition(out pos);
            if (pos > 4000 && pos < 6500)
            {
                Vibration.CreateOneShot(75);
                StartCoroutine(WinPuzzleCassette());
            }
        }
    }

    //When winning
    IEnumerator WinPuzzleCassette()
    {
        win = true;
        Constantes.CAN_MOVE = false;
        yield return new WaitForSeconds(WaitForVO);
        Player.GetComponent<PuzzleController_Puzzle5>().WinPuzzle();
        once = true;
        Pista_Event.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
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
        //CassetteButtondEvent.setPaused(false);
    }

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
            stopPressed = true;

            CassetteButton.setValue(3.5f);
            CassetteButtondEvent.start();

            CassetteRecordEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Solution.setValue(0.0f);
        }
    }

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
            forwardPressed = true;

            if (puzzleState != 1)
            {
                puzzleState = 1;
                CassetteButton.setValue(2.5f);
                CassetteButtondEvent.start();

                rewindOrForward.setValue(1.0f);
                Solution.setValue(0.0f);

                Repeated.setValue(0.0f);


                CassetteRecordEvent.start();
            }
            else if(puzzleState == 1)
            {
                CassetteButton.setValue(2.5f);
                CassetteButtondEvent.start();

                Repeated.setValue(1.0f);


                rewindOrForward.setValue(1.0f);
                Solution.setValue(0.0f);
                CassetteRecordEvent.start();
            }
        }

    }

    public void OnBackwards()
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
            rewindPressed = true;

            if (puzzleState != -1)
            {
                puzzleState = -1;
                CassetteButton.setValue(1.5f);
                CassetteButtondEvent.start();

                rewindOrForward.setValue(0.0f);
                Solution.setValue(0.0f);

                Repeated.setValue(0.0f);

                CassetteRecordEvent.start();
            }
            else if(puzzleState == -1)
            {
                CassetteButton.setValue(1.5f);
                CassetteButtondEvent.start();
                rewindOrForward.setValue(0.0f);
                Solution.setValue(0.0f);

                Repeated.setValue(1.0f);
                CassetteRecordEvent.start();

            }
        }
    }

}
