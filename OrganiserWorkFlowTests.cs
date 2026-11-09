using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RaceDay.Web.Controllers;
using RaceDay.Web.Models;
using RaceDay.Web.Services;
using Xunit;

namespace RaceDay.Web.Tests
{
    public class OrganiserWorkflowTests
    {
        private readonly Mock<RaceDayClientService> _mockService;
        private readonly OrganiserController _controller;

        public OrganiserWorkflowTests()
        {
            var mockHttp = new Mock<HttpClient>();
            var mockAccessor = new Mock<IHttpContextAccessor>();

            _mockService = new Mock<RaceDayClientService>(mockHttp.Object, mockAccessor.Object);
            _controller = new OrganiserController(_mockService.Object);
        }

        [Fact]
        public async Task CreateEvent_ShouldRedirectToDashboard_WhenFormSubmissionIsSuccessful()
        {
            // Arrange
            var input = new CreateEventFormWrapper { EventDetails = new EventDisplayDto { Name = "Cape Town Cycle Tour Qualifier" } };
            _mockService.Setup(s => s.DispatchNewEventAsync(It.IsAny<EventDisplayDto>(), It.IsAny<IFormFile>())).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateEvent(input);

            // Assert
            var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirect.ActionName.Should().Be("Dashboard");
        }
    }
}
