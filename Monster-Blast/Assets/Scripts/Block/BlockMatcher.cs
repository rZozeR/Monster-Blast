using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlockMatcher : MonoBehaviour
{
    private SpawnManager _spawnManager;
    private BlockMover _blockMover;
    private GridManager _gridManager;
    private LevelSettings _levelSettings;
    private AudioManager _audioManager;


    public bool Destroying { get; private set; }


    private List<List<Vector2Int>> _combinations = new();

    private bool[,] _usedObjects;


    private int _width, _height;

    private float _animationTime;


    private void Awake()
    {
        _gridManager = GameManager.Instance.GridManager;
        _spawnManager = GameManager.Instance.SpawnManager;
        _blockMover = GameManager.Instance.BlockMover;
        _levelSettings = GameManager.Instance.LevelSettings;
        _audioManager = AudioManager.Instance;

        _width = GameManager.Instance.LevelSettings.width;
        _height = GameManager.Instance.LevelSettings.height;

        _animationTime = GameManager.Instance.GameSettings.animation_time;
    }

    private void Start()
    {
        ProcessBoard(true);
    }

    public void ProcessBoard(bool _checkShuffle)
    {
        _combinations.Clear();
        _usedObjects = new bool[_width, _height];

        for (int h = 0; h < _height; h++)
        {
            for (int w = 0; w < _width; w++)
            {
                if (_usedObjects[w, h]) continue;

                List<Vector2Int> _group = new()
                {
                    new Vector2Int(w, h)
                };

                _group = CalculateCombinations(_group);

                if (_group.Count > 1)
                {
                    _combinations.Add(_group);
                }
            }
        }

        if (_checkShuffle)
            CheckShuffle();
    }

    private void CheckShuffle()
    {
        if (_combinations.Count <= 0 && !_blockMover.Shuffling)
        {
            _ = _blockMover.StartCoroutine(_blockMover.ShuffleBoard());
        }
    }

    private List<Vector2Int> CalculateCombinations(List<Vector2Int> _group)
    {
        void CheckAdjacent(int _x, int _y)
        {
            if (_x < 0 || _x >= _width || _y < 0 || _y >= _height)
                return;

            if (!CheckEqual(_spawnManager.blockObjects[_group[0].x, _group[0].y], _spawnManager.blockObjects[_x, _y]))
                return;

            if (!_group.Contains(new Vector2Int(_x, _y)))
            {
                _group.Add(new Vector2Int(_x, _y));
            }
        }

        for (int i = 0; i < _group.Count; i++)
        {
            _usedObjects[_group[i].x, _group[i].y] = true;
            CheckAdjacent(_group[i].x + 1, _group[i].y);
            CheckAdjacent(_group[i].x - 1, _group[i].y);
            CheckAdjacent(_group[i].x, _group[i].y + 1);
            CheckAdjacent(_group[i].x, _group[i].y - 1);
        }

        AdjustSprites(_group);
        return _group;
    }

    private bool CheckEqual(Transform _block1, Transform _block2)
    {
        if (_block1 == null || _block2 == null) return false;

        return _block1.GetComponent<Block>().Color == _block2.GetComponent<Block>().Color;
    }

    private void AdjustSprites(List<Vector2Int> _group)
    {
        int _index = _levelSettings.GetSpriteIndex(_group.Count);

        _group.ForEach(_temp =>
        {
            if (_spawnManager.blockObjects[_temp.x, _temp.y] != null)
                _spawnManager.blockObjects[_temp.x, _temp.y].GetComponent<Block>().Change_Sprite(_index);
        });
    }

    /// <summary> Checks if the coordinate is in a Combination, If it is Destroy the Combination </summary>
    /// <param name="_coordinates">Coordinate to check from</param>
    /// <returns>If there is a problem, return true</returns>
    public bool CheckCoordinates(Vector2Int _coordinates)
    {
        List<Vector2Int> FindCombination()
        {
            int _listIndex = _combinations.FindIndex(_list => _list.Contains(_coordinates));
            return (_listIndex != -1) ? _combinations[_listIndex] : null;
        }

        if (_spawnManager.blockObjects[_coordinates.x, _coordinates.y] == null || Destroying)
            return true;

        List<Vector2Int> _combination = FindCombination();

        if (_combination == null)
            return true;

        DestroyBlocks(_combination, _coordinates);

        return false;
    }

    private void DestroyBlocks(List<Vector2Int> _combination, Vector2Int _coordinates)
    {
        Destroying = true;

        List<int> _changed_columns = new();
        int[] _changed_rows = new int[_width];
        List<Transform> _trashedBlocks = new();

        //Could use Hashlist instead
        foreach (Vector2Int _search in _combination)
        {
            if (!_changed_columns.Contains(_search.x))
            {
                _changed_columns.Add(_search.x);
            }

            _changed_rows[_search.x]++;
            _trashedBlocks.Add(_spawnManager.blockObjects[_search.x, _search.y]);
            _spawnManager.blockObjects[_search.x, _search.y] = null;
        }

        DestroyAnimation(_trashedBlocks, _gridManager.Positions[_coordinates.x, _coordinates.y]);

        float _maxWait = 0;
        float _wait = 0;

        foreach (int i in _changed_columns)
        {
            UpdateColumn(i);
            _spawnManager.StartCoroutine(_spawnManager.SpawnBlocks(i, _changed_rows[i]));

            _wait = _changed_rows[i] * _animationTime;
            if (_wait > _maxWait)
                _maxWait = _wait;
        }

        Invoke(nameof(AfterDestroyed), _maxWait + _animationTime);
    }

    private void DestroyAnimation(List<Transform> _group, Vector2 _position)
    {
        _audioManager.PlaySound((_group.Count > _levelSettings.matchRules.secondRule) ? 2 : 0, true);

        foreach (Transform t in _group)
        {
            _ = t.DOMove(_position, _animationTime).SetEase(Ease.InSine).OnComplete(() =>
            {
                Destroy(t.gameObject);
            });
        }
    }

    private void UpdateColumn(int lol)
    {
        for (int h = 0; h < _height; h++)
        {
            if (_spawnManager.blockObjects[lol, h] == null)
                continue;

            _blockMover.DropBlock(new(lol, h));
        }
    }

    private void AfterDestroyed()
    {
        ProcessBoard(true);
        Destroying = false;
    }
}
