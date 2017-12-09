using System.Collections.Generic;

namespace RedditCore.DataModel.Repositories
{
    public interface IRepository<T> where T : class
    {
        void Save(T item);

        void Save(IEnumerable<T> items);

        IList<T> GetAll();
    }
}
