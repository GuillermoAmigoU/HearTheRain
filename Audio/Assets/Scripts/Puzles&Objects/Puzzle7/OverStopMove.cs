using Assets.Scripts.Auxiliar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OverStopMove : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        Constantes.CAN_MOVE = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Constantes.CAN_MOVE = true;
    }
}
