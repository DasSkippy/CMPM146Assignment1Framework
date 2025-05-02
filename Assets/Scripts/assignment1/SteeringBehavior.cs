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
    private bool pathStart;
    private int i = 0;

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
        if (path == null)
        {
            pathStart = true;
        }
        if (path != null && (pathStart == true || (transform.position - target).magnitude <= 1.5))
        {
            pathStart = false;
            if (i > path.Count - 1)
            {
                return;
            }
            else
            {
                target = path[i];
                i++;
            }
        }

        // Assignment 1: If a single target was set, move to that target
        //                If a path was set, follow that path ("tightly")
        //calculates the distance from car to target
        float dist = (target - transform.position).magnitude;
        //calculates the vector direction from the car to the target
        Vector3 dir = target - transform.position;
        //calculates the angle from the front of the car to the target
        float angle = Vector3.SignedAngle(transform.forward, dir, Vector3.up);
        //labels the dist and angle variables to text in game
        if (label != null) {
            label.text = dist.ToString() + " " + angle.ToString();
        }

        //calls KinematicBehavior to set the rotational velocity to the angle squared
        //calls KinematicBehavior to set the speed to its max speed
        if (dist > 20)
        {
            kinematic.SetDesiredSpeed(kinematic.GetMaxSpeed());
            kinematic.SetDesiredRotationalVelocity(angle * 2 * Mathf.Sign(angle));
        }
        else if (dist > 1.5)
        {
            kinematic.SetDesiredSpeed(kinematic.GetMaxSpeed() * dist / 20);
            kinematic.SetDesiredRotationalVelocity(kinematic.GetMaxRotationalVelocity() * Mathf.Sign(angle) * dist / 20);
        }
        else
        {
            kinematic.SetDesiredSpeed(0);
        }
        // you can use kinematic.SetDesiredSpeed(...) and kinematic.SetDesiredRotationalVelocity(...)
        //    to "request" acceleration/decceleration to a target speed/rotational velocity
        if (path != null && transform.position == target)
        {
            for(int i = 1; i < path.Count; i++)
            {
                target = path[i];
                Debug.Log("called");
            }
        }
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
