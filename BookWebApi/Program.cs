using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// builder.Services.AddDbContext<BooksContext>(options =>
//     options.UseSqlite("Data Source=books.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//Commenting out this as it throws a warning when running the app
//app.UseHttpsRedirection();

var file = "books.json";

List<Book> LoadBooks() =>
    File.Exists(file)
        ? JsonSerializer.Deserialize<List<Book>>(File.ReadAllText(file)) ?? new List<Book>()
        : new List<Book>();

void SaveBooks(List<Book> books) =>
    File.WriteAllText(file, JsonSerializer.Serialize(books, new JsonSerializerOptions { WriteIndented = true }));

    var books = LoadBooks();

// Shows all contents of the list of books
app.MapGet("/getbooks", () =>
{
    return books;
})
.WithName("GetBooks");

// Adds a new book to the list and sets the publication status to "Planned"
app.MapPost("/createbook", (Book book) =>
{
    if (string.IsNullOrEmpty(book.Title))
        return Results.BadRequest("Book title is required.");

    var nextId = books.Count > 0 ? books.Max(b => b.Id) + 1 : 1;
    book.Id = nextId;
    book.PubStatus = "Planned";
    books.Add(book);

    SaveBooks(books);

    return Results.Created($"/books/{book.Id}", book);

}).WithName("CreateBook");


// Publishes the book and sets the publication date to today
app.MapPatch("/publishbook", (int bid) =>
{
    var curBook = books.Find(b => b.Id == bid);
    if (curBook is null)
        return Results.NotFound("The book you are looking for does not exist.");

    if (curBook.PubStatus == "Published")
        return Results.BadRequest("Book is already published.");

    curBook.PubStatus = "Published";
    curBook.PubDate = DateTime.Now.ToString("yyyy-MM-dd");

    SaveBooks(books); 

    return Results.Ok(curBook);

}).WithName("PublishBook");


// Lists all books with their title and publication status
app.MapGet("/listbooks", () =>
{
    return books.Select(b => new {
        b.Title,
        b.PubStatus,
    });

}).WithName("ListBooks");

//     Code for the database endpoints

// // Shows all contents of the list of books
// app.MapGet("/getbooks", async (BooksContext db) =>
// {
//     await db.Books.ToListAsync();
// })
// .WithName("GetBooks");

// // Adds a new book to the list and sets the publication status to "Planned"
// app.MapPost("/createbook", async (Book book, BooksContext db) => {

//     if (string.IsNullOrEmpty(book.Title))
//         return Results.BadRequest("Book title is required.");

//     book.PubStatus = "Planned";
//     db.Books.Add(book);
//     await db.SaveChangesAsync();


//     return Results.Created($"/books/{book.Id}", book);

// }).WithName("CreateBook");

// // Publishes the book and sets the publication date to today
// app.MapPatch("publishbook", async (int id, BooksContext db) =>
// {
//     var curBook = await db.Books.FindAsync(id);
//     if (curBook is null)
//         return Results.NotFound("The book you are looking for does not exist.");

//     if (curBook.PubStatus == "Published")
//         return Results.BadRequest("The book is already published.");

//     curBook.PubStatus = "Published";
//     curBook.PubDate = DateTime.Now.ToString("yyyy-MM-dd");

//     await db.SaveChangesAsync();
//     return Results.Ok(curBook);

// }).WithName("PublishBook");

// // Lists all books with their title and publication status
// app.MapGet("/listbooks", async (BooksContext db) =>
// {
//     await db.Books.Select(b => new {b.Title, b.PubStatus}).ToListAsync();
// })
// .WithName("ListBooks");


app.Run();

public partial class Program { }

public class Book
{
    public required int? Id { get; set; }

    public required string? Title { get; set; }

    public required string? PubStatus { get; set; }

    public string? PubDate { get; set; }
}