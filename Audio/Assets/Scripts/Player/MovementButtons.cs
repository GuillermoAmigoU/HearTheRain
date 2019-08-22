using Assets.Scripts.Auxiliar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementButtons : MonoBehaviour
{

     GameObject player;
    private int VerticalI;
    private int HorizontalI;

    public int getVerticalI()
    {
        return VerticalI;
    }

    public int getHorizontalI()
    {
        return HorizontalI;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = transform.gameObject;
    }

    public void OnForward()
    {
        if (Input.touchCount <= 1)
            VerticalI = 1;
    }

    public void OnBackwards()
    {
        if (Input.touchCount <= 1)
            VerticalI = -1;
    }

    public void OnLeft()
    {
        if (Input.touchCount <= 1)
            HorizontalI = -1;
    }
    

    public void OnRight()
    {
        if (Input.touchCount <= 1)
            HorizontalI = 1;
    }

    public void OnRelease()
    {
        HorizontalI = 0;
        VerticalI = 0;
    }

    private PlayerMove playerMove;      // reference to PlayerMove script


    // Use this for initialization
    void Awake()
    {
        // init references
        playerMove = GetComponent<PlayerMove>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // if player is not moving (return value of IsMoving is false)
        if (!playerMove.IsMoving())
        {
            GetPlayerInput();		// call function GetPlayerInput from this script
        }

    }

    //Get player input method is different between Android and PC
#if UNITY_ANDROID
    // function to manage player keyboard input
    private void GetPlayerInput()
    {
        // variables of horizontal moving, vertical moving and turning   
        int vertical = 0;
        int turning = 0;

        // cache movement and turning values based on input axis
        vertical = VerticalI;
        turning = HorizontalI;

        //Debug.Log("Horizontal " + (horizontal));
        //Debug.Log("Vertical " + (vertical));


        // limit movement and turning values so that only one value is something than zero
        if (vertical != 0)
        {
            if (vertical >= 0.5f || vertical <= -0.5f)

            {
                turning = 0;

                //Update orientation
                //Forward
                if (vertical > 0)
                {
                    Constantes.myOrientation = Constantes.Orientation.FORWARD;
                }
                //Behind
                else
                {
                    Constantes.myOrientation = Constantes.Orientation.BACKWARDS;
                }

                playerMove.MovePlayer(0, vertical, 0);          // call function MovePlayer from this script
            }
        }
        if (turning != 0)
        {
           
            if (turning >= 0.5f || turning <= -0.5f)
            {
                vertical = 0;

                playerMove.MovePlayer(0, 0, turning);           // call function MovePlayer from this script
            }
        }

    }
#endif

#if UNITY_STANDALONE_WIN
    // function to manage player keyboard input
    private void GetPlayerInput()
    {
		// variables of horizontal moving, vertical moving and turning
        int horizontal = 0;
        int vertical = 0;
        int turning = 0;

		// cache movement and turning values based on input axis
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));
        turning = (int)(Input.GetAxisRaw("Turning"));

		// limit movement and turning values so that only one value is something than zero
        if (horizontal != 0)
        {
            vertical = 0;
            turning = 0;

            //Update orientation
            //Right
            if (horizontal>0)
            {
                Constantes.myOrientation= Constantes.Orientation.RIGHT;
            }
            //Left
            else
            {
                Constantes.myOrientation = Constantes.Orientation.LEFT;
            }

            playerMove.MovePlayer(horizontal, 0, 0);		// call function MovePlayer from this script
        }
        else if (vertical != 0)
        {

            horizontal = 0;
            turning = 0;

            //Update orientation
            //Forward
            if (vertical > 0)
            {
                Constantes.myOrientation = Constantes.Orientation.FORWARD;
            }
            //Behind
            else
            {
                Constantes.myOrientation = Constantes.Orientation.BACKWARDS;
            }

            playerMove.MovePlayer(0, vertical, 0);			// call function MovePlayer from this script
        }
        else if (turning != 0)
        {

            horizontal = 0;
            vertical = 0;

            playerMove.MovePlayer(0, 0, turning);			// call function MovePlayer from this script
        }
    }
#endif
}

