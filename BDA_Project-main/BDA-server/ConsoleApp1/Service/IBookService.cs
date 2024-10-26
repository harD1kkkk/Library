using ConsoleApp1.Entity;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Service
{
    public interface IBookService
    {
        void AddBook(Book book, HttpFile file);
     
        Book GetBookById(int id);
        IEnumerable<Book> GetAllBooks();
        void DeleteBook(int id);
        void UpdateBook(Book book);
    }
}
