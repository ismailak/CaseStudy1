using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public PlayerState PlayerState;
    
    [SerializeField] private Transform _playerCameraTransform;
    [SerializeField] private GameObject _computerPanel;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private GameObject _interactReminder;
    [SerializeField] private GameObject _breakReminder;
    [SerializeField] private Transform _productCarryPosition;
    
    // TODO: Carry to GameSettings.
    [SerializeField] private float _interactRange = 100;
    [SerializeField] private KeyCode _interactKey = KeyCode.E;
    [SerializeField] private KeyCode _breakProductKey = KeyCode.Q;
    [SerializeField] private float _playerSpeed = 5f;
    [SerializeField] private float _cameraRotateSpeed = 5f;

    private float _speedY;
    private float _rotationX;
    private ProductController _carryProduct;

    
    private void Update()
    {
        InteractableType interactableType = InteractableType.None;
        if (Physics.Raycast(_playerCameraTransform.position, _playerCameraTransform.forward, out var hit, _interactRange))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactableType = interactable.InteractableType;
            }
        }

        if(PlayerState == PlayerState.Free || PlayerState == PlayerState.CarryProduct) MovePlayer();

        switch (PlayerState)
        {
            case PlayerState.Free:
                _breakReminder.SetActive(false);

                if (interactableType == InteractableType.None) _interactReminder.SetActive(false);
                else
                {
                    if (Input.GetKeyDown(_interactKey))
                    {
                        _interactReminder.SetActive(false);
                        switch (interactableType)
                        {
                            case InteractableType.Computer:
                                PlayerState = PlayerState.Computer;
                                _computerPanel.SetActive(true);
                                break;
                            case InteractableType.Product:
                                PlayerState = PlayerState.CarryProduct;
                                ProductController productController = hit.collider.GetComponent<ProductController>();
                                productController.GetComponent<Rigidbody>().isKinematic = true;
                                productController.GetComponent<BoxCollider>().enabled = false;
                                productController.transform.SetParent(_productCarryPosition);
                                productController.transform.localPosition = Vector3.zero;
                                productController.transform.localRotation = Quaternion.Euler(0, 0, 0);
                                _carryProduct = productController;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else _interactReminder.SetActive(true); 
                }

                break;
            case PlayerState.CarryProduct:
                switch (interactableType)
                {
                    case InteractableType.Shelf:
                        ShelfController shelfController = hit.collider.GetComponent<ShelfController>();
                        if (Input.GetKeyDown(_interactKey))
                        {
                            if (shelfController.IsFull) return;
                            if (_carryProduct.RemoveCount())
                            {
                                shelfController.AddProduct(_carryProduct.ProductType);
                            }
                        }
                        else 
                        if (Input.GetKeyDown(_breakProductKey))
                        {
                            if (!_carryProduct.IsFull() && shelfController.RemoveProduct(_carryProduct.ProductType))
                            {
                                _carryProduct.AddCount();
                            }
                        }
                        
                        _interactReminder.SetActive(true);
                        _breakReminder.SetActive(true);
                        break;
                    default:
                        if (Input.GetKeyDown(_breakProductKey))
                        {
                            GameObject product = _productCarryPosition.GetChild(0).gameObject;
                            product.GetComponent<Rigidbody>().isKinematic = false;
                            product.GetComponent<BoxCollider>().enabled = true;
                            product.transform.SetParent(null);
                            PlayerState = PlayerState.Free;
                            _breakReminder.SetActive(false);
                        }
                        _interactReminder.SetActive(false);
                        _breakReminder.SetActive(true);

                        break;
                }

                break;
            case PlayerState.Computer:
                _interactReminder.SetActive(false);
                _breakReminder.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private void MovePlayer()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        if ( _characterController.isGrounded)  _speedY = 0;
        else _speedY -= 9.81f * Time.deltaTime;
        
        float speedX = _playerSpeed * horizontal;
        float speedZ = _playerSpeed * vertical;
        
        Vector3 moveVector = new Vector3(speedX, _speedY, speedZ);

        moveVector = transform.TransformDirection(moveVector);

        _characterController.Move(moveVector * Time.deltaTime);
        
        _rotationX += -mouseY * _cameraRotateSpeed;
        _rotationX = Mathf.Clamp(_rotationX, -80, 80);

        var currentRotation = _playerCameraTransform.localRotation.eulerAngles;
        _playerCameraTransform.localRotation = Quaternion.Euler(_rotationX, currentRotation.y, currentRotation.z);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        transform.rotation *= Quaternion.Euler(0, mouseX * _cameraRotateSpeed , 0);
    }
    
    
    public void ButtonExitComputer()
    {
        _computerPanel.SetActive(false);
        PlayerState = PlayerState.Free;
    }
}


public enum PlayerState
{
    Free,
    Computer,
    CarryProduct
}


public enum InteractableType
{
    None,
    Computer,
    Product,
    Shelf
}