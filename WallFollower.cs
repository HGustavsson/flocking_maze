using UnityEngine;
using System.Collections;

public class WallFollower : Vehicle 
{
	private Vector3 steeringForce;

	public float distanceFromWall;


	// Use this for initialization
	override public void Start () 
	{
		base.Start();
	}
	
	// Update is called once per frame
	protected override void CalcSteeringForces() 
	{
        GameObject closestWall = gm.Walls[0];
        for (int i = 0; i < gm.Walls.Length; i++)
        {
            if((gm.Walls[i].transform.position - transform.position).sqrMagnitude < (closestWall.transform.position - transform.position).sqrMagnitude)
            {
                closestWall = gm.Walls[i];
            }
        }
        steeringForce += FollowWall(closestWall, distanceFromWall);

        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);
        ApplyForce(steeringForce);
	}
}
