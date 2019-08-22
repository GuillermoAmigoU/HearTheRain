using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FMOD.Studio;
using System.Collections;
using SimpleInputNamespace;
using FMODUnity;
using Assets.Scripts.Auxiliar;

public class Puzzle2 : MonoBehaviour, ISimpleInputDraggable
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



    [SerializeField]
    float TimeToPlayHint = 15.0f;

    [SerializeField]
    [FMODUnity.EventRef]
    public string Pista = "event:/Puzzle/Radio/RadioPuzzle";
    EventInstance Pista_Event;

    //Fmod Sound
    [SerializeField]
    [FMODUnity.EventRef]
    public string Radio = "event:/Puzzle/Radio/RadioPuzzle";
    EventInstance Radio_Event;

    //parameters to use
    ParameterInstance r1;
    public ParameterInstance r2;

    //PArameter to be dial 1 or dial 2
    public bool dial1 = true;

    //Player prefab
    public GameObject Player;

    //the other dial
    public GameObject Dial;

    //WinHearTime
    public float waitTime = 3.0f;

    //To win just once.
    private bool once = false;

    private bool win = false;

    private float time = 0.0f;
    private int wheelAnlge;

    private void Awake()
    {
        Constantes.CAN_MOVE = true;

        //if this is not the dial 2, we create and reproduce sound
        if (Dial != null)
        {
            Radio_Event = RuntimeManager.CreateInstance(Radio);
            Pista_Event = RuntimeManager.CreateInstance(Pista);

            //we get the parameters from fmod to use them here
            Radio_Event.getParameter("Radio", out r1);
            Radio_Event.getParameter("Radio2", out r2);

            //We atach and play
            RuntimeManager.AttachInstanceToGameObject(Radio_Event, Player.transform, Player.transform.GetComponent<Rigidbody>());
            RuntimeManager.AttachInstanceToGameObject(Pista_Event, Player.transform, Player.transform.GetComponent<Rigidbody>());

            Radio_Event.start();
            if (dial1)
                StartCoroutine(PlayPista(2.0f));
        }

        wheel = GetComponent<Graphic>();
        wheelTR = wheel.rectTransform;

        SimpleInputDragListener eventReceiver = gameObject.AddComponent<SimpleInputDragListener>();
        eventReceiver.Listener = this;
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

    IEnumerator PlayPista(float time)
    {
        yield return new WaitForSeconds(time);
        Pista_Event.start();
    }

    private void OnUpdate()
    {


        if (Constantes.CAN_MOVE)
        {

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


            //To make the wheel block at negative angles
            if (wheelAngle < 0)
            {
                wheelAngle = 0;
            }


            m_value = wheelAngle * valueMultiplier / maximumSteeringAngle;


            axis.value = m_value;


        }
    }

    private void Update()
    {

        time += Time.deltaTime;

        //Hint
        if (dial1 && !win)
        {
            if (time >= TimeToPlayHint)
            {
                time = 0.0f;
                StartCoroutine(PlayPista(0.0f));
            }
        }


        if (Input.GetButton("Snap") && Constantes.CAN_MOVE)
        {
            //Stop sounds if we re leaving the puzzle
            Radio_Event.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            Dial.GetComponent<Puzzle2>().Radio_Event.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    private void UpdateSound()
    {

        Vibration.CreateOneShot(10);

        //Dial 1 parameter changes
        if (dial1)
        {
            //Set value to the FMOD parameter
            r1.setValue(m_value);
            StartCoroutine(WaitForWin());
        }
        //Dial 2 parameter changes
        else if (!dial1)
        {
            //Change value to the parameter of the main object
            Dial.GetComponent<Puzzle2>().r2.setValue(m_value);
            Dial.GetComponent<Puzzle2>().UpdateSound();
        }

    }

    private IEnumerator WaitForWin()
    {

        //If after certain time in the right channel, we finish the puzle and return
        yield return new WaitForSeconds(waitTime);
        CheckForWin();


    }


    void CheckForWin()
    {
        float r;
        r2.getValue(out r);
        if (r <= 1.0f && r >= 0.8f && !once)
        {
            r1.getValue(out r);
            if (r <= 0.7f && r >= 0.5f)
            {
                once = true;
                Pista_Event.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                win = true;

                //Disable input until VO has stopped.
                Constantes.CAN_MOVE = false;
                StartCoroutine(WaitAudioAndFinish(Radio_Event));
            }
        }
    }

    IEnumerator WaitAudioAndFinish(EventInstance audio)
    {
        //We check if the audio has finished
        PLAYBACK_STATE pbState;
        audio.getPlaybackState(out pbState);

        while (!pbState.Equals(PLAYBACK_STATE.STOPPED))
        {
            //Wait for it to end
            audio.getPlaybackState(out pbState);
            yield return null;
        }

        //Finish the puzzle
        Vibration.CreateOneShot(150);

        //Disable UI elements to avoid extra interaction after winning
        this.GetComponent<Puzzle2>().enabled = false;
        Dial.GetComponent<Puzzle2>().enabled = false;

        //When it does finish, we teleport the player and stop all audios.
        Player.GetComponent<PuzzleController>().WinPuzzle();
        Radio_Event.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        Dial.GetComponent<Puzzle2>().Radio_Event.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Executed when mouse/finger starts touching the steering wheel
        wheelBeingHeld = true;
        centerPoint = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, wheelTR.position);
        wheelPrevAngle = Vector2.Angle(Vector2.up, eventData.position - centerPoint);
        Constantes.CAN_MOVE = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Constantes.CAN_MOVE)
        {
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
            UpdateSound();
        }

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Executed when mouse/finger stops touching the steering wheel
        // Performs one last OnDrag calculation, just in case
        OnDrag(eventData);

        wheelBeingHeld = false;
    }
}
