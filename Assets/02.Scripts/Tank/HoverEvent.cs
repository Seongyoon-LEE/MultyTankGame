using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverEvent : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    public static HoverEvent event_Instance;
    public bool isDown = false;
    public bool isClick = false;
    public bool isEnter = false;    
    void Start()
    {
        event_Instance = this;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        isClick = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDown = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isEnter = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isEnter = false;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        isClick = false;
    }

    void Update()
    {
        
    }
}
