namespace SampleServer
{
    /// <summary>
    /// Represents a Tableau Connected App.
    /// <seealso href="https://help.tableau.com/current/online/en-us/connected_apps_direct.htm" />
    /// </summary>
    public class ConnectedApp
    {
        /// <summary>
        /// Gets or sets the Connected App's ID, aka the client ID.
        /// </summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Connected App's secret ID.
        /// </summary>
        public string SecretId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Connected App's secret value.
        /// </summary>
        public string SecretValue { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user name (email address) of the authenticated Tableau Cloud user.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user attributes.
        /// </summary>
        public Dictionary<string, string> UserAttributes { get; set; } = new();
    }
}
