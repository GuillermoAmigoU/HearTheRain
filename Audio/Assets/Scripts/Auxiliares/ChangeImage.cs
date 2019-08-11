using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImage : MonoBehaviour
{
    bool first = false; 

    // Start is called before the first frame update
    void Start()
    {
        
        StartCoroutine(FadeIn());
        
    }

    IEnumerator waitToDestroy()
    {
        yield return new WaitForSeconds(30.0f);
        Destroy(this.gameObject);
    }

    
    IEnumerator FadeIn()
    {
        if (!first)
            yield return new WaitForSeconds(10.0f);
        first = true;

        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        while(canvasGroup.alpha>0)
        {
            canvasGroup.alpha -= Time.deltaTime / 2;
            yield return null;
        }
        canvasGroup.interactable = false;
        yield return null;
        Destroy(this.gameObject);
    }
   
}
