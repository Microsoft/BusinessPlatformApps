using System.Collections.Generic;
using System.Data;
using Ninject;
using RedditCore.Logging;

namespace RedditCore.DataModel.Repositories
{
    internal class DocumentRemover : IDocumentRemover
    {
        private readonly IDbConnectionFactory connectionFactory;
        private readonly ILog log;

        [Inject]
        public DocumentRemover(IDbConnectionFactory connectionFactory, ILog log)
        {
            this.log = log;
            this.connectionFactory = connectionFactory;
        }

        public void RemoveDocuments(IList<IDocument> documents)
        {
            log.Verbose($"Starting removal of {documents.Count} documents");
            var ids = new DataTable();
            ids.Columns.Add("id", typeof(string));

            foreach (var item in documents)
            {
                var row = ids.NewRow();
                row["id"] = item.Id;
                ids.Rows.Add(row);
            }

            using (IDbConnection connection = connectionFactory.CreateDbConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "reddit.DeleteDocuments";
                    command.CommandType = CommandType.StoredProcedure;
                    var param = command.CreateParameter();
                    param.ParameterName = "DocIds";
                    param.Value = ids;
                    command.Parameters.Add(param);

                    command.ExecuteNonQuery();
                }
            }

            log.Verbose("Finished document removal");
        }
    }
}
