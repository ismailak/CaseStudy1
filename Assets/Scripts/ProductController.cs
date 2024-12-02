using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductController : MonoBehaviour, IInteractable
{
    [SerializeField, Range(0,2)] private int _productType;
    [SerializeField, Range(0, 2)] private int _limit = 10;
    private int _count = 10;


    public bool AddCount()
    {
        if(_count >= _limit) return false;
        _count++;
        return true;
    }
    
    
    public bool RemoveCount()
    {
        if(_count == 0) return false;
        _count--;
        return true;
    }

    public bool IsFull()
    {
        return _count == _limit;
    }

    public InteractableType InteractableType { get => InteractableType.Product; }
    public int ProductType { get => _productType; }
}
