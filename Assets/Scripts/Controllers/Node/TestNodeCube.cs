using UnityEngine;

public class TestNodeCube : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    [SerializeField] private Color idleColour = Color.white;
    [SerializeField] private Color underAttackColour = Color.red;
    [SerializeField] private Color safeColour = Color.blue;
    [SerializeField] private Color activeColour = Color.green;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetIdle()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null) return;
        meshRenderer.material.color = idleColour;
    }

    public void SetUnderAttack()
    {
        meshRenderer.material.color = underAttackColour;
    }

    public void SetSafe()
    {
        meshRenderer.material.color = safeColour;
    }

    public void SetActive()
    {
        meshRenderer.material.color = activeColour;
    }
}
