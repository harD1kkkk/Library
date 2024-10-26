using ConsoleApp1.Entity;
using ConsoleApp1.Repository;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleApp1.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public void Register(User user)
        {
            if (user == null)
            {
                _logger.LogWarning("Attempted to register a null user.");
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }

            try
            {
                user.Password = HashPassword(user.Password);
                user.PositiveRating = 0; // Default rating
                user.NegativeRating = 0; // Default rating
                user.createdAt = DateTime.UtcNow; // Set creation date

                _userRepository.CreateUser(user);
                _logger.LogInformation("User '{Email}' registered successfully.", user.Email);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Registration failed for user '{Email}'.", user.Email);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred during registration for user '{Email}'.", user.Email);
                throw new ApplicationException("An error occurred while registering the user.", ex);
            }
        }


        public User? Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {    
                _logger.LogWarning("Login attempt with missing email or password.");
                throw new ArgumentException("Email and password are required.");
            }

            try
            {
                var user = _userRepository.GetUserByEmail(email);
                if (user != null)
                {
                    _logger.LogInformation($"Found user with email: {email}. Verifying password...");

                    if (VerifyPassword(password, user.Password))
                    {
                        _logger.LogInformation($"User '{email}' logged in successfully.");
                        return user;
                    }
                    else
                    {
                        _logger.LogWarning($"Login failed for user '{email}': Incorrect password.");
                    }
                }
                else
                {
                    _logger.LogWarning($"Login failed: User with email '{email}' not found.");
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred during login for user '{email}'.");
                throw new ApplicationException("An error occurred while logging in.", ex);
            }
        } 


        public User GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Attempted to retrieve a user with an empty email.");
                throw new ArgumentException("Email cannot be null or empty", nameof(email));
            }

            try
            {
                var user = _userRepository.GetUserByEmail(email);
                if (user == null)
                {
                    _logger.LogWarning("User with email '{Email}' not found.", email);
                }
                else
                {
                    _logger.LogInformation("User '{Email}' retrieved successfully.", email);
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while retrieving user '{Email}'.", email);
                throw new ApplicationException("An error occurred while retrieving the user.", ex);
            }
        }

        public User GetUserById(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Attempted to retrieve a user with invalid ID {UserId}.", id);
                throw new ArgumentException("Invalid user ID", nameof(id));
            }

            try
            {
                var user = _userRepository.GetUserById(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", id);
                }
                else
                {
                    _logger.LogInformation("User with ID {UserId} retrieved successfully.", id);
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while retrieving user with ID {UserId}.", id);
                throw new ApplicationException("An error occurred while retrieving the user.", ex);
            }
        }

        private static string HashPassword(string password)
        {
            // Generate a 128-bit (16 byte) salt
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Combine salt and password bytes
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000); // 100,000 iterations
            byte[] hash = pbkdf2.GetBytes(32); // Generate a 256-bit hash

            // Combine salt and hash into one byte array
            byte[] hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);

            // Convert to Base64 for storage
            return Convert.ToBase64String(hashBytes);
        }

        private static bool VerifyPassword(string password, string storedHash)
        {
            // Extract salt and hash from stored hash
            byte[] hashBytes = Convert.FromBase64String(storedHash);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Generate hash using the stored salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(32);

            // Compare the stored hash with the newly generated hash
            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }

        public void UpdatePositiveRating(int userId, bool increment)
        {
            var user = GetUserById(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", userId);
                throw new InvalidOperationException("User does not exist.");
            }

            if (increment)
            {
                user.PositiveRating++;
            }
            else if (user.PositiveRating > 0)
            {
                user.PositiveRating--;
            }

            _userRepository.UpdateUser(user);
            _logger.LogInformation("User with ID {UserId} positive rating updated successfully.", userId);
        }

        public void UpdateNegativeRating(int userId, bool increment)
        {
            var user = GetUserById(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", userId);
                throw new InvalidOperationException("User does not exist.");
            }

            if (increment)
            {
                user.NegativeRating++;
            }
            else if (user.NegativeRating > 0)
            {
                user.NegativeRating--;
            }

            _userRepository.UpdateUser(user);
            _logger.LogInformation("User with ID {UserId} negative rating updated successfully.", userId);
        }

    }
}
