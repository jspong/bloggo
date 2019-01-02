using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{

    [Range(0, 100)]
    public int percentFilled = 50;

    public int width;
    public int height;

    CellularSimulation<int> cells;
    MeshGenerator meshGen;

    System.Random rand;

    // Start is called before the first frame update
    void Start() {
        rand = new System.Random();
        cells = new CellularSimulation<int>(width, height);
        meshGen = GetComponent<MeshGenerator>();
        cells.UpdateInPlace(cell => rand.Next(0, 100) <= percentFilled ? 1 : 0);
        meshGen.GenerateMesh(cells.Values(), 1);
        StartCoroutine(RunLoop());
    }

    IEnumerator RunLoop() {
        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        while (true) {
            cells.Update(Tick);
            meshGen.GenerateMesh(cells.Values(), 1);
            yield return new WaitForSeconds(0.5f);
        }
    }

    int Tick(CellularSimulation<int>.Cell cell) {
        switch (cell.CountNeighbors((x, y) => y.Value == 1)) {
            case 0:
            case 1:
                return 0;
            case 2:
                return cell.Value;
            case 3:
                return 1;
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            default:
                return 0;
        }
    }
}
