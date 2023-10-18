using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    public GridManager GridManager { get; private set; }
    public SpawnManager SpawnManager { get; private set; }
    public BlockMatcher BlockMatcher { get; private set; }
    public BlockMover BlockMover { get; private set; }

    public AudioManager AudioManager { get; private set; }


    public GameSettings GameSettings { get; private set; }

    [Space][SerializeField][Tooltip("Game Settings")] private GameSettings _gameSettings;

    public LevelSettings LevelSettings { get; private set; }

    [Space][SerializeField][Tooltip("Current Level's Settings")] private LevelSettings _levelSettings;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        LevelSettings = _levelSettings;
        GameSettings = _gameSettings;

        GridManager = GetComponent<GridManager>();
        SpawnManager = GetComponent<SpawnManager>();
        BlockMatcher = GetComponent<BlockMatcher>();
        BlockMover = GetComponent<BlockMover>();
    }
}
