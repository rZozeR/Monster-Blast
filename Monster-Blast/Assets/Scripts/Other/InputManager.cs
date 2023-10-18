using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    private PlayerInput _playerInput;

    private InputAction _actionClick, _actionPosition;

    private Camera _camera;


    private Vector2 _inputPosition = Vector2.zero;

    private bool _inputCooldown;


    private GridManager _gridManager;
    private BlockMatcher _blockMatcher;
    private AudioManager _audioManager;


    private Rect _playArea;

    private int _width, _height;

    private float _cooldownTimer;

    private void Awake()
    {
        _gridManager = GameManager.Instance.GridManager;
        _blockMatcher = GameManager.Instance.BlockMatcher;
        _audioManager = AudioManager.Instance;

        _width = GameManager.Instance.LevelSettings.width;
        _height = GameManager.Instance.LevelSettings.height;

        _cooldownTimer = GameManager.Instance.GameSettings.input_cooldown;

        _camera = Camera.main;
        _playerInput = GetComponent<PlayerInput>();

        _actionClick = _playerInput.actions["Click"];
        _actionPosition = _playerInput.actions["Position"];

        CreatePlayArea();
    }

    private void CreatePlayArea()
    {
        float _offset = _gridManager.WidthScale * Mathf.Sqrt(2) / 2f;
        Vector2 bottom = _gridManager.Positions[0, 0] - new Vector2(_offset, _offset);
        Vector2 top = _gridManager.Positions[_width - 1, _height - 1] + new Vector2(_offset, _offset);

        _playArea = Rect.MinMaxRect(bottom.x, bottom.y, top.x, top.y);
    }

    private void OnEnable()
    {
        _actionClick.performed += InputClicked;
    }

    private void OnDisable()
    {
        _actionClick.performed -= InputClicked;
    }

    private void InputClicked(InputAction.CallbackContext _context)
    {
        if (_inputCooldown)
            return;

        _inputPosition = _camera.ScreenToWorldPoint(_actionPosition.ReadValue<Vector2>());

        if (!_playArea.Contains(_inputPosition, true))
            return;

        Vector2Int _inputCoordinates = GetCoordinates(_inputPosition);

        if (!_blockMatcher.CheckCoordinates(_inputCoordinates))
            return;

        _audioManager.PlaySound(1, true);
        _ = StartCoroutine(ResetCooldown());
    }

    private Vector2Int GetCoordinates(Vector2 _inputCoordinates)
    {
        int _coordinateX = 0, _coordinateY = 0;
        float _minDifference = float.MaxValue;

        for (int i = 0; i < _width; i++)
        {
            float _difference = Mathf.Abs(_gridManager.Positions[i, 0].x - _inputCoordinates.x);
            if (_difference < _minDifference)
            {
                _minDifference = _difference;
                _coordinateX = i;
            }
        }

        _minDifference = float.MaxValue;

        for (int j = 0; j < _height; j++)
        {
            float _difference = Mathf.Abs(_gridManager.Positions[0, j].y - _inputCoordinates.y);
            if (_difference < _minDifference)
            {
                _minDifference = _difference;
                _coordinateY = j;
            }
        }

        return new Vector2Int(_coordinateX, _coordinateY);
    }

    IEnumerator ResetCooldown()
    {
        _inputCooldown = true;
        yield return new WaitForSeconds(_cooldownTimer);
        _inputCooldown = false;
    }
}
