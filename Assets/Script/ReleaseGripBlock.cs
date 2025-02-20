using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseGripBlock : MonoBehaviour
{
    // Start is called before the first frame update
    public Material nearMaterial;
    public bool isColliding;
    public bool isReleased;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);
        if (!isColliding)
        {
            isColliding = true;
            StartCoroutine(IE_CheckForRelease(other));

        }
    }

    public IEnumerator IE_CheckForRelease(Collider collision)
    {
        GetComponent<MeshRenderer>().sharedMaterial = nearMaterial;
        yield return new WaitUntil(() => collision.GetComponent<HandAnimator>().isGripping == false);
        isReleased = true;

        float time = 0;
        while (time <= 1)
        {
            foreach (var material in GetComponent<MeshRenderer>().materials)
            {
                material.SetFloat("_cutoff", time / 1);
                yield return null;
            }
            time += Time.deltaTime;
            yield return null;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        
    }
}
