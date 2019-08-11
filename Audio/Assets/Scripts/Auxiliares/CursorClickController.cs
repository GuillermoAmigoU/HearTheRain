using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorClickController : MonoBehaviour
{
    [SerializeField]
    float time = 1.5f;

    WaitForSeconds wait;
    // Start is called before the first frame update
    void Start()
    {
        wait = new WaitForSeconds(time);
        StartCoroutine(destroy());
    }


    IEnumerator destroy()
    {
        yield return wait;
        if (gameObject != null)
            Destroy(this.gameObject);
    }
}
