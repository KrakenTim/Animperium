using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexPathfinding
{
    public struct LastPath
    {
        public HexCell startCell;
        public int steps;
        public HashSet<HexCell> canStepOn;

        public LastPath(HexCell start, int steps, HashSet<HexCell> canStepOn)
        {
            this.startCell = start;
            this.steps = steps;
            this.canStepOn = canStepOn;
        }

        public override string ToString()
        {
            return $"Start: {startCell.coordinates}, Steps: {0}, Found Cells {canStepOn.Count}";
        }
    }

    /// <summary>
    /// the last search request.
    /// </summary>
    public static LastPath LastSearch { get; private set; } = new LastPath();

    /// <summary>
    /// Compared to path counter in cells:
    /// smaller: not checked yet;
    /// equal: checked already, maybe better connection
    /// </summary>
    static int pathfindingCounter = int.MinValue;

    static List<HexDirection> directions;

    private static void Setup()
    {
        directions = new List<HexDirection>();
        foreach (HexDirection item in Enum.GetValues(typeof(HexDirection)))
            directions.Add(item);
    }

    /// <summary>
    /// returns HexCells that can be stepped on from the given start
    /// </summary>
    public static HashSet<HexCell> GetCellsInWalkingRange(HexCell startCell, int steps, bool useCached = false)
    {
        if (useCached && LastSearch.startCell == startCell && LastSearch.steps == steps)
            return LastSearch.canStepOn;

        HashSet<HexCell> canStepOn = new HashSet<HexCell>();

        if (steps <= 0) return canStepOn;
        if (directions == null) Setup();

        Queue<HexCell> pool = new Queue<HexCell>();
        pathfindingCounter++;

        startCell.pathCounter = pathfindingCounter;
        startCell.pathBestDistance = 0;
        pool.Enqueue(startCell);

        HexCell currentCell;
        HexCell nextNeighbor;

        do
        {
            currentCell = pool.Dequeue();

            foreach (var dir in directions)
            {
                nextNeighbor = currentCell.GetNeighbor(dir);
                if (nextNeighbor == null)
                {
                    Debug.Log($"{currentCell.coordinates}, no neighbor for {dir}");
                    continue;
                }
                if (nextNeighbor.pathCounter < pathfindingCounter)
                {
                    // first time we meet this cell
                    if (!nextNeighbor.CanMoveOnto(currentCell) || !nextNeighbor.IsntBlocked) continue;

                    canStepOn.Add(nextNeighbor);

                    nextNeighbor.pathCounter = pathfindingCounter;
                    nextNeighbor.pathBestDistance = currentCell.pathBestDistance + 1;
                    nextNeighbor.pathPrevious =dir.Opposite();

                    if (nextNeighbor.pathBestDistance < steps)
                    {
                        Debug.Log("Added");
                        pool.Enqueue(nextNeighbor);
                    }
                }
                else
                {
                    // check if there is a better connection
                    if (nextNeighbor.pathBestDistance > currentCell.pathBestDistance + 1)
                    {
                        nextNeighbor.pathBestDistance = currentCell.pathBestDistance + 1;
                        nextNeighbor.pathPrevious = dir.Opposite();

                        if (!pool.Contains(nextNeighbor))
                            pool.Enqueue(nextNeighbor);
                    }
                }
            }
        }
        while (pool.Count > 0);

        LastSearch = new LastPath(startCell, steps, canStepOn);

        Debug.Log(LastSearch);

        return canStepOn;
    }

    /// <summary>
    /// returns path to the given cell, if it's on the same layer (Including target, excluding start).
    /// Otherwise just returns target cell.
    /// </summary>
    public static List<HexCell> GetPath(HexCell startCell, HexCell targetCell, int stepMax)
    {
        GetCellsInWalkingRange(startCell, stepMax);

        List<HexCell> path = new List<HexCell>();

        if(startCell.gridLayer != targetCell.gridLayer)
        {
            path.Add(targetCell);
            return path;
        }

        HexCell nextCell = targetCell;
        while (nextCell != startCell)
        {
            Debug.Log(nextCell);
            Debug.Log(nextCell.coordinates, nextCell);

            path.Add(nextCell);
            nextCell = nextCell.GetNeighbor(nextCell.pathPrevious);
        }
        return path;
    }
}
