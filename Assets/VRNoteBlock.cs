using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        gameObject.layer = LayerMask.NameToLayer(left ? "Blue Block" : "Red Block");
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
        if (isDestroyed)
            return;
        

        StartCoroutine(IE_Destroy(collision));
    }

    public IEnumerator IE_Destroy(Collision collision)
    {
        isDestroyed = true;
        yield return null;

        bool isCorrect = false;
        if (LayerMask.LayerToName(gameObject.layer) == "Blue Block" && LayerMask.LayerToName(collision.gameObject.layer) == "Blue Hand")
        {
            isCorrect = true;
        }

        if (LayerMask.LayerToName(gameObject.layer) == "Red Block" && LayerMask.LayerToName(collision.gameObject.layer) == "Red Hand")
        {
            isCorrect = true;
        }

        if (!isCorrect)
        {
            selectedBlock.GetComponentInChildren<TMP_Text>(true).gameObject.SetActive(true);
            selectedBlock.GetComponentInChildren<Image>(true).gameObject.SetActive(false);
        }

       
        GetComponent<Rigidbody>().isKinematic = false;  
        //GetComponent<Rigidbody>().useGravity = true;
        ContactPoint contact = collision.contacts[0];
        Vector3 forceDirection = -collision.relativeVelocity.normalized;
        GetComponent<Rigidbody>().AddForceAtPosition(forceDirection * 5, contact.point, ForceMode.Impulse);

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
