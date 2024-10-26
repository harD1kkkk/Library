using System;
using System.Security.Cryptography.X509Certificates;
using ConsoleApp1.Modules;
using ConsoleApp1.Repository;
using ConsoleApp1.Service;
using Microsoft.Extensions.Logging;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;
using Nancy.TinyIoc;

namespace ConsoleApp1
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        // Dependency Injection and CORS setup
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            // MySQL connection string
            var connectionString = "Server=localhost;Database=elibrary;User ID=root;Password=pass;Port=3306;";

            // Create a logger factory
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            // Register repositories with their respective loggers
            container.Register<IPostRepository>(new PostRepository(connectionString, loggerFactory.CreateLogger<PostRepository>()));
            container.Register<IUserRepository>(new UserRepository(connectionString, loggerFactory.CreateLogger<UserRepository>()));
            container.Register<IBookRepository>(new BookRepository(connectionString, loggerFactory.CreateLogger<BookRepository>()));
            container.Register<ILoanRepository>(new LoanRepository(connectionString, loggerFactory.CreateLogger<LoanRepository>()));
            container.Register<IReviewRepository>(new ReviewRepository(connectionString, loggerFactory.CreateLogger<ReviewRepository>()));

            // Register services with their respective loggers
            container.Register<IPostService>(new PostService(
                container.Resolve<IPostRepository>(),
                container.Resolve<IUserRepository>(),
                loggerFactory.CreateLogger<PostService>()
            ));

            container.Register<IUserService>(new UserService(
                container.Resolve<IUserRepository>(),
                loggerFactory.CreateLogger<UserService>()
            ));

            container.Register<IBookService>(new BookService(
                container.Resolve<IBookRepository>(),
                container.Resolve<IUserRepository>(),
                loggerFactory.CreateLogger<BookService>()
            ));

            container.Register<ILoanService>(new LoanService(
                container.Resolve<ILoanRepository>(),
                loggerFactory.CreateLogger<LoanService>()
            ));

            container.Register<IReviewService>(new ReviewService(
                container.Resolve<IReviewRepository>(),
                loggerFactory.CreateLogger<ReviewService>()
            ));

            // Register module loggers
            container.Register<ILogger<BookModule>>(loggerFactory.CreateLogger<BookModule>());
            container.Register<ILogger<LoanModule>>(loggerFactory.CreateLogger<LoanModule>());
            container.Register<ILogger<PostModule>>(loggerFactory.CreateLogger<PostModule>());
            container.Register<ILogger<ReviewModule>>(loggerFactory.CreateLogger<ReviewModule>());
            container.Register<ILogger<UserModule>>(loggerFactory.CreateLogger<UserModule>());
        }

        // CORS Configuration
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            // Add CORS headers to the response
            pipelines.AfterRequest += (ctx) =>
            {
                ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
                            .WithHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS")
                            .WithHeader("Access-Control-Allow-Headers", "Accept, Content-Type, Origin, X-Requested-With");
            };

            // Handle CORS preflight (OPTIONS) requests
            pipelines.BeforeRequest += (ctx) =>
            {
                if (ctx.Request.Method == "OPTIONS")
                {
                    ctx.Response = new Response { StatusCode = HttpStatusCode.OK };
                    return ctx.Response;
                }
                return null;
            };
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            // Start Nancy application
            var hostConfig = new HostConfiguration
            {
                RewriteLocalhost = true,
                // No SSL configuration here
            };

            using (var host = new NancyHost(hostConfig, new Uri("http://127.0.0.1:5000"))) 
            {
                host.Start();
                while (true)
                {
                    string input = "";
                    input = Console.ReadLine();

                    if (input == "exit")
                    {
                        host.Stop();
                        Environment.Exit(0);  
                    }
                    else if (input == "restart")
                    {
                        host.Stop();
                        Environment.Exit(66);  
                    }
                }
            }
        }
    }
}
