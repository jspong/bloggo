---
title: Cellular Automata
author: Johnald
layout: post
categories: Unity
tags: Unity tutorial procgen learning
---

I decided to further abstract the cellular automata into its own class.

```csharp
void GenerateCave() {
    // ...
    cells = new CellularSimulation<int>(width, height);

    cells.UpdateInPlace(InitialValue);
    for (int i = 0; i < smoothCount; i++) {
        cells.Update(Smooth);
    }
    // ...
}

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
```

And now I'm free to make simulations of other cellular automata. The simple [cave generation](https://jspong.github.io/bloggo/unity/2019/01/01/Cave_Generation/) example I did last time was pretty awesome, and piqued my interest in what else could be done. I did a cursory google search while walking my dogs, but will link to further literature that I find useful.

## Conway's Game of Life

![game of life](https://github.com/jspong/bloggo/raw/master/_posts/game_of_life.gif)

[Conway's Game of Life](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life) is probably the most famous example of a cellular automata simulation. The field is seeded with an initial value, after which each time epoch updates the board according to the following [rules](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#Rules):

1. Any live cell with fewer than two live neighbors dies, as if by underpopulation.
1. Any live cell with two or three live neighbors lives on to the next generation.
1. Any live cell with more than three live neighbors dies, as if by overpopulation.
1. Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction.

We can model this as a simple Cell visitor in our new class:

```csharp
// Runs in Coroutine.
IEnumerator RunLoop() {
    while (true) {
        cells.Update(UpdateCell);
        meshGen.GenerateMesh(cells.Values(), 1);
        yield return new WaitForSeconds(0.5f);
    }
}

int UpdateCell(CellularSimulation<int>.Cell cell) {
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
```

I'm excited to see what else I can learn from here! More to come, hopefully!