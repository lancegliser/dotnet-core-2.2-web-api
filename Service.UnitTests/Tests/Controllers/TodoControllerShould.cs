using Xunit;
using System;    
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Controllers;
using Service.UnitTests.Utilities;

namespace Service.UnitTests.Tests.Controllers
{
    public class TodoControllerShould
    {
        protected readonly ILogger<TodoController> _logger;
        protected Models.TodoContext _context;

        public TodoControllerShould()
        {
            _logger = TestingUtility.GetNullLogger<TodoController>();

            var databaseName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<Models.TodoContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

             _context = new Models.TodoContext(options);
            Seed(_context);
        }

        /**
         * Returns a stand alone database for use in each test.
         */
        protected TodoController GetController()
        {
            return new TodoController(_context, _logger);
        }

        protected void Seed(Models.TodoContext context)
        {
            context.Database.EnsureCreated();
            var items = GetSeedItems();
            context.TodoItems.AddRange(items);
            context.SaveChanges();
        }

        protected IEnumerable<Models.TodoItem> GetSeedItems()
        {
            return new[]
            {
                new Models.TodoItem { Id = 1, Name = "Item 1" },
                new Models.TodoItem { Id = 2, Name = "Item 2" },
                new Models.TodoItem { Id = 3, Name = "Item 3" },
            };
        }

        [Fact]
        public async Task Index_ReturnsAnActionResult_WithAListOfTodoItems()
        {
            // Arrange
            var controller = this.GetController();
            var seedItems = GetSeedItems();
            var firstSeeditem = seedItems.FirstOrDefault();

            // Act
            var response = await controller.GetAll();

            // Assert
            var result = Assert.IsType<ActionResult<List<Models.TodoItem>>>(response);
            // Assert.IsType<OkObjectResult>(actionResult.Result);
            var items = Assert.IsAssignableFrom<List<Models.TodoItem>>(result.Value);
            Assert.Equal(seedItems.Count(), items.Count());
            var firstItem = items.FirstOrDefault();
            Assert.Equal(firstSeeditem.Name, firstItem.Name);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetById_ReturnsAnActionResult_WithTheRequestedItem(long itemId)
        {
            // Arrange
            var controller = this.GetController();

            // Act
            var response = await controller.GetById(itemId);

            // Assert
            var result = Assert.IsType<OkObjectResult>(response.Result);
            var item = Assert.IsAssignableFrom<Models.TodoItem>(result.Value);
            Assert.Equal(itemId, item.Id);
        }

        [Fact]
        public async Task GetById_ReturnsAnActionResult_WithNotFoundResponseForUnknownItems()
        {
            // Arrange
            var controller = this.GetController();

            // Act
            var response = await controller.GetById(0);

            // Assert
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task Delete_ReturnsAnActionResult_WithTheItemNoLongerFound()
        {
            // Arrange
            var controller = this.GetController();
            var seedItem = GetSeedItems().FirstOrDefault();

            // Act
            var deleteResponse = await controller.Delete(seedItem.Id);
            var delete2Response = await controller.Delete(seedItem.Id);
            var getResponse = await controller.GetById(seedItem.Id);

            // Assert
            Assert.IsType<NoContentResult>(deleteResponse);
            Assert.IsType<NoContentResult>(delete2Response);

            Assert.IsType<NotFoundResult>(getResponse.Result);
        }

        [Fact]
        public async Task Post_ReturnsAnActionResult_WithTheCreatedItem()
        {
            // Arrange
            var controller = this.GetController();
            var seedItems = GetSeedItems();
            long id = seedItems.Count() + 1;
            var name = "Created item";
            var potentialItem = new Models.TodoItem { Id = id, Name = name };

            // Act
            var postResponse = await controller.Post(potentialItem);
            var getResponse = await controller.GetById(id);

            // Assert
            var postResult = Assert.IsType<CreatedAtRouteResult>(postResponse.Result);
            Assert.Equal(id, postResult.RouteValues.Values.FirstOrDefault());
            var postItem = Assert.IsAssignableFrom<Models.TodoItem>(postResult.Value);
            Assert.Equal(name, postItem.Name);
            var getResult = Assert.IsType<OkObjectResult>(getResponse.Result);
            var getItem = Assert.IsAssignableFrom<Models.TodoItem>(getResult.Value);
            Assert.Equal(name, getItem.Name);
        }

        [Fact]
        public async Task Put_ReturnsAnActionResult_WithTheUpdatedItem()
        {
            // Arrange
            var controller = this.GetController();
            var seedItem = GetSeedItems().ElementAt(1);
            var newName = "Updated item name";
            seedItem.Name = newName;

            // Act
            var putResult = await controller.Put(seedItem.Id, seedItem);
            var getResponse = await controller.GetById(seedItem.Id);

            // Assert
            Assert.IsType<NoContentResult>(putResult.Result);
            var getResult = Assert.IsType<OkObjectResult>(getResponse.Result);
            var getItem = Assert.IsAssignableFrom<Models.TodoItem>(getResult.Value);
            Assert.Equal(newName, getItem.Name);
        }

        [Fact]
        public async Task Put_ReturnsAnActionResult_WithErrorDetailsWhenIdsDoNotMatch()
        {
            // Arrange
            var controller = this.GetController();
            var seedItem = GetSeedItems().ElementAt(1);
            var expectedStatus = (int)HttpStatusCode.BadRequest;

            // Act
            var putResult = await controller.Put(seedItem.Id + 1, seedItem);

            // Assert
            var getResult = Assert.IsType<BadRequestObjectResult>(putResult.Result);
            var exception = Assert.IsAssignableFrom<ValidationProblemDetails>(getResult.Value);
            Assert.NotEmpty(exception.Title);
            Assert.NotEmpty(exception.Errors);
            Assert.Equal(expectedStatus, exception.Status);
        }

        [Fact]
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

    } 
}
