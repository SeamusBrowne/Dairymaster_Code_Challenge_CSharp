using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using BookWebApi;


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

    //Test book creation
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
            Id = bid,
            Title = "",
            PubStatus = "",
            PubDate = ""
        };

        var client = CreateClient();
        var response = await client.PatchAsJsonAsync($"/publishbook?bid={bid}", targetBook);

        var publishedBook = await response.Content.ReadFromJsonAsync<Book>();
        publishedBook!.Id.Should().Be(bid);
        publishedBook.PubStatus.Should().Be("Published");
        publishedBook.PubDate.Should().Be(DateTime.Now.ToString("yyyy-MM-dd"));
        publishedBook.PubDate.Should().NotBeNullOrEmpty();

    }

        // Tests book publishing of a created book
    [Fact]
    public async Task Test5_CreatePublishBook()
    {
        var createPublishBook = new Book
        {
            Title = "Book For Publish",
            PubStatus = "Planned",
            PubDate = ""
        };

        var client = CreateClient();
        var createResponse = await client.PostAsJsonAsync("/createbook", createPublishBook);
        createResponse.EnsureSuccessStatusCode();

        var createdBook = await createResponse.Content.ReadFromJsonAsync<Book>();
        createdBook.Should().NotBeNull();
        createdBook!.Title.Should().Be("Book For Publish");
        createdBook.PubStatus.Should().Be("Planned");


        var response = await client.PatchAsJsonAsync<Book>($"/publishbook?bid={createdBook.Id}", createdBook);
        response.EnsureSuccessStatusCode();
        var publishedBook = await response.Content.ReadFromJsonAsync<Book>();
        publishedBook!.Id.Should().Be(createdBook.Id);
        publishedBook.PubStatus.Should().Be("Published"); 
        publishedBook.PubDate.Should().Be(DateTime.Now.ToString("yyyy-MM-dd"));
        publishedBook.PubDate.Should().NotBeNullOrEmpty();

    }

    public class Book
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? PubStatus { get; set; }
        public string? PubDate { get; set; }
    }
}
