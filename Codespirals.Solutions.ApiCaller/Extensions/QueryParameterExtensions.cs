using Codespirals.Base.Extensions;
using Codespirals.Base.Filtering;
using System.Text;

namespace Codespirals.Solutions.ApiCaller;
internal static class QueryParameterExtensions
{
    internal static string ToQueryString<TFilterParameters>(this TFilterParameters filterParameters, bool startWithAmp = true)
        where TFilterParameters : IFilterParameters
    {
        System.Reflection.PropertyInfo[] properties = filterParameters.GetType().GetProperties();
        StringBuilder sb = new();
        foreach (System.Reflection.PropertyInfo property in properties)
        {
            if (startWithAmp)
                sb.Append('&');
            if (!property.CanRead)
                continue;
            object? value = property.GetValue(filterParameters);
            if (value is null || !value.IsBaseType())
                continue;
            string? valueAsString = value.ToString();
            if (string.IsNullOrWhiteSpace(valueAsString))
                continue;
            // make name camelcase (API standard) as this is often pascal case
            string name = $"{char.ToLowerInvariant(property.Name[0])}{property.Name[1..]}";
            sb.Append($"{name}={valueAsString}");
            startWithAmp = true;
        }
        return sb.ToString();
    }
}
