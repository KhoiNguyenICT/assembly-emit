using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace TestCodeDOM.Context
{
    public class MyModelCustomizer : ModelCustomizer
    {
        private readonly TypeHelper _typeHelper;

        public MyModelCustomizer(
            ModelCustomizerDependencies dependencies
            , TypeHelper typeHelper
        ) : base(dependencies)
        {
            _typeHelper = typeHelper;
        }

        public override void Customize(ModelBuilder modelBuilder, DbContext dbContext)
        {
            base.Customize(modelBuilder, dbContext);

            var types = _typeHelper.GetTypes();
            foreach (var type in types)
                modelBuilder.Entity(type);
        }
    }
}
