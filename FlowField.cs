using UnityEngine;
using System.Collections;

public class FlowField : MonoBehaviour 
{
    Vector3 desiredDirection;

	// Use this for initialization
	void Start () 
    {
        desiredDirection = new Vector3(-1 * (transform.position.z), 5*Mathf.Cos(transform.position.x*transform.position.z), 1 * (transform.position.x));
	}
	
	// Update is called once per frame
	void Update () 
    {
        Debug.DrawLine(transform.position, transform.position+desiredDirection.normalized);
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody)
        {
            other.GetComponent<Seeker>().seekerTarget.transform.position = transform.position + Vector3.ClampMagnitude(desiredDirection,2.5f);
        }
    }
}
