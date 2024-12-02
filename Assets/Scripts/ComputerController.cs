using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerController : MonoBehaviour, IInteractable
{
    [SerializeField] private List<GameObject> _products;
    [SerializeField] private Transform _boughtProductArea;

    public void ButtonBuy(int index)
    {
        Instantiate(_products[index],_boughtProductArea.position + Vector3.up * 5, Quaternion.identity);
    }

    public InteractableType InteractableType { get => InteractableType.Computer; }
}
public interface IInteractable
{
    public InteractableType InteractableType{get;}
}