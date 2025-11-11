namespace Directus.Net.Models;

/// <summary>
/// Fluent query builder for Directus queries
/// </summary>
public class QueryBuilder
{
    private readonly DirectusQuery _query = new();

    /// <summary>
    /// Adds a filter to the query
    /// </summary>
    /// <param name="filter">The filter object</param>
    /// <returns>This query builder</returns>
    public QueryBuilder WithFilter(object filter)
    {
        _query.Filter = filter;
        return this;
    }

    /// <summary>
    /// Sorts by fields (prefix with - for descending)
    /// </summary>
    /// <param name="fields">The fields to sort by</param>
    /// <returns>This query builder</returns>
    public QueryBuilder Sort(params string[] fields)
    {
        _query.Sort = fields;
        return this;
    }

    /// <summary>
    /// Limits the number of results
    /// </summary>
    /// <param name="limit">Maximum number of items</param>
    /// <returns>This query builder</returns>
    public QueryBuilder Limit(int limit)
    {
        _query.Limit = limit;
        return this;
    }

    /// <summary>
    /// Skips a number of items
    /// </summary>
    /// <param name="offset">Number of items to skip</param>
    /// <returns>This query builder</returns>
    public QueryBuilder Offset(int offset)
    {
        _query.Offset = offset;
        return this;
    }

    /// <summary>
    /// Specifies the page number
    /// </summary>
    /// <param name="page">Page number</param>
    /// <returns>This query builder</returns>
    public QueryBuilder Page(int page)
    {
        _query.Page = page;
        return this;
    }

    /// <summary>
    /// Selects specific fields
    /// </summary>
    /// <param name="fields">The fields to include</param>
    /// <returns>This query builder</returns>
    public QueryBuilder Fields(params string[] fields)
    {
        _query.Fields = fields;
        return this;
    }

    /// <summary>
    /// Adds a search query
    /// </summary>
    /// <param name="search">The search query</param>
    /// <returns>This query builder</returns>
    public QueryBuilder Search(string search)
    {
        _query.Search = search;
        return this;
    }

    /// <summary>
    /// Requests metadata (e.g., "total_count", "filter_count")
    /// </summary>
    /// <param name="meta">Metadata options</param>
    /// <returns>This query builder</returns>
    public QueryBuilder WithMeta(string meta = "*")
    {
        _query.Meta = meta;
        return this;
    }

    /// <summary>
    /// Adds deep query parameters
    /// </summary>
    /// <param name="relation">The relation name</param>
    /// <param name="query">The nested query</param>
    /// <returns>This query builder</returns>
    public QueryBuilder Deep(string relation, object query)
    {
        _query.Deep ??= new Dictionary<string, object>();
        _query.Deep[relation] = query;
        return this;
    }

    /// <summary>
    /// Adds an aggregation function
    /// </summary>
    /// <param name="field">The field to aggregate</param>
    /// <param name="function">The aggregation function (count, sum, avg, min, max)</param>
    /// <returns>This query builder</returns>
    public QueryBuilder Aggregate(string field, string function)
    {
        _query.Aggregate ??= new Dictionary<string, string>();
        _query.Aggregate[field] = function;
        return this;
    }

    /// <summary>
    /// Groups results by fields
    /// </summary>
    /// <param name="fields">The fields to group by</param>
    /// <returns>This query builder</returns>
    public QueryBuilder GroupBy(params string[] fields)
    {
        _query.GroupBy = fields;
        return this;
    }

    /// <summary>
    /// Builds the query
    /// </summary>
    /// <returns>The built query</returns>
    public DirectusQuery Build()
    {
        return _query;
    }
}

/// <summary>
/// Static class for building filter expressions
/// </summary>
public static class Filter
{
    /// <summary>
    /// Equals operator
    /// </summary>
    public static object Eq(object value) => new { _eq = value };

    /// <summary>
    /// Not equals operator
    /// </summary>
    public static object Neq(object value) => new { _neq = value };

    /// <summary>
    /// In operator
    /// </summary>
    public static object In(params object[] values) => new { _in = values };

    /// <summary>
    /// Not in operator
    /// </summary>
    public static object Nin(params object[] values) => new { _nin = values };

    /// <summary>
    /// Less than operator
    /// </summary>
    public static object Lt(object value) => new { _lt = value };

    /// <summary>
    /// Less than or equal operator
    /// </summary>
    public static object Lte(object value) => new { _lte = value };

    /// <summary>
    /// Greater than operator
    /// </summary>
    public static object Gt(object value) => new { _gt = value };

    /// <summary>
    /// Greater than or equal operator
    /// </summary>
    public static object Gte(object value) => new { _gte = value };

    /// <summary>
    /// Contains operator (case-sensitive)
    /// </summary>
    public static object Contains(string value) => new { _contains = value };

    /// <summary>
    /// Contains operator (case-insensitive)
    /// </summary>
    public static object IContains(string value) => new { _icontains = value };

    /// <summary>
    /// Starts with operator
    /// </summary>
    public static object StartsWith(string value) => new { _starts_with = value };

    /// <summary>
    /// Ends with operator
    /// </summary>
    public static object EndsWith(string value) => new { _ends_with = value };

    /// <summary>
    /// Is null operator
    /// </summary>
    public static object IsNull() => new { _null = true };

    /// <summary>
    /// Is not null operator
    /// </summary>
    public static object IsNotNull() => new { _nnull = true };

    /// <summary>
    /// Empty operator
    /// </summary>
    public static object IsEmpty() => new { _empty = true };

    /// <summary>
    /// Not empty operator
    /// </summary>
    public static object IsNotEmpty() => new { _nempty = true };

    /// <summary>
    /// AND logical operator
    /// </summary>
    public static object And(params object[] conditions) => new { _and = conditions };

    /// <summary>
    /// OR logical operator
    /// </summary>
    public static object Or(params object[] conditions) => new { _or = conditions };
}
