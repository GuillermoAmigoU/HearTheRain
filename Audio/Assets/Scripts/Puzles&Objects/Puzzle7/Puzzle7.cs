using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FMOD.Studio;
using System.Collections;
using SimpleInputNamespace;
using Assets.Scripts.Auxiliar;
using FMODUnity;
using UnityEngine.SceneManagement;

public class Puzzle7 : MonoBehaviour, ISimpleInputDraggable
{
    public SimpleInput.AxisInput axis = new SimpleInput.AxisInput("Horizontal");

    private Graphic wheel;

    private RectTransform wheelTR;
    private Vector2 centerPoint;

    public float maximumSteeringAngle = 200f;
    public float wheelReleasedSpeed = 350f;
    public float valueMultiplier = 1f;

    private float wheelAngle = 0f;
    private float wheelPrevAngle = 0f;

    private bool wheelBeingHeld = false;

    private float m_value;
    public float Value { get { return m_value; } }

    public float Angle { get { return wheelAngle; } }

    float coolDown = 0.6f;
    float coolDownTimer = 0.0f;

    [SerializeField]
    [FMODUnity.EventRef]
    public string Pista = "event:/Puzzle/Radio/RadioPuzzle";
    EventInstance Pista_Event;

    //Sound when turning
    [FMODUnity.EventRef]
    public string TurnSound;
    EventInstance TurnSoundEvent;

    //Sound when turning
    [FMODUnity.EventRef]
    public string openingSound;
    EventInstance openingSoundEvent;

    //Sound when going wrong
    [FMODUnity.EventRef]
    public string brokenDoorSound;
    EventInstance brokenDoorEvent;

    //Sound
    [FMODUnity.EventRef]
    public string endCinematic;
    FMOD.Studio.EventInstance endCinematicEvent;

    public GameObject Ambient;
    public GameObject Reverb;

    //0-1 First lock opened, 1-2 second lock opened, 2-3 whole door opened.
    ParameterInstance openingState;

    [SerializeField]
    GameObject Player = null;

    //If the player has gone through one thrid or mid

    bool third = false;
    bool mid = false;
    bool once = false;
    private int wheelAnlge;
    private float oldValue;

    float elapsed = 0f;
    bool win = false;

    private void Start()
    {

        TurnSoundEvent = FMODUnity.RuntimeManager.CreateInstance(TurnSound);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(TurnSoundEvent, Player.transform, Player.GetComponent<Rigidbody>());

        openingSoundEvent = FMODUnity.RuntimeManager.CreateInstance(openingSound);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(openingSoundEvent, Player.transform, Player.GetComponent<Rigidbody>());

        Pista_Event = FMODUnity.RuntimeManager.CreateInstance(Pista);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(Pista_Event, Player.transform, Player.GetComponent<Rigidbody>());

        brokenDoorEvent = FMODUnity.RuntimeManager.CreateInstance(brokenDoorSound);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(brokenDoorEvent, Player.transform, Player.GetComponent<Rigidbody>());

        endCinematicEvent = RuntimeManager.CreateInstance(endCinematic);
        RuntimeManager.AttachInstanceToGameObject(endCinematicEvent, Player.transform, Player.GetComponent<Rigidbody>());

        openingSoundEvent.getParameter("OpenState", out openingState);


        wheel = GetComponent<Graphic>();
        wheelTR = wheel.rectTransform;

        SimpleInputDragListener eventReceiver = gameObject.AddComponent<SimpleInputDragListener>();
        eventReceiver.Listener = this;

        Constantes.CAN_MOVE = true;

        //First time hint
        StartCoroutine(PlayPista(0.0f));

    }

    private void Awake()
    {

        TurnSoundEvent = FMODUnity.RuntimeManager.CreateInstance(TurnSound);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(TurnSoundEvent, Player.transform, Player.GetComponent<Rigidbody>());

        openingSoundEvent = FMODUnity.RuntimeManager.CreateInstance(openingSound);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(openingSoundEvent, Player.transform, Player.GetComponent<Rigidbody>());

        Pista_Event = FMODUnity.RuntimeManager.CreateInstance(Pista);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(Pista_Event, Player.transform, Player.GetComponent<Rigidbody>());

        brokenDoorEvent = FMODUnity.RuntimeManager.CreateInstance(brokenDoorSound);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(brokenDoorEvent, Player.transform, Player.GetComponent<Rigidbody>());

        endCinematicEvent = RuntimeManager.CreateInstance(endCinematic);
        RuntimeManager.AttachInstanceToGameObject(endCinematicEvent, Player.transform, Player.GetComponent<Rigidbody>());

        openingSoundEvent.getParameter("OpenState", out openingState);

        wheel = GetComponent<Graphic>();
        wheelTR = wheel.rectTransform;

        SimpleInputDragListener eventReceiver = gameObject.AddComponent<SimpleInputDragListener>();
        eventReceiver.Listener = this;

        Constantes.CAN_MOVE = true;

        //First time hint
        StartCoroutine(PlayPista(0.0f));

    }

    private void OnEnable()
    {
        axis.StartTracking();
        SimpleInput.OnUpdate += OnUpdate;
    }

    private void OnDisable()
    {
        axis.StopTracking();
        SimpleInput.OnUpdate -= OnUpdate;
    }

    private void OnUpdate()
    {

        //We are going the wrong way
        if (oldValue > wheelAngle)
        {
            TurnSoundEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        // If the wheel is released, reset the rotation
        // to initial (zero) rotation by wheelReleasedSpeed degrees per second
        if (!wheelBeingHeld && wheelAngle != 0f)
        {
            float deltaAngle = wheelReleasedSpeed * Time.deltaTime;
            if (Mathf.Abs(deltaAngle) > Mathf.Abs(wheelAngle))
                wheelAngle = 0f;
            else if (wheelAngle > 0f)
                wheelAngle -= deltaAngle;
            else
                wheelAngle += deltaAngle;
        }

        // Rotate the wheel image
        wheelTR.localEulerAngles = new Vector3(0f, 0f, -wheelAngle);

        if (wheelAngle < 0)
        {
            wheelAngle = 0;

            RuntimeManager.PlayOneShot(brokenDoorSound);

            TurnSoundEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        m_value = wheelAngle * valueMultiplier / maximumSteeringAngle;

        axis.value = m_value;

        oldValue = wheelAngle;
    }

    void Sound()
    {
        if (coolDownTimer == 0.0f)
        {
            TurnSoundEvent.start();
            coolDownTimer = coolDown;
        }
    }

    IEnumerator PlayPista(float time)
    {
        yield return new WaitForSeconds(time);
        Pista_Event.start();
        Constantes.CAN_MOVE = true;
    }

    private void Update()
    {

        //Clue i player has not the wheel onits hands
        if (!win && !wheelBeingHeld)
        {
            elapsed += Time.deltaTime;
            //Every X seconds
            if (elapsed >= 13.0f)
            {
                elapsed = elapsed % 1f;
                StartCoroutine(PlayPista(0.0f));
            }

        }

        //Cooldown
        if (coolDownTimer > 0)
        {
            coolDownTimer -= Time.deltaTime;
        }

        if (coolDownTimer < 0)
        {
            coolDownTimer = 0;
        }


        if (m_value >= 0.33f && !third)
        {
            third = true;
            openingState.setValue(0.0f);
            openingSoundEvent.start();
            Vibration.CreateOneShot(40);

        }
        else if (m_value >= 0.66f && !mid)
        {
            mid = true;
            openingState.setValue(1.5f);
            openingSoundEvent.start();
            Vibration.CreateOneShot(40);

        }
        else if (m_value == 1f && !once)
        {
            once = true;
            StartCoroutine(Win());

            openingSoundEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            openingState.setValue(2.5f);
            openingSoundEvent.start();

            Vibration.CreateOneShot(60);
        }
    }


    private IEnumerator Win()
    {
        Constantes.CAN_MOVE = false;
        win = true;

        //Wait for precious sounds to end
        yield return new WaitForSeconds(4.0f);

        //Game Ending Cinematic
        StartCoroutine(EndCinematic(25.0f));

    }


    public void OnPointerDown(PointerEventData eventData)
    {
        // Executed when mouse/finger starts touching the steering wheel
        wheelBeingHeld = true;
        centerPoint = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, wheelTR.position);
        wheelPrevAngle = Vector2.Angle(Vector2.up, eventData.position - centerPoint);
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        Debug.Log("Outside the wheel");

        // Executed when mouse/finger stops touching the steering wheel
        // Performs one last OnDrag calculation, just in case
        OnDrag(eventData);

        wheelBeingHeld = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Constantes.CAN_MOVE)
        {

            Debug.Log("draging");

            Vibration.CreateOneShot(10);

            //Turning sound
            Sound();

            // Executed when mouse/finger is dragged over the steering wheel
            Vector2 pointerPos = eventData.position;

            float wheelNewAngle = Vector2.Angle(Vector2.up, pointerPos - centerPoint);

            // Do nothing if the pointer is too close to the center of the wheel
            if ((pointerPos - centerPoint).sqrMagnitude >= 400f)
            {
                if (pointerPos.x > centerPoint.x)
                    wheelAngle += wheelNewAngle - wheelPrevAngle;
                else
                    wheelAngle -= wheelNewAngle - wheelPrevAngle;
            }

            // Make sure wheel angle never exceeds maximumSteeringAngle
            wheelAngle = Mathf.Clamp(wheelAngle, -maximumSteeringAngle, maximumSteeringAngle);
            wheelPrevAngle = wheelNewAngle;
        }
    }


    //When the player ends the game. 
    private IEnumerator EndCinematic(float time)
    {

        //Deactivate all effects except the audiomatic
        Destroy(Reverb);
        Destroy(Ambient);

        //Start cinematic
        endCinematicEvent.start();

        //Wait for it to end
        yield return new WaitForSeconds(time);

        //Exit game? Return to Menu? "Thanks for playing"? HERE:
        StartCoroutine(TeleportToMenu());
    }

    IEnumerator TeleportToMenu()
    {
        //After 1 second, to wait to all Player Coroutines to end
        yield return new WaitForSeconds(0.1f);

        Vibration.CreateOneShot(150);

        //Load the map
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
    }

}
