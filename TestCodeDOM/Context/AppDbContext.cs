using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace TestCodeDOM.Context
{
    public class AppDbContext : DbContext
    {
        private readonly TypeHelper _typeHelper;

        public AppDbContext(DbContextOptions options, TypeHelper typeHelper)
            : base(options)
        {
            _typeHelper = typeHelper;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var serviceCollection = new ServiceCollection()
                .AddEntityFrameworkSqlServer();


            serviceCollection = serviceCollection.AddSingleton<IModelCustomizer, MyModelCustomizer>();

            serviceCollection.AddSingleton(_typeHelper);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            optionsBuilder
                .UseInternalServiceProvider(serviceProvider);
        }
    }
}
