using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class VRNoteBlock : Note
{

    public Material leftMaterial;
    public Material rightMaterial;

    public GameObject selectedBlock;
    public GameObject blueBlock;
    public GameObject redBlock;

    bool isDestroyed;

    public float dissolveTimer;
    public void InitializeBlock(bool left)
    {

        selectedBlock = left ? blueBlock : redBlock;
        selectedBlock.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        if (isDestroyed == false)
        {
            transform.Translate(Vector3.back * speed * Time.deltaTime);  // Move note downward

            // Destroy note if it goes off-screen
            if (transform.position.z < -5f )
            {
                Destroy(gameObject);
            }
        }
      
    }

    private void OnCollisionEnter(Collision collision)
    {

        StartCoroutine(IE_Destroy());
    }

    public IEnumerator IE_Destroy()
    {
        isDestroyed = true;
        yield return null;
        GetComponent<Rigidbody>().isKinematic = false;  
        yield return null;

        float time = 0;

        while (time <= dissolveTimer)
        {
            foreach (var material in selectedBlock.GetComponent<MeshRenderer>().materials)
            {
                material.SetFloat("_cutoff", time / dissolveTimer);
            }

            time += Time.deltaTime;
            yield return null;

        }

        Destroy(gameObject);

    }
}
