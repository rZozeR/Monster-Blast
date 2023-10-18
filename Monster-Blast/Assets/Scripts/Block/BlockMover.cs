using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMover : MonoBehaviour
{
    private SpawnManager _spawnManager;
    private GridManager _gridManager;
    private BlockMatcher _blockMacther;

    public bool Shuffling { get; private set; }


    private int _width, _height;

    private float _animationTime;


    private void Awake()
    {
        _spawnManager = GameManager.Instance.SpawnManager;
        _gridManager = GameManager.Instance.GridManager;
        _blockMacther = GameManager.Instance.BlockMatcher;

        _width = GameManager.Instance.LevelSettings.width;
        _height = GameManager.Instance.LevelSettings.height;

        _animationTime = GameManager.Instance.GameSettings.animation_time;
    }

    public void DropBlock(Vector2Int _pos)
    {
        int _nextBlockY = _pos.y - 1;

        while (_nextBlockY >= 0 && _spawnManager.blockObjects[_pos.x, _nextBlockY] == null)
            _nextBlockY--;

        if (_nextBlockY + 1 == _pos.y)
            return;

        _nextBlockY++;

        _spawnManager.blockObjects[_pos.x, _nextBlockY] = _spawnManager.blockObjects[_pos.x, _pos.y];
        _spawnManager.blockObjects[_pos.x, _pos.y] = null;

        _spawnManager.blockObjects[_pos.x, _nextBlockY].name = _pos.x + " " + _nextBlockY;

        float _anim_timer = _animationTime * (_pos.y - _nextBlockY);
        _ = _spawnManager.blockObjects[_pos.x, _nextBlockY].DOMoveY(_gridManager.Positions[_pos.x, _nextBlockY].y, _anim_timer).SetEase(Ease.InSine).OnComplete(() =>
        {
            _ = _spawnManager.blockObjects[_pos.x, _nextBlockY].DOJump(_gridManager.Positions[_pos.x, _nextBlockY], 0.1f, 1, _animationTime);
        });
    }

    [ContextMenu("Shuffle Board")]
    private void Shuffle()
    {
        StartCoroutine(ShuffleBoard());
    }

    public IEnumerator ShuffleBoard()
    {
        List<Transform> _transforms = new();
        Shuffling = true;

        Vector2 _middle_position = Vector2.Lerp(_gridManager.Positions[0, _height - 1], _gridManager.Positions[_width - 1, _height - 1], 0.5f) + Vector2.up;

        for (int h = 0; h < _height; h++)
        {
            for (int w = 0; w < _width; w++)
            {
                _transforms.Add(_spawnManager.blockObjects[w, h]);
                _ = _spawnManager.blockObjects[w, h].DOMove(_middle_position, _animationTime).SetEase(Ease.InSine);
            }
        }

        for (int h = 0; h < _height; h++)
        {
            for (int w = 0; w < _width; w++)
            {
                int _random = Random.Range(0, _transforms.Count);

                _spawnManager.blockObjects[w, h] = _transforms[_random];
                _spawnManager.blockObjects[w, h].name = w + " " + h;

                _ = _spawnManager.blockObjects[w, h].DOMove(_gridManager.Positions[w, h], _animationTime).SetEase(Ease.InSine);
                _ = _spawnManager.blockObjects[w, h].DOLocalRotate(new(0, 0, 360), _animationTime, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear);

                _transforms.RemoveAt(_random);
                yield return new WaitForSeconds(0.03f);
                _blockMacther.ProcessBoard(false);
            }
        }

        yield return new WaitForSeconds(0.2f);
        Shuffling = false;
        _blockMacther.ProcessBoard(true);
    }
}
