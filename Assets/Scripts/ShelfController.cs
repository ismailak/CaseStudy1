using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfController : MonoBehaviour, IInteractable
{
    [SerializeField] private int _limit;
    [SerializeField] private List<GameObject> _productPrefabs;
    
    private int _count;
    private int[] _productsIndexOnShelf;
    private GameObject[] _productOnShelf;
    private Vector3 _referencePosition = new Vector3(-5, 1.1f, 9.2f);

    private void Awake()
    {
        _productsIndexOnShelf = new int[_limit];
        _productOnShelf = new GameObject[_limit];

        for (int i = 0; i < _limit; i++) _productsIndexOnShelf[i] = -1;
    }

    public void AddProduct(int productIndex)
    {
        _count++;
        for (int i = 0; i < _limit; i++)
        {
            if (_productsIndexOnShelf[i] == -1)
            {
                _productsIndexOnShelf[i] = productIndex;
                _productOnShelf[i] = Instantiate(_productPrefabs[productIndex]);
                _productOnShelf[i].transform.position = _referencePosition + new Vector3(.2f * (i % 20), 0, .2f * (i / 20));
                return;
            }
        }
    }

    public bool RemoveProduct(int productIndex)
    {
        if(_count == 0) return false;
        
        for (int i = _limit - 1; i >= 0; i--)
        {
            if (_productsIndexOnShelf[i] == productIndex)
            {
                _productsIndexOnShelf[i] = -1;
                Destroy(_productOnShelf[i]);
                _count--;
                return true;
            }
        }

        return false;
    }

    public InteractableType InteractableType { get => InteractableType.Shelf; }
    public bool IsFull => _limit == _count;
}
