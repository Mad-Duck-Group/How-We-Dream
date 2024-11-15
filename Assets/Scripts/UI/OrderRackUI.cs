using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderRackUI : MonoBehaviour
{
    [SerializeField] private OrderUI orderUIPrefab;
    [SerializeField] private int maxOrders = 3;
    
    private int orderCount;
    private void OnEnable()
    {
        OrderUI.OnOrderDestroyed += OnOrderDestroy;
    }
    
    private void OnDisable()
    {
        OrderUI.OnOrderDestroyed -= OnOrderDestroy;
    }
    private void Start()
    {
        PopulateRack();
    }

    private void PopulateRack()
    {
        int freeSlots = maxOrders - orderCount;
        for (int i = 0; i < freeSlots; i++)
        {
            var randomOrder = RecipeManager.Instance.GetRandomRecipe();
            if (!randomOrder) continue;
            var order = Instantiate(orderUIPrefab, transform);
            order.Initialize(randomOrder);
            orderCount++;
        }
    }

    private void OnOrderDestroy()
    {
        orderCount--;
        PopulateRack();
    }
}
