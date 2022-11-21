using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStats : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI level;

    [SerializeField]
    private TextMeshProUGUI points;


    [SerializeField]
    private GameBehaviour gameBehaviour;

    [SerializeField]
    private CellsCreator cellsCreator;

    private float pointsValue;

    // Start is called before the first frame update
    void Start()
    {
        gameBehaviour.OnCellCompleted += GameBehaviour_OnCellCompleted;
        gameBehaviour.OnLevelCompleted += GameBehaviour_OnLevelCompleted;
    }

    private void GameBehaviour_OnLevelCompleted()
    {
        level.text = $"Current level: {cellsCreator.currentLevel}";
    }

    private void GameBehaviour_OnCellCompleted()
    {
        pointsValue += 100;
        points.text = $"Your points: {pointsValue}";
    }
}
