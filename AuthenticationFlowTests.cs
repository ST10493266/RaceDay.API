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
    public class AuthenticationFlowTests
    {
        private readonly Mock<RaceDayClientService> _mockService;
        private readonly HomeController _controller;
        private readonly Mock<ISession> _mockSession;

        public AuthenticationFlowTests()
        {
            var mockHttp = new Mock<HttpClient>();
            var mockAccessor = new Mock<IHttpContextAccessor>();
            
            _mockService = new Mock<RaceDayClientService>(mockHttp.Object, mockAccessor.Object);
            _controller = new HomeController(_mockService.Object);

            _mockSession = new Mock<ISession>();
            var context = new DefaultHttpContext { Session = _mockSession.Object };
            _controller.ControllerContext = new ControllerContext { HttpContext = context };
        }

        [Fact]
        public async Task Login_ShouldRedirectToOrganiserDashboard_WhenCredentialsAreValidAsOrganiser()
        {
            var input = new LoginDto { Email = "admin@raceday.co.za", Password = "ValidPassword123!" };
            var output = new AuthSuccessResponseDto { Token = "MOCK_TOKEN", Role = "Organiser", Email = "admin@raceday.co.za" };

            _mockService.Setup(s => s.ExecuteLoginAsync(It.IsAny<LoginDto>())).ReturnsAsync(output);

            var result = await _controller.Login(input);

            var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirect.ActionName.Should().Be("Dashboard");
            redirect.ControllerName.Should().Be("Organiser");
        }
    }
}
