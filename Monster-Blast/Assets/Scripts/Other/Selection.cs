using System.Collections;
using UnityEngine;

public class Selection : MonoBehaviour
{
    private GridManager _gridManager;
    private BlockMatcher _blockMatcher;
    private AudioManager _audioManager;


    private Rect _playArea;

    private bool _cooldown;

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
    }

    private void Start()
    {
        CreatePlayArea();
    }

    private void CreatePlayArea()
    {
        float _offset = _gridManager.WidthScale * Mathf.Sqrt(2) / 2f;
        Vector2 bottom = _gridManager.Positions[0, 0] - new Vector2(_offset, _offset);
        Vector2 top = _gridManager.Positions[_width - 1, _height - 1] + new Vector2(_offset, _offset);

        _playArea = Rect.MinMaxRect(bottom.x, bottom.y, top.x, top.y);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_cooldown)
        {
            GetInput();
        }
    }

    private void GetInput()
    {
        Vector2 _inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(_inputPosition);

        if (!_playArea.Contains(_inputPosition, true))
            return;

        Vector2Int cor = GetCoordinates(_inputPosition);

        if (!_blockMatcher.CheckCoordinates(cor))
            return;

        _audioManager.PlaySound(1, true);
        StartCoroutine(Cooldown());
    }

    private Vector2Int GetCoordinates(Vector2 _inputPosition)
    {
        int _coordinateX = 0, _coordinateY = 0;
        float _minDifference = float.MaxValue;

        for (int i = 0; i < _width; i++)
        {
            float _difference = Mathf.Abs(_gridManager.Positions[i, 0].x - _inputPosition.x);
            if (_difference < _minDifference)
            {
                _minDifference = _difference;
                _coordinateX = i;
            }
        }

        _minDifference = float.MaxValue;

        for (int j = 0; j < _height; j++)
        {
            float _difference = Mathf.Abs(_gridManager.Positions[0, j].y - _inputPosition.y);
            if (_difference < _minDifference)
            {
                _minDifference = _difference;
                _coordinateY = j;
            }
        }

        return new Vector2Int(_coordinateX, _coordinateY);
    }

    IEnumerator Cooldown()
    {
        _cooldown = true;
        yield return new WaitForSeconds(_cooldownTimer);
        _cooldown = false;
    }
}
