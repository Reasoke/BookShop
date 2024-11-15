
using server.Services;

namespace server {
    public class Program {

        public static void Main(string[] args) {

            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else {
                app.UseHttpsRedirection();
            }

            app.UseAuthorization();
            app.UseCors(policy => {
                policy
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowAnyOrigin()
                //.AllowCredentials()
                //.WithOrigins(allowedHosts)
                //.SetIsOriginAllowedToAllowWildcardSubdomains()
                ;
            });

            app.MapControllers();

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration) {

            // Add services to the container.
            services.AddSingleton<ISettingsProvider>(provider => new SettingsProvider(configuration));
            services.AddSingleton<Repository>(provider => new Repository(
                provider.GetService<ISettingsProvider>()
            ));

            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

        }
    }
}
