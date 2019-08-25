using Assets.Scripts.Auxiliar;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(AudioSource))]
// a parent class to manage unit movement (player, enemies)
//[Serializable]
public abstract class MovingUnit : MonoBehaviour
{
    [Header("Occluder")]
    public GameObject occluder;

    [Header("Movement Values")]
    public float moveTime = 0.3f;		// move time
    public float turnTime = 0.3f;		// turn time
    public float moveSpeed = 0.5f;		// unit move speed
    public LayerMask blockLayer;		// layer to check obstacles (walls etc)

    private float invMoveTime;          // inverse movement time

    private bool isMoving;				// is unit moving

    //Boolean to control movement 
    public bool MovementBlock = false;

    //public bool isMovingOperator { get { return isMoving; } set { isMoving = value; } }

    private bool canMove;               // is unit able to move

    //------------------------TURNING-------------------------------


    //WallHit
    //bool canHitSound = true;
    WaitForSeconds esperaHitWal;
    public float waitHitWall = 0.3f;

    public GameObject UI;

#if UNITY_ANDROID
    [Header("Turning")]
    //Giro en android
    public float waitGiro = 0.2f;
    WaitForSeconds esperaGiro;
    bool giroAvariable = true;
#endif

    //-----------FingerSnap----------
    [Header("FingerSnap")]
    //Prefab de las colisiones.
    //public GameObject colision;
    public GameObject snapAgent;

    //Tiempo de cooldown
    public float waitSnap = 1.0f;
    bool snapAvariable = true;
    WaitForSeconds esperaChasquido;

    [FMODUnity.EventRef]
    public string SnapFingerSound;
    FMOD.Studio.EventInstance SnapFingerEvent;

    //------------SOUND-----------------

    //FMOD variables
    //[FMODUnity.EventRef]
    //public string HitWallSound;
    //FMOD.Studio.EventInstance HitWallEvent;


    [Header("FMOD Sounds")]
    //Nombres
    [FMODUnity.EventRef]
    public string footstepSound;
    [FMODUnity.EventRef]
    public string TurningSound;
    [FMODUnity.EventRef]
    public string HitWallSound;

    EventInstance footstepSoundEvent;
    EventInstance HitWallSoundEvent;
    EventInstance TurningEvent;


    ParameterInstance LPF_Move;
    ParameterInstance LPF_Wall;

    ParameterInstance PanMove;
    ParameterInstance panTurn;
    ParameterInstance PanHitWall;





    //tratar los axis como botones, para hacerlo universal Android/PC
    private bool m_isAxisInUseH = false;
    private bool m_isAxisInUseV = false;

    //------------ANDROID------------------

    //Get the axis used from the input.
    private int vertical;
    private int horizontal;

    //----------Puzzle List, to use it with the snapFinger-------------------
    [Header("Puzzles")]
    public GameObject[] PuzzleList;

    //----------List of savepoints and dialogsvolumes
    [Header("SavePoint and DialogVolume Lists")]

    [SerializeField]
    GameObject[] audioVolumes = null;
    [SerializeField]
    GameObject[] savePoints = null;

    void Start()
    {
        //occluder = GameObject.FindGameObjectWithTag("Occluder");
        //Sound initialize
        //HitWallEvent = FMODUnity.RuntimeManager.CreateInstance(HitWallSound);
        SnapFingerEvent = FMODUnity.RuntimeManager.CreateInstance(SnapFingerSound);

        //GameObject.FindGameObjectWithTag("GameUI")
        UI = GameObject.FindGameObjectWithTag("GameUI");
        Constantes.MAIN_UI = UI;

        //Hitwall and footstep sound events and parameters
        footstepSoundEvent = FMODUnity.RuntimeManager.CreateInstance(footstepSound);
        HitWallSoundEvent = FMODUnity.RuntimeManager.CreateInstance(HitWallSound);
        TurningEvent = FMODUnity.RuntimeManager.CreateInstance(TurningSound);

        footstepSoundEvent.getParameter("Panner", out PanMove);
        HitWallSoundEvent.getParameter("Panner", out PanHitWall);
        TurningEvent.getParameter("Panner", out panTurn);

        footstepSoundEvent.getParameter("LPF", out LPF_Move);
        HitWallSoundEvent.getParameter("LPF", out LPF_Wall);

        //Input keys
        vertical = GetComponent<MovementButtons>().getVerticalI();
        horizontal = GetComponent<MovementButtons>().getHorizontalI();

        //Walls and Floor parameters, that will be changed via Constant values

        //Prueba
        //HitWallSoundEvent.setParameterValue(Constantes.MAT_P_PAVEMENT, 1.0f);

        //audiosource = GetComponent<AudioSource>();
        esperaChasquido = new WaitForSeconds(waitSnap);
        esperaHitWal = new WaitForSeconds(waitHitWall);
        Constantes.MAIN_PLAYER = this.gameObject;

        //Carga de sonidos de movimiento
        RuntimeManager.AttachInstanceToGameObject(SnapFingerEvent, transform, GetComponent<Rigidbody>());
        RuntimeManager.AttachInstanceToGameObject(HitWallSoundEvent, transform, GetComponent<Rigidbody>());
        RuntimeManager.AttachInstanceToGameObject(footstepSoundEvent, transform, GetComponent<Rigidbody>());
        RuntimeManager.AttachInstanceToGameObject(TurningEvent, transform, GetComponent<Rigidbody>());
        SnapFingerEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        HitWallSoundEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        footstepSoundEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        TurningEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));


#if UNITY_ANDROID
        esperaGiro = new WaitForSeconds(waitGiro);

#endif
    }

    // Use this for initialization
    protected virtual void Awake()
    {
#if UNITY_STANDALONE_WIN
        //Debug.Log("Me he reactivado");
        invMoveTime = 1f / moveTime;        // calculate invMoveTime

        SnapFingerEvent = FMODUnity.RuntimeManager.CreateInstance(SnapFingerSound);
        esperaChasquido = new WaitForSeconds(waitSnap);
        esperaHitWal = new WaitForSeconds(waitHitWall);
#endif

#if UNITY_ANDROID
        invMoveTime = 1f / moveTime;		// calculate invMoveTime
#endif

    }


    // Use this for initialization
    public void Reload()
    {
        //Debug.Log("Me he reactivado");
        invMoveTime = 1f / moveTime;        // calculate invMoveTime

        StopAllCoroutines();

        SnapFingerEvent = FMODUnity.RuntimeManager.CreateInstance(SnapFingerSound);
        esperaChasquido = new WaitForSeconds(waitSnap);
        esperaHitWal = new WaitForSeconds(waitHitWall);


    }

    // function to start moving
    protected virtual void StartMove(Vector3 newDir)
    {
        // if unit is not moving and we didnt block the movement
        if (!isMoving && !MovementBlock && Constantes.CAN_MOVE)
        {
            // make a RaycastHit and call function Move from this script
            RaycastHit hit;
            Move(newDir, out hit);
        }
    }

    // function to start turning
    protected virtual void StartRotation(int newRot)
    {
        // if unit is not moving and we didnt block the turning
        if (!isMoving && !MovementBlock && Constantes.CAN_MOVE)
        {
            StartCoroutine(Turning(newRot));		// start ienumerator Turning
        }
    }

    // function to check if movement is possible
    protected virtual void Move(Vector3 newDir, out RaycastHit hit)
    {


        Vector3 start = transform.position;
        Vector3 end = newDir;

        // Colission
        if (Physics.Linecast(start, end, out hit, blockLayer))

        {
            //Draw
            Debug.DrawRay(start, end - start, Color.cyan, 0.0f);

            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                //We set the wall's material
                if (hit.transform.GetComponent<WallSound>() != null)
                    hit.transform.GetComponent<WallSound>().SetMaterials();
                //If we are on Puzle 4
                if (hit.transform.GetComponent<Puzzle4>() != null)
                    hit.transform.GetComponent<Puzzle4>().HitWall();
            }

            //WallHit sound
            //Could use GetAxisRaw(Direction) ==1 o == -1 
#if UNITY_STANDALONE_WIN
            if (Input.GetAxisRaw("Vertical") > 0)
#endif

#if UNITY_ANDROID
            if (GetComponent<MovementButtons>().getVerticalI() > 0 && Input.touchCount <= 1)
#endif
            {
                if (m_isAxisInUseV == false)
                {
                    m_isAxisInUseV = true;

                    HitWallSoundEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
                    PanHitWall.setValue(0.0f);
                    LPF_Wall.setValue(22000.0f);
                    HitWallSoundEvent.start();
                    Vibration.CreateOneShot(150);
                    Constantes.IS_HITTING_WALL = true;
                }
            }
#if UNITY_STANDALONE_WIN
            else if (Input.GetAxisRaw("Horizontal") < 0)
#endif

#if UNITY_ANDROID
            else if (GetComponent<MovementButtons>().getHorizontalI() < 0 && Input.touchCount <= 1)
#endif
            {
                if (m_isAxisInUseH == false)
                {
                    m_isAxisInUseH = true;
                    HitWallSoundEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
                    PanHitWall.setValue(-1.0f);
                    LPF_Wall.setValue(22000.0f);
                    HitWallSoundEvent.start();
                    Vibration.CreateOneShot(150);
                    Constantes.IS_HITTING_WALL = true;
                }
            }
#if UNITY_STANDALONE_WIN
            else if (Input.GetAxisRaw("Vertical") < 0)
#endif

#if UNITY_ANDROID
            else if (GetComponent<MovementButtons>().getVerticalI() < 0 && Input.touchCount <= 1)
#endif
            {
                //Sound behind the player
                if (m_isAxisInUseV == false)
                {
                    m_isAxisInUseV = true;
                    HitWallSoundEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
                    //Pan of sound
                    PanHitWall.setValue(0.0f);
                    //LowPassFilter to sound behind
                    LPF_Wall.setValue(16500.0f);
                    HitWallSoundEvent.start();
                    Vibration.CreateOneShot(150);
                    Constantes.IS_HITTING_WALL = true;
                }
            }
#if UNITY_STANDALONE_WIN
            else if (Input.GetAxisRaw("Horizontal") > 0)
#endif

#if UNITY_ANDROID
            else if (GetComponent<MovementButtons>().getHorizontalI() > 0 && Input.touchCount <= 1)
#endif

            {
                if (m_isAxisInUseH == false)
                {
                    m_isAxisInUseH = true;
                    HitWallSoundEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
                    PanHitWall.setValue(1.0f);
                    LPF_Wall.setValue(22000.0f);
                    HitWallSoundEvent.start();
                    Vibration.CreateOneShot(150);
                    Constantes.IS_HITTING_WALL = true;
                }
            }




            canMove = false;

        }
        // otherwise movement is possible: call ienumerator Movement
        else
        {
            //canHitSound = true;
            if (Constantes.CAN_MOVE == true)
            {
                canMove = true;
                Constantes.IS_HITTING_WALL = false;
                StartCoroutine(Movement(newDir));
            }
        }


    }

    // function to do actual movement from grid to grid
    protected IEnumerator Movement(Vector3 end)
    {
        // unit is moving and define the distance from unit to end position
        isMoving = true;
        float remainDist = (transform.position - end).sqrMagnitude;

        //Sonido en funcion de adonde nos movemos
        switch (Constantes.myOrientation)
        {
            case Constantes.Orientation.FORWARD:

                footstepSoundEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
                PanMove.setValue(0.0f);
                LPF_Move.setValue(22000.0f);
                footstepSoundEvent.start();
                Vibration.CreateOneShot(70); ;
                break;

            case Constantes.Orientation.BACKWARDS:

                footstepSoundEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
                PanMove.setValue(0.0f);
                LPF_Move.setValue(16500.0f);

                footstepSoundEvent.start();
                Vibration.CreateOneShot(70); ;

                break;

            case Constantes.Orientation.LEFT:

                footstepSoundEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
                PanMove.setValue(-1.0f);
                LPF_Move.setValue(22000.0f);
                footstepSoundEvent.start();
                Vibration.CreateOneShot(70); ;

                break;

            case Constantes.Orientation.RIGHT:

                footstepSoundEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
                PanMove.setValue(1.0f);
                LPF_Move.setValue(22000.0f);
                footstepSoundEvent.start();
                Vibration.CreateOneShot(70); ;

                break;

        }


        // while remaining distance is greater than 0
        while (remainDist > float.Epsilon)
        {
            // define new position, move unit by time and check new distance from unit to end position
            Vector3 newPosition = Vector3.MoveTowards(transform.position, end, invMoveTime * Time.deltaTime);
            transform.position = newPosition;
            remainDist = (transform.position - end).sqrMagnitude;
            yield return null;
        }

        isMoving = false;		// unit is no longer moving
    }



    // function to do actual turning from side to side in 90 degrees
    protected IEnumerator Turning(int newRot)
    {
#if UNITY_ANDROID
        if (giroAvariable)
        {
            giroAvariable = false;
#endif
            // unit is moving, define start and end rotation, define turn rate and time
            isMoving = true;
            int degrees = newRot * 90;
            Quaternion startRot = transform.rotation;
            Quaternion endRot = transform.rotation * Quaternion.Euler(0, degrees, 0);
            float rate = 1f / turnTime;
            float t = 1f;

            Vibration.CreateOneShot(70); ;

            //Turn sound 
            //Debug.Log(newRot);
            if (newRot == 1)
            {
                panTurn.setValue(1);
                TurningEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
                TurningEvent.start();
            }
            else
            {
                panTurn.setValue(-1);
                TurningEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
                TurningEvent.start();
            }

            // while time is greater than 0
            while (t > float.Epsilon)
            {
                // reduce turning time and rotate unit by time
                t -= Time.deltaTime * rate;
                transform.rotation = Quaternion.Slerp(endRot, startRot, t);
                yield return new WaitForEndOfFrame();
            }

            isMoving = false;       // unit is no longer moving

#if UNITY_ANDROID

            StartCoroutine(EsperaGiro());
        }
#endif

    }

    // function to handle no moving situations
    protected abstract void CannotMove<T>(T component)
        where T : Component;

    // function to get isMoving value
    public bool IsMoving()
    {
        return isMoving;
    }


    // function to get canMove value
    public bool CanMove()
    {
        return canMove;
    }

    //Snap
    private void Update()
    {

        //Snap
        if (snapAvariable)
        {

            {

#if UNITY_STANDALONE_WIN
                //Get Snap Imput
                if (Input.GetButton("Snap"))
#endif

#if UNITY_ANDROID
                if (Input.touchCount > 1)
#endif
                {
                    if (Constantes.DONT_SNAP == false && Constantes.CAN_MOVE)
                    {
                        //Cooldown
                        snapAvariable = false;
                        //Llamamos al metodo que crea las colisiones

                        //handheld.Vibrate();
                        Vibration.CreateOneShot(300);

                        //Snap sound
                        SnapFingerEvent.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
                        SnapFingerEvent.start();
                        Instantiate(snapAgent, transform.position, transform.rotation);
                        //We wait a second to make the other snap sound

                        //Snap Cooldown.
                        StartCoroutine(EsperaChasquido());

                    }
                }
            }
        }

        //Occluder
        if (Input.GetButtonDown("Occlude"))
        {
            occluder.SetActive(!occluder.activeSelf);
        }


    }

    private void FixedUpdate()
    {

        //If there is no key held down, restart the collision booleans
#if UNITY_STANDALONE_WIN
        if (Input.GetAxisRaw("Horizontal") == 0)
#endif

#if UNITY_ANDROID
        if (GetComponent<MovementButtons>().getHorizontalI() == 0)
#endif
        {
            m_isAxisInUseH = false;
        }

#if UNITY_STANDALONE_WIN
        if (Input.GetAxisRaw("Vertical") == 0)
#endif

#if UNITY_ANDROID
        if (GetComponent<MovementButtons>().getVerticalI() == 0)
#endif
        {
            m_isAxisInUseV = false;
        }
    }


    private IEnumerator EsperaChasquido()
    {
        yield return esperaChasquido;
        snapAvariable = true;
    }


#if UNITY_ANDROID
    private IEnumerator EsperaGiro()
    {
        yield return esperaGiro;
        giroAvariable = true;
    }
#endif
    


    IEnumerator playSoundDelayed(string path, Vector3 position, float time, float volume)
    {
        yield return new WaitForSeconds(time);
        FMODUnity.RuntimeManager.PlayOneShot(path, volume, position); // This will play the event once and discard it from memory when finished
    }

    public EventInstance getFootsteps()
    {
        return footstepSoundEvent;
    }

    public EventInstance getHitWall()
    {
        return HitWallSoundEvent;
    }


    public void LoadPlayer()
    {
        
        //UI.GetComponent<CompassScript>().initiate();
        //Awake();
        //Start();

        //To erase or disable past events or dialogs.
        Constantes.IS_PLAYER_LOADED = true;

        PlayerData data = SaveSystem.LoadPlayer();

        Constantes.AUDIOS_PLAYED = data.audios_Played;
        Constantes.SAVED_USED = data.saved_Used;

        //Disable used savegames and audios played
        for (int i=0;i<data.audios_Played-1;i++)
        {
            audioVolumes[i].SetActive(false);
        }

        for (int i = 0; i < data.saved_Used; i++)
        {
            savePoints[i].SetActive(false);
        }


        //We update player position to match the saved one.
        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];
        transform.position = position;

        Vector3 rotation;

        //If error rounding data, reset to 0,0,0
        if(data.rotation[0] != 0 || data.rotation[0] != 90 || data.rotation[0] != 360 || data.rotation[0] != 270)
        {
            data.rotation[0] = 0;
        }
        if (data.rotation[1] != 0 || data.rotation[1] != 90 || data.rotation[1] != 360 || data.rotation[1] != 270)
        {
            data.rotation[1] = 0;
        }
        if (data.rotation[2] != 0 || data.rotation[2] != 90 || data.rotation[2] != 360 || data.rotation[2] != 270)
        {
            data.rotation[2] = 0;
        }

        //Debug.Log("Rotacion: " + data.rotation[0] +" , "+ data.rotation[1] + " , " + data.rotation[2]);

        rotation.x = data.rotation[0];
        rotation.y = data.rotation[1];
        rotation.z = data.rotation[2];
        transform.rotation = Quaternion.Euler(rotation);

        //we update the game's data  with the one saved
        Constantes.IS_PUZZLE1_SOLVED = data.puzlesSolved[0];
        Constantes.IS_PUZZLE2_SOLVED = data.puzlesSolved[1];
        Constantes.IS_PUZZLE3_SOLVED = data.puzlesSolved[2];
        Constantes.IS_PUZZLE4_SOLVED = data.puzlesSolved[3];
        Constantes.IS_PUZZLE5_SOLVED = data.puzlesSolved[4];
        Constantes.IS_PUZZLE6_SOLVED = data.puzlesSolved[5];
        Constantes.IS_PUZZLE7_SOLVED = data.puzlesSolved[6];

        
        //Debug.Log("Position: " + data.position[0] + ", " + data.position[1] + ", " + data.position[2] + "     " + "Rotation: " + data.rotation[0] + ", " + data.rotation[1] + ", " + data.rotation[2]);     
        /*
        for (int i = 0; i < 7; i++)
        {
            Debug.Log("Puzle " + i + 1 + "solved? " + data.puzlesSolved[i]);
        }
        Debug.Log("Audios y saves: " + data.audios_Played + ", " + data.saved_Used);
        */


        //Constantes.CAN_MOVE = true;
        //Constantes.DONT_SNAP = false;

        StartCoroutine(stopLoadingProcesses());
        
    }

    IEnumerator stopLoadingProcesses()
    {
        yield return new WaitForSeconds(2.0f);
        Constantes.IS_PLAYER_LOADED = false;
        Constantes.CAN_MOVE = true;
    }
}
