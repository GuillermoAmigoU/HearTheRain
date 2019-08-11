using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmo : MonoBehaviour {

    //Gizmo Color
    public Color c;


    private void OnDrawGizmos()
    {
        Gizmos.matrix = this.transform.localToWorldMatrix;
        Gizmos.color = c;
        Gizmos.DrawCube(Vector3.zero,transform.localScale);
    }
}
