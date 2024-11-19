using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Redcode.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TransformExtensions = Redcode.Extensions.TransformExtensions;

public interface IIngredientContainer : IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool SetIngredient(IngredientSO ingredient);
}

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class IngredientUI : MonoBehaviour
{
    [SerializeField, NaughtyAttributes.ReadOnly, Expandable] private IngredientSO ingredient;
    private SpriteRenderer image;
    private GameObject owner;
    private Rigidbody2D rb;
    //Mouse position in world space
    private Vector3 MousePosition => Camera.main.ScreenToWorldPoint(Input.mousePosition).WithZ(0);
    private Vector3 originalScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.simulated = false;
        originalScale = transform.localScale;
    }

    public void Initialize(IngredientSO ingredient, GameObject owner)
    {
        image = GetComponent<SpriteRenderer>();
        this.ingredient = ingredient;
        var sprite = ingredient.GetSprite(ingredient.CookState);
        if (sprite)
            image.sprite = sprite;
        this.owner = owner;
        transform.SetParent(transform.root);
        transform.SetPositionZ(0);
    }
    
    private Vector3 Divide(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }
    
    public void SetSizeAndPhysics(bool physics, bool resetSize = false, float size = 1f)
    {
        rb.simulated = physics;
        rb.velocity = Vector2.zero;
        if (resetSize)
            transform.localScale = originalScale;
        else
        {
            var targetScale = Vector3.one * size;
            transform.localScale = Divide(targetScale, transform.lossyScale);
        }
    }

    public void Drag()
    {
        transform.position = MousePosition;
        image.sortingOrder = 3;
    }
    
    public bool EndDragCheck(PointerEventData eventData)
    {
        if (eventData.hovered.Count == 0) return false;
        eventData.hovered.ForEach(x => Debug.Log(x.name));
        //var top = eventData.hovered[^1];
        foreach (var hover in eventData.hovered)
        {
            if (hover == owner) continue;
            if (hover.TryGetComponent(out IIngredientContainer container))
            {
                if (container is IngredientRackUI or IngredientSlotUI && ingredient.CookState is not CookStates.Raw)
                    return false;
                return container.SetIngredient(ingredient);
            }
        }
        //Debug.Log(eventData.hovered[0]);
        return false;
    }
    
}
