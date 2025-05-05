using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;


namespace IntegrationTest;

public class UnitTest1 : WebApplicationFactory<Program>

{
    [Fact]
    public async Task Test1_ListBooks()
    {
        var client = CreateClient();
        var response = await client.GetAsync("/listbooks");
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

    }

    [Fact]
    public async Task Test2_GetBooks()
    {
        var client = CreateClient();
        var response = await client.GetAsync("/getbooks");
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

    }

    [Fact]
    public async Task Test3_CreateBook()
    {
        var newBook = new Book
        {
            Title = "Title Test",
            PubStatus = "",
            PubDate = ""
        };

        var client = CreateClient();
        var response = await client.PostAsJsonAsync("/createbook", newBook);
        response.EnsureSuccessStatusCode();

        var createdBook = await response.Content.ReadFromJsonAsync<Book>();
        createdBook.PubStatus.Should().Be("Planned");


    }

    // Tests book publishing of an already planned book (Book 4)
    [Fact]
    public async Task Test4_PublishBook()
    {
        var bid = 4;
        var targetBook = new Book
        {
            Bid = bid,
            Title = "",
            PubStatus = "",
            PubDate = ""
        };

        var client = CreateClient();
        var response = await client.PatchAsJsonAsync($"/publishbook?bid={bid}", targetBook);

        var publishedBook = await response.Content.ReadFromJsonAsync<Book>();
        publishedBook!.Bid.Should().Be(bid);
        publishedBook.PubStatus.Should().Be("Published");
        publishedBook.PubDate.Should().Be(DateTime.Now.ToString("yyyy-MM-dd"));
        publishedBook.PubDate.Should().NotBeNullOrEmpty();

    }

    public class Book
    {
        public int? Bid { get; set; }
        public string? Title { get; set; }
        public string? PubStatus { get; set; }
        public string? PubDate { get; set; }
    }
}
