using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Text;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.PowerApp;
using System.Data.SqlClient;

namespace Microsoft.Deployment.Actions.AzureCustom.PowerApp
{
    [Export(typeof(IAction))]
    public class ConfigurePowerAppTables : BaseAction
    {
        private char[] TERMINATORS = new char[] { ' ', '(', ')', '"' };

        private const string INSERT_INTO_TWITTER_QUERY = "INSERT INTO pbist_twitter.twitter_query ([Id], [IsAdvanced], [QueryString]) VALUES (@p1, @p2, @p3)";
        private const string INSERT_INTO_TWITTER_QUERY_DETAILS = "INSERT INTO pbist_twitter.twitter_query_details ([Id], [ReadableId], [Operator], [Operand]) VALUES (@p1, @p2, @p3, @p4)";
        private const string INSERT_INTO_TWITTER_QUERY_READABLE = "INSERT INTO pbist_twitter.twitter_query_readable ([Id], [QueryId], [QueryReadable], [Query]) VALUES (@p1, @p2, @p3, @p4)";

        private const string TWITTER_OPERATOR_AND = " AND ";
        private const string TWITTER_OPERATOR_CONTAINS = "Contains";
        private const string TWITTER_OPERATOR_DOES_NOT_CONTAIN = "Does not contain";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string searchQuery = request.DataStore.GetValue("SearchQuery") ?? string.Empty;
            string sqlConnectionString = request.DataStore.GetValueAtIndex("SqlConnectionString", "SqlServerIndex");

            searchQuery = searchQuery.Replace("'", "''");
            searchQuery = searchQuery.Replace("“", "\"");
            searchQuery = searchQuery.Replace("”", "\"");

            bool isAdvanced = IsAdvancedTwitterQuery(searchQuery);

            SqlParameter[] parameters = SqlUtility.MapValuesToSqlParameters(1, isAdvanced, searchQuery);
            SqlUtility.ExecuteQueryWithParameters(sqlConnectionString, INSERT_INTO_TWITTER_QUERY, parameters);

            if (!isAdvanced)
            {
                List<string> conditions = SplitQueryByOr(searchQuery);
                int detailId = 1;
                for (int i = 0; i < conditions.Count; i++)
                {
                    ReadableQueryPart readableQueryPart = GetReadableQueryPart(conditions[i]);
                    parameters = SqlUtility.MapValuesToSqlParameters(i + 1, 1, readableQueryPart.Query, conditions[i]);
                    SqlUtility.ExecuteQueryWithParameters(sqlConnectionString, INSERT_INTO_TWITTER_QUERY_READABLE, parameters);

                    for (int j = 0; j < readableQueryPart.Details.Count; j++)
                    {
                        string detail = readableQueryPart.Details[j];
                        if (detail[0] == 'C')
                        {
                            parameters = SqlUtility.MapValuesToSqlParameters(detailId, i + 1, TWITTER_OPERATOR_CONTAINS, detail.Substring(TWITTER_OPERATOR_CONTAINS.Length + 2).TrimEnd('"'));
                            SqlUtility.ExecuteQueryWithParameters(sqlConnectionString, INSERT_INTO_TWITTER_QUERY_DETAILS, parameters);
                        }
                        else
                        {
                            parameters = SqlUtility.MapValuesToSqlParameters(detailId, i + 1, TWITTER_OPERATOR_DOES_NOT_CONTAIN, detail.Substring(TWITTER_OPERATOR_DOES_NOT_CONTAIN.Length + 2).TrimEnd('"'));
                            SqlUtility.ExecuteQueryWithParameters(sqlConnectionString, INSERT_INTO_TWITTER_QUERY_DETAILS, parameters);
                        }
                        detailId++;
                    }
                }
            }

            return new ActionResponse(ActionStatus.Success);
        }

        private ReadableQueryPart GetReadableQueryPart(string query)
        {
            ReadableQueryPart readableQueryPart = new ReadableQueryPart();

            StringBuilder sbPart = new StringBuilder();
            StringBuilder sbRead = new StringBuilder();

            bool isOpenQuote = false;
            bool isTerminated = false;

            for (int i = 0; i < query.Length; i++)
            {
                switch (query[i])
                {
                    case '"':
                        if (isOpenQuote)
                        {
                            isTerminated = true;
                        }
                        isOpenQuote = !isOpenQuote;
                        break;
                    case ' ':
                        if (isOpenQuote)
                        {
                            sbPart.Append(query[i]);
                        }
                        else
                        {
                            isTerminated = true;
                        }
                        break;
                    case '(':
                        if (isOpenQuote)
                        {
                            sbPart.Append(query[i]);
                        }
                        break;
                    case ')':
                        if (isOpenQuote)
                        {
                            sbPart.Append(query[i]);
                        }
                        else
                        {
                            isTerminated = true;
                        }
                        break;
                    default:
                        sbPart.Append(query[i]);
                        break;
                }

                if (isTerminated || i == query.Length - 1)
                {
                    string read = sbPart.ToString();
                    if (read.Length > 0)
                    {
                        if (read[0] == '-')
                        {
                            string detail = $"{TWITTER_OPERATOR_DOES_NOT_CONTAIN} \"{read.Substring(1)}\"";
                            readableQueryPart.Details.Add(detail);
                            sbRead.Append(detail);
                        }
                        else
                        {
                            string detail = $"{TWITTER_OPERATOR_CONTAINS} \"{read}\"";
                            readableQueryPart.Details.Add(detail);
                            sbRead.Append(detail);
                        }
                        if (i < query.Length - 2)
                        {
                            sbRead.Append(TWITTER_OPERATOR_AND);
                        }
                    }
                    sbPart.Clear();
                    isTerminated = false;
                }
            }

            readableQueryPart.Query = sbRead.ToString();

            return readableQueryPart;
        }

        private bool IsAdvancedTwitterQuery(string query)
        {
            bool isAdvanced = false;

            bool isOpenBracket = false;
            bool isOpenQuote = false;

            for (int i = 0; i < query.Length && !isAdvanced; i++)
            {
                switch (query[i])
                {
                    case '"':
                        isOpenQuote = !isOpenQuote;
                        break;
                    case ':':
                        isAdvanced = !isOpenQuote;
                        break;
                    case '(':
                        if (!isOpenQuote)
                        {
                            if (isOpenBracket)
                            {
                                isAdvanced = true;
                            }
                            else
                            {
                                isOpenBracket = true;
                            }
                        }
                        break;
                    case ')':
                        if (!isOpenQuote)
                        {
                            if (isOpenBracket)
                            {
                                isOpenBracket = false;
                            }
                            else
                            {
                                isAdvanced = true;
                            }
                        }
                        break;
                    case 'O':
                        isAdvanced = !isOpenQuote && i < query.Length - 1 && query[i + 1] == 'R' && isOpenBracket;
                        break;
                    default:
                        break;
                }
            }

            return isAdvanced;
        }

        private bool IsOrTerminated(string query, int i)
        {
            return (i == 0 && query.Length > 2 && IsTerminator(query[2])) || // OR is the first part of the query
                   (i > 0 && i < query.Length - 2 && IsTerminator(query[i - 1]) && IsTerminator(query[i + 2])) || // OR is somewhere in the middle
                   (i != 0 && i == query.Length - 2 && IsTerminator(query[i - 1])); // OR is the last part of the query
        }

        private bool IsTerminator(char c)
        {
            bool isTerminator = false;
            for (int i = 0; i < TERMINATORS.Length && !isTerminator; i++)
            {
                isTerminator = c == TERMINATORS[i];
            }
            return isTerminator;
        }

        private List<string> SplitQueryByOr(string query)
        {
            List<string> parts = new List<string>();
            StringBuilder sb = new StringBuilder();

            bool isOpenQuote = false;

            for (int i = 0; i < query.Length; i++)
            {
                switch (query[i])
                {
                    case 'O': // found an OR if
                        if (!isOpenQuote && // don't have an open quote active
                            i < query.Length - 1 && // not the last character
                            query[i + 1] == 'R' && // following character is an 'R'
                            IsOrTerminated(query, i))
                        {
                            string partTrimmed = sb.ToString().Trim();
                            if (partTrimmed.Length > 0)
                            {
                                parts.Add(partTrimmed);
                                sb.Clear();
                            }
                            i++;
                        }
                        else
                        {
                            sb.Append(query[i]);
                        }
                        break;
                    case '"':
                        isOpenQuote = !isOpenQuote;
                        sb.Append(query[i]);
                        break;
                    default:
                        sb.Append(query[i]);
                        break;
                }
            }

            string lastTrim = sb.ToString().Trim();
            if (lastTrim.Length > 0)
            {
                parts.Add(lastTrim);
            }

            return parts;
        }
    }
}