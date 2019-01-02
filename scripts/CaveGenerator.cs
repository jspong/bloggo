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

    CellularSimulation<int> cells;

    System.Random rand;

    #region Unity Event Handlers

    // Start is called before the first frame update
    void Start() {
        GenerateCave();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            GenerateCave();
        }
    }

    #endregion

    void GenerateCave() {
        if (!useCustomSeed) {
            randomSeed = Time.time.ToString();
        }

        rand = new System.Random(randomSeed.GetHashCode());
        cells = new CellularSimulation<int>(width, height);

        cells.UpdateInPlace(InitialValue);
        for (int i = 0; i < smoothCount; i++) {
            cells.Update(Smooth);
        }

        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(cells.Values(), 1);
    }

    #region Visitor Methods

    int InitialValue(CellularSimulation<int>.Cell cell) {
        if (cell.IsEdge) {
            return 1;
        } else {
            return rand.Next(0, 100) <= percentFilled ? 1 : 0;
        }
    }

    int Smooth(CellularSimulation<int>.Cell cell) {
        int outOfBoundsNeighbors = 8 - cell.CountNeighbors();
        int wallNeighbors = cell.CountNeighbors((me, other) => other.Value == 1);
        int wallCount = outOfBoundsNeighbors + wallNeighbors;
        if (wallCount > 4) {
            return 1;
        } else if (wallCount < 4) {
            return 0;
        }
        return cell.Value;
    }

    #endregion

}
