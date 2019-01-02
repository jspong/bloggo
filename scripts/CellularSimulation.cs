using System;

public class CellularSimulation<T> {

    public class Cell {
        public int X { get; internal set; }
        public int Y { get; internal set; }
        public T Value { get; internal set; }

        public delegate bool NeighborMatch(Cell me, Cell neighbor);

        CellularSimulation<T> simulation;

        internal Cell(CellularSimulation<T> simulation, int x, int y) {
            this.simulation = simulation;
            X = x;
            Y = y;
        }

        public int CountNeighbors(NeighborMatch matcher) {
            int matches = 0;
            for (int x = X-1; x <= X+1; x++) { 
                for (int y = Y-1; y <= Y+1; y++) {
                    if (x == X && y == Y) {
                        continue;
                    }
                    if (x < 0 || y <= 0 || x >= simulation.width || y >= simulation.height) {
                        continue;
                    }
                    if (matcher(this, this.simulation.cells[x, y])) {
                        matches++;
                    }
                }
            }
            return matches;
        }

        public int CountNeighbors() {
            return CountNeighbors((x, y) => true);
        }

        public bool IsEdge {
            get {
                return IsTopEdge || IsRightEdge || IsBottomEdge || IsLeftEdge;
            }
        }

        public bool IsTopEdge {
            get {
                return Y == 0;
            }
        }

        public bool IsRightEdge {
            get {
                return X == simulation.width - 1;
            }
        }

        public bool IsBottomEdge {
            get {
                return Y == simulation.height - 1;
            }
        }

        public bool IsLeftEdge {
            get {
                return X == 0;
            }
        }

    }

    public delegate void CellVisitor(Cell cell);
    public delegate T CellUpdator(Cell cell);

    readonly int width;
    readonly int height;
    private Cell[,] cells;

    public CellularSimulation(int width, int height) {
        this.width = width;
        this.height = height;
        cells = NewBoard();
    }

    public void Update(CellUpdator updator) {
        Update(updator, true);
    }

    public void UpdateInPlace(CellUpdator updator) {
        Update(updator, false);
    }

    public void Visit(CellVisitor visitor) {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                visitor(cells[x, y]);
            }
        }
    }

    public T[,] Values() {
        T[,] values = new T[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                values[x, y] = cells[x, y].Value;
            }
        }
        return values;
    }

    Cell[,] NewBoard() {
        Cell[,] newBoard = new Cell[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                newBoard[x, y] = new Cell(this, x, y);
            }
        }
        return newBoard;
    }

    void Update(CellUpdator updator, bool createBuffer) {
        Cell[,] target;
        if (createBuffer) {
            target = NewBoard();
        } else {
            target = cells;
        }
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                target[x, y].Value = updator(cells[x, y]);
            }
        }
        cells = target;
    }
}
