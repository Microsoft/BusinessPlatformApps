namespace RedditAzureFunctions
{
    /// <summary>
    /// These constants are in a separate file to prevent us accidently checking them in.  They must be changed
    /// for local debugging but we do not want to check them in to the repository.  It will really mess up 
    /// </summary>
    public static class WebServiceRunConstants
    {
        /// <summary>
        /// True to run the Reddit query on startup.  Falso not to run the Reddit query on host startup or deploy.
        /// </summary>
        public const bool RunRedditOnStartup = false;

        /// <summary>
        /// True to run AzureML on startup.  False not to run AzureML on host startup or deploy.
        /// </summary>
        public const bool RunAmlOnStartup = false;
    }
}
