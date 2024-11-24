using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Redcode.Extensions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OrderRackUI : MonoBehaviour
{
    [SerializedDictionary("OrderUI", "Clock")] 
    public SerializedDictionary<OrderUI, Image> orderUIData;
    private int orderCount;
    
    public delegate void OutOfRecipe();
    public static event OutOfRecipe OnOutOfRecipe;

    private void OnEnable()
    {
        OrderUI.OnOrderDestroy += OnOrderDestroy;
        LevelManager.OnLevelStart += OnLevelStart;
    }
    
    private void OnDisable()
    {
        OrderUI.OnOrderDestroy -= OnOrderDestroy;
        LevelManager.OnLevelStart -= OnLevelStart;
    }

    private void Start()
    {
        orderUIData.Keys.ToList().ForEach(x => x.Initialize(orderUIData[x]));
        orderUIData.Values.ToList().ForEach(x => x.color = Color.clear);
    }
    private void OnLevelStart()
    {
        PopulateRack();
    }

    private void PopulateRack()
    {
        foreach (var order in orderUIData.Keys.Where(x => x.Empty))
        {
            var randomOrder = RecipeManager.Instance.GetRandomRecipe();
            if (!randomOrder)
            {
                order.SetEmpty();
                continue;
            }
            order.SetOrder(randomOrder);
        }
        if (orderUIData.Keys.All(x => x.Empty))
        {
            OnOutOfRecipe?.Invoke();
        }
    }

    private void OnOrderDestroy(OrderUI orderUI)
    {
        var randomOrder = RecipeManager.Instance.GetRandomRecipe();
        if (randomOrder)
        {
            orderUI.SetOrder(randomOrder);
        }
        if (orderUIData.Keys.All(x => x.Empty))
        {
            OnOutOfRecipe?.Invoke();
        }
    }
}
