using ConsoleApp1.Entity;
using ConsoleApp1.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ConsoleApp1.Service
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly ILogger<LoanService> _logger;

        public LoanService(ILoanRepository loanRepository, ILogger<LoanService> logger)
        {
            _loanRepository = loanRepository;
            _logger = logger;
        }

        public void CreateLoan(Loan loan)
        {
            if (loan == null)
            {
                _logger.LogWarning("Attempted to create a null loan.");
                throw new ArgumentNullException(nameof(loan), "Loan cannot be null");
            }

            try
            {
                _loanRepository.CreateLoan(loan);
                _logger.LogInformation("Loan created successfully for User ID {UserId} and Book ID {BookId}.", loan.UserId, loan.BookId);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while creating a loan for User ID {UserId}.", loan.UserId);
                throw new ApplicationException("An error occurred while creating the loan.", ex);
            }
        }

        public Loan GetLoanById(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to retrieve a loan with invalid ID {LoanId}.", id);
                throw new ArgumentException("Invalid loan ID", nameof(id));
            }

            try
            {
                var loan = _loanRepository.GetLoanById(id);
                if (loan == null)
                {
                    _logger.LogWarning("Loan with ID {LoanId} not found.", id);
                }
                else
                {
                    _logger.LogInformation("Loan with ID {LoanId} retrieved successfully.", id);
                }
                return loan;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while retrieving loan with ID {LoanId}.", id);
                throw new ApplicationException("An error occurred while retrieving the loan.", ex);
            }
        }

        public IEnumerable<Loan> GetAllLoans()
        {
            try
            {
                var loans = _loanRepository.GetAllLoans();
                _logger.LogInformation("Retrieved all loans successfully.");
                return loans;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while retrieving all loans.");
                throw new ApplicationException("An error occurred while retrieving all loans.", ex);
            }
        }

        public void UpdateLoan(Loan loan)
        {
            if (loan == null)
            {
                _logger.LogWarning("Attempted to update a null loan.");
                throw new ArgumentNullException(nameof(loan), "Loan cannot be null");
            }

            try
            {
                _loanRepository.UpdateLoan(loan);
                _logger.LogInformation("Loan with ID {LoanId} updated successfully.", loan.Id);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while updating loan with ID {LoanId}.", loan.Id);
                throw new ApplicationException("An error occurred while updating the loan.", ex);
            }
        }

        public void DeleteLoan(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to delete a loan with invalid ID {LoanId}.", id);
                throw new ArgumentException("Invalid loan ID", nameof(id));
            }

            try
            {
                _loanRepository.DeleteLoan(id);
                _logger.LogInformation("Loan with ID {LoanId} deleted successfully.", id);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while deleting loan with ID {LoanId}.", id);
                throw new ApplicationException("An error occurred while deleting the loan.", ex);
            }
        }
    }
}
