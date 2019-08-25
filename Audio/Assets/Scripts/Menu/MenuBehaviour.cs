using Assets.Scripts.Auxiliar;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehaviour : MonoBehaviour
{

    public GameObject Player;
    public GameObject Ambient;

    [SerializeField]
    GameObject[] button = new GameObject[2];


    bool once = false;

    //Boolean to check if sound has played
    bool canContinue = false;

    //Sound
    [FMODUnity.EventRef]
    public string newGame;
    FMOD.Studio.EventInstance newGameEvent;

    //Sound
    [FMODUnity.EventRef]
    public string tutorial;
    FMOD.Studio.EventInstance tutorialEvent;

    //Sound
    [FMODUnity.EventRef]
    public string loadGame;
    FMOD.Studio.EventInstance loadGameEvent;

    //Sound
    [FMODUnity.EventRef]
    public string intro;
    FMOD.Studio.EventInstance introEvent;

    //Sound
    [FMODUnity.EventRef]
    public string introCinematic;
    FMOD.Studio.EventInstance introCinematicEvent;

    bool oncedebug =true;

    private void Awake()
    {
        canContinue = false;
        Constantes.CAN_MOVE = true;
        Constantes.DONT_SNAP = false;
    }

    private void Start()
    {
        Constantes.CAN_MOVE = true;
        Constantes.DONT_SNAP = false;

        DontDestroyOnLoad(this.gameObject);

        canContinue = false;

        newGameEvent = RuntimeManager.CreateInstance(newGame);
        RuntimeManager.AttachInstanceToGameObject(newGameEvent, Player.transform, Player.GetComponent<Rigidbody>());

        loadGameEvent = RuntimeManager.CreateInstance(loadGame);
        RuntimeManager.AttachInstanceToGameObject(loadGameEvent, Player.transform, Player.GetComponent<Rigidbody>());

        tutorialEvent = RuntimeManager.CreateInstance(tutorial);
        RuntimeManager.AttachInstanceToGameObject(tutorialEvent, Player.transform, Player.GetComponent<Rigidbody>());

        introEvent = RuntimeManager.CreateInstance(intro);
        RuntimeManager.AttachInstanceToGameObject(introEvent, Player.transform, Player.GetComponent<Rigidbody>());

        introCinematicEvent = RuntimeManager.CreateInstance(introCinematic);
        RuntimeManager.AttachInstanceToGameObject(introCinematicEvent, Player.transform, Player.GetComponent<Rigidbody>());


#if UNITY_ANDROID
        button[0].SetActive(false);
        button[1].SetActive(false);
#endif
        //First audio welcoming
        introEvent.start();
    }

    //Type:1 for new game, 0 for loading level
    private IEnumerator CheckContinue(float type)
    {
        while (!canContinue)
        {
            yield return null;
        }

        if (type == 0)
        {
            if (canContinue)
            {

                tutorialEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainMap", LoadSceneMode.Single);

                while (!asyncLoad.isDone)
                {
                    //Debug.Log("Loading");
                    yield return null;
                }

                //Debug.Log("Game Loaded");
                GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider>().enabled = false;
                GameObject.FindGameObjectWithTag("Player").GetComponent<MovingUnit>().LoadPlayer();
                StartCoroutine(LoadPlayer());
                //Debug.Log("Player loaded.");


                //Debug.Log("Deleting this script when its done");
                Destroy(this.gameObject, 2.0f);
            }
        }

        else if (type == 1)
        {

            if (canContinue)
            {

                tutorialEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

                once = true;
                //Debug.Log("Starting a new game");
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainMap", LoadSceneMode.Single);

                while (!asyncLoad.isDone)
                {
                    //Debug.Log("Loading");
                    yield return null;
                }
                //Debug.Log("Game Loaded");
                //Debug.Log("Deleting this script when its done");
                Destroy(this.gameObject, 2.0f);
            }
        }

    }
    private IEnumerator LoadPlayer()
    {
        yield return new WaitForSeconds(0.5f);
        //GameObject.FindGameObjectWithTag("Player").GetComponent<MovingUnit>().LoadPlayer();
        Constantes.CAN_MOVE = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider>().enabled = true;

        Destroy(this.gameObject, 1.5f);

    }
    //We make a sound and wait certain ammount of seconds
    private IEnumerator SoundAndWait(float time, EventInstance audio)
    {

        //audio.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        audio.start();

        yield return new WaitForSeconds(time);
        //Debug.Log("Im finished. Now you can continue");
        canContinue = true;
    }

    public void OnLoadGame()
    {
        if (!once)
        {
            //Stop all sounds
            Ambient.gameObject.SetActive(false);
            tutorialEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            introEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            once = true;
            //Go to the main map and load Player inside it.       
            StartCoroutine(SoundAndWait(5.0f, loadGameEvent));
            StartCoroutine(CheckContinue(0));

        }
    }

    public void OnStartNewGame()
    {
        tutorialEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        introEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        if (!once)
        {
            once = true;
            //Go to the main map and load Player inside it.

            StartCoroutine(SoundAndWait(6.0f, newGameEvent));

            //Stop all sounds
            Ambient.gameObject.SetActive(false);
            loadGameEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            tutorialEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            introEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            //Play first Cinematic in the game. Aproximately 90 seconds.
            StartCoroutine(InitCinematic(88.0f));

        }
    }

    public void OnTutorial()
    {
        if (Constantes.CAN_MOVE)
        {
            introEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            tutorialEvent.start();
        }
    }

    private void Update()
    {
        //To avoid screen from turning of on the cinematic
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

#if UNITY_ANDROID
        if (Constantes.CAN_MOVE == true)
        {
            if (Input.touchCount > 2)
            {
                OnStartNewGame();
                tutorialEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }

            if (Input.touchCount == 2)
            {
                OnLoadGame();
                tutorialEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }

            if (Input.touchCount == 1)
            {
                //If we hit only with one and no other finger
                if (!(Input.touchCount >= 2))
                    OnTutorial();
            }
        }
#endif

        /*
        //Saltarnos la intro. DEBUG.
        if (Constantes.CAN_MOVE == false && oncedebug)
        {
            
            if (Input.touchCount == 1)
            {
                oncedebug = false;
                Constantes.CAN_MOVE = true;
                StartCoroutine(CheckContinue(1));
            }
        }
        /**/

    }

    //When the player starts the game for the first Time. it waits to it to finish to continue.
    private IEnumerator InitCinematic(float time)
    {
        Constantes.CAN_MOVE = false;
        yield return new WaitForSeconds(6.0f);
        introCinematicEvent.start();

        //Wait for the audiomatic to finish
        yield return new WaitForSeconds(time);
        StartCoroutine(CheckContinue(1));
    }


}
