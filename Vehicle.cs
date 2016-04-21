using UnityEngine;
using System.Collections;

//use the Generic system here to make use of a Flocker list later on
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]

abstract public class Vehicle : MonoBehaviour 
{
    //-------------------------------------------------------
    //Class Fields
    //-------------------------------------------------------

    //no position - transform.position is already made
    protected Vector3 acceleration;
    protected Vector3 velocity;
    protected Vector3 desired;

    public Vector3 Velocity { get { return velocity; } }

    public float maxSpeed;
    public float maxForce;
    public float mass;
    public float rad;

    CharacterController charControl;

    //access GM script
    protected GameManager gm;

	virtual public void Start()
    {
        acceleration = Vector3.zero;
        velocity = transform.forward;
        desired = Vector3.zero;

        charControl = GetComponent<CharacterController>();

        gm = GameObject.Find("GameManagerGO").GetComponent<GameManager>();
	}

	
	// Update is called once per frame
	protected void Update () 
    {
        //calculate steering forces
        CalcSteeringForces();

        //add r" to r'
        velocity += acceleration * Time.deltaTime;

        //limit using max speed
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        //move the character
        charControl.Move(velocity * Time.deltaTime);

        //reset r"
        acceleration = Vector3.zero;

        //orient the dude
        transform.forward = velocity.normalized;
	}

    abstract protected void CalcSteeringForces();

    protected void ApplyForce(Vector3 steeringForce)
    {
        acceleration += steeringForce / mass;
    }

	protected Vector3 FollowWall(GameObject wall, float safeDist)
	{
        Vector3 vecToCenter = wall.transform.position - transform.position;
		if (vecToCenter.magnitude < 16 && (Vector3.Dot(vecToCenter, transform.right) > 0 || Vector3.Dot(vecToCenter, transform.forward) < 0))
        {
            Debug.DrawLine(transform.position, wall.transform.position, Color.cyan);


            //Get it's future position
            Vector3 futurePos = transform.forward * (maxSpeed);
            Debug.DrawLine(transform.position, transform.position + futurePos, Color.blue);
            //Project it against the wall
            Vector3 desiredPos = Vector3.Project(futurePos, wall.transform.forward);
            //Move it X away from the wall
            desiredPos += transform.position;
            if (vecToCenter.magnitude < safeDist)
            {
                desiredPos -= transform.right * safeDist;
            }
            desiredPos.y = 0;
            Debug.DrawLine(desiredPos, transform.position);

            //Seek that point
            desired = Seek(desiredPos);

            desired.y = 0;
            Debug.DrawLine(transform.position, transform.position + desired, Color.magenta);

            return desired;
		}
        else if((Vector3.Dot(vecToCenter, transform.right) < 0) && (Vector3.Dot(vecToCenter, transform.forward) < 0))
        {
            desired = Seek(transform.right * safeDist);
            return desired;
        }
        else
            return Vector3.zero;

	}

    protected Vector3 Flow(int switchNum)
    {
        switch(switchNum)
        {
            case 4:
                desired = new Vector3(-1 * (transform.position.z), 0, (transform.position.x)).normalized;
                break;
            case 5:
                desired = new Vector3(-1 * (transform.position.x), 2 * Mathf.Sin(transform.position.x * transform.position.z), -1 * (transform.position.z)).normalized;
                break;
            case 6:
                desired = new Vector3((transform.position.x), 2 * Mathf.Cos(transform.position.x / transform.position.z), (transform.position.z)).normalized;
                break;
            case 7:
                desired = new Vector3(transform.position.x * transform.position.x, 0, transform.position.z * transform.position.z);
                break;
            default:
                desired = new Vector3(0, 0, 0);
                break;
        }
		

        desired *= maxSpeed/4;
        desired -= velocity;

        return desired;
    }

    protected Vector3 Seek(Vector3 targetPosition)
    {
        desired = targetPosition - transform.position;
        desired = desired.normalized * maxSpeed;
        desired -= velocity;
        return desired;
    }

    protected Vector3 Flee(Vector3 targetPosition)
    {
        desired = Seek(targetPosition);
        desired *= -1;
        return desired;
    }

    protected Vector3 AvoidObstacle(GameObject obst, float safeDistance)
    {
        desired = Vector3.zero;

        //get vector to center of obstacle
        Vector3 vecToCenter = obst.transform.position - transform.position;

        float obRad = obst.GetComponent<ObstacleScript>().Radius;
        //"Is it close to me?"
        if(safeDistance < vecToCenter.magnitude)
        {
            return Vector3.zero;
        }
        //"Is it behind me?"
        if(Vector3.Dot(vecToCenter, transform.forward) < 0)
        {
            return Vector3.zero;
        }
        //"Should I worry about it now?"
        if(Mathf.Abs(Vector3.Dot(vecToCenter, transform.right)) > safeDistance + obRad)
        {
            return Vector3.zero;
        }
        //"Turn right!"
        if(Vector3.Dot(vecToCenter, transform.right) < 0)
        {
            desired = transform.right * maxSpeed;
            Debug.DrawLine(transform.position, obst.transform.position, Color.green);
        }
        //"Turn left!"
        else
        {
            desired = transform.right * -maxSpeed;
            Debug.DrawLine(transform.position, obst.transform.position, Color.red);
        }
        //"Turn up!"
        if(Vector3.Dot(vecToCenter, transform.up) < 0)
        {
            desired += transform.up * maxSpeed;
            desired /= 2;
            Debug.DrawLine(transform.position, obst.transform.position, Color.blue);
        }
        //"Turn down!"
        else
        {
            desired += transform.up * -maxSpeed;
            desired /= 2;
            Debug.DrawLine(transform.position, obst.transform.position, Color.yellow);
        }

        return desired;
    }

    public Vector3 Separation(float separationDistance)
    {
        desired = Vector3.zero;
        for (int i = 0; i < gm.Flock.Count; i++ )
        {
            float deltaPosition = Vector3.Distance(gm.Flock[i].transform.position, transform.position);
            if (deltaPosition < separationDistance && deltaPosition != 0)
            {
                desired += Flee(gm.Flock[i].transform.position).normalized * (1/deltaPosition);
            }
        }
        desired.Normalize();
        desired *= maxSpeed;
        desired -= velocity;
        return desired;
    }

    public Vector3 Alignment(Vector3 alignVector)
    {
        desired = alignVector * maxSpeed;
        desired -= velocity;
        return desired;
    }

    public Vector3 Cohesion(Vector3 cohesionVector)
    {
        desired = Seek(cohesionVector);
        return desired;
    }

    public Vector3 StayInBounds(float radius, Vector3 center)
    {
        return Vector3.zero;
    }
}
