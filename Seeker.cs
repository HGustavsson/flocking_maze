using UnityEngine;
using System.Collections;

public class Seeker : Vehicle 
{
    //-------------------------------------------------------
    //Class Fields
    //-------------------------------------------------------
    public GameObject seekerTarget;
    private Vector3 steeringForce;

    //Weighting
    public float seekWeight = 75.0f;
    public float safeDist;
    public float avoidWeight;
    public float seperationDist;
    public float seperationWeight;
    public float alignWeight;
    public float cohesionWeight;


	// Call Inherited Start and then do our own
	override public void Start () 
    {
		base.Start();
	}

    protected override void CalcSteeringForces()
    {
        steeringForce = Vector3.zero; //reset to zero
        
        steeringForce += Seek(seekerTarget.transform.position)*seekWeight;

        steeringForce += Separation(seperationDist) * seperationWeight; //Why isn't this working?
        steeringForce += Alignment(gm.FlockDirection) * alignWeight;
        steeringForce += Cohesion(gm.Centroid) * cohesionWeight;

        for (int i = 0; i < gm.Obstacles.Length; i++ )
            steeringForce += AvoidObstacle(gm.Obstacles[i], safeDist) * avoidWeight;
        
        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);
        ApplyForce(steeringForce);
    }
}
