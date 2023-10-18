using UnityEngine;

public class GridManager : MonoBehaviour
{
    private Camera _camera;
    private SpriteRenderer _spriteRenderer;


    public Vector2[,] Positions { get; private set; }
    public float WidthScale { get; private set; }


    //Add space between the background and the screen width
    private const float _backgroundOffset = 0.95f;
    //Add space between the background and the blocks
    private const float _bevelOffset = 0.4f;


    private Vector2 _startPosition;
    private int _width, _height;


    private void Awake()
    {
        _camera = Camera.main;
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _width = GameManager.Instance.LevelSettings.width;
        _height = GameManager.Instance.LevelSettings.height;


        float _camera_width = _camera.orthographicSize * _camera.aspect * 2f;
        _spriteRenderer.size = new Vector2(_camera_width, _camera_width / _width * _height) * _backgroundOffset;

        WidthScale = (_spriteRenderer.size.x - _bevelOffset) / _width;
        _startPosition = transform.position - new Vector3((_spriteRenderer.size.x - _bevelOffset - WidthScale) / 2f, WidthScale / 2f * (_height - 1), 0);

        CreatePositions();
    }

    private void CreatePositions()
    {
        Positions = new Vector2[_width, _height];

        for (int h = 0; h < _height; h++)
        {
            for (int w = 0; w < _width; w++)
            {
                Positions[w, h] = _startPosition + new Vector2(w * WidthScale, h * WidthScale);
            }
        }
    }
}
