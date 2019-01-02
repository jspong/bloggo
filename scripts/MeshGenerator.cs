using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGenerator : MonoBehaviour {

    public SquareGrid squareGrid;
    List<Vector3> vertices;
    List<int> triangles;

    public void GenerateMesh(int[,] map, float squareSize) {
        squareGrid = new SquareGrid(map, squareSize);

        vertices = new List<Vector3>();
        triangles = new List<int>();

        for (int x = 0; x < squareGrid.squares.GetLength(0); x++) {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++) {
                MeshFromPoints(TriangulateSquare(squareGrid.squares[x, y]));
            }
        }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    Node[] TriangulateSquare(Square square) {
        switch (square.configuration) {
            case 0:
                return new Node[0];

            // 1 points:
            case 1:
                return new[] { square.centreBottom, square.bottomLeft, square.centreLeft };
            case 2:
                return new[] { square.centreRight, square.bottomRight, square.centreBottom };
            case 4:
                return new[] { square.centreTop, square.topRight, square.centreRight };
            case 8:
                return new[] { square.topLeft, square.centreTop, square.centreLeft };

            // 2 points:
            case 3:
                return new[] { square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft };
            case 6:
                return new[] { square.centreTop, square.topRight, square.bottomRight, square.centreBottom };
            case 9:
                return new[] { square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft };
            case 12:
                return new[] { square.topLeft, square.topRight, square.centreRight, square.centreLeft };
            case 5:
                return new[] { square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft };
            case 10:
                return new[] { square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft };

            // 3 point:
            case 7:
                return new[] { square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft };
            case 11:
                return new[] { square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft };
            case 13:
                return new[] { square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft };
            case 14:
                return new[] { square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft };

            // 4 point:
            case 15:
                return new[] { square.topLeft, square.topRight, square.bottomRight, square.bottomLeft };
            default:
                return new Node[0];
        }
    }

    void AddPointToMesh(Node node) {
        if (node.vertexIndex == -1) {
            node.vertexIndex = vertices.Count;
            vertices.Add(node.position);
        }
        triangles.Add(node.vertexIndex);
    }


    void MeshFromPoints(Node[] points) {
        for (int vertex = 1; vertex < points.Length - 1; vertex++) {
            AddPointToMesh(points[0]);
            AddPointToMesh(points[vertex]);
            AddPointToMesh(points[vertex+1]);
        }

    }

    public class SquareGrid {
        public Square[,] squares;

        public SquareGrid(int[,] map, float squareSize) {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++) {
                for (int y = 0; y < nodeCountY; y++) {
                    Vector3 pos = new Vector3(x - nodeCountX / 2, 0, y - nodeCountY / 2) * squareSize;
                    controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);
                }
            }

            squares = new Square[nodeCountX - 1, nodeCountY - 1];
            for (int x = 0; x < nodeCountX - 1; x++) {
                for (int y = 0; y < nodeCountY - 1; y++) {
                    squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
                }
            }

        }
    }

    public class Square {

        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centreTop, centreRight, centreBottom, centreLeft;
        public int configuration;

        public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft) {
            topLeft = _topLeft;
            topRight = _topRight;
            bottomRight = _bottomRight;
            bottomLeft = _bottomLeft;

            centreTop = topLeft.right;
            centreRight = bottomRight.above;
            centreBottom = bottomLeft.right;
            centreLeft = bottomLeft.above;

            if (topLeft.active)
                configuration += 8;
            if (topRight.active)
                configuration += 4;
            if (bottomRight.active)
                configuration += 2;
            if (bottomLeft.active)
                configuration += 1;
        }
    }

    public class Node {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 _pos) {
            position = _pos;
        }
    }

    public class ControlNode : Node {

        public bool active;
        public Node above, right;

        public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos) {
            active = _active;
            above = new Node(position + Vector3.forward * squareSize / 2f);
            right = new Node(position + Vector3.right * squareSize / 2f);
        }
    }
}
