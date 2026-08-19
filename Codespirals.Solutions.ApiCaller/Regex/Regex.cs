using System.Text.RegularExpressions;

namespace Codespirals.Solutions.ApiCaller;

/// <summary>
/// 
/// </summary>
public static partial class RegularExpressions
{
    [GeneratedRegex(@"^(?:http(?:s)?://)?(?:www(?:[0-9]+)?\.)?")]
    public static partial Regex MatchDomainPrefixes();
}

