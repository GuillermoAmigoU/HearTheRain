using Assets.Scripts.Auxiliar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]

public class Door : MonoBehaviour {

    bool once = false;

    [SerializeField]
    bool locked = false;


    [SerializeField]
    [Range(1,7)]
    int Key = 0;

    // Update is called once per frame
    void Update()
    {
        if(!locked && !once)
        {
            this.gameObject.layer = 0;
            once = true;
        }

        if (locked)
        {
            switch (Key)
            {
                case 1:
                    if (Constantes.IS_PUZZLE1_SOLVED)
                    {
                        locked = false;
                    }
                    break;
                case 2:
                    if (Constantes.IS_PUZZLE2_SOLVED)
                    {
                        locked = false;
                    }
                    break;
                case 3:
                    if (Constantes.IS_PUZZLE3_SOLVED)
                    {
                        locked = false;
                    }
                    break;
                case 4:
                    if (Constantes.IS_PUZZLE4_SOLVED)
                    {
                        locked = false;
                    }
                    break;
                case 5:
                    if (Constantes.IS_PUZZLE5_SOLVED)
                    {
                        locked = false;
                    }
                    break;
                case 6:
                    if (Constantes.IS_PUZZLE6_SOLVED)
                    {
                        locked = false;
                    }
                    break;
                case 7:
                    if (Constantes.IS_PUZZLE7_SOLVED)
                    {
                        locked = false;
                    }
                    break;

            }
        }


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position,this.transform.localScale);
    }

    public bool getLocked()
    {
        return locked;
    }
}
