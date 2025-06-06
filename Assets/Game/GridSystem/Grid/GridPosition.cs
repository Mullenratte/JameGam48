using System;

public struct GridPosition {
    public int x, z;

    public GridPosition(int x, int z) { 
        this.x = x; 
        this.z = z;
    }

    public static bool operator ==(GridPosition a, GridPosition b) {
        return a.x == b.x && a.z == b.z;
    }

    public static bool operator !=(GridPosition a, GridPosition b) {
        return !(a == b);
    }

    public override bool Equals(object obj) {
        return obj is GridPosition position &&
               x == position.x &&
               z == position.z;
    }

    public override int GetHashCode() {
        return HashCode.Combine(x, z);
    }

    public override string ToString() {
        return "<color=#00AA00>x: " + this.x + ", z: " + this.z + "</color>";
    }

    public bool Equals(GridPosition other) {
        return this == other;
    }

    public static GridPosition operator +(GridPosition a, GridPosition b) {
        return new GridPosition(a.x + b.x, a.z + b.z);
    }
    public static GridPosition operator -(GridPosition a, GridPosition b) {
        return new GridPosition(a.x - b.x, a.z - b.z);
    }

    public static GridPosition operator *(GridPosition a, float b) {
        return new GridPosition((int)(a.x * b), (int)(a.z * b));
    }
}