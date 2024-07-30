using Microsoft.OpenApi.Models;
using WebApi.Services;

namespace WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            return services;
        }

        //public static void AddSwagger(this IServiceCollection services)
        //{
        //    services.AddSwaggerGen(options =>
        //    {
        //        options.SwaggerDoc("V1", new OpenApiInfo
        //        {
        //            Title = "Auth api - Role based authorization",
        //            Version = "v1",
        //            Description = "ASP.NET Core - Role Based Authorization API"
        //        });
        //    });
        //}
    }
}