using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FollowCursor : MonoBehaviour
{
    [SerializeField]
    GameObject CursorClickParticle = null;
    [SerializeField]
    float speed = 8.0f;
    [SerializeField]
    float distanceFromCamera = 0.2f;
    bool showTrail = false;

    ParticleSystem clickParticle = null;

    private void Start()
    {
        clickParticle = CursorClickParticle.GetComponent<ParticleSystem>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            showTrail = true;
            clickParticle.Play();
        }
        if (Input.GetMouseButtonUp(0))
        {
            showTrail = false;
        }
    }

    void FixedUpdate()
    {
        if (showTrail)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = distanceFromCamera;

            Vector3 mouseScreenToWorld = Camera.main.ScreenToWorldPoint(mousePosition);

            Vector3 position = mouseScreenToWorld;

            transform.position = position;
        }

    }
  

}
