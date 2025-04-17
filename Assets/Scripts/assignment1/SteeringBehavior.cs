using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class SteeringBehavior : MonoBehaviour
{
    public Vector3 target;
    public KinematicBehavior kinematic;
    public List<Vector3> path;
    public float wpthreshold = 1.0f;
    // you can use this label to show debug information,
    // like the distance to the (next) target
    public TextMeshProUGUI label;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        kinematic = GetComponent<KinematicBehavior>();
        target = transform.position;
        path = null;
        EventBus.OnSetMap += SetMap;
    }

    // Update is called once per frame
    void Update()
    {   

        if (path != null && path.Count > 0) {

            Vector3 waypoint = path[0];
            float waypointDist = Vector3.Distance(waypoint, transform.position);
            target = waypoint;

            if (waypointDist < wpthreshold) {
                path.RemoveAt(0);
                if (path.Count == 0) {
                    path = null;
                }
            }

        } else {
            // Assignment 1: If a single target was set, move to that target
            //                If a path was set, follow that path ("tightly")
            //calculates the distance from car to target
            float dist = (target - transform.position).magnitude;
            //calculates the vector direction from the car to the target
            Vector3 dir = target - transform.position;
            //calculates the angle from the front of the car to the target
            float angle = Vector3.SignedAngle(transform.forward, dir, Vector3.up);
            //labels the dist and angle variables to text in game
            label.text = dist.ToString() + " " + angle.ToString();

            //calls KinematicBehavior to set the rotational velocity to the angle squared
            //calls KinematicBehavior to set the speed to its max speed
            if(dist > 20)
            {
                kinematic.SetDesiredSpeed(kinematic.GetMaxSpeed());
                kinematic.SetDesiredRotationalVelocity(angle * angle * Mathf.Sign(angle));
            }
            if (dist > 1)
            {
                kinematic.SetDesiredSpeed(kinematic.GetMaxSpeed() * dist / 20);
                kinematic.SetDesiredRotationalVelocity(kinematic.GetMaxRotationalVelocity() * Mathf.Sign(angle));
            }
            else
            {
                kinematic.SetDesiredSpeed(0);
            }
            // you can use kinematic.SetDesiredSpeed(...) and kinematic.SetDesiredRotationalVelocity(...)
            //    to "request" acceleration/decceleration to a target speed/rotational velocity
        }
        kinematic.setTargetPosition(target);
    }

    public void SetTarget(Vector3 target)
    {
        this.target = target;
        path = null;
        EventBus.ShowTarget(target);
    }

    public void SetPath(List<Vector3> path)
    {
        this.path = path;
    }

    public void SetMap(List<Wall> outline)
    {
        this.path = null;
        this.target = transform.position;
    }
}
