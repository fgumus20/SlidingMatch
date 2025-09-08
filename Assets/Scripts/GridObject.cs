public interface GridObject
{
    public abstract ObjectType GetType1();
    public abstract int GetX();
    public abstract int GetY();
    public void SetXandY(int x, int y);
    public CubeFall GetFall();
    public void SetFalse();
}