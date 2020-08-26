using FastMember;
using Microsoft.AspNetCore.Mvc;
using System;
using TestCodeDOM.Context;

namespace TestCodeDOM.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly TypeHelper _typeHelper;
        private readonly AppDbContext _dbContext;

        public WeatherForecastController(
              TypeHelper typeHelper
            , AppDbContext dbContext
        )
        {
            _typeHelper = typeHelper;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var type = _typeHelper.GetType("Student1");
            var accessor = TypeAccessor.Create(type);
            var obj = Activator.CreateInstance(type);

            accessor[obj, "Id"] = new Random().Next(0, 1000000000);
            accessor[obj, "Name"] = $"Test {new Random().Next(0, 1000000000)}";
            accessor[obj, "Address"] = $"Test address {new Random().Next(0, 1000000000)}";

            _dbContext.Add(obj);
            _dbContext.SaveChanges();

            return Ok(_dbContext.Set(type));
        }
    }
}
