using ConsoleApp1.Entity;
using System.Collections.Generic;

namespace ConsoleApp1.Service
{
    public interface ILoanService
    {
        void CreateLoan(Loan loan);
        Loan GetLoanById(int id);
        IEnumerable<Loan> GetAllLoans();
        void UpdateLoan(Loan loan);
        void DeleteLoan(int id);
    }
}
