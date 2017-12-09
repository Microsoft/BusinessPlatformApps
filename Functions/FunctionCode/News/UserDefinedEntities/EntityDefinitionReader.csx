#load "EntityDefinition.csx"

using System.Collections.Generic;
using System.Data.SqlClient;

public class EntityDefinitionReader
{
	private readonly string connectionString;

	public EntityDefinitionReader(string connectionString)
	{
		this.connectionString = connectionString;
	}

	public IEnumerable<EntityDefinition> LoadEntityDefinitions()
	{
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			connection.Open();

            var command = new SqlCommand("SELECT regex, entityType, entityValue FROM bpst_news.userdefinedentitydefinitions", connection);

            SqlDataReader reader = command.ExecuteReader();
            var returnObject = new LinkedList<EntityDefinition>();

            if (reader.HasRows)
            {
                while (reader.Read())
				{
					returnObject.AddLast(new EntityDefinition()
					{
						Regex = reader["regex"].ToString(),
						EntityType = reader["entityType"].ToString(),
						EntityValue = reader["entityValue"].ToString()
					});
                }
            }

            return returnObject;
		}
	}
}
