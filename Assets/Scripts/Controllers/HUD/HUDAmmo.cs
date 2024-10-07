using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDAmmo : MonoBehaviour
{
    [SerializeField] private TMP_Text ammoText;
    private int currentAmmo = 10;

    void Start()
    {
        UpdateAmmo();
    }

    public void UpdateAmmo()
    {
        if (HUDManager.Instance == null)
        {
            Debug.Log("HUDMananger not instantiated!");
            return;
        }
        if (ammoText == null)
        {
            Debug.Log("ammoText not assigned!");
            return;
        }

        currentAmmo = HUDManager.Instance.Ammo;

        ammoText.text = $"{currentAmmo}/∞";
    }
}
