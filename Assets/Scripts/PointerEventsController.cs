using UnityEngine;
using UnityEngine.EventSystems;

public class PointerEventsController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.PlaySound(SoundManager.Sound.ButtonOver);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
    }
}
