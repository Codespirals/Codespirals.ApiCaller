namespace Codespirals.Solutions.ApiCaller
{
    /// <summary>
    /// The credentials that get sent in the header of every API request
    /// </summary>
    public sealed class ApiCredentials
    {
        /// <summary>
        /// What the key or password token is named (e.g. "[MyApp]_Key") and the value of the token
        /// </summary>
        public (string Name, string Value) Key { get; init; }
        /// <summary>
        /// What the user token is named (e.g. "[MyApp]_User") and the value of the token
        /// </summary>
        /// <remarks>This is optional. Most apis use a single token.</remarks>
        public (string Name, string Value)? Id { get; init; }
        public ApiCredentials()
        {

        }
        public ApiCredentials(string keyName, string keyValue)
        {
            Key = (keyName, keyValue);
        }
        public ApiCredentials(string keyName, string keyValue, string idName, string idValue)
        {
            Id = (idName, idValue);
            Key = (keyName, keyValue);
        }
    }
}
