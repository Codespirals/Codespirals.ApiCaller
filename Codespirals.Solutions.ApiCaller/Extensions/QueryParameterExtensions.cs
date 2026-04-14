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
        StringBuilder sb = new();

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

            if (startWithQuestionMark && sb.Length is 0)
                sb.Append('?');
            else
                sb.Append('&');
            // make name pascalCase (API standard) as properties in C# are CamelCase standard
            string name = $"{char.ToLowerInvariant(property.Name[0])}{property.Name[1..]}";
            sb.Append($"{name}={valueAsString}");
        }
        return sb.ToString();
    }
}
