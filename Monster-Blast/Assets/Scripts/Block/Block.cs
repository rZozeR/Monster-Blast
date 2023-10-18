using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public enum Colors
    {
        Red,
        Green,
        Blue,
        Yellow,
        Pink,
        Purple
    }

    public Colors Color;

    [SerializeField] private List<Sprite> _sprites;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_sprites.Count <= 0)
        {
            Debug.LogError("Sprite List is empty!", this);
            return;
        }

        _spriteRenderer.sprite = _sprites[0];
    }

    public void Change_Sprite(int _index)
    {
        _spriteRenderer.sprite = _sprites[_index];
    }
}
