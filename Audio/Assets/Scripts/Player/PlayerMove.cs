using Assets.Scripts.Auxiliar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerMove: MovingUnit
{
    private CompassScript compass;


    protected override void Awake()
        {
            base.Awake();
        Constantes.MAIN_PLAYER = this.gameObject;
        compass = GameObject.FindGameObjectWithTag("GameUI").GetComponent<CompassScript>();

        }


    
    public void MovePlayer (int hor, int ver, int rot)
    {
        Vector3 endPos = Vector3.zero;

        if(ver!= 0)
        {
            endPos = transform.position + transform.forward * ver;
            StartMove(endPos);
        }
        else if(hor!=0)
        {
            endPos = transform.position + transform.right * hor;
            StartMove(endPos);
        }
        else if(rot!=0)
        {
            base.StartRotation(rot);
            compass.UpdateCompass(base.turnTime);

        }
    }

    protected override void StartMove(Vector3 newDir)
    {
        base.StartMove(newDir);

        if(CanMove())
            {
                //print("Move!");
            }
    }

    protected override void CannotMove<T>(T component)
    {
        //

        print("blocked!");
    }

}
