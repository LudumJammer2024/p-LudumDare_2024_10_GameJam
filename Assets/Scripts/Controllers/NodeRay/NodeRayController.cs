using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class NodeRayController : MonoBehaviour
{
    public enum raySate
    {
        DISABLED,
        LINKED,
        ATTACKED
    }
    private LineRenderer m_lineRenderer;
    [Tooltip("Add the MainNodeRayDestination as the reference")]
    [SerializeField] private Transform m_linkRayDestination;
    [SerializeField] private bool m_linkState = false; // Default state is false
    public bool LinkState { get => m_linkState; set => m_linkState = value; }
    public raySate currenRayState;
    private void Awake()
    {
        m_lineRenderer = GetComponent<LineRenderer>();
        m_lineRenderer.enabled = false;
        m_lineRenderer.positionCount = 2;

        if (!m_linkRayDestination)
        {
            Debug.LogError("The MainNodeRayDestination is missing, the ray has no destination");
            return;
        }

        LinkRayInit();
    }

    private void Update()
    {
        if (!m_linkRayDestination) return;
        //if (!m_linkState) return;
        //m_lineRenderer.enabled = m_linkState;
        // switch (currenRayState)
        // {
        //     case raySate.DISABLED:
        //         m_lineRenderer.enabled = false;
        //         break;
        //     case raySate.ATTACKED:
        //         UnderAttackAlertRay();
        //         break;
        //     case raySate.LINKED:
        //         break;
        // }

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
    public void LinkRay()
    {
        m_lineRenderer = GetComponent<LineRenderer>();
        if (!m_lineRenderer) return;
        Vector3[] rayPositions = new Vector3[2];
        rayPositions[0] = transform.position;
        rayPositions[1] = m_linkRayDestination.position;
        m_lineRenderer.SetPositions(rayPositions);
        m_lineRenderer.startColor = Color.green;
        m_lineRenderer.endColor = Color.green;
        m_lineRenderer.startWidth = 0.5f;
        m_lineRenderer.endWidth = 0.5f;
        m_lineRenderer.enabled = true;
    }

    public void UnderAttackAlertRay()
    {
        if (!m_lineRenderer) return;
        Vector3[] rayPositions = new Vector3[2];
        rayPositions[0] = transform.position;
        rayPositions[1] = transform.up * 1000f;
        m_lineRenderer.SetPositions(rayPositions);
        m_lineRenderer.startColor = Color.red;
        m_lineRenderer.endColor = Color.white;
        m_lineRenderer.startWidth = 1;
        m_lineRenderer.endWidth = 1;
        m_lineRenderer.enabled = true;
    }
}
