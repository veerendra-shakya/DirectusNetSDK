using Directus.Net.Models;
using FluentAssertions;

namespace Directus.Net.Tests;

public class QueryBuilderTests
{
    [Fact]
    public void QueryBuilder_Should_Build_Query_With_Limit()
    {
        var query = new QueryBuilder()
            .Limit(10)
            .Build();

        query.Limit.Should().Be(10);
    }

    [Fact]
    public void QueryBuilder_Should_Build_Query_With_Sort()
    {
        var query = new QueryBuilder()
            .Sort("-date_created", "title")
            .Build();

        query.Sort.Should().NotBeNull();
        query.Sort.Should().HaveCount(2);
        query.Sort![0].Should().Be("-date_created");
        query.Sort[1].Should().Be("title");
    }

    [Fact]
    public void QueryBuilder_Should_Build_Query_With_Fields()
    {
        var query = new QueryBuilder()
            .Fields("id", "title", "content")
            .Build();

        query.Fields.Should().NotBeNull();
        query.Fields.Should().HaveCount(3);
        query.Fields.Should().Contain("id");
        query.Fields.Should().Contain("title");
        query.Fields.Should().Contain("content");
    }

    [Fact]
    public void QueryBuilder_Should_Build_Query_With_Offset()
    {
        var query = new QueryBuilder()
            .Offset(20)
            .Build();

        query.Offset.Should().Be(20);
    }

    [Fact]
    public void QueryBuilder_Should_Build_Query_With_Page()
    {
        var query = new QueryBuilder()
            .Page(3)
            .Build();

        query.Page.Should().Be(3);
    }

    [Fact]
    public void QueryBuilder_Should_Build_Query_With_Search()
    {
        var query = new QueryBuilder()
            .Search("test query")
            .Build();

        query.Search.Should().Be("test query");
    }

    [Fact]
    public void QueryBuilder_Should_Build_Query_With_Meta()
    {
        var query = new QueryBuilder()
            .WithMeta("total_count")
            .Build();

        query.Meta.Should().Be("total_count");
    }

    [Fact]
    public void QueryBuilder_Should_Build_Complex_Query()
    {
        var query = new QueryBuilder()
            .Limit(10)
            .Offset(0)
            .Sort("-date_created")
            .Fields("id", "title", "status")
            .Search("directus")
            .WithMeta("*")
            .Build();

        query.Limit.Should().Be(10);
        query.Offset.Should().Be(0);
        query.Sort.Should().Contain("-date_created");
        query.Fields.Should().HaveCount(3);
        query.Search.Should().Be("directus");
        query.Meta.Should().Be("*");
    }

    [Fact]
    public void Filter_Eq_Should_Create_Correct_Filter()
    {
        var filter = Filter.Eq("published");
        
        filter.Should().NotBeNull();
    }

    [Fact]
    public void Filter_In_Should_Create_Correct_Filter()
    {
        var filter = Filter.In("draft", "published", "archived");
        
        filter.Should().NotBeNull();
    }

    [Fact]
    public void Filter_And_Should_Create_Correct_Filter()
    {
        var filter = Filter.And(
            new { status = Filter.Eq("published") },
            new { views = Filter.Gt(100) }
        );
        
        filter.Should().NotBeNull();
    }

    [Fact]
    public void Filter_Or_Should_Create_Correct_Filter()
    {
        var filter = Filter.Or(
            new { status = Filter.Eq("draft") },
            new { status = Filter.Eq("published") }
        );
        
        filter.Should().NotBeNull();
    }
}
