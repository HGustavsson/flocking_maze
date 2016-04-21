using UnityEngine;
using System.Collections;

public class FlowFollower : Vehicle 
{
    private Vector3 steeringForce;
    public int switchNum;

    public float flowWeight;

    //Flocking Things
    public float seperationDist;
    public float seperationWeight;
    public float alignWeight;
    public float cohesionWeight;
    public float avoidWeight;

	// Use this for initialization
	override public void Start () 
    {
        base.Start();
	}
	
	// Update is called once per frame
	protected override void CalcSteeringForces() 
    {
        steeringForce = Vector3.zero;

        steeringForce += Flow(switchNum) * flowWeight;
        steeringForce += Separation(seperationDist) * seperationWeight;
        steeringForce += Alignment(gm.FlockDirection) * alignWeight;
        steeringForce += Cohesion(gm.Centroid) * cohesionWeight;
        
        GameObject closestWall = gm.Walls[0];
        for (int i = 0; i < gm.Walls.Length; i++)
        {
            if((gm.Walls[i].transform.position - transform.position).sqrMagnitude < (closestWall.transform.position - transform.position).sqrMagnitude)
            {
                closestWall = gm.Walls[i];
            }
        }

        steeringForce += AvoidObstacle(closestWall, 5);

        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);
        ApplyForce(steeringForce);
	}
}
