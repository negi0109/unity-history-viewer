public struct GitCommit
{
    public readonly string _hashId;
    public readonly string _name;

    public GitCommit(string hashId, string name)
    {
        _hashId = hashId;
        _name = name;
    }
}
