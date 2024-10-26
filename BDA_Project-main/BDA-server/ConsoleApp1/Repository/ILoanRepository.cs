using ConsoleApp1.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Repository
{
    public interface ILoanRepository
    {
        void CreateLoan(Loan loan);
        Loan GetLoanById(int id);
        IEnumerable<Loan> GetAllLoans();
        void UpdateLoan(Loan loan);
        void DeleteLoan(int id);
    }
}
