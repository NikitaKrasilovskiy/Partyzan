using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloudBG : MonoBehaviour
{
	public enum CloudBGState { Left, Up, Right}
    [SerializeField]
    CloudBGState _state;

    [SerializeField]
    Sprite[] stateSprites;

    [SerializeField]
    Image _view;

    private void Awake()
    { SetState(_state); }

    public void SetState(CloudBGState cloudBGState)
    {
        _state = cloudBGState;
        _view.sprite = stateSprites[(int)cloudBGState];
    }
}