namespace RedditCore.DataModel
{
    public interface IDocument
    {
        string Id { get; }

        string Content { get; }
    }
}
