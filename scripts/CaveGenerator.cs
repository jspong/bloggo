using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveGenerator : MonoBehaviour {
    [Range(0, 100)]
    public int percentFilled;
    [Range(0, 10)]
    public int smoothCount = 5;

    public int width;
    public int height;

    public bool useCustomSeed;
    public string randomSeed;

    int[,] map;
    System.Random rand;

    #region Unity Event Handlers

    // Start is called before the first frame update
    void Start() {
        GenerateCaves();
    }

    void Update() {
        if (Input.GetMouseButton(0)) {
            GenerateCaves();
        }
    }

    void OnDrawGizmos() {
        ForEachSpace(DrawGizmo);
    }

    #endregion

    void GenerateCaves() {
        if (!useCustomSeed) {
            randomSeed = Time.time.ToString();
        }

        rand = new System.Random(randomSeed.GetHashCode());
        map = new int[width, height];

        UpdateSpaces(SetSpace);
        for (int i = 0; i < smoothCount; i++) {
            UpdateSpaces(Smooth);
        }
    }

    #region Visitor Methods

    int SetSpace(int x, int y) {
        if (IsEdge(x, y)) {
            return 1;
        } else {
            return rand.Next(0, 100) <= percentFilled ? 1 : 0;
        }
    }

    int Smooth(int x, int y) {
        int wallCount = NeighborWallCount(x, y);
        if (wallCount > 4) {
            return 1;
        } else if (wallCount < 4) {
            return 0;
        }
        return map[x, y];
    }

    void DrawGizmo(int x, int y) {
        Gizmos.color = map[x, y] == 1 ? Color.black : Color.white;
        Gizmos.DrawCube(SquarePosition(x, y), Vector3.one);
    }

    #endregion

    #region Helper Methods

    private delegate void SpaceVisitor(int x, int y);

    void ForEachSpace(SpaceVisitor visitor) {
        if (map == null) {
            return;
        }
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                visitor(x, y);
            }
        }
    }

    private delegate int SpaceUpdater(int x, int y);

    void UpdateSpaces(SpaceUpdater updater) {
        int[,] newMap = new int[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                newMap[x, y] = updater(x, y);
            }
        }
        map = newMap;
    }

    bool IsInGrid(int x, int y) {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    bool IsEdge(int x, int y) {
        return x == 0 || y == 0 || x == width - 1 || y == height - 1;
    }

    int NeighborWallCount(int x, int y) {
        int walls = 0;
        for (int xPos = x-1; xPos <= x+1; xPos++) {
            for (int yPos = y-1; yPos <= y+1; yPos++) {
                if (IsInGrid(xPos, yPos)) {
                    if (xPos != x || yPos != y) {
                        walls += map[xPos, yPos];
                    }
                } else {
                    walls++;
                }
            }
        }
        return walls;
    }

    Vector3 SquarePosition(int x, int y) {
        return new Vector3(x - width / 2 + 0.5f, 0, y - height / 2 + 0.5f);
    }

    #endregion

}
