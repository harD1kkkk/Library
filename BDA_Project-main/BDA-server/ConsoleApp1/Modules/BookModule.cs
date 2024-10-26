using ConsoleApp1.Entity;
using ConsoleApp1.Service;
using Microsoft.Extensions.Logging;
using Nancy;
using Nancy.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ConsoleApp1.Modules
{
    public class BookModule : NancyModule
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BookModule> _logger;

       

        public BookModule(IBookService bookService, ILogger<BookModule> logger) : base("/api/books")
        {
            _bookService = bookService;
            _logger = logger;

            // Create a book
            Post("/addBook", parameters =>
            {
                _logger.LogInformation("Request to create a new book with image.");

                // Parsing
                var files = this.Request.Files;
                var book = this.Bind<Book>();

                // Model validation
                var validationErrors = ValidateBook(book);
                if (validationErrors.Count > 0)
                {
                    _logger.LogWarning("Validation error when creating book: {Errors}", string.Join(", ", validationErrors));
                    return Response.AsJson(new { errors = validationErrors }, HttpStatusCode.BadRequest);
                }
                if (!files.Any())  
                {
                    _logger.LogWarning("No image file provided.");
                    return Response.AsJson(new { message = "Image file is required." }, HttpStatusCode.BadRequest);
                }

                try
                {
                    // Save the image to local storage
                    var file = files.First(); // Assuming one image per book
                    string uploadsFolder = "/Projects/booksImages"; // Local storage path
                    Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists

                    string fileExtension = Path.GetExtension(file.Name);
                    string fileName = $"{Guid.NewGuid()}{fileExtension}";
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    // Set the imagePath to the saved file path
                    book.imagePath = filePath;

                    // Add the book to the service (database)
                    _bookService.AddBook(book, file);

                    _logger.LogInformation("Book '{title}' successfully created with image.", book.title);
                    return Response.AsJson(book, HttpStatusCode.Created);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while creating book '{title}'", book.title);
                    return Response.AsJson(new { message = "An error occurred while creating the book.", details = ex.Message }, HttpStatusCode.InternalServerError);
                }
            });


            // Get all books
            Get("/allBooks", parameters =>
            {
                _logger.LogInformation("Request to retrieve all books.");
                try
                {
                    var books = _bookService.GetAllBooks();

                    // Load the images for each book
                    var bookWithImages = books.Select(book =>
                    {
                        string filePath = book.imagePath; // Full file path stored in the database
                        if (File.Exists(filePath))
                        {
                            byte[] imageBytes = File.ReadAllBytes(filePath);
                            string base64Image = Convert.ToBase64String(imageBytes);
                            return new
                            {
                                book.Id,
                                book.title,
                                book.author,
                                book.genre,
                                book.description,
                                Image = $"data:image/{Path.GetExtension(filePath).Replace(".", "")};base64,{base64Image}", // Embeds the image as Base64
                                book.averageRating,
                                book.totalReviews,
                                book.createdAt
                            };
                        }
                        else
                        {
                            return new
                            {
                                book.Id,
                                book.title,
                                book.author,
                                book.genre,
                                book.description,
                                Image = "Image not found", // Handle missing images
                                book.averageRating,
                                book.totalReviews,
                                book.createdAt
                            };
                        }
                    });

                    _logger.LogInformation("Books retrieved successfully.");
                    return Response.AsJson(bookWithImages, HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while retrieving books.");
                    return Response.AsJson(new { message = "An error occurred while retrieving books.", details = ex.Message }, HttpStatusCode.InternalServerError);
                }
            });

            // Get a book by ID
            Get("/{id:int}", parameters =>
            {
                int id = parameters.id;
                _logger.LogInformation("Request to retrieve book with ID {Id}", id);
                try
                {
                    var book = _bookService.GetBookById(id);
                    if (book == null)
                    {
                        _logger.LogWarning("Book with ID {Id} not found.", id);
                        return Response.AsJson(new { message = "Book not found." }, HttpStatusCode.NotFound);
                    }

                    string filePath = book.imagePath;
                    if (File.Exists(filePath))
                    {
                        byte[] imageBytes = File.ReadAllBytes(filePath);
                        string base64Image = Convert.ToBase64String(imageBytes);
                        var bookWithImage = new
                        {
                            book.Id,
                            book.title,
                            book.author,
                            book.genre,
                            book.description,
                            Image = $"data:image/{Path.GetExtension(filePath).Replace(".", "")};base64,{base64Image}", 
                            book.averageRating,
                            book.totalReviews,
                            book.createdAt
                        };

                        _logger.LogInformation("Book with ID {Id} retrieved successfully.", id);
                        return Response.AsJson(bookWithImage, HttpStatusCode.OK);
                    }
                    else
                    {
                        var bookWithoutImage = new
                        {

                            book.Id,
                            book.title,
                            book.author,
                            book.genre,
                            book.description,
                            Image = "Image not found", 
                            book.averageRating,
                            book.totalReviews,
                            book.createdAt
                        };

                        _logger.LogWarning("Image for book with ID {Id} not found.", id);
                        return Response.AsJson(bookWithoutImage, HttpStatusCode.OK);
                    }
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex, "Invalid request to retrieve book with ID {Id}", id);
                    return Response.AsJson(new { message = ex.Message }, HttpStatusCode.BadRequest);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while retrieving book with ID {Id}", id);
                    return Response.AsJson(new { message = "An error occurred while retrieving the book.", details = ex.Message }, HttpStatusCode.InternalServerError);
                }
            });

            // Delete a book by ID
            Delete("/{id:int}", parameters =>
            {
                int id = parameters.id;
                _logger.LogInformation("Request to delete book with ID {Id}", id);
                try
                {
                    _bookService.DeleteBook(id);
                    _logger.LogInformation("Book with ID {Id} successfully deleted.", id);
                    return HttpStatusCode.NoContent;
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex, "Invalid request to delete book with ID {Id}", id);
                    return Response.AsJson(new { message = ex.Message }, HttpStatusCode.BadRequest);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while deleting book with ID {Id}", id);
                    return Response.AsJson(new { message = "An error occurred while deleting the book.", details = ex.Message }, HttpStatusCode.InternalServerError);
                }
            });

            // Update a book by ID
            Put("/{id:int}", parameters =>
            {
                int id = parameters.id;
                _logger.LogInformation("Request to update book with ID {Id}", id);
                var updatedBook = this.Bind<Book>();

                // Model validation
                var validationErrors = ValidateBook(updatedBook);
                if (validationErrors.Count > 0)
                {
                    _logger.LogWarning("Validation error when updating book with ID {Id}: {Errors}", id, string.Join(", ", validationErrors));
                    return Response.AsJson(new { errors = validationErrors }, HttpStatusCode.BadRequest);
                }

                try
                {
                    updatedBook.Id = id;
                    _bookService.UpdateBook(updatedBook);
                    _logger.LogInformation("Book with ID {Id} successfully updated.", id);
                    return Response.AsJson(updatedBook, HttpStatusCode.OK);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogError(ex, "Conflict while updating book with ID {Id}", id);
                    return Response.AsJson(new { message = ex.Message }, HttpStatusCode.Conflict);
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex, "Invalid request to update book with ID {Id}", id);
                    return Response.AsJson(new { message = ex.Message }, HttpStatusCode.BadRequest);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while updating book with ID {Id}", id);
                    return Response.AsJson(new { message = "An error occurred while updating the book.", details = ex.Message }, HttpStatusCode.InternalServerError);
                }
            });
        }

        private static List<string> ValidateBook(Book book)
        {
            var results = new List<string>();
            var context = new ValidationContext(book);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(book, context, validationResults, true))
            {
                foreach (var validationResult in validationResults)
                {
                    results.Add(validationResult.ErrorMessage);
                }
            }

        

            return results;
        }
    }
}
