using ConsoleApp1.Entity;
using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Data;

namespace ConsoleApp1.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(string connectionString, ILogger<UserRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public void CreateUser(User user)
        {
            if (user == null)
            {
                _logger.LogWarning("Attempted to create a null user.");
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }

            // Validate user fields
            ValidateUser(user);

            try
            {
                using (IDbConnection db = new MySqlConnection(_connectionString))
                {
                    var sql = "INSERT INTO users (name, email, password, positive_rating, negative_rating, created_at) " +
                              "VALUES (@Name, @Email, @Password, @PositiveRating, @NegativeRating, @createdAt)";
                    db.Execute(sql, user);
                    _logger.LogInformation("User '{Email}' created successfully.", user.Email);
                }
            }
            catch (MySqlException ex)
            {
                HandleMySqlException(ex, user);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogError(argEx, "Invalid argument error occurred while creating user '{Email}'.", user.Email);
                throw new InvalidOperationException("Invalid arguments provided.", argEx);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while creating user '{Email}'.", user.Email);
                throw new InvalidOperationException("An unexpected error occurred.", ex);
            }
        }

        public User? GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Attempted to retrieve a user with an empty email.");
                throw new ArgumentException("Email cannot be null or empty", nameof(email));
            }

            try
            {
                using (IDbConnection db = new MySqlConnection(_connectionString))
                {
                    var user = db.QueryFirstOrDefault<User>("SELECT * FROM users WHERE email = @Email", new { Email = email });
                    return LogUserRetrieval(user, email);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while retrieving user '{Email}'.", email);
                throw new ApplicationException("An error occurred while retrieving the user.", ex);
            }
        }

        public User? GetUserById(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to retrieve a user with invalid ID {UserId}.", id);
                throw new ArgumentException("Invalid user ID", nameof(id));
            }

            try
            {
                using (IDbConnection db = new MySqlConnection(_connectionString))
                {
                    var user = db.QueryFirstOrDefault<User>("SELECT * FROM users WHERE id = @Id", new { Id = id });
                    return LogUserRetrieval(user, id.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while retrieving user with ID {UserId}.", id);
                throw new ApplicationException("An error occurred while retrieving the user.", ex);
            }
        }

        public void UpdateUser(User user)
        {
            if (user == null)
            {
                _logger.LogWarning("Attempted to update a null user.");
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }

            ValidateUser(user);

            try
            {
                using (IDbConnection db = new MySqlConnection(_connectionString))
                {
                    var sql = "UPDATE users SET name = @Name, email = @Email, password = @Password, " +
                              "positive_rating = @PositiveRating, negative_rating = @NegativeRating " +
                              "WHERE id = @Id";

                    int rowsAffected = db.Execute(sql, user);
                    if (rowsAffected > 0)
                    {
                        _logger.LogInformation("User with ID {UserId} updated successfully.", user.Id);
                    }
                    else
                    {
                        _logger.LogWarning("No user found with ID {UserId} to update.", user.Id);
                    }
                }
            }
            catch (MySqlException ex)
            {
                HandleMySqlException(ex, user);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while updating user with ID {UserId}.", user.Id);
                throw new ApplicationException("An error occurred while updating the user.", ex);
            }
        }

        private void ValidateUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Name))
            {
                _logger.LogWarning("User operation failed: Name is required.");
                throw new ArgumentException("Name is required.", nameof(user.Name));
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                _logger.LogWarning("User operation failed: Email is required.");
                throw new ArgumentException("Email is required.", nameof(user.Email));
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                _logger.LogWarning("User operation failed: Password is required.");
                throw new ArgumentException("Password is required.", nameof(user.Password));
            }

            if (user.Password.Length < 6)
            {
                _logger.LogWarning("User operation failed: Password must be at least 6 characters long.");
                throw new ArgumentException("Password must be at least 6 characters long.", nameof(user.Password));
            }
        }

        private void HandleMySqlException(MySqlException ex, User user)
        {
            // Log the MySQL specific errors
            switch (ex.Number)
            {
                case 1062: // Duplicate entry
                    _logger.LogWarning("Attempt to create user failed: A user with this email already exists. Email: {Email}", user.Email);
                    throw new InvalidOperationException("A user with this email already exists.", ex);
                case 1045: // Access denied
                    _logger.LogError(ex, "Access denied while trying to create user '{Email}'. Please check your database credentials.", user.Email);
                    throw new InvalidOperationException("Access denied. Please check your database credentials.", ex);
                case 1049: // Unknown database
                    _logger.LogError(ex, "The specified database does not exist when creating user '{Email}'.", user.Email);
                    throw new InvalidOperationException("The specified database does not exist.", ex);
                case 2002: // Connection error
                    _logger.LogError(ex, "Could not connect to the database server while creating user '{Email}'. Please check your connection settings.", user.Email);
                    throw new InvalidOperationException("Could not connect to the database server. Please check your connection settings.", ex);
                case 1054: // Unknown column
                    _logger.LogError(ex, "One or more columns in the insert statement do not exist for user '{Email}'.", user.Email);
                    throw new InvalidOperationException("One or more columns in the insert statement do not exist.", ex);
                case 1146: // Table doesn't exist
                    _logger.LogError(ex, "The specified table does not exist while creating user '{Email}'.", user.Email);
                    throw new InvalidOperationException("The specified table does not exist.", ex);
                case 1213: // Deadlock
                    _logger.LogWarning("A deadlock occurred while trying to create user '{Email}'.", user.Email);
                    throw new InvalidOperationException("A deadlock occurred. Please try again.", ex);
                default:
                    _logger.LogError(ex, "An error occurred while creating the user '{Email}'.", user.Email);
                    throw new InvalidOperationException("An error occurred while creating the user.", ex);
            }
        }

        private User? LogUserRetrieval(User? user, string identifier)
        {
            if (user == null)
            {
                _logger.LogWarning("User with identifier '{Identifier}' not found.", identifier);
            }
            else
            {
                _logger.LogInformation("User with identifier '{Identifier}' retrieved successfully.", identifier);
            }
            return user;
        }
    }
}
