using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OrderRackUI : MonoBehaviour
{
    [SerializeField] private OrderUI orderUIPrefab;
    [SerializeField] private int maxOrders = 3;
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
        int freeSlots = maxOrders - orderCount;
        for (int i = 0; i < freeSlots; i++)
        {
            var randomOrder = RecipeManager.Instance.GetRandomRecipe();
            if (!randomOrder) continue;
            var order = Instantiate(orderUIPrefab, transform);
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
