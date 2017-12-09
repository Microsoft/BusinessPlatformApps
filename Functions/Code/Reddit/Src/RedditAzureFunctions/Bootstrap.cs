using System.Data.Entity;
using RedditCore.DataModel;

namespace RedditAzureFunctions
{
    public class Bootstrap
    {
        public void Init()
        {
            // Normally you do this initialization by using the DbConfigurationTypeAttribute
            // on the DbContext object.  Could not get the attribute method to work
            // so fell back on this static initialization
            DbConfiguration.SetConfiguration(new RedditDatabaseConfiguration());
        }
    }
}
