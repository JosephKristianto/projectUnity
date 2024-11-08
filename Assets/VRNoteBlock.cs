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

    public GameObject blueGripBlock;
    public GameObject redGripBlock;

    public GameObject twinBlock;

    bool isDestroyed;

    public float dissolveTimer;

    private GameObject twin;

    public bool colliding;
    private ColorSide blockType;

    public Material gripBlockMaterial;

    [Header("Grip")]
    public GameObject gripEnd;

    public void InitializeBlock(ColorSide type , GameObject twin = null , int gripLong = 0)
    {
        if(type == ColorSide.Twin)
        {
            selectedBlock = twinBlock;
            this.twin = twin;
            gameObject.layer = LayerMask.NameToLayer("Twin Block");

            selectedBlock.SetActive(true);
            CreateLine(twin);
            blockType = type ;
        }
        else if(type == ColorSide.GripL || type == ColorSide.GripR)
        {
            transform.GetComponent<BoxCollider>().size = new Vector3(0.4f, 0.4f, 0.6f);
            selectedBlock = type == ColorSide.GripL ? blueGripBlock : redGripBlock;
            gripEnd = Instantiate(selectedBlock);
            gripEnd.transform.SetParent(transform);
            var collider = gripEnd.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size += new Vector3(0.5f, 0.5f, 0.5f);
            gripEnd.AddComponent<ReleaseGripBlock>();
            gripEnd.GetComponent<ReleaseGripBlock>().nearMaterial = gripBlockMaterial;
            gripEnd.transform.localPosition = selectedBlock.transform.localPosition;
            gripEnd.transform.GetComponent<BoxCollider>().size = new Vector3(2, 2, 6);

            gripEnd.transform.localPosition += (Vector3.forward * (gripLong+1));
            gripEnd.layer = LayerMask.NameToLayer(type == ColorSide.GripL ? "Blue Block" : "Red Block");


            gameObject.layer = LayerMask.NameToLayer(type == ColorSide.GripL ? "Blue Block" : "Red Block");
            selectedBlock.SetActive(true);
            gripEnd.SetActive(true);
            blockType = type;
            CreateLine(gripEnd);

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
            if (blockType == ColorSide.GripL || blockType == ColorSide.GripR) 
            {
                if (gripEnd != null)
                {
                    if(gripEnd.transform.position.z < 0)
                    {
                        EndOfLifetime();

                    }
                }
            }
            else
            {
                if (transform.position.z < -5f)
                {
                    EndOfLifetime();

                }
            }
           


        }
      
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (isDestroyed)
            return;   
        if (isColliding)
            return;
        colliding = true;

        StartCoroutine(IE_Destroy(collision));
    }

    private void OnTriggerExit(Collider collision)
    {
        if (isDestroyed) 
            return;

        colliding = false;

    }

    public void EndOfLifetime()
    {
        EndOfLife = true;

          Destroy(gameObject);
            GameManager.Instance.SetAnswer(isCorrect);
        

    }

    public bool isColliding = false;
    public bool EndOfLife = false;
    public bool isCorrect = false;
    public bool isGripped = false;
    public IEnumerator IE_Destroy(Collider collision)
    {
        isColliding = true; ;
        if (blockType == ColorSide.Twin)
        {
            yield return new WaitUntil(() => twin.GetComponent<VRNoteBlock>().colliding == true);
            isDestroyed = true;
            lineRenderer.gameObject.SetActive(false);
            yield return null;

            isCorrect = true;

        }
        else if (blockType == ColorSide.GripL || blockType == ColorSide.GripR)
        {
            bool isCollidingCorrect =  false;
            if (LayerMask.LayerToName(gameObject.layer) == "Blue Block" && LayerMask.LayerToName(collision.gameObject.layer) == "Blue Hand")
            {
                isCollidingCorrect = true;
                isGripped = true;

            }

            if (LayerMask.LayerToName(gameObject.layer) == "Red Block" && LayerMask.LayerToName(collision.gameObject.layer) == "Red Hand")
            {
                isCollidingCorrect = true;
                isGripped = true;
            }
            if (isCollidingCorrect)
            {
                yield return new WaitUntil(() => collision.GetComponent<HandAnimator>().isGripping == false);
                yield return new WaitUntil(() => collision.GetComponent<HandAnimator>().isGripping == true);
                var routine = StartCoroutine(IE_FollowLine(collision.transform));

                float internalTime = 0;
                while (internalTime <= dissolveTimer)
                {
                    foreach (var material in selectedBlock.GetComponent<MeshRenderer>().materials)
                    {
                        material.SetFloat("_cutoff", internalTime / dissolveTimer);
                    }

                    internalTime += Time.deltaTime;
                    yield return null;
                }

                while (true)
                {
                    if (collision.GetComponent<HandAnimator>().isGripping == false)
                    {
                        yield return null;  
                        yield return null;
                        isCorrect = gripEnd.GetComponent<ReleaseGripBlock>().isReleased;

                        StopCoroutine(routine);
                        lineRenderer.gameObject.SetActive(false) ;

                        float gripEndTime = 0;
                        while (gripEndTime <= dissolveTimer)
                        {
                            foreach (var material in selectedBlock.GetComponent<MeshRenderer>().materials)
                            {
                                material.SetFloat("_cutoff", gripEndTime / dissolveTimer);
                            }

                            gripEndTime += Time.deltaTime;
                            yield return null;
                        }
                        break;

                    }

                    yield return null;
                }

                isDestroyed = true;
                yield return null;

            }
            else
            {
                isCorrect = false;
                isDestroyed = true;

            }


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
        else
        {
        }

        GameManager.Instance.SetAnswer(isCorrect);

        if (blockType == ColorSide.GripL || blockType == ColorSide.GripR) 
        {

        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().useGravity = true;

            Vector3 contactPoint = collision.ClosestPoint(transform.position);
            Vector3 forceDirection = (contactPoint - transform.position).normalized;


            GetComponent<Rigidbody>().AddForceAtPosition(forceDirection * -3, contactPoint, ForceMode.Impulse);

            yield return null;
        }
       

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

    void CreateLine(GameObject target)
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
        lineRenderer.SetPosition(1, target.transform.position);  // End point
        lineRenderer.useWorldSpace = false;
        lineObject.transform.SetParent(gameObject.transform);
    }


    public IEnumerator IE_FollowLine(Transform target)
    {
        GameObject point = new GameObject("Point");
        point.transform.SetParent(transform);
        lineRenderer.useWorldSpace = true;

        while (true) 
        {
            lineRenderer.SetPosition(0, target.transform.position);
            lineRenderer.SetPosition(1, gripEnd.transform.position);
            yield return null;  
        }
    }

    

}
