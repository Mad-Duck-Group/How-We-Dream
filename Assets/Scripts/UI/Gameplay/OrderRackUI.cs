using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OrderRackUI : MonoBehaviour
{
    [SerializeField] private OrderUI[] orderUI;
    private int orderCount;
    
    public delegate void OutOfRecipe();
    public static event OutOfRecipe OnOutOfRecipe;

    private void OnEnable()
    {
        OrderUI.OnOrderDestroy += OnOrderDestroy;
    }
    
    private void OnDisable()
    {
        OrderUI.OnOrderDestroy -= OnOrderDestroy;
    }
    private void Start()
    {
        PopulateRack();
    }

    private void PopulateRack()
    {
        foreach (var order in orderUI.Where(x => x.Empty))
        {
            var randomOrder = RecipeManager.Instance.GetRandomRecipe();
            if (!randomOrder)
            {
                order.SetEmpty();
                continue;
            }
            order.Initialize(randomOrder);
            orderCount++;
        }
        Debug.Log("Order Count: " + orderCount);
        if (orderCount == 0)
        {
            OnOutOfRecipe?.Invoke();
        }
    }

    private void OnOrderDestroy()
    {
        orderCount--;
        PopulateRack();
    }
}
