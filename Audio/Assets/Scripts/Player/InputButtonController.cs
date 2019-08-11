using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Assets.Scripts.Auxiliar;

public class InputButtonController : MonoBehaviour , IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private UnityEvent OnClick = null;

    [SerializeField]
    private UnityEvent OnRelease = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Constantes.CAN_MOVE)
            OnClick.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(Constantes.CAN_MOVE)
            OnClick.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
            OnRelease.Invoke();
    }

}
