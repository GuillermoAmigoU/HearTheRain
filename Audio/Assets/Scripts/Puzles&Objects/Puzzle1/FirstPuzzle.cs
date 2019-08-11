using Assets.Scripts.Auxiliar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirstPuzzle : MonoBehaviour
{
    //Sound
    [FMODUnity.EventRef]
    public string winSound, lowSound, midSound, highSound, VoiceTutorial;
    FMOD.Studio.EventInstance winSoundEvent;

    FMOD.Studio.EventInstance lowSoundEvent;
    FMOD.Studio.EventInstance midSoundEvent;
    FMOD.Studio.EventInstance highSoundEvent;
    FMOD.Studio.EventInstance VoiceTutorialEvent;

    public GameObject player;
    public Image handler;

    //To trigger the exit only 1 time.
    bool once = false;

    //Time to travel, to make sure the sound its played till the end
    public float WaitToTP = 4.0f;
    private WaitForSeconds wait;

    private bool firstTime = true;

    private void Start()
    {
        winSoundEvent = FMODUnity.RuntimeManager.CreateInstance(winSound);
        lowSoundEvent = FMODUnity.RuntimeManager.CreateInstance(lowSound);
        midSoundEvent = FMODUnity.RuntimeManager.CreateInstance(midSound);
        highSoundEvent = FMODUnity.RuntimeManager.CreateInstance(highSound);

        VoiceTutorialEvent = FMODUnity.RuntimeManager.CreateInstance(VoiceTutorial);

        winSoundEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.transform.position));
        lowSoundEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.transform.position));
        midSoundEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.transform.position));
        highSoundEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.transform.position));

        VoiceTutorialEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.transform.position));
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(VoiceTutorialEvent, player.transform, player.GetComponent<Rigidbody>());

        wait = new WaitForSeconds(WaitToTP);

        VoiceTutorialEvent.start();


    }

    //Set a random position for the puzzle. We enter the values of the mouse on the first click.
    public void SetPosition(float xi, float yi)
    {
        float x = 0;
        float y = 0;

            x = Random.Range(-312, 313);
            y = Random.Range(-178, 179);

        handler.transform.localPosition = new Vector2(x, y);

    }

    IEnumerator waitToEnd()
    {
        if (!once)
        {
            once = true;
            yield return wait;
            player.GetComponent<PuzzleController_Puzzle1>().WinPuzzle();
            Vibration.CreateOneShot(120);
        }
    }

    // Use this for initialization
    public void WinPuzzle()
    {
        if (!firstTime)
        {
            //Once we won, we disable all buttons to avoid extra interaction by the player.
            Button[] children = GetComponentsInChildren<Button>();

            foreach (Button b in children)
            {
                b.interactable = false;
            }

            //Keys found
            winSoundEvent.start();
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(winSoundEvent, player.transform, player.GetComponent<Rigidbody>());

            StartCoroutine(waitToEnd());
        }
        //If we hit the solution on the first touch input
        if (firstTime)
        {
            float xPosition = Input.mousePosition.x;
            float yPosition = Input.mousePosition.y;
            SetPosition(xPosition, yPosition);
            firstTime = false;
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(lowSoundEvent, player.transform, player.GetComponent<Rigidbody>());
            lowSoundEvent.start();
        }

    }


    //When the player touches the zones, a sound from low to high will sound to make the player get close to the solution
    public void lowSink()
    {
        if (!firstTime)
        {
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(lowSoundEvent, player.transform, player.GetComponent<Rigidbody>());
            lowSoundEvent.start();
            Vibration.CreateOneShot(40);
        }
        if (firstTime)
        {
            float xPosition = Input.mousePosition.x;
            float yPosition = Input.mousePosition.y;
            SetPosition(xPosition, yPosition);
            firstTime = false;
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(lowSoundEvent, player.transform, player.GetComponent<Rigidbody>());
            lowSoundEvent.start();
        }
    }

    //Mid area to solution
    public void MidSink()
    {
        if (!firstTime)
        {
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(midSoundEvent, player.transform, player.GetComponent<Rigidbody>());
            midSoundEvent.start();
            Vibration.CreateOneShot(60);
        }
        if (firstTime)
        {
            float xPosition = Input.mousePosition.x;
            float yPosition = Input.mousePosition.y;
            SetPosition(xPosition, yPosition);
            firstTime = false;
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(lowSoundEvent, player.transform, player.GetComponent<Rigidbody>());
            lowSoundEvent.start();
        }

    }

    //Closest area to solution
    public void HighSink()
    {
        if (!firstTime)
        {
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(highSoundEvent, player.transform, player.GetComponent<Rigidbody>());
            highSoundEvent.start();
            Vibration.CreateOneShot(80);
        }
        if (firstTime)
        {
            float xPosition = Input.mousePosition.x;
            float yPosition = Input.mousePosition.y;
            SetPosition(xPosition, yPosition);
            firstTime = false;
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(lowSoundEvent, player.transform, player.GetComponent<Rigidbody>());
            lowSoundEvent.start();
        }

    }


}


