using Codespirals.Base.Extensions;
using Codespirals.Base.Helpers;
using Codespirals.Base.Filtering;
using System.Text;

namespace Codespirals.Solutions.ApiCaller;

/// <summary>
/// 
/// </summary>
public static class QueryParameterExtensions
{
    /// <summary>
    /// Turns an object of <see cref="IFilterParameters"/> into a string
    /// </summary>
    /// <typeparam name="TFilterParameters"></typeparam>
    /// <param name="filterParameters"></param>
    /// <param name="startWithQuestionMark"></param>
    /// <returns></returns>
    public static string ToQueryString<TFilterParameters>(this TFilterParameters filterParameters, bool startWithQuestionMark = true)
        where TFilterParameters : IFilterParameters
    {
        System.Reflection.PropertyInfo[] properties = filterParameters.GetType().GetProperties();
        var parameters = new List<KeyValuePair<string, string>>();

        foreach (System.Reflection.PropertyInfo property in properties)
        {
            if (!property.CanRead)
                continue;
            object? value = property.GetValue(filterParameters);
            if (value is null || !IdentificationHelper.IsBaseType(value))
                continue;
            string? valueAsString = value.ToString();
            if (string.IsNullOrWhiteSpace(valueAsString))
                continue;

            // make name pascalCase (API standard) as properties in C# are CamelCase standard
            string name = $"{char.ToLowerInvariant(property.Name[0])}{property.Name[1..]}";
            parameters.Add(new(name, valueAsString));
        }
        return parameters.ToQueryString(startWithQuestionMark);
    }
    /// <summary>
    /// Turns a list of <see cref="KeyValuePair"/>s of <see cref="string"/>s into a parameter string
    /// </summary>
    /// <param name="queryparams"></param>
    /// <param name="startWithQuestionMark"></param>
    /// <returns></returns>
    public static string ToQueryString(this IEnumerable<KeyValuePair<string, string>> queryparams, bool startWithQuestionMark = true)
    {
        if (!queryparams.Any())
            return string.Empty;
        StringBuilder parameterStringBuilder = new();
        if (startWithQuestionMark)
            parameterStringBuilder.Append('?');
        bool addAmpersand = false;

        foreach (KeyValuePair<string, string> parameter in queryparams)
        {
            if (addAmpersand)
                parameterStringBuilder.Append('&');
            parameterStringBuilder.Append(Uri.EscapeDataString(parameter.Key))
                .Append('=').Append(Uri.EscapeDataString(parameter.Value));
            addAmpersand = true;
        }
        return parameterStringBuilder.ToString();
    }
}
