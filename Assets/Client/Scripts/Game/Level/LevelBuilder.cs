using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField] private int _mapSizeX = 20;
    [SerializeField] private int _mapSizeZ = 20;

    [SerializeField] private int _playersCount = 30;
    [SerializeField] private int _coinsCount = 10;

    [SerializeField] private Block _defaultBlockPrefub;
    [SerializeField] private Block[] _othersBlockPrefabs;

    [SerializeField] private PlayerController _playerPrefab;
    [SerializeField] private PlayerController _botPrefab;

    [SerializeField] private Coin _coinPrefub;
    [SerializeField] private Trap _trapPrefab;

    private PlayerController _player;

    private List<Block> _blocksList;
    private List<PlayerController> _playersList;
    private List<Coin> _coinsList;

    private float _totalDestructionTime;
    private Transform _transform;

    public List<Block> BlocksList => _blocksList;
    public List<PlayerController> PlayersList => _playersList;
    public PlayerController Player => _player;
    public List<Coin> CoinsList => _coinsList;

    public float TotalDestructionTime => _totalDestructionTime;

    public void BuildLevel()
    {
        _transform = transform;

        _blocksList = SpawnBlocks();
        _playersList = SpawnPlayers(_blocksList, _playersCount);
        _coinsList = SpawnCoins(_blocksList, _player.transform, _coinsCount);

        _totalDestructionTime = GetTotalDestructionTime();

        Invoke("InvokeLevelBuiltEvent", 0.5f);

        GameEvents.OnGameStartEvent.AddListener(() =>
        {
            StartCoroutine(WaitAndDestroyBlock());
            StartCoroutine(WaitAndSpawnTrap());
        });
    }

    private void InvokeLevelBuiltEvent()
    {
        GameEvents.InvokeLevelBuiltEvent();
    }

    private List<Block> SpawnBlocks()
    {
        List<Block> blocksList = new List<Block>();

        for (int x = 0; x < _mapSizeX; x++)
        {
            for (int z = 0; z < _mapSizeZ; z++)
            {
                Vector3 blockPosition = new Vector3(x, 0, z);

                if (Random.Range(0, 10) == 0)
                {
                    float rotationAngle = Random.Range(0, 4) * 90f * Mathf.Deg2Rad;
                    Quaternion randomRotation = Quaternion.Euler(0, rotationAngle, 0);

                    Block block = Instantiate(_othersBlockPrefabs[Random.Range(0, _othersBlockPrefabs.Length)], blockPosition, randomRotation, _transform);

                    blocksList.Add(block);
                }
                else
                {
                    Block block = Instantiate(_defaultBlockPrefub, blockPosition, Quaternion.identity, _transform);
                    blocksList.Add(block);
                }
            }
        }

        return blocksList;
    }

    private List<PlayerController> SpawnPlayers(List<Block> blocksList, int playersCount)
    {
        List<PlayerController> playersList = new List<PlayerController>();
        List<Block> spawnBlocks = new List<Block>();

        for (int i = 0; i < playersCount; i++)
        {
            for (int f = 0; f < 20; f++)
            {
                Block block = blocksList[Random.Range(0, blocksList.Count)];

                if (block.IsEmpty)
                {
                    spawnBlocks.Add(block);
                    blocksList.Remove(block);

                    Vector3 blockPosition = block.transform.position;
                    Vector3 playerPosition = new Vector3(blockPosition.x, blockPosition.y + 1, blockPosition.z);

                    if (i == 0)
                    {
                        PlayerController player = Instantiate(_playerPrefab, playerPosition, Quaternion.identity, _transform);
                        player.Nickname = GetRandomNickname();

                        _player = player;
                        playersList.Add(player);
                    }
                    else
                    {
                        PlayerController bot = Instantiate(_botPrefab, playerPosition, Quaternion.identity, _transform);
                        bot.Nickname = GetRandomNickname();

                        playersList.Add(bot);
                    }

                    break;
                }
            }
        }

        blocksList.AddRange(spawnBlocks);

        return playersList;
    }

    private List<Coin> SpawnCoins(List<Block> blocksList, Transform player, int coinsCount)
    {
        List<Coin> coinsList = new List<Coin>();
        List<Block> spawnBlocks = new List<Block>();

        for (int i = 0; i < coinsCount + 1; i++)
        {
            for (int f = 0; f < 20; f++)
            {
                Block block = blocksList[Random.Range(0, blocksList.Count)];

                if (block.IsEmpty)
                {
                    spawnBlocks.Add(block);
                    blocksList.Remove(block);

                    Vector3 blockPosition = block.transform.position;
                    Vector3 coinPosition = new Vector3(blockPosition.x, blockPosition.y + 1, blockPosition.z);

                    float distance = Vector3.Distance(player.position, blockPosition);

                    if (distance > 3)
                    {
                        Coin coin = Instantiate(_coinPrefub, coinPosition, Quaternion.identity, block.transform);
                        coinsList.Add(coin);

                        break;

                    }
                }
            }
        }

        blocksList.AddRange(spawnBlocks);

        return coinsList;
    }

    private string GetRandomNickname()
    {
        string filePath = "nicknames_ru";

        TextAsset textAsset = Resources.Load<TextAsset>("Nicknames/" + filePath);

        if (textAsset != null)
        {
            string[] lines = textAsset.text.Split('\n');

            int randomIndex = Random.Range(0, lines.Length);
            string randomNickname = lines[randomIndex].Trim();

            return randomNickname;
        }

        return null;
    }

    private float GetTotalDestructionTime()
    {
        float totalTime = 0f;
        int blocksCount = _blocksList.Count;
        float seconds = 1f;

        for (int i = 0; i < blocksCount - 1; i++)
        {
            totalTime += seconds;

            if (seconds > 0.3f)
                seconds -= 0.04f;
        }

        return totalTime + 2f;
    }

    IEnumerator WaitAndDestroyBlock()
    {
        int blocksCount = _blocksList.Count;
        float seconds = 1f;

        for (int i = 0; i < blocksCount - 1; i++)
        {
            yield return new WaitForSeconds(seconds);

            int r = Random.Range(0, _blocksList.Count);
            Block block = _blocksList[r];

            block.Destroy();
            _blocksList.RemoveAt(r);

            if (seconds > 0.3f)
                seconds -= 0.04f;
        }
    }

    IEnumerator WaitAndSpawnTrap()
    {
        for (int i = 0; i < 50; i++)
        {
            yield return new WaitForSeconds(Random.Range(5f, 10f));

            for (int l = 0; l < 5; l++)
            {
                int r = Random.Range(0, _blocksList.Count);
                Block block = _blocksList[r];

                if (block.IsEmpty)
                {
                    block.EnableTrapShadow();

                    Transform blockTransform = block.transform;
                    Trap trap = Instantiate(_trapPrefab);

                    trap.transform.position = new Vector3(blockTransform.position.x, blockTransform.position.y + 20f, blockTransform.position.z);
                    trap.transform.SetParent(blockTransform);
                    trap.FallAnimation();

                    break;
                }
            }
        }
    }
}
