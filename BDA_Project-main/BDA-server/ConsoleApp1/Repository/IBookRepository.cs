using ConsoleApp1.Entity;

namespace ConsoleApp1.Repository
{
    public interface IBookRepository
    {
        void AddBook(Book book);
        Book GetBookById(int id);
        IEnumerable<Book> GetAllBooks();
        void DeleteBook(int id);
        void UpdateBook(Book book);
    }
}
