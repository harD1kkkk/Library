using ConsoleApp1.Entity;
using ConsoleApp1.Service;
using Microsoft.Extensions.Logging;
using Nancy;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConsoleApp1.Modules
{
    public class LoanModule : NancyModule
    {
        private readonly ILoanService _loanService;
        private readonly ILogger<LoanModule> _logger;

        public LoanModule(ILoanService loanService, ILogger<LoanModule> logger) : base("/api/loans")
        {
            _loanService = loanService;
            _logger = logger;

            // Create a loan
            Post("/addLoan", parameters =>
            {
                _logger.LogInformation("Request to create a new loan.");
                var loan = this.Bind<Loan>();

                // Model validation
                var validationErrors = ValidateLoan(loan);
                if (validationErrors.Count > 0)
                {
                    _logger.LogWarning("Validation error when creating loan: {Errors}", string.Join(", ", validationErrors));
                    return Response.AsJson(new { errors = validationErrors }, HttpStatusCode.BadRequest);
                }

                try
                {
                    _loanService.CreateLoan(loan);
                    _logger.LogInformation("Loan successfully created.");
                    return Response.AsJson(loan, HttpStatusCode.Created);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogError(ex, "Conflict while creating loan.");
                    return Response.AsJson(new { message = ex.Message }, HttpStatusCode.Conflict);
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex, "Invalid data when creating loan.");
                    return Response.AsJson(new { message = ex.Message }, HttpStatusCode.BadRequest);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while creating loan.");
                    return Response.AsJson(new { message = "An error occurred while creating the loan.", details = ex.Message }, HttpStatusCode.InternalServerError);
                }
            });

            // Get all loans
            Get("/allLoans", parameters =>
            {
                _logger.LogInformation("Request to retrieve all loans.");
                try
                {
                    var loans = _loanService.GetAllLoans();
                    _logger.LogInformation("Loans retrieved successfully.");
                    return Response.AsJson(loans, HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while retrieving loans.");
                    return Response.AsJson(new { message = "An error occurred while retrieving loans.", details = ex.Message }, HttpStatusCode.InternalServerError);
                }
            });

            // Get a loan by ID
            Get("/{id:int}", parameters =>
            {
                int id = parameters.id;
                _logger.LogInformation("Request to retrieve loan with ID {Id}", id);
                try
                {
                    var loan = _loanService.GetLoanById(id);
                    if (loan == null)
                    {
                        _logger.LogWarning("Loan with ID {Id} not found.", id);
                        return Response.AsJson(new { message = "Loan not found." }, HttpStatusCode.NotFound);
                    }
                    _logger.LogInformation("Loan with ID {Id} retrieved successfully.", id);
                    return Response.AsJson(loan, HttpStatusCode.OK);
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex, "Invalid request to retrieve loan with ID {Id}", id);
                    return Response.AsJson(new { message = ex.Message }, HttpStatusCode.BadRequest);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while retrieving loan with ID {Id}", id);
                    return Response.AsJson(new { message = "An error occurred while retrieving the loan.", details = ex.Message }, HttpStatusCode.InternalServerError);
                }
            });

            // Delete a loan by ID
            Delete("/{id:int}", parameters =>
            {
                int id = parameters.id;
                _logger.LogInformation("Request to delete loan with ID {Id}", id);
                try
                {
                    _loanService.DeleteLoan(id);
                    _logger.LogInformation("Loan with ID {Id} successfully deleted.", id);
                    return HttpStatusCode.NoContent;
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex, "Invalid request to delete loan with ID {Id}", id);
                    return Response.AsJson(new { message = ex.Message }, HttpStatusCode.BadRequest);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while deleting loan with ID {Id}", id);
                    return Response.AsJson(new { message = "An error occurred while deleting the loan.", details = ex.Message }, HttpStatusCode.InternalServerError);
                }
            });

            // Update a loan by ID
            Put("/{id:int}", parameters =>
            {
                int id = parameters.id;
                _logger.LogInformation("Request to update loan with ID {Id}", id);
                var updatedLoan = this.Bind<Loan>();

                // Model validation
                var validationErrors = ValidateLoan(updatedLoan);
                if (validationErrors.Count > 0)
                {
                    _logger.LogWarning("Validation error when updating loan with ID {Id}: {Errors}", id, string.Join(", ", validationErrors));
                    return Response.AsJson(new { errors = validationErrors }, HttpStatusCode.BadRequest);
                }

                try
                {
                    updatedLoan.Id = id;
                    _loanService.UpdateLoan(updatedLoan);
                    _logger.LogInformation("Loan with ID {Id} successfully updated.", id);
                    return Response.AsJson(updatedLoan, HttpStatusCode.OK);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogError(ex, "Conflict while updating loan with ID {Id}", id);
                    return Response.AsJson(new { message = ex.Message }, HttpStatusCode.Conflict);
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex, "Invalid request to update loan with ID {Id}", id);
                    return Response.AsJson(new { message = ex.Message }, HttpStatusCode.BadRequest);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while updating loan with ID {Id}", id);
                    return Response.AsJson(new { message = "An error occurred while updating the loan.", details = ex.Message }, HttpStatusCode.InternalServerError);
                }
            });
        }

        private static List<string> ValidateLoan(Loan loan)
        {
            var results = new List<string>();
            var context = new ValidationContext(loan);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(loan, context, validationResults, true))
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
