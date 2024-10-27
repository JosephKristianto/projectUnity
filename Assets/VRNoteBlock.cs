using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRBeats;

public class VRNoteBlock : Note
{

    public Material leftMaterial;
    public Material rightMaterial;

    public GameObject selectedBlock;
    public GameObject blueBlock;
    public GameObject redBlock;

    public GameObject twinBlock;

    bool isDestroyed;

    public float dissolveTimer;

    private GameObject twin;

    public bool colliding;
    private ColorSide blockType;


    public void InitializeBlock(ColorSide type , GameObject twin = null)
    {
        if(type == ColorSide.Twin)
        {
            selectedBlock = twinBlock;
            this.twin = twin;
            gameObject.layer = LayerMask.NameToLayer("Twin Block");

            selectedBlock.SetActive(true);
            CreateLine();
            blockType = type ;
        }
        else
        {

            selectedBlock = type == ColorSide.Left ? blueBlock : redBlock;
            gameObject.layer = LayerMask.NameToLayer(type == ColorSide.Left ? "Blue Block" : "Red Block");
            selectedBlock.SetActive(true);
            blockType = type;

        }

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
                Miss(); 
            }

         
        }
      
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isDestroyed)
            return;
        colliding = true;

        StartCoroutine(IE_Destroy(collision));
    }

    private void OnCollisionExit(Collision collision)
    {
        if (isDestroyed) 
            return;

        colliding = false;

    }

    public void Miss()
    {
        Destroy(gameObject);

    }

    public IEnumerator IE_Destroy(Collision collision)
    {
        bool isCorrect = false;

        if(blockType == ColorSide.Twin)
        {
            yield return new WaitUntil(() => twin.GetComponent<VRNoteBlock>().colliding == true);
            isDestroyed = true;
            lineRenderer.gameObject.SetActive(false);
            yield return null;

            isCorrect = true;

        }
        else
        {
            isDestroyed = true;
            yield return null;

            if (LayerMask.LayerToName(gameObject.layer) == "Blue Block" && LayerMask.LayerToName(collision.gameObject.layer) == "Blue Hand")
            {
                isCorrect = true;
            }

            if (LayerMask.LayerToName(gameObject.layer) == "Red Block" && LayerMask.LayerToName(collision.gameObject.layer) == "Red Hand")
            {
                isCorrect = true;
            }
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

    public Material lineMaterial;  // Assign a material in the inspector
    public Color lineColor = Color.white;  // Color of the line
    public float lineWidth = 0.1f;  // Width of the line

    private LineRenderer lineRenderer;

    void CreateLine()
    {
        // Create a new GameObject and attach a LineRenderer component
        GameObject lineObject = new GameObject("LineRendererObject");
        lineRenderer = lineObject.AddComponent<LineRenderer>();

        // Set the material, color, and width
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        // Enable World Space for 3D positioning
        lineRenderer.useWorldSpace = true;

        // Set the initial points of the line (example: two points)
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, gameObject.transform.position);  // Start point
        lineRenderer.SetPosition(1, twin.transform.position);  // End point
        lineRenderer.useWorldSpace = false;
        lineObject.transform.SetParent(gameObject.transform);
    }


    

}
