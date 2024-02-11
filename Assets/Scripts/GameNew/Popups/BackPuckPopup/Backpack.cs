using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Backpack : MonoBehaviour
{
    public enum BackpuckType { Small, Middle, Big};

    [SerializeField]
    Sprite[] _backpucks = new Sprite[3];

    [SerializeField]
    Sprite[] _backpucksOpened = new Sprite[3];

    BackpuckType _backpuckType = BackpuckType.Small;

    bool _isOpened = false;

    Image _image;

    private void Awake()
    { _image = GetComponent<Image>(); }

    public BackpuckType GetBackpuckType()
    { return _backpuckType; }

    public bool IsOpened()
    { return _isOpened; }

    public void SetBackpuck(BackpuckType backpuckType, bool isOpened)
    {
        _backpuckType = backpuckType;

        int index = (int)backpuckType;

        _isOpened = isOpened;

        if (_isOpened)
        { _image.sprite = _backpucksOpened[index]; }
        else
        { _image.sprite = _backpucks[index]; }
    }
}