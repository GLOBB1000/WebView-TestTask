using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;
using UnityRandom = UnityEngine.Random;

public class CellsCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject cellPrefab;

    [SerializeField]
    private GridLayoutGroup layoutGroup;

    [SerializeField]
    private GameSettings gameSettings;

    [SerializeField]
    private List<Sprite> cellSprites;

    [SerializeField]
    private GameBehaviour gameBehaviour;

    public List<Cell> cells { get; set; }

    public int currentLevel { get; private set; }

    public event Action OnCellsSetted;

    [SerializeField]
    private HashSet<Sprite> sprites = new HashSet<Sprite>();

    private bool isCoroutineStarted;

    // Start is called before the first frame update
    void Start()
    {
        gameBehaviour.OnLevelCompleted += GameBehaviour_OnLevelCompleted;
        //SetCells();
    }

    private void OnDisable()
    {
        gameBehaviour.OnLevelCompleted -= GameBehaviour_OnLevelCompleted;
    }

    private void GameBehaviour_OnLevelCompleted()
    {
        ++currentLevel;

        foreach (var item in cells)
        {
            Destroy(item.gameObject);
        }

        SetCells();
    }

    public void SetCells()
    {
        cells = new List<Cell>();

        layoutGroup.cellSize = new Vector2(gameSettings.cellSizes[currentLevel], gameSettings.cellSizes[currentLevel]);
        for (int i = 0; i < gameSettings.countOfSpawnedObjects[currentLevel]; i++)
        {
            var c = Instantiate(cellPrefab, layoutGroup.GetComponent<RectTransform>());
            cells.Add(c.GetComponent<Cell>());
        }

        SetCellsImages();
        ShowCellsOnStart();

        OnCellsSetted?.Invoke();
    }

    private void SetCellsImages()
    {
        Dictionary<int, List<Image>> imagePairs = new Dictionary<int, List<Image>>();
        var listCells = new List<Cell>(cells);

        List<Sprite> gameSprites = new List<Sprite>();

        while(gameSprites.Count < gameSettings.countOfSpawnedObjects[currentLevel])
        {
            var index = UnityRandom.Range(0, cellSprites.Count);
            if (sprites.Add(cellSprites[index]))
            {
                gameSprites.Add(cellSprites[index]);
                gameSprites.Add(cellSprites[index]);
            }
            else
            {
                continue;
            }
        }

        Shuffle<Sprite>(gameSprites);

        for (int i = 0; i < listCells.Count; i++)
        {
            listCells[i].cellSprite.sprite = gameSprites[i];
        }
        
    }

    // arr - массив для перестановки, N - количество элементов в массиве
    private void Shuffle<T>(List<T> list)
    {
        Random rand = new Random();

        for (int i = list.Count - 1; i >= 1; i--)
        {
            int j = rand.Next(i + 1);

            T tmp = list[j];
            list[j] = list[i];
            list[i] = tmp;
        }
    }

    private void ShowCellsOnStart()
    {
        foreach (var cel in cells)
        {
            cel.cellState = Cell.CellState.OPEN;
            cel.cellSprite.gameObject.SetActive(true);
            cel.cellSprite.transform.DOScale(1, 5).OnComplete(() =>
            {
                cel.cellState = Cell.CellState.CLOSE;
                cel.cellSprite.rectTransform.DOScale(Vector3.zero, 1).OnComplete(() =>
                {
                    cel.cellSprite.gameObject.SetActive(false);
                    cel.layoutElement.ignoreLayout = true;
                });
            });
        }

        
    }
}
