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
    private CircleCollider2D col2d;
    //Mouse position in world space
    private Vector3 MousePosition => Camera.main.ScreenToWorldPoint(Input.mousePosition).WithZ(0);
    private Vector3 originalScale;
    private static bool _holding;
    private static IngredientSO _ingredient;
    public static bool Holding => _holding;
    public static CookStates HoldingCookState => _ingredient.CookState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col2d = GetComponent<CircleCollider2D>();
        rb.simulated = false;
        originalScale = transform.localScale;
    }

    public void Initialize(IngredientSO ingredient, GameObject owner)
    {
        image = GetComponent<SpriteRenderer>();
        image.sortingOrder = 2;
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
            var targetScale = originalScale * size;
            transform.localScale = Divide(targetScale, transform.lossyScale);
        }
        var spriteSize = image.sprite.bounds.size;
        col2d.radius = spriteSize.x / 2;
    }
    
    public void BeingDrag()
    {
        image.sortingOrder = 5;
        GlobalSoundManager.Instance.PlayUISFX("Drag");
        _holding = true;
        _ingredient = ingredient;
    }

    public void Drag()
    {
        transform.position = MousePosition;
    }
    
    public bool EndDragCheck(PointerEventData eventData)
    {
        image.sortingOrder = 2;
        _holding = false;
        _ingredient = null;
        if (eventData.hovered.Count == 0) return false;
        eventData.hovered.ForEach(x => Debug.Log(x.name));
        foreach (var hover in eventData.hovered)
        {
            if (hover == owner) continue;
            if (hover.TryGetComponent(out IIngredientContainer container))
            {
                if (container is IngredientRackUI or IngredientSlotUI && ingredient.CookState is not CookStates.Raw)
                {
                    return false;
                }
                return container.SetIngredient(ingredient);
            }
        }
        return false;
    }
    
}
