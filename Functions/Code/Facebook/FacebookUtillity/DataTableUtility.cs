using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookUtillity
{
    public class DataTableUtility
    {
        #region Standard FB Tables
        public static DataTable GetCommentsDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id");
            table.Columns.Add("Created Date", typeof(DateTime));
            table.Columns.Add("Message");
            table.Columns.Add("From Id");
            table.Columns.Add("From Name");
            table.Columns.Add("Post Id");
            table.Columns.Add("Page");
            table.Columns.Add("PageDisplayName");
            table.Columns.Add("PageId");
            return table;
        }

        public static DataTable GetReactionsDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id");
            table.Columns.Add("Reaction Type");
            table.Columns.Add("Count", typeof(long));
            return table;
        }

        public static DataTable GetPostsDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id");
            table.Columns.Add("Created Date", typeof(DateTime));
            table.Columns.Add("Message");
            table.Columns.Add("From Id");
            table.Columns.Add("From Name");
            table.Columns.Add("Media");
            table.Columns.Add("Page");
            table.Columns.Add("PageDisplayName");
            table.Columns.Add("PageId");
            table.Columns.Add("Total Comments", typeof(double));
            return table;
        }


        public static DataTable GetSentimentDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id");
            table.Columns.Add("Sentiment", typeof(float));
            return table;
        }

        public static DataTable GetKeyPhraseDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id");
            table.Columns.Add("KeyPhrase");
            return table;
        }

        public static DataTable GetHashTagDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id");
            table.Columns.Add("HashTags");
            return table;
        }

        public static DataTable GetErrorDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Date", typeof(DateTime));
            table.Columns.Add("Error");
            table.Columns.Add("Posts");
            return table;
        }
        #endregion

        #region Page Analytics Tables
        public static DataTable GetPagePostStoriesAndPeopleTalkingAboutThisTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("EndTime", typeof(DateTime));
            table.Columns.Add("Name");
            table.Columns.Add("Entry Name");
            table.Columns.Add("Value");
            table.Columns.Add("Period");
            table.Columns.Add("Title");
            table.Columns.Add("Description");
            table.Columns.Add("Id");
            table.Columns.Add("PageId");
            return table;
        }

        public static DataTable GePageImpressionsTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("EndTime", typeof(DateTime));
            table.Columns.Add("Name");
            table.Columns.Add("Entry Name");
            table.Columns.Add("Value");
            table.Columns.Add("Period");
            table.Columns.Add("Title");
            table.Columns.Add("Description");
            table.Columns.Add("Id");
            table.Columns.Add("PageId");
            return table;
        }

        public static DataTable GetPageEngagementTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("EndTime", typeof(DateTime));
            table.Columns.Add("Name");
            table.Columns.Add("Entry Name");
            table.Columns.Add("Value");
            table.Columns.Add("Period");
            table.Columns.Add("Title");
            table.Columns.Add("Description");
            table.Columns.Add("Id");
            table.Columns.Add("PageId");
            return table;
        }

        public static DataTable GetPageReactionsTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("EndTime", typeof(DateTime));
            table.Columns.Add("Name");
            table.Columns.Add("Entry Name");
            table.Columns.Add("Value");
            table.Columns.Add("Period");
            table.Columns.Add("Title");
            table.Columns.Add("Description");
            table.Columns.Add("Id");
            table.Columns.Add("PageId");
            return table;
        }

        public static DataTable GetPageClicksDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("EndTime", typeof(DateTime));
            table.Columns.Add("Name");
            table.Columns.Add("Entry Name");
            table.Columns.Add("Value");
            table.Columns.Add("Period");
            table.Columns.Add("Title");
            table.Columns.Add("Description");
            table.Columns.Add("Id");
            table.Columns.Add("PageId");
            return table;
        }

        public static DataTable GetPageUserDemographicsTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("EndTime", typeof(DateTime));
            table.Columns.Add("Name");
            table.Columns.Add("Entry Name");
            table.Columns.Add("Value");
            table.Columns.Add("Period");
            table.Columns.Add("Title");
            table.Columns.Add("Description");
            table.Columns.Add("Id");
            table.Columns.Add("PageId");
            return table;
        }

        public static DataTable GetPageContentTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("EndTime", typeof(DateTime));
            table.Columns.Add("Name");
            table.Columns.Add("Entry Name");
            table.Columns.Add("Value");
            table.Columns.Add("Period");
            table.Columns.Add("Title");
            table.Columns.Add("Description");
            table.Columns.Add("Id");
            table.Columns.Add("PageId");
            return table;
        }

        public static DataTable GetPageViewsTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("EndTime", typeof(DateTime));
            table.Columns.Add("Name");
            table.Columns.Add("Entry Name");
            table.Columns.Add("Value");
            table.Columns.Add("Period");
            table.Columns.Add("Title");
            table.Columns.Add("Description");
            table.Columns.Add("Id");
            table.Columns.Add("PageId");
            return table;
        }

        public static DataTable GetPageVideoViewsTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("EndTime", typeof(DateTime));
            table.Columns.Add("Name");
            table.Columns.Add("Entry Name");
            table.Columns.Add("Value");
            table.Columns.Add("Period");
            table.Columns.Add("Title");
            table.Columns.Add("Description");
            table.Columns.Add("Id");
            table.Columns.Add("PageId");
            return table;
        }

        public static DataTable GetPagePostsTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("EndTime", typeof(DateTime));
            table.Columns.Add("Name");
            table.Columns.Add("Entry Name");
            table.Columns.Add("Value");
            table.Columns.Add("Period");
            table.Columns.Add("Title");
            table.Columns.Add("Description");
            table.Columns.Add("Id");
            table.Columns.Add("PageId");
            return table;
        }

        public static DataTable GetPagePostImpressionsTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Name");
            table.Columns.Add("Entry Name");
            table.Columns.Add("Value");
            table.Columns.Add("Period");
            table.Columns.Add("Title");
            table.Columns.Add("Description");
            table.Columns.Add("Id");
            table.Columns.Add("PageId");
            return table;
        }

        public static DataTable GetPagePostEngagementTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("EndTime", typeof(DateTime));
            table.Columns.Add("Name");
            table.Columns.Add("Entry Name");
            table.Columns.Add("Value");
            table.Columns.Add("Period");
            table.Columns.Add("Title");
            table.Columns.Add("Description");
            table.Columns.Add("Id");
            table.Columns.Add("PageId");
            return table;
        }

        public static DataTable GetPagePostReactionsTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Name");
            table.Columns.Add("Entry Name");
            table.Columns.Add("Value");
            table.Columns.Add("Period");
            table.Columns.Add("Title");
            table.Columns.Add("Description");
            table.Columns.Add("Id");
            table.Columns.Add("PageId");
            return table;
        }

        public static DataTable GetPageVideoPostsTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Name");
            table.Columns.Add("Entry Name");
            table.Columns.Add("Value");
            table.Columns.Add("Period");
            table.Columns.Add("Title");
            table.Columns.Add("Description");
            table.Columns.Add("Id");
            table.Columns.Add("PageId");
            return table;
        }

        public static DataTable GetPostsInfoTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id");
            table.Columns.Add("PageId");
            table.Columns.Add("Message");
            table.Columns.Add("Created Time", typeof(DateTime));
            table.Columns.Add("Updated Time", typeof(DateTime));
            table.Columns.Add("Icon");
            table.Columns.Add("Story");
            table.Columns.Add("Link");
            table.Columns.Add("Status Type");
            table.Columns.Add("Is Hidden");
            table.Columns.Add("Is Published");
            table.Columns.Add("Name");
            table.Columns.Add("Object");
            table.Columns.Add("Permalink URL");
            table.Columns.Add("Picture");
            table.Columns.Add("Source");
            table.Columns.Add("Shares");
            table.Columns.Add("Type");
            return table;
        }

        public static DataTable GetPostsToTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id");
            table.Columns.Add("PageId");
            table.Columns.Add("Created Time", typeof(DateTime));
            table.Columns.Add("Updated Time", typeof(DateTime));
            table.Columns.Add("To Id");
            table.Columns.Add("To Name");
            return table;
        }
        #endregion
    }
}
