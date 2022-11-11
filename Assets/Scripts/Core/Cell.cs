using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public Image cellSprite { get; set; }

    public event Action<Cell> OnCellClicked;

    private Button cellButton;
    public LayoutElement layoutElement { get; set; }

    public enum CellState
    {
        OPEN,
        CLOSE,
        COMPLETED
    }

    public CellState cellState = CellState.CLOSE;

    // Start is called before the first frame update
    void Awake()
    {
        cellSprite = transform.GetChild(0).GetComponent<Image>();
        layoutElement = GetComponent<LayoutElement>();
    }

    private void OnEnable()
    {
        
        cellButton = GetComponent<Button>();
        cellButton.onClick.AddListener(CellClicked);
    }

    private void OnDisable()
    {
        cellButton.onClick.RemoveListener(CellClicked);
    }

    private void CellClicked()
    {
        if(cellState == CellState.CLOSE)
        {
            OnCellClicked?.Invoke(this);
        }
    }

    public void ChangeState()
    {
        if (cellState == CellState.OPEN)
        {
            cellState = CellState.CLOSE;
            cellSprite.gameObject.SetActive(false);
        }
        else
        {
            cellState = CellState.OPEN;
            cellSprite.gameObject.SetActive(true);
        }
            
    }
}
