using System.Text.RegularExpressions;

namespace Codespirals.Solutions.ApiCaller;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public static partial class RegularExpressions
{
    [GeneratedRegex(@"^(?:http(?:s)?://)?(?:www(?:[0-9]+)?\.)?")]
    public static partial Regex MatchDomainPrefixes();
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

