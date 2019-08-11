using Assets.Scripts.Auxiliar;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Father of all PuzzleControllers to have the exit functionability
public class PuzzleController : MonoBehaviour
{


    [FMODUnity.EventRef]
    public string exitPuzzleSound;

    //Sound when failing
    [FMODUnity.EventRef]
    public string Win_Sound;
    EventInstance winSoundEvent;

    public float TimeToTP = 3.5f;
    public string mapToUnload = "FirstPuzzle";
    bool once = false;

    private void Start()
    {
        once = false;

        Constantes.CAN_MOVE = true;

        FMODUnity.RuntimeManager.CreateInstance(Win_Sound);
    }
    // Update is called once per frame
    void Update()
    {
        CheckExit();
    }

    //Check if we exit the puzzle
    protected void CheckExit()
    {
        if (Constantes.CAN_MOVE == true)
        {

#if UNITY_STANDALONE_WIN
        if (Input.GetButton("Snap"))
        {
            StartCoroutine(exitPuzzle(mapToUnload));
        }
#endif

#if UNITY_ANDROID
            //If we tap with two fingers the screen
            if (Input.touchCount > 1)
            {
                StartCoroutine(exitPuzzle(mapToUnload));
            }
#endif
        }

    }


    public IEnumerator exitPuzzle(string m)
    {
        if (!once)
        {
            once = true;
            Vibration.CreateOneShot(150);
            FMODUnity.RuntimeManager.PlayOneShot(exitPuzzleSound, this.transform.position);

            //We wait to Coroutines to end
            yield return new WaitForSeconds(TimeToTP);


            //Back to the Main Scene
            SceneManager.UnloadSceneAsync(m);

            //Enable input
            Constantes.CAN_MOVE = true;
            //Get main player and UI
            Constantes.MAIN_PLAYER.SetActive(true);
#if UNITY_ANDROID
            Constantes.MAIN_UI.SetActive(true);
#endif
            Constantes.DONT_SNAP = false;



            //Enable PC Player controller movement
#if UNITY_STANDALONE_WIN
        Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().MovementBlock = false;
#endif
        }
    }


    public virtual void WinPuzzle()
    {

        //Play the exit sound
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(winSoundEvent, transform, GetComponent<Rigidbody>());
        winSoundEvent.start();



        StartCoroutine(this.exitPuzzle(mapToUnload));


    }
}
