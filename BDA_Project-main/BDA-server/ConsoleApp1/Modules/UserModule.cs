using ConsoleApp1.Entity;
using ConsoleApp1.Service;
using Microsoft.Extensions.Logging;
using Nancy;
using Nancy.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace ConsoleApp1.Modules
{
    public class UserModule : NancyModule
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserModule> _logger;

        public UserModule(IUserService userService, ILogger<UserModule> logger) : base("/api/users")
        {
            _userService = userService;
            _logger = logger;

            Get("/", _ =>
            {
                return "Welcome to the API! Please use /api/users/register to register.";
            });

            // Registration
            Post("/register", parameters =>
            {
                _logger.LogInformation("Attempting to register a new user.");
                var user = this.Bind<User>();

                // Model validation
                var validationResults = ValidateUser(user);
                if (validationResults.Count > 0)
                {
                    _logger.LogWarning("Validation errors during user registration: {Errors}", string.Join(", ", validationResults));
                    return Response.AsJson(new { errors = validationResults }, HttpStatusCode.BadRequest);
                }

                // Check if user already exists
                if (_userService.GetUserByEmail(user.Email) != null)
                {
                    _logger.LogWarning("Registration attempt failed: User with email {Email} already exists.", user.Email);
                    return Response.AsJson(new { message = "A user with this email already exists." }, HttpStatusCode.Conflict);
                }

                try
                {
                    _userService.Register(user);
                    _logger.LogInformation("User {Email} successfully registered.", user.Email);
                    return Response.AsJson(new { message = "User registered successfully." }, HttpStatusCode.OK);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogError(ex, "Conflict occurred while registering user {Email}.", user.Email);
                    return Response.AsJson(new { message = ex.Message }, HttpStatusCode.Conflict);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while registering user {Email}.", user.Email);
                    return Response.AsJson(new { message = "An error occurred while processing your request.", details = ex.Message }, HttpStatusCode.InternalServerError);
                }
            });

            // Login
            Post("/login", parameters =>
            {
                _logger.LogInformation("Attempting to log in user.");

                // Bind the incoming request data to the User object
                User loginData = this.Bind<User>();
                _logger.LogWarning($"Login attempt for email: {loginData.Email}");
                _logger.LogWarning(loginData.Email, loginData.Password);
                try
                {
                    // Attempt to log in the user using the provided email and password
                    User user = _userService.Login(loginData.Email, loginData.Password);

                    // If user is found, return the user details in the response with status 200 OK
                    if (user != null)
                    {
                        _logger.LogInformation($"User {user.Email} successfully logged in.");
                        return Response.AsJson(new
                        {
                            user.Id,
                            user.Name,
                            user.Email,
                            user.PositiveRating,
                            user.NegativeRating,
                            user.createdAt
                        }, HttpStatusCode.OK);
                    }

                    // If login fails (incorrect email or password), return 401 Unauthorized
                    _logger.LogWarning("Login attempt failed: Invalid credentials for {Email}.", loginData.Email);
                    return Response.AsJson(new { message = "Invalid email or password." }, HttpStatusCode.Unauthorized);
                }
                catch (Exception ex)
                {
                    // Log the error and return a 500 Internal Server Error response if an exception occurs
                    _logger.LogError(ex, "Error occurred while logging in user {Email}.", loginData.Email);
                    return Response.AsJson(new { message = "An error occurred while processing your request.", details = ex.Message }, HttpStatusCode.InternalServerError);
                }
            });

        }

        private List<string> ValidateUser(User user)
        {
            List<string> results = new List<string>();
            var context = new ValidationContext(user);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(user, context, validationResults, true))
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
