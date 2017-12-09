using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Validation;
using System.Linq;
using RedditCore.Logging;

namespace RedditCore.DataModel.Repositories
{
    internal abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        private readonly string itemType;

        internal RepositoryBase(EntityConnection connection, ILog log, string itemType)
        {
            Connection = connection;
            Log = log;
            this.itemType = itemType;
        }

        protected ILog Log { get; }
        protected EntityConnection Connection { get; }

        public virtual void Save(T item)
        {
            Save(new List<T> {item});
        }

        /// <summary>
        /// Pre-save hook.  Allows user to filter items before the actual write.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="db"></param>
        /// <returns>Items that will be written to the database</returns>
        protected virtual IEnumerable<T> PreSave(IEnumerable<T> items, RedditEntities db)
        {
            return items;
        }

        public virtual void Save(IEnumerable<T> items)
        {
            if (items.Any())
                try
                {
                    using (var db = new RedditEntities(Connection))
                    {
                        var filtered = PreSave(items, db);

                        GetDbSet(db).AddRange(filtered);

                        db.SaveChanges();
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    var list = string.Join(",", items);
                    Log.Error($"Error validating item of type {itemType}: {list}", ex);
                    var errors =
                        ex.EntityValidationErrors.Select(
                            e => string.Join(Environment.NewLine,
                                     e.ValidationErrors.Select(v => $"{v.PropertyName} - {v.ErrorMessage}")) +
                                 "   Entity: " + e.Entry.Entity);

                    foreach (var error in errors)
                        Log.Error($"----Validation error: {error}");
                }
                catch (Exception e)
                {
                    var list = string.Join(",", items);
                    Log.Error($"Error writing items of type {itemType}: {list}", e);
                }
        }

        public virtual IList<T> GetAll()
        {
            using (var db = new RedditEntities(Connection))
            {
                return new List<T>(GetDbSet(db)).AsReadOnly();
            }
        }

        protected abstract DbSet<T> GetDbSet(RedditEntities db);
    }
}