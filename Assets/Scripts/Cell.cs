using UnityEngine;

namespace Softsam
{
    public class Cell
    {
        public CellState CellState { get; set; }

        public Vector3Int Pos { get; private set; }

        public Cell(Vector3Int cellPos, CellState cellState)
        {
            Pos = cellPos;
            CellState = cellState;
        }
    }
}