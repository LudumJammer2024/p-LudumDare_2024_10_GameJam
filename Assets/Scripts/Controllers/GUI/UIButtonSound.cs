using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private float volume = 1f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayOneShot(hoverSound, volume);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayOneShot(clickSound, volume);
        }
    }
}
