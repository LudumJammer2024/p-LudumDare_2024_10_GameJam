using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class NodeRayController : MonoBehaviour
{
    private LineRenderer m_lineRenderer;
    [Tooltip("Add the MainNodeRayDestination as the reference")]
    [SerializeField] private Transform m_linkRayDestination;
    [SerializeField] private bool m_linkState = false; // Default state is false
    public bool LinkState { get => m_linkState; set => m_linkState = value; }

    private void Awake()
    {
        if (!m_linkRayDestination)
            throw new System.Exception("The MainNodeRayDestination is missig, the ray has no destination");

        LinkRayInit();
    }

    private void Update()
    {
        //if (!m_linkState) return;
        m_lineRenderer.enabled = m_linkState;

    }

    private void LinkRayInit()
    {
        m_lineRenderer = GetComponent<LineRenderer>();
        if (!m_lineRenderer) return;
        Vector3[] rayPositions = new Vector3[2];
        rayPositions[0] = transform.position;
        rayPositions[1] = m_linkRayDestination.position;
        m_lineRenderer.SetPositions(rayPositions);
        m_lineRenderer.enabled = false;

    }
}
