using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CompleteNavMeshSystem : MonoBehaviour
{
    [Header("Configuraciones Principales")]
    public Transform targetMarker;
    public float defaultSampleRange = 5f;
    public float edgeDetectionRadius = 2f;

    [Header("Debug Visual")]
    public Color validColor = Color.green;
    public Color invalidColor = Color.red;
    public Color pathColor = Color.blue;

    private NavMeshAgent agent;
    private Vector3 currentTarget;
    private NavMeshPath currentPath;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        InitializeTargetSystem();
    }

    private void InitializeTargetSystem()
    {
        if (targetMarker != null)
        {
            currentTarget = targetMarker.position;
            targetMarker.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        HandleMouseInput();
        HandleKeyboardInput();
        UpdateTargetVisuals();
    }

    #region Input Handling
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                SetNewTarget(hit.point);
            }
        }
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ExecuteSamplePosition();
        if (Input.GetKeyDown(KeyCode.Alpha2)) ExecuteMovement();
        if (Input.GetKeyDown(KeyCode.Alpha3)) CalculateAndShowPath();
        if (Input.GetKeyDown(KeyCode.Alpha4)) FindAndDisplayEdge();
    }
    #endregion

    #region Core Functionality
    public bool SamplePosition(Vector3 sourcePosition, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = sourcePosition + Random.insideUnitSphere * range;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                if (Vector3.Distance(sourcePosition, hit.position) <= range)
                {
                    result = hit.position;
                    return true;
                }
            }
        }
        result = sourcePosition;
        return NavMesh.SamplePosition(sourcePosition, out NavMeshHit fallbackHit, range, NavMesh.AllAreas);
    }

    private void ExecuteSamplePosition()
    {
        Debug.Log("Ejecutando SamplePosition");
        if (SamplePosition(currentTarget, defaultSampleRange, out Vector3 newPos))
        {
            currentTarget = newPos;
            Debug.DrawLine(transform.position, newPos, validColor, 2f);
        }
    }

    private void ExecuteMovement()
    {
        Debug.Log("Ejecutando MoveToTarget");
        if (IsValidNavMeshPosition(currentTarget))
        {
            agent.SetDestination(currentTarget);
        }
    }

    private void CalculateAndShowPath()
    {
        Debug.Log("Calculando camino");
        currentPath = new NavMeshPath();
        if (agent.CalculatePath(currentTarget, currentPath))
        {
            VisualizePath(currentPath);
        }
    }

    private void FindAndDisplayEdge()
    {
        Debug.Log("Buscando borde cercano");
        if (NavMesh.FindClosestEdge(currentTarget, out NavMeshHit edgeHit, NavMesh.AllAreas))
        {
            Debug.DrawLine(currentTarget, edgeHit.position, Color.yellow, 3f);
            ShowDebugSphere(edgeHit.position, Color.yellow);
        }
    }
    #endregion

    #region Helpers & Visualization
    private void SetNewTarget(Vector3 position)
    {
        currentTarget = position;
        if (targetMarker != null)
        {
            targetMarker.position = position;
            targetMarker.gameObject.SetActive(true);
        }
    }

    private bool IsValidNavMeshPosition(Vector3 position)
    {
        return NavMesh.SamplePosition(position, out NavMeshHit hit, 0.1f, NavMesh.AllAreas);
    }

    private void VisualizePath(NavMeshPath path)
    {
        if (path.corners.Length > 1)
        {
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Debug.DrawLine(path.corners[i], path.corners[i + 1], pathColor, 2f);
            }
        }
    }

    private void ShowDebugSphere(Vector3 position, Color color)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * 0.5f;
        sphere.GetComponent<Renderer>().material.color = color;
        Destroy(sphere, 2f);
    }

    private void UpdateTargetVisuals()
    {
        if (targetMarker != null)
        {
            targetMarker.GetComponent<Renderer>().material.color =
                IsValidNavMeshPosition(currentTarget) ? validColor : invalidColor;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(currentTarget, defaultSampleRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(currentTarget, edgeDetectionRadius);
    }
    #endregion
}