using System.Threading.Tasks;
using RedditCore.DataModel;

namespace RedditCore.SocialGist
{
    public interface IThreadProcessor
    {
        Task Process(SocialGistPostId batch);
    }
}
