using System;
using UnityEngine;

namespace ProxFramework.Utils.Grid
{
    public class Grid3DLayout : MonoBehaviour
    {
        public enum AnchorPoint
        {
            Center,
            TopLeft,
            TopRight,
            TopCenter,
            BottomLeft,
            BottomRight,
            BottomCenter
        }


        public int rows;
        public int columns;
        public int layers;
        public Vector3 cellSize;
        public Vector3 spacing;
        public Vector3 offset;
        public AnchorPoint anchor = AnchorPoint.Center;

        private Transform[,,] _transforms;

        public void ForEach(Action<Transform> action)
        {
            foreach (var trans in _transforms)
            {
                action(trans);
            }
        }

        public Transform GetTransform(int row, int column, int layer = 0)
        {
            return _transforms[column, row, layer];
        }

        public void Generate()
        {
            _transforms = new Transform[columns, rows, layers];

            var gridDimensions = new Vector3(
                columns * (cellSize.x + spacing.x),
                rows * (cellSize.y + spacing.y),
                layers * (cellSize.z + spacing.z)
            );

            var anchorOffset = CalcAnchorOffset(gridDimensions, anchor);

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    for (int z = 0; z < layers; z++)
                    {
                        // 计算每个物体的位置，考虑间隔、偏移和锚点
                        var position = GetPosition(x, y, z, gridDimensions, anchorOffset);
                        var go = new GameObject($"Grid_{x}_{y}_{z}");
                        go.transform.SetParent(transform);
                        go.transform.localPosition = position;
                        go.SetActive(true);

                        _transforms[x, y, z] = go.transform;
                    }
                }
            }
        }

        private Vector3 CalcAnchorOffset(Vector3 gridDimensions, AnchorPoint anchorPoint)
        {
            return anchorPoint switch
            {
                AnchorPoint.Center => -gridDimensions / 2 + new Vector3(cellSize.x / 2, cellSize.y / 2, cellSize.z / 2),
                AnchorPoint.TopLeft => new Vector3(0, -gridDimensions.y + cellSize.y, 0),
                AnchorPoint.TopRight => new Vector3(-gridDimensions.x + cellSize.x, -gridDimensions.y + cellSize.y, 0),
                AnchorPoint.TopCenter => new Vector3(-gridDimensions.x / 2 + cellSize.x / 2,
                    -gridDimensions.y + cellSize.y, 0),
                AnchorPoint.BottomLeft => Vector3.zero,
                AnchorPoint.BottomRight => new Vector3(-gridDimensions.x + cellSize.x, 0, 0),
                AnchorPoint.BottomCenter => new Vector3(-gridDimensions.x / 2 + cellSize.x / 2, 0, 0),
                _ => Vector3.zero
            };
        }

        private Vector3 GetPosition(int x, int y, int z, Vector3 gridDimensions, Vector3 anchorOffset)
        {
            return new Vector3(
                x * (cellSize.x + spacing.x),
                y * (cellSize.y + spacing.y),
                z * (cellSize.z + spacing.z)
            ) + offset + anchorOffset;
        }

        private void OnDrawGizmos()
        {
            var gridDimensions = new Vector3(
                columns * (cellSize.x + spacing.x),
                rows * (cellSize.y + spacing.y),
                layers * (cellSize.z + spacing.z)
            );

            var anchorOffset = CalcAnchorOffset(gridDimensions, anchor) -
                               CalcAnchorOffset(gridDimensions, AnchorPoint.Center);
            var gizmoOffset = offset + anchorOffset;


            Gizmos.matrix = transform.localToWorldMatrix;

            //draw anchor point
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(Vector3.zero, 0.1f);

            //draw wire cube
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(gizmoOffset, gridDimensions);

            //reset 
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}