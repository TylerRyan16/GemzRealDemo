using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeCursorOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private PrefabManager prefabManager;

    private Texture2D cursorTexture;
    private Vector2 hotSpot = Vector2.zero;


    private void Start()
    {
        prefabManager = FindObjectOfType<PrefabManager>();
        cursorTexture = prefabManager.GetCursorTexture();
        Vector2 hotspot = new Vector2(0.2f, 0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

}
