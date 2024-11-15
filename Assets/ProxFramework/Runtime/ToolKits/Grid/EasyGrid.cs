using System;
using UnityEngine;

namespace ProxFramework.Utils.Grid
{
    public class EasyGrid<T>
    {
        private T[,] _tiles;
        public int Row { get; set; }
        public int Column { get; set; }
        public int TotalCount => Row * Column;


        public EasyGrid(int row, int column)
        {
            Row = row;
            Column = column;
            _tiles = new T[row, column];
        }

        public void Fill(T t)
        {
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    _tiles[i, j] = t;
                }
            }
        }

        public void Fill(Func<int, int, T> onFill)
        {
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    _tiles[i, j] = onFill(i, j);
                }
            }
        }

        public void Resize(int row, int colum, Func<int, int, T> onAdd)
        {
            var newGrid = new T[row, colum];
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    newGrid[i, j] = _tiles[i, j];
                }

                for (int j = Column; j < colum; j++)
                {
                    newGrid[i, j] = onAdd(i, j);
                }
            }

            for (int i = Row; i < row; i++)
            {
                for (int j = 0; j < colum; j++)
                {
                    newGrid[i, j] = onAdd(i, j);
                }
            }

            Fill(default(T));

            Row = row;
            Column = colum;
            _tiles = newGrid;
        }
        
        public Vector2Int Index2Pos(int index)
        {
            return new Vector2Int(index / Column, index % Column);
        }

        public T this[int row, int column]
        {
            get
            {
                if (row < 0 || row >= Row || column < 0 || column >= Column)
                {
                    throw new IndexOutOfRangeException();
                }

                return _tiles[row, column];
            }

            set
            {
                if (row < 0 || row >= Row || column < 0 || column >= Column)
                {
                    throw new IndexOutOfRangeException();
                }

                _tiles[row, column] = value;
            }
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= TotalCount)
                {
                    throw new IndexOutOfRangeException();
                }

                return _tiles[index / Column, index % Column];
            }

            set
            {
                if (index < 0 || index >= TotalCount)
                {
                    throw new IndexOutOfRangeException();
                }

                _tiles[index / Column, index % Column] = value;
            }
        }

        public void ForEach(Action<T> action)
        {
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    action(_tiles[i, j]);
                }
            }
        }

        public void ForEach(Action<int, int, T> action)
        {
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Column; j++)
                {
                    action(i, j, _tiles[i, j]);
                }
            }
        }

        public void Clear(Action<T> cleanupItem = null)
        {
            for (int x = 0; x < Row; x++)
            {
                for (int y = 0; y < Column; y++)
                {
                    cleanupItem?.Invoke(_tiles[x, y]);
                    _tiles[x, y] = default;
                }
            }

            _tiles = null;
        }

        public void MoveNullToColumnEnd(int col, T nullValue)
        {
            for (int i = 0; i < Row; i++)
            {
                if (!this[i, col].Equals(nullValue)) continue;
                for (int j = i + 1; j < Row; j++)
                {
                    if (this[j, col].Equals(nullValue)) continue;
                    this[i, col] = this[j, col];
                    this[j, col] = nullValue;
                    break;
                }
            }
        }

        public void MoveNullToColumnHead(int col, T nullValue)
        {
            for (int i = Row - 1; i >= 0; i--)
            {
                if (!this[i, col].Equals(nullValue)) continue;
                for (int j = i - 1; j >= 0; j--)
                {
                    if (this[j, col].Equals(nullValue)) continue;
                    this[i, col] = this[j, col];
                    this[j, col] = nullValue;
                    break;
                }
            }
        }
    }
}