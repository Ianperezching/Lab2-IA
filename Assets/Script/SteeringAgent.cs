using System.Collections.Generic;
using UnityEngine;

public enum SteeringBehavior
{
    Seek,
    Flee,
    Evade,
    Arrive,
    Pursuit,
    Wander,
    PathFollowing,
    ObstacleAvoidance
}
public class SteeringAgent : MonoBehaviour
{
    public SteeringBehavior behavior;
    public Transform target;
    public float speed = 5f;
    public float arrivalRadius = 1f;
    public float wanderRadius = 2f;
    public float pathOffset = 0.5f;
    public float rotationSpeed = 5f;
    public List<Transform> pathPoints = new List<Transform>();
    private int currentPathIndex = 0;
    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        Vector3 force = Vector3.zero;

        switch (behavior)
        {
            case SteeringBehavior.Seek:
                force = Seek(target.position);
                break;
            case SteeringBehavior.Flee:
                force = Flee(target.position);
                break;
            case SteeringBehavior.Arrive:
                force = Arrive(target.position);
                break;
            case SteeringBehavior.Pursuit:
                force = Pursuit(target);
                break;
            case SteeringBehavior.Evade:
                force = Evade(target);
                break;
            case SteeringBehavior.Wander:
                force = Wander();
                break;
            case SteeringBehavior.PathFollowing:
                force = PathFollowing();
                break;
            case SteeringBehavior.ObstacleAvoidance:
                force = ObstacleAvoidance();
                break;
        }

        velocity += force * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        if (velocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    Vector3 Seek(Vector3 targetPos)
    {
        Vector3 desired = (targetPos - transform.position).normalized * speed;
        return desired - velocity;
    }

    Vector3 Flee(Vector3 targetPos)
    {
        Vector3 desired = (transform.position - targetPos).normalized * speed;
        return desired - velocity;
    }

    Vector3 Arrive(Vector3 targetPos)
    {
        Vector3 toTarget = targetPos - transform.position;
        float distance = toTarget.magnitude;
        float decelerationFactor = Mathf.Clamp01(distance / arrivalRadius);
        Vector3 desired = toTarget.normalized * speed * decelerationFactor;
        return desired - velocity;
    }

    Vector3 Pursuit(Transform target)
    {
        Vector3 futurePosition = target.position + target.forward * speed;
        return Seek(futurePosition);
    }

    Vector3 Evade(Transform target)
    {
        Vector3 futurePosition = target.position + target.forward * speed;
        return Flee(futurePosition);
    }

    Vector3 Wander()
    {
        Vector3 wanderTarget = Random.insideUnitSphere * wanderRadius;
        wanderTarget.y = 0;
        return Seek(transform.position + wanderTarget);
    }

    Vector3 PathFollowing()
    {
        if (pathPoints.Count == 0) return Vector3.zero;
        Vector3 targetPos = pathPoints[currentPathIndex].position;
        if (Vector3.Distance(transform.position, targetPos) < pathOffset)
        {
            currentPathIndex = (currentPathIndex + 1) % pathPoints.Count;
        }
        return Seek(targetPos);
    }

    Vector3 ObstacleAvoidance()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
        {
            Vector3 avoidDirection = Vector3.Reflect(transform.forward, hit.normal);
            return Seek(transform.position + avoidDirection * 2f);
        }
        return Vector3.zero;
    }
}