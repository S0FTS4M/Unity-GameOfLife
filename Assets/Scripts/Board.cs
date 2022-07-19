using UnityEngine;
using UnityEngine.Tilemaps;

namespace Softsam
{
    public class Board : MonoBehaviour
    {
        [SerializeField]
        private Tilemap m_tilemap;

        [SerializeField]
        private float m_updateTime = .1f;

        private Cell[,] m_cells;

        private int m_width;
        private int m_heigth;
        private float m_currTime;

        void Start()
        {
            m_width = m_tilemap.cellBounds.max.x;
            m_heigth = m_tilemap.cellBounds.max.y;
            m_cells = new Cell[m_heigth, m_width];

            //Read Tile Information
            foreach (var pos in m_tilemap.cellBounds.allPositionsWithin)
            {
                var localPlace = new Vector3Int(pos.x, pos.y, pos.z);

                if (!m_tilemap.HasTile(localPlace)) continue;

                //We will find the tiles based on world space so convert its positions
                var WorldLocation = m_tilemap.CellToWorld(localPlace);
                var locationAsIndex = new Vector3Int((int)WorldLocation.x, (int)WorldLocation.y, (int)WorldLocation.z);

                // Flag the tile, inidicating that it can change colour.
                // By default it's set to "Lock Colour".
                m_tilemap.SetTileFlags(locationAsIndex, TileFlags.None);


                var color = m_tilemap.GetColor(locationAsIndex);

                if (color == Color.black)
                {
                    m_cells[locationAsIndex.y, locationAsIndex.x] = new Cell(locationAsIndex, CellState.Alive);
                }
                else
                {
                    m_cells[locationAsIndex.y, locationAsIndex.x] = new Cell(locationAsIndex, CellState.Dead);
                }
            }
        }

        void Update()
        {
            m_currTime += Time.deltaTime;

            if (m_currTime >= m_updateTime)
            {
                GenerateNextPopulation();
                UpdateBoard();

                m_currTime = 0;
            }
        }

        private void GenerateNextPopulation()
        {
            Cell[,] newCells = new Cell[m_heigth, m_width];
            for (int row = 0; row < m_heigth; row++)
            {
                for (int col = 0; col < m_width; col++)
                {
                    var currCell = m_cells[row, col];
                    var neighbourCount = GetNeighbourCount(currCell);

                    if (currCell.CellState == CellState.Dead && neighbourCount == 3)
                    {
                        newCells[row, col] = new Cell(currCell.Pos, CellState.Alive);
                    }

                    else if (currCell.CellState == CellState.Alive && (neighbourCount == 3 || neighbourCount == 2))
                    {
                        newCells[row, col] = new Cell(currCell.Pos, CellState.Alive);
                    }

                    else
                    {
                        newCells[row, col] = new Cell(currCell.Pos, CellState.Dead);
                    }
                }
            }

            m_cells = newCells;
        }

        private int GetNeighbourCount(Cell cell)
        {
            int aliveNeighbourCount = 0;
            for (int row = -1; row < 2; row++)
            {
                for (int col = -1; col < 2; col++)
                {
                    if (row == 0 && col == 0)
                        continue;

                    if (row + cell.Pos.y < 0 || row + cell.Pos.y > m_heigth - 1 || col + cell.Pos.x < 0 || col + cell.Pos.x > m_width - 1)
                        continue;

                    if (m_cells[row + cell.Pos.y, col + cell.Pos.x].CellState == CellState.Alive)
                        aliveNeighbourCount++;
                }
            }

            return aliveNeighbourCount;
        }

        private void UpdateBoard()
        {
            for (int row = 0; row < m_heigth; row++)
            {
                for (int col = 0; col < m_width; col++)
                {
                    m_tilemap.SetTileFlags(m_cells[row, col].Pos, TileFlags.None);

                    if (m_cells[row, col].CellState == CellState.Alive)
                        m_tilemap.SetColor(m_cells[row, col].Pos, Color.black);
                    else
                        m_tilemap.SetColor(m_cells[row, col].Pos, Color.white);
                }
            }
        }
    }
}