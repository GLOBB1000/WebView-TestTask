using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class GameBehaviour : MonoBehaviour
{
    [SerializeField]
    private CellsCreator cellsCreator;

    private List<Cell> cells;

    private Cell firstClicked;
    private Cell secondClicked;

    public event Action OnCellCompleted;
    public event Action OnLevelCompleted;

    // Start is called before the first frame update
    void Awake()
    {
        cellsCreator.OnCellsSetted += CellsCreator_OnCellsSetted;
    }

    private void CellsCreator_OnCellsSetted()
    {
        Debug.Log("Setted invoke");
        cells = cellsCreator.cells;

        foreach (Cell cell in cells)
        {
            cell.OnCellClicked += Cell_OnCellClicked;
        }
    }

    private void OnDestroy()
    {
        foreach (Cell cell in cells)
        {
            cell.OnCellClicked -= Cell_OnCellClicked;
        }
    }

    private void Cell_OnCellClicked(Cell obj)
    {
        if(firstClicked == null)
        {
            firstClicked = obj;
            firstClicked.cellSprite.gameObject.SetActive(true);
            firstClicked.cellSprite.rectTransform.DOScale(Vector3.one, 1).OnComplete(() =>
            {
                firstClicked.cellState = Cell.CellState.OPEN;
            });

            return;
        }

        else if(secondClicked == null)
        {
            secondClicked = obj;
            secondClicked.cellSprite.gameObject.SetActive(true);
            secondClicked.cellSprite.rectTransform.DOScale(Vector3.one, 1).OnComplete(() =>
            {
                secondClicked.cellState = Cell.CellState.OPEN;
                CheckRightCells();
            });
        }
    }

    private void CheckRightCells()
    {
        if(firstClicked.cellSprite.sprite == secondClicked.cellSprite.sprite)
        {
            firstClicked.cellState = Cell.CellState.COMPLETED;
            firstClicked.GetComponent<RectTransform>().DOScale(Vector3.zero, 1).OnComplete(() =>
            {
                firstClicked.gameObject.SetActive(false);
                firstClicked = null;
            });

            secondClicked.cellState = Cell.CellState.COMPLETED;
            secondClicked.GetComponent<RectTransform>().DOScale(Vector3.zero, 1).OnComplete(() =>
            {
                secondClicked.gameObject.SetActive(false);
                secondClicked = null;
                OnCellCompleted?.Invoke();
                CheckLevelFinished();
            });
        }

        else
        {
            firstClicked.cellState = Cell.CellState.CLOSE;
            firstClicked.cellSprite.rectTransform.DOScale(Vector3.zero, 1).OnComplete(() =>
            {
                firstClicked.cellSprite.gameObject.SetActive(false);
                
                firstClicked = null;
            });

            secondClicked.cellState = Cell.CellState.CLOSE;
            secondClicked.cellSprite.rectTransform.DOScale(Vector3.zero, 1).OnComplete(() =>
            {
                secondClicked.cellSprite.gameObject.SetActive(false);
                
                secondClicked = null;
            });
        }
    }

    private void CheckLevelFinished()
    {
        cells = cellsCreator.cells;
        if (cells.Exists(x => x.cellState == Cell.CellState.OPEN || x.cellState == Cell.CellState.CLOSE))
            return;

        OnLevelCompleted?.Invoke();
    }
}
