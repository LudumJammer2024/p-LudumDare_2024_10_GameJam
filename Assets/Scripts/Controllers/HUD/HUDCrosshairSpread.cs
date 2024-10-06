using UnityEngine;
using UnityEngine.UI;

public class HUDCrosshairSpread : MonoBehaviour
{
    [SerializeField] private Image m_crosshairIcon;
    [SerializeField] private Sprite[] m_crosshairArray;
    [SerializeField] private float spreadReductionSpeed = 0.1f;
    [SerializeField] private float bulletSpread = 0.3f;
    private float spread = 0f;

    void Awake()
    {
        UpdateIcon();
    }

    void Update()
    {
        if (spread > 0)
        {
            spread = Mathf.MoveTowards(spread, 0, spreadReductionSpeed * Time.deltaTime);
            UpdateIcon(SpreadToIndex());
        }
    }

    public void Fire()
    {
        spread += bulletSpread;
        spread = Mathf.Clamp(spread, 0f, 1f);
        UpdateIcon(SpreadToIndex());
    }

    private void UpdateIcon(int iconIndex = 0)
    {
        if (m_crosshairIcon == null)
        {
            Debug.LogError("No crosshair reference!");
            return;
        }
        if (m_crosshairArray.Length < 1)
        {
            Debug.LogError("No sprites in the crosshair array");
            return;
        }

        m_crosshairIcon.sprite = m_crosshairArray[iconIndex];
    }

    private int SpreadToIndex()
    {
        if (m_crosshairArray.Length < 1) return 0;

        spread = Mathf.Clamp(spread, 0, 1);

        return Mathf.RoundToInt(spread * (m_crosshairArray.Length - 1));
    }
}
