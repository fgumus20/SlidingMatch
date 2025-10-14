public interface IMatcher
{
    void ResolveCascade(int maxLoops = 20);
    bool ResolveOnce();
}
