using Assets.Scripts.Auxiliar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleEnter : MonoBehaviour
{

    //Enter to puzzle sound
    [FMODUnity.EventRef]
    public string EnterSound = "event:/Puzzle/Common/PuzzleIn";
    FMOD.Studio.EventInstance EnterSoundEvent;

    //Time to travel, to make sure the sound its played till the end
    public float WaitToTP = 4.0f;
    private WaitForSeconds wait;

    //public Object mapa;
    public string mapName = "FirstPuzzle";

    //So we teleport only 1 time
    private bool tpActive = false;

    private void Start()
    {
        EnterSoundEvent = FMODUnity.RuntimeManager.CreateInstance(EnterSound);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(EnterSoundEvent, Constantes.MAIN_PLAYER.transform, Constantes.MAIN_PLAYER.GetComponent<Rigidbody>());
        EnterSoundEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(this.transform.position));

        wait = new WaitForSeconds(WaitToTP);
    }

    //If the player interacts with the puzzle Object, it gets teleported to the puzzle
    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {


#if UNITY_STANDALONE_WIN
            //If player interacts with object
            if (Input.GetButton("Snap") && !tpActive)
#endif

#if UNITY_ANDROID
          //If player interacts with object
          if ((Input.touchCount > 1) && !tpActive)
#endif
            {
                Constantes.DONT_SNAP = true;
                Constantes.CAN_MOVE = false;

                //disable player UI
                Constantes.MAIN_UI.SetActive(false);
                //Into the puzzle sound
                EnterSoundEvent.start();


                //Teleport to puzzle
                StartCoroutine(Teleport());
                tpActive = true;
            }
        }

    }


    IEnumerator Teleport()
    {
        //After 1 second, to wait to all Player Coroutines to end
        yield return wait;

        //Enter puzzle sound

        Vibration.CreateOneShot(150);

        //Load the map
        SceneManager.LoadSceneAsync(mapName, LoadSceneMode.Additive);

        //If we are on PC, we disable player's movement
#if UNITY_STANDALONE_WIN
        Constantes.MAIN_PLAYER.GetComponent<MovingUnit>().MovementBlock = true;
#endif

        Constantes.MAIN_PLAYER.SetActive(false);
        tpActive = false;
        Constantes.CAN_MOVE = true;
    }


}
