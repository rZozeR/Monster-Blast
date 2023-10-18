using DG.Tweening;
using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private GridManager _gridManager;
    private BlockMatcher _blockMatcher;
    private BlockMover _blockMover;
    private LevelSettings _levelSettings;


    public Transform[,] blockObjects;


    //Scale of Blocks, Adjusts their distance between each other, Range between 0f and 1f
    private const float _scaleOffset = 0.9f;


    private Vector2[] _spawnPoints;

    private Transform _blockParent;

    private Vector3 _blockScale;

    private float _animationTime;


    private void Awake()
    {
        _blockMover = GameManager.Instance.BlockMover;
        _gridManager = GameManager.Instance.GridManager;
        _blockMatcher = GameManager.Instance.BlockMatcher;
        _levelSettings = GameManager.Instance.LevelSettings;

        _animationTime = GameManager.Instance.GameSettings.animation_time;

        _blockParent = new GameObject("Blocks").transform;
        _blockParent.SetParent(transform);

        blockObjects = new Transform[_levelSettings.width, _levelSettings.height];
        _spawnPoints = new Vector2[_levelSettings.width];

        _blockScale = new(_gridManager.WidthScale * _scaleOffset, _gridManager.WidthScale * _scaleOffset, 1);

        CreateSpawn();
        FillBoard();
    }

    private void CreateSpawn()
    {
        Vector2 _start_position = _gridManager.Positions[0, 0];
        float _posY = _levelSettings.height * _gridManager.WidthScale;

        for (int w = 0; w < _levelSettings.width; w++)
        {
            _spawnPoints[w] = _start_position + new Vector2(w * _gridManager.WidthScale, _posY);
        }
    }

    private void FillBoard()
    {
        for (int h = 0; h < _levelSettings.height; h++)
        {
            for (int w = 0; w < _levelSettings.width; w++)
            {
                Transform _temp = Instantiate(_levelSettings.GetRandomBlock(), _gridManager.Positions[w, h], Quaternion.identity, _blockParent).transform;
                _temp.localScale = _blockScale;
                _temp.name = w + " - " + h;

                blockObjects[w, h] = _temp;
            }
        }
    }

    /// <summary> Spawn new blocks and Adjust & Move them to correct position. </summary>
    /// <param name="_column">Spawn's x position</param>
    /// <param name="_amount">Amount of blocks to spawn</param>
    public IEnumerator SpawnBlocks(int _column, int _amount)
    {
        for (int i = 0; i < _amount; i++)
        {
            Vector2Int _pos = new(_column, _levelSettings.height - 1);
            Transform _spawned = Instantiate(_levelSettings.GetRandomBlock(), _spawnPoints[_column], Quaternion.identity, _blockParent).transform;

            _spawned.localScale = Vector2.zero;
            _spawned.name = _pos.x + " " + _pos.y;
            blockObjects[_pos.x, _pos.y] = _spawned;

            _blockMatcher.ProcessBoard(false);
            yield return new WaitForSeconds(_animationTime / 2f);

            _ = _spawned.DOMoveY(_gridManager.Positions[_pos.x, _pos.y].y, _animationTime).SetEase(Ease.InSine);
            _ = _spawned.DOScale(_blockScale, _animationTime);

            _blockMover.DropBlock(_pos);
            yield return new WaitForSeconds(_animationTime);
        }
    }
}
