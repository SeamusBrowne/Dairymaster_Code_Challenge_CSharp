using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//Commenting out this as it throws a warning when running the app
//app.UseHttpsRedirection();

// Populates List of books
var books = new List<Book>{
    new Book { Bid = 1, Title = "Lure of Silence", PubStatus = "Published", PubDate = "1960-01-01" },
    new Book { Bid = 2, Title = "War of the Rose", PubStatus = "Published", PubDate = "2023-11-27" },
    new Book { Bid = 3, Title = "Rally of the Lakes Magazine", PubStatus = "Published", PubDate = "2025-05-01" },
    new Book { Bid = 4, Title = "The Copper Man", PubStatus = "Planned", PubDate = "" }
};

// Shows all contents of the list of books
app.MapGet("/getbooks", () =>
{
    return books;
})
.WithName("GetBooks");

// Adds a new book to the list and sets the publication status to "Planned"
app.MapPost("/createbook", (Book book) => {

    if (string.IsNullOrEmpty(book.Title))
        return Results.BadRequest("Book title is required.");

    var nextId = books.Count > 0 ? books.Max(b => b.Bid) + 1 : 1;
    book.Bid = nextId;
    book.PubStatus = "Planned";
    books.Add(book);

    return Results.Created($"/books/{book.Bid}", book);

});

// Publishes the book and sets the publication date to today
app.MapPatch("publishbook", (int bid, Book updateBook) =>
{
    var curBook = books.Find(b => b.Bid == bid);
    if (curBook is null)
        return Results.NotFound("The book you are looking for does not exist.");
    
        curBook.PubStatus = "Published";
        curBook.PubDate = DateTime.Now.ToString("yyyy-MM-dd");

    return Results.Ok(curBook);

}).WithName("PublishBook");

// Lists all books with their title and publication status
app.MapGet("/listbooks", () =>
{
    return books.Select(b => new {
        b.Title,
        b.PubStatus,
    });
})
.WithName("ListBooks");


app.Run();

public partial class Program { }

public class Book
{
    public required int? Bid { get; set; }

    public required string? Title { get; set; }

    public required string? PubStatus { get; set; }

    public string? PubDate { get; set; }
}
