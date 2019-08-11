/// Credit mgear, SimonDarksideJ
/// Sourced from - https://forum.unity3d.com/threads/radial-slider-circle-slider.326392/#post-3143582
/// Updated to include lerping features and programatic access to angle/value

using Assets.Scripts.Auxiliar;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Radial Slider")]
    [RequireComponent(typeof(Image))]
    public class RadialSlider : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
    {
        private bool isPointerDown, isPointerReleased, lerpInProgress;
        private Vector2 m_localPos; 
        private float m_targetAngle, m_lerpTargetAngle, m_startAngle, m_currentLerpTime, m_lerpTime;
        private Camera m_eventCamera;
        private Image m_image;

        [SerializeField]
        [Tooltip("Radial Gradient Start Color")]
        private Color m_startColor = Color.green;
        [SerializeField]
        [Tooltip("Radial Gradient End Color")]
        private Color m_endColor = Color.red;
        [Tooltip("Move slider absolute or use Lerping?\nDragging only supported with absolute")]
        [SerializeField]
        private bool m_lerpToTarget;
        [Tooltip("Curve to apply to the Lerp\nMust be set to enable Lerp")]
        [SerializeField]
        private AnimationCurve m_lerpCurve;
        [Tooltip("Event fired when value of control changes, outputs an INT angle value")]
        [SerializeField]
        private RadialSliderValueChangedEvent _onValueChanged = new RadialSliderValueChangedEvent();
        [Tooltip("Event fired when value of control changes, outputs a TEXT angle value")]
        [SerializeField]
        private RadialSliderTextValueChangedEvent _onTextValueChanged = new RadialSliderTextValueChangedEvent();

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
        bool once = false;


        public float Angle
        {
            get { return RadialImage.fillAmount * 360f; }
            set
            {
                if (LerpToTarget)
                {
                    StartLerp(value / 360f);
                }
                else
                {
                    UpdateRadialImage(value / 360f);
                }
            }
        }

        public float Value
        {
            get { return RadialImage.fillAmount; }
            set
            {
                if (LerpToTarget)
                {
                    StartLerp(value);
                }
                else
                {
                    UpdateRadialImage(value);
                }
            }
        }

        
        public Color EndColor
        {
            get { return m_endColor; }
            set { m_endColor = value; }
        }

        public Color StartColor
        {
            get { return m_startColor; }
            set { m_startColor = value; }
        }
        

        public bool LerpToTarget
        {
            get { return m_lerpToTarget; }
            set { m_lerpToTarget = value; }
        }

        public AnimationCurve LerpCurve
        {
            get { return m_lerpCurve; }
            set { m_lerpCurve = value; m_lerpTime = LerpCurve[LerpCurve.length - 1].time; }
        }

        public bool LerpInProgress
        {
            get { return lerpInProgress; }
        }

        [Serializable]
        public class RadialSliderValueChangedEvent : UnityEvent<int> { }
        [Serializable]
        public class RadialSliderTextValueChangedEvent : UnityEvent<string> { }

        public Image RadialImage
        {
            get
            {
                if (m_image == null)
                {
                    m_image = GetComponent<Image>();
                    m_image.type = Image.Type.Filled;
                    m_image.fillMethod = Image.FillMethod.Radial360;
                    m_image.fillOrigin = 3;
                    m_image.fillAmount = 0;
                }
                return m_image;
            }
        }

        public RadialSliderValueChangedEvent onValueChanged
        {
            get { return _onValueChanged; }
            set { _onValueChanged = value; }
        }
        public RadialSliderTextValueChangedEvent onTextValueChanged
        {
            get { return _onTextValueChanged; }
            set { _onTextValueChanged = value; }
        }

        private void Start()
        {
            //if this is not the dial 2, we create and reproduce sound
                if (Dial != null) {
                Radio_Event = RuntimeManager.CreateInstance(Radio);

                //we get the parameters from fmod to use them here
                Radio_Event.getParameter("Radio", out r1);
                Radio_Event.getParameter("Radio2", out r2);

                //We atach and play
                RuntimeManager.AttachInstanceToGameObject(Radio_Event, Player.transform, Player.transform.GetComponent<Rigidbody>());
                Radio_Event.start();
                }
        }

        private void Awake()
        {
            if (LerpCurve != null && LerpCurve.length > 0)
            {
                m_lerpTime = LerpCurve[LerpCurve.length - 1].time;
            }
            else
            {
                m_lerpTime = 1;
            }

            //We atach and play
            RuntimeManager.AttachInstanceToGameObject(Radio_Event, Player.transform, Player.transform.GetComponent<Rigidbody>());
            //Debug.Log("play");
            Radio_Event.start();

        }

        private void Update()
        {

            if (Input.GetButton("Snap"))
            {
                //Stop sounds if we re leaving the puzzle
                Radio_Event.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                Dial.GetComponent<RadialSlider>().Radio_Event.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }
            
            if (isPointerDown)
            {
                m_targetAngle = GetAngleFromMousePoint();
                //Debug.Log(m_targetAngle + " es el angulo que buscas");
                if (!lerpInProgress)
                {
                    if (!LerpToTarget)
                    {
                        UpdateRadialImage(m_targetAngle);
                        UpdateSound();
                        NotifyValueChanged();
                    }
                    else
                    {
                        if (isPointerReleased) StartLerp(m_targetAngle);
                        isPointerReleased = false;
                    }
                }
            }
            if (lerpInProgress && Value != m_lerpTargetAngle)
            {
                m_currentLerpTime += Time.deltaTime;
                float perc = m_currentLerpTime / m_lerpTime;
                if (LerpCurve != null && LerpCurve.length > 0)
                {
                    UpdateRadialImage(Mathf.Lerp(m_startAngle, m_lerpTargetAngle, LerpCurve.Evaluate(perc)));
                }
                else
                {
                    UpdateRadialImage(Mathf.Lerp(m_startAngle, m_lerpTargetAngle, perc));
                }
            }
            if (m_currentLerpTime >= m_lerpTime || Value == m_lerpTargetAngle)
            {
                lerpInProgress = false;
                UpdateRadialImage(m_lerpTargetAngle);
                NotifyValueChanged();
            }
        }

        private void StartLerp(float targetAngle)
        {
            if (!lerpInProgress)
            {
                m_startAngle = Value;
                m_lerpTargetAngle = targetAngle;
                m_currentLerpTime = 0f;
                lerpInProgress = true;
            }
        }

        private float GetAngleFromMousePoint()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, m_eventCamera, out m_localPos);

            // radial pos of the mouse position.
            return (Mathf.Atan2(-m_localPos.y, m_localPos.x) * 180f / Mathf.PI + 180f) / 360f;
        }

        private void UpdateRadialImage(float targetAngle)
        {
            RadialImage.fillAmount = targetAngle;

            RadialImage.color = Color.Lerp(m_startColor, m_endColor, targetAngle);
        }

        private void NotifyValueChanged()
        {
            _onValueChanged.Invoke((int)(m_targetAngle * 360));
            _onTextValueChanged.Invoke(((int)(m_targetAngle * 360)).ToString());
        }

        private void UpdateSound()
        {

            Vibration.CreateOneShot(10);

            //Dial 1 parameter changes
            if (dial1){ 
                //Set value to the FMOD parameter
                r1.setValue(m_targetAngle);

                /* Debug
                r1.getValue(out r);
                Debug.Log("r1 "+r);

                r2.getValue(out r);
                Debug.Log("r2 "+r);
                */
                StartCoroutine(WaitForWin());
                //CheckForWin();
            }
            //Dial 2 parameter changes
            else if (!dial1) { 
                //Change value to the parameter of the main object
                Dial.GetComponent<RadialSlider>().r2.setValue(m_targetAngle);
                Dial.GetComponent<RadialSlider>().UpdateSound();
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
                    //We check if the audio has finished
                    PLAYBACK_STATE pLAYBACK_STATE;
                    Radio_Event.getPlaybackState(out pLAYBACK_STATE);

                    //While the sound hasnt finished
                    while (!pLAYBACK_STATE.Equals(PLAYBACK_STATE.STOPPED))
                    {
                        Radio_Event.getPlaybackState(out pLAYBACK_STATE);
                        //Wait for it to end
                    }

                    Vibration.CreateOneShot(150);

                    //Disable UI elements toa void extra interaction after winning
                    this.GetComponent<RadialSlider>().enabled = false;
                    Dial.GetComponent<RadialSlider>().enabled = false;

                    //When it does finish, we teleport the player and stop all audios.
                    Player.GetComponent<PuzzleController>().WinPuzzle();
                    Radio_Event.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    Dial.GetComponent<RadialSlider>().Radio_Event.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    once = true;
                }
            }
        }

        #region Interfaces
        // Called when the pointer enters our GUI component.
        // Start tracking the mouse
        public void OnPointerEnter(PointerEventData eventData)
        {
            m_eventCamera = eventData.enterEventCamera;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_eventCamera = eventData.enterEventCamera;
            isPointerDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
            isPointerReleased = true;
        }
        #endregion
    }
}