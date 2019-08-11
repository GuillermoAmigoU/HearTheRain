using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts.Auxiliar;

public class Puzzle6 : MonoBehaviour
{
    float elapsed = 0f;
    bool win = false;

    [SerializeField]
    [FMODUnity.EventRef]
    public string FirstHint = "event:/Puzzle/Radio/RadioPuzzle";
    EventInstance FirstHint_Event;

    [SerializeField]
    [FMODUnity.EventRef]
    public string Pista = "event:/Puzzle/Radio/RadioPuzzle";
    EventInstance Pista_Event;

    [FMODUnity.EventRef]
    public string ButtonSound = "event:/Puzzle/Living Room/WindowPuzzle";
    EventInstance ButtonSoundEvent;

    [FMODUnity.EventRef]
    public string VO_Phrase = "event:/Puzzle/Living Room/WindowPuzzle";
    EventInstance VO_Phrase_Event;

    [FMODUnity.EventRef]
    public string SolutionPattern = "event:/Puzzle/Living Room/WindowPuzzle";
    EventInstance SolutionPatternEvent;

    //"1-2" Button 1, "2.1-3" Button 2, "3.1-4" Button 3, "4.1-5" Button 4, "5.1-6" Failed, "6.1-7" Correct
    ParameterInstance ButtonState;


    //The player
    public GameObject Player;


    //Solution from the player

    int[] possibleSolution = new int[4];

    //Solution
    int[] solution = new int[4] { 1, 4, 3, 2 };

    int buttonsPressed = 0;

    bool once = false;

    public float WaitForVO = 1.0f;

    bool firstTime = true;

    //To avoid two phrases at once when narrative VO is on
    bool phraseNotSound = false;
    bool pistaPlaying = false;

    // Use this for initialization
    void Start()
    {

        FirstHint_Event = RuntimeManager.CreateInstance(FirstHint);
        ButtonSoundEvent = RuntimeManager.CreateInstance(ButtonSound);
        SolutionPatternEvent = RuntimeManager.CreateInstance(SolutionPattern);
        VO_Phrase_Event = RuntimeManager.CreateInstance(VO_Phrase);
        Pista_Event = RuntimeManager.CreateInstance(Pista);
        //we get the parameters from fmod to use them here. 1-2 button 1
        ButtonSoundEvent.getParameter("ButtonState", out ButtonState);

        RuntimeManager.AttachInstanceToGameObject(Pista_Event, Player.transform, Player.transform.GetComponent<Rigidbody>());
        RuntimeManager.AttachInstanceToGameObject(FirstHint_Event, Player.transform, Player.transform.GetComponent<Rigidbody>());
        RuntimeManager.AttachInstanceToGameObject(ButtonSoundEvent, Player.transform, Player.transform.GetComponent<Rigidbody>());
        RuntimeManager.AttachInstanceToGameObject(SolutionPatternEvent, Player.transform, Player.transform.GetComponent<Rigidbody>());
        RuntimeManager.AttachInstanceToGameObject(VO_Phrase_Event, Player.transform, Player.transform.GetComponent<Rigidbody>());

        Constantes.CAN_MOVE = true;

        //Reproduce first hint, number and placement of buttons
        FirstHint_Event.start();
    }



    private void Update()
    {

        //We check if the hints are playing, to not play the other hint at the same time
        PLAYBACK_STATE pbState;
        Pista_Event.getPlaybackState(out pbState);

        if (!pbState.Equals(PLAYBACK_STATE.STOPPED))
            pistaPlaying = true;
        else if (pbState.Equals(PLAYBACK_STATE.STOPPED))
            pistaPlaying = false;


        //NormalHint, and if we ain't reproducing the other hint
        if (!firstTime && !win && !pistaPlaying)
        {
            elapsed += Time.deltaTime;
            //Every X seconds
            if (elapsed >= 90.0f)
            {
                elapsed = elapsed % 1f;
                FirstHint_Event.start();
            }

        }

        //First clue, if we ain't reproducing the other hint
        if (firstTime && !win && !pistaPlaying)
        {
            elapsed += Time.deltaTime;
            //Every X seconds
            if (elapsed >= 15.0f)
            {
                elapsed = elapsed % 1f;
                FirstHint_Event.start();
            }

        }

        if (buttonsPressed == 4)
        {
            CheckWin();
        }
    }

    private float Randombetween(float min, float max) => Random.Range(min, max);

    IEnumerator PlayVO(float time)
    {
        yield return new WaitForSeconds(time);
        VO_Phrase_Event.start();
    }

    //When winning
    IEnumerator WinPuzzlePad()
    {
        if (!once)
        {
            once = true;
            Constantes.CAN_MOVE = false;
            yield return new WaitForSeconds(0.8f);
            ButtonState.setValue(6.5f);
            ButtonSoundEvent.start();
            Vibration.CreateOneShot(70);
            yield return new WaitForSeconds(WaitForVO);
            Player.GetComponent<PuzzleController_Puzzle6>().WinPuzzle();
            win = true;
        }
    }

    void CheckWin()
    {
        if (solution.SequenceEqual(possibleSolution))
        {
            //Play clear sound and coroutine to exit puzzle if VO is neccesary
            //Play win sound
            StartCoroutine(WinPuzzlePad());
        }
        else
        {
            if (firstTime)
            {
                firstTime = false;
                StartCoroutine(PlayVO(4.0f));
                phraseNotSound = true;
            }

            //Play error sound
            buttonsPressed = 0;
            StartCoroutine(Failed());
            Vibration.CreateOneShot(70);
        }

    }

    IEnumerator Failed()
    {
        yield return new WaitForSeconds(0.6f);
        //Play failed sound
        ButtonSoundEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        ButtonState.setValue(5.5f);
        ButtonSoundEvent.start();

        //Disable input when receiving feedback
        Constantes.CAN_MOVE = false;

        yield return new WaitForSeconds(2.0f);
        //Play the solution reference
        SolutionPatternEvent.start();
        float val=1;

        if (!phraseNotSound)
        {
            yield return new WaitForSeconds(2.0f);

        val = Random.value;
            //We play randomly. Aprox half the times
            if (val < 0.3f)
                Pista_Event.start();

        }

        if(val < 0.3f)
        yield return new WaitForSeconds(3.5f);

        //enable input after hint
        Constantes.CAN_MOVE = true;

        if (phraseNotSound)
            phraseNotSound = false;

    }

    public void onButton1()
    {
        if (Constantes.CAN_MOVE)
        {
            ButtonSoundEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            //Play button sound
            ButtonState.setValue(1.5f);
            ButtonSoundEvent.start();
            Vibration.CreateOneShot(20);

            //Logistic of value
            buttonsPressed++;
            possibleSolution[buttonsPressed - 1] = 1;
        }
    }

    public void onButton2()
    {
        if (Constantes.CAN_MOVE)
        {
            ButtonSoundEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            //Play button sound
            ButtonState.setValue(2.5f);
            ButtonSoundEvent.start();
            Vibration.CreateOneShot(20);

            //Logistic of value
            buttonsPressed++;
            possibleSolution[buttonsPressed - 1] = 2;
        }
    }

    public void onButton3()
    {
        if (Constantes.CAN_MOVE)
        {
            ButtonSoundEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            //Play button sound
            ButtonState.setValue(3.5f);
            ButtonSoundEvent.start();
            Vibration.CreateOneShot(20);

            //Logistic of value
            buttonsPressed++;
            possibleSolution[buttonsPressed - 1] = 3;
        }
    }

    public void onButton4()
    {
        if (Constantes.CAN_MOVE)
        {
            ButtonSoundEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            //Play button sound
            ButtonState.setValue(4.5f);
            ButtonSoundEvent.start();
            Vibration.CreateOneShot(20);

            //Logistic of value
            buttonsPressed++;
            possibleSolution[buttonsPressed - 1] = 4;
        }
    }
}
