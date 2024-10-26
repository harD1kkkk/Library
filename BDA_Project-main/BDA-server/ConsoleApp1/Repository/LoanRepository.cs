using ConsoleApp1.Entity;
using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ConsoleApp1.Repository
{
    public class LoanRepository : ILoanRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<LoanRepository> _logger;

        public LoanRepository(string connectionString, ILogger<LoanRepository> logger)
        {
            _connectionString = connectionString;
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
                using (IDbConnection db = new MySqlConnection(_connectionString))
                {
                    var sql = "INSERT INTO loans (user_id, book_id, loan_date, return_date, due_date, is_extended) " +
                              "VALUES (@UserId, @BookId, @LoanDate, @ReturnDate, @DueDate, @IsExtended)";
                    db.Execute(sql, loan);
                    _logger.LogInformation("Loan created successfully for User ID {UserId} and Book ID {BookId}.", loan.UserId, loan.BookId);
                }
            }
            catch (MySqlException ex)
            {
                HandleMySqlException(ex, loan);
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
                using (IDbConnection db = new MySqlConnection(_connectionString))
                {
                    var loan = db.QueryFirstOrDefault<Loan>("SELECT * FROM loans WHERE id = @Id", new { Id = id });
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
                using (IDbConnection db = new MySqlConnection(_connectionString))
                {
                    return db.Query<Loan>("SELECT * FROM loans").ToList();
                }
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
                using (IDbConnection db = new MySqlConnection(_connectionString))
                {
                    var sql = "UPDATE loans SET user_id = @UserId, book_id = @BookId, loan_date = @LoanDate, " +
                              "return_date = @ReturnDate, due_date = @DueDate, is_extended = @IsExtended WHERE id = @Id";
                    int rowsAffected = db.Execute(sql, loan);
                    if (rowsAffected > 0)
                    {
                        _logger.LogInformation("Loan with ID {LoanId} updated successfully.", loan.Id);
                    }
                    else
                    {
                        _logger.LogWarning("No loan found with ID {LoanId} to update.", loan.Id);
                    }
                }
            }
            catch (MySqlException ex)
            {
                HandleMySqlException(ex, loan);
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
                using (IDbConnection db = new MySqlConnection(_connectionString))
                {
                    var sql = "DELETE FROM loans WHERE id = @Id";
                    int rowsAffected = db.Execute(sql, new { Id = id });
                    if (rowsAffected > 0)
                    {
                        _logger.LogInformation("Loan with ID {LoanId} deleted successfully.", id);
                    }
                    else
                    {
                        _logger.LogWarning("No loan found with ID {LoanId} to delete.", id);
                    }
                }
            }
            catch (MySqlException ex)
            {
                HandleMySqlException(ex, null);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while deleting loan with ID {LoanId}.", id);
                throw new ApplicationException("An error occurred while deleting the loan.", ex);
            }
        }

        private void HandleMySqlException(MySqlException ex, Loan? loan)
        {
            // Log MySQL specific errors with proper handling
            switch (ex.Number)
            {
                case 1062: // Duplicate entry
                    _logger.LogWarning("Attempt to add loan failed: Duplicate entry. Loan: UserId {UserId}, BookId {BookId}", loan?.UserId, loan?.BookId);
                    throw new InvalidOperationException("Duplicate entry for loan.", ex);
                case 1045: // Access denied
                    _logger.LogError(ex, "Access denied while trying to add/update/delete loan. Loan: UserId {UserId}, BookId {BookId}", loan?.UserId, loan?.BookId);
                    throw new InvalidOperationException("Access denied. Please check your database credentials.", ex);
                case 1049: // Unknown database
                    _logger.LogError(ex, "The specified database does not exist when accessing loan. Loan: UserId {UserId}, BookId {BookId}", loan?.UserId, loan?.BookId);
                    throw new InvalidOperationException("The specified database does not exist.", ex);
                case 2002: // Connection error
                    _logger.LogError(ex, "Could not connect to the database server while accessing loan. Loan: UserId {UserId}, BookId {BookId}", loan?.UserId, loan?.BookId);
                    throw new InvalidOperationException("Could not connect to the database server. Please check your connection settings.", ex);
                case 1054: // Unknown column
                    _logger.LogError(ex, "One or more columns in the query statement do not exist for loan. Loan: UserId {UserId}, BookId {BookId}", loan?.UserId, loan?.BookId);
                    throw new InvalidOperationException("One or more columns in the query statement do not exist.", ex);
                case 1146: // Table doesn't exist
                    _logger.LogError(ex, "The specified table does not exist when accessing loan. Loan: UserId {UserId}, BookId {BookId}", loan?.UserId, loan?.BookId);
                    throw new InvalidOperationException("The specified table does not exist.", ex);
                case 1213: // Deadlock
                    _logger.LogWarning("A deadlock occurred while trying to access loan. Loan: UserId {UserId}, BookId {BookId}", loan?.UserId, loan?.BookId);
                    throw new InvalidOperationException("A deadlock occurred. Please try again.", ex);
                default:
                    _logger.LogError(ex, "An error occurred while accessing loan. Loan: UserId {UserId}, BookId {BookId}", loan?.UserId, loan?.BookId);
                    throw new InvalidOperationException("An error occurred while accessing the loan.", ex);
            }
        }
    }
}
