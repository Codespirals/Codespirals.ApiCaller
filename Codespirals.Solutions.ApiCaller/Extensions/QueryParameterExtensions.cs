namespace Codespirals.Solutions.ApiCaller;
internal static class QueryParameterExtensions
{
    internal static void AddFilterParameters<TFilterParameters>(this List<KeyValuePair<string, string>> queryParameters, TFilterParameters filterParameters)
        where TFilterParameters : IFilterParameters
    {
        System.Reflection.PropertyInfo[] properties = filterParameters.GetType().GetProperties();
        foreach (System.Reflection.PropertyInfo property in properties)
        {
            if (!property.CanRead)
                continue;
            object? value = property.GetValue(filterParameters);
            if (value is null || !value.IsBaseType())
                continue;
            string? valueAsString = value.ToString();
            if (string.IsNullOrWhiteSpace(valueAsString))
                continue;
            // make name camelcase (API standard) as this is often pascal case
            string name = char.ToLowerInvariant(property.Name[0]) + property.Name[1..]; ;
            queryParameters.Add(new KeyValuePair<string, string>(name, valueAsString));
        }
    }
}
