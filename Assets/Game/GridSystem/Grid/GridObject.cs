public class GridObject {
    private GridSystem gridSystem;
    private GridPosition gridPosition;
    public Tile tile;

    public GridObject(GridSystem gridSystem, GridPosition gridPosition, Tile tile) {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        this.tile = tile;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }
}
