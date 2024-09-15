using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using YoYo_Web_App.Controllers;
using YoYo_Web_App.Mapper;
using YoYo_Web_App.Models;
using YoYo_Web_App.Models.Yoyo;
using YoYo_Web_App.ViewModels;

namespace YoYo_Web_App.Test
{
    public class YoyoTest
    {
        /*Below test cases covers positive scenarios */
        [Fact]
        public async Task Controller_YoyoView_ReturnsAViewResult_WithAListOfAthleteInfoAndLevelShuttle()
        {
            //Arrange
            var mockRepo = new Mock<IYoyoRepository>();
            mockRepo.Setup(repo => repo.GetViewModelAsync()).ReturnsAsync(YoyoTestMockModels.GetYoyoViewModel());
            var controller = new YoyoController(mockRepo.Object);

            // Act
            var result = await controller.YoyoView();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsAssignableFrom<YoyoViewModel>(viewResult.ViewData.Model);
        }
        [Fact]
        public async Task Controller_GetBeepTestData_ReturnsAJsonResult_BeepTestData()
        {
            //Arrange
            var mockRepo = new Mock<IYoyoRepository>();
            mockRepo.Setup(repo => repo.GetBeepTestData()).ReturnsAsync(YoyoTestMockModels.GetBeepTestModel());
            var controller = new YoyoController(mockRepo.Object);

            // Act
            var result = await controller.GetBeepTestData();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
        }
        [Fact]
        public void Controller_UpdateAthleteStatus_PassLevelShuttleDetail_ReturnsAOkObjectResult_WithUpdatedAthletesObject()
        {
            var levelShuttleDetail = new LevelShuttleDetail()
            {
                Level = "1",
                Shuttle = "1"
            };

            //Arrange
            var mockRepo = new Mock<IYoyoRepository>();
            mockRepo.Setup(repo => repo.UpdateAthleteStatus(1, ActionType.Completed, levelShuttleDetail))
                .Returns(YoyoTestMockModels.GetAthleteInfoModel());

            var controller = new YoyoController(mockRepo.Object);

            // Act
            var result = controller.UpdateAthleteStatus(1, ActionType.Completed, levelShuttleDetail);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var obj = Assert.IsAssignableFrom<AthleteInfo>(okObjectResult.Value);
        }
        [Fact]
        public void Controller_OnTestCompleted_PassListOfCompletedLevelShuttleDetails_ReturnsOkObjectResult_WithListOfUpdatedAthletes()
        {
            //Arrange
            var mockRepo = new Mock<IYoyoRepository>();
            var levelShuttleDetails = new List<LevelShuttleDetails>()
            {
                new LevelShuttleDetails()
                {
                    LevelShuttleDetail = new LevelShuttleDetail(){ Level ="1", Shuttle ="1"},
                    Id = 1
                },
                new LevelShuttleDetails()
                {
                    LevelShuttleDetail = new LevelShuttleDetail(){ Level ="2", Shuttle ="2"},
                    Id = 2
                }
            };
            mockRepo.Setup(repo => repo.OnTestCompleted(levelShuttleDetails))
                .Returns(YoyoTestMockModels.MockTestCompletedResponse());
            var controller = new YoyoController(mockRepo.Object);

            // Act
            var result = controller.OnTestCompleted(levelShuttleDetails);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var obj = Assert.IsAssignableFrom<TestCompletedResponse>(okObjectResult.Value);
        }
        [Fact]
        public async Task Repo_GetViewModelAsync_ReturnsYoyoViewModel_WithAthleteInfosLevelShuttle()
        {
            //Arrange
            var mockConfig = YoyoTestMockModels.GetAppConfig();
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile());
            });
            var mapper = mockMapper.CreateMapper();

            var repository = new YoyoRepository(mockConfig, mapper);

            // Act
            var result = await repository.GetViewModelAsync();

            // Assert
            var viewModel = Assert.IsAssignableFrom<YoyoViewModel>(result);
            Assert.True(viewModel.AthleteInfos.Count() == 7);
            Assert.True(viewModel.LevelShuttles.Count() == 91);

        }
        [Fact]
        public async Task Repo_GetBeepTestData_Returns_ListOfBeepTestData()
        {
            //Arrange
            var mockConfig = YoyoTestMockModels.GetAppConfig();

            var repository = new YoyoRepository(mockConfig, null);

            // Act
            var result = await repository.GetBeepTestData();

            // Assert
            var beepTestData = Assert.IsAssignableFrom<IEnumerable<BeepTestData>>(result);
            Assert.NotNull(beepTestData);
            Assert.True(beepTestData.Count() == 91);

        }
        [Fact]
        public async Task Repo_UpdateAthleteStatusToCompleted_PassLevelShuttleDetail_Returns_UpdatedAthleteInfo()
        {
            //Arrange
            var mockConfig = YoyoTestMockModels.GetAppConfig();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile());
            });
            var mapper = mockMapper.CreateMapper();

            var levelShuttleDetail = new LevelShuttleDetail()
            {
                Level = "1",
                Shuttle = "1"
            };
            var repository = new YoyoRepository(mockConfig, mapper);
            var yoyoViewModel = await repository.GetViewModelAsync();

            // Act
            var result = repository.UpdateAthleteStatus(1, ActionType.Completed, levelShuttleDetail);

            // Assert
            var athleteInfo = Assert.IsAssignableFrom<AthleteInfo>(result);
            Assert.NotNull(athleteInfo);
            Assert.NotNull(athleteInfo.Completed);
            Assert.True(athleteInfo.Completed.Shuttle == "1");
            Assert.True(athleteInfo.Completed.Level == "1");

        }
        [Fact]
        public async Task Repo_UpdateAthleteStatusToWarned_PassLevelShuttleDetail_Returns_UpdatedAthleteInfo()
        {
            //Arrange
            var mockConfig = YoyoTestMockModels.GetAppConfig();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile());
            });
            var mapper = mockMapper.CreateMapper();

            var levelShuttleDetail = new LevelShuttleDetail()
            {
                Level = "1",
                Shuttle = "1"
            };
            var repository = new YoyoRepository(mockConfig, mapper);
            var yoyoViewModel = await repository.GetViewModelAsync();

            // Act
            var result = repository.UpdateAthleteStatus(2, ActionType.Warned, levelShuttleDetail);

            // Assert
            var athleteInfo = Assert.IsAssignableFrom<AthleteInfo>(result);
            Assert.NotNull(athleteInfo);
            Assert.Null(athleteInfo.Completed);
            Assert.True(athleteInfo.Warned.Shuttle == "1");
            Assert.True(athleteInfo.Warned.Level == "1");

        }
        [Fact]
        public async Task Repo_OnTestCompleted_PassListOfCompletedLevelShuttleDetails_Returns_ListOfAthleteInfo()
        {
            //Arrange
            var mockConfig = YoyoTestMockModels.GetAppConfig();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile());
            });
            var mapper = mockMapper.CreateMapper();

            var repository = new YoyoRepository(mockConfig, mapper);
            var yoyoViewModel = await repository.GetViewModelAsync();

            //Act
            var result = repository.OnTestCompleted(YoyoTestMockModels.GetListOfLevelShuttleDetails());

            // Assert
            var obj = Assert.IsAssignableFrom<TestCompletedResponse>(result);
            var athleteInfos = Assert.IsAssignableFrom<IEnumerable<AthleteInfo>>(obj.AthleteInfos);
            Assert.NotNull(obj);
        }
        [Fact]
        public async Task Repo_OnTestCompleted_PassListOfCompletedLevelShuttleDetails_WhenOneAthleteStoppedByCoach_Returns_ListOfAthleteInfo()
        {
            //Arrange
            var mockConfig = YoyoTestMockModels.GetAppConfig();
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile());
            });
            var mapper = mockMapper.CreateMapper();
            var repository = new YoyoRepository(mockConfig, mapper);
            var yoyoViewModel = await repository.GetViewModelAsync();
            var levelShuttleDetail = new LevelShuttleDetail()
            {
                Level = "1",
                Shuttle = "1"
            };
            repository.UpdateAthleteStatus(1, ActionType.Completed, levelShuttleDetail);

            //Act
            var result = repository.OnTestCompleted(YoyoTestMockModels.GetListOfLevelShuttleDetails());

            // Assert
            var obj = Assert.IsAssignableFrom<TestCompletedResponse>(result);
            var athleteInfos = Assert.IsAssignableFrom<IEnumerable<AthleteInfo>>(obj.AthleteInfos).ToList();
            Assert.NotNull(athleteInfos);
            Assert.True(athleteInfos.Count() == 1);
            Assert.True(athleteInfos[0].Completed.Level == "2");
            Assert.True(athleteInfos[0].Completed.Shuttle == "2");
            Assert.True(athleteInfos[0].Id == 2);
        }
        /*Below test cases covers negative scenarios */
        [Fact]
        public async Task Controller_YoyoView_ReturnsNoTestFoundView_WhenNoBeeptestFileFound()
        {
            //Arrange
            var mockRepo = new Mock<IYoyoRepository>();
            mockRepo.Setup(repo => repo.GetViewModelAsync()).ReturnsAsync((YoyoViewModel)null);
            var controller = new YoyoController(mockRepo.Object);

            // Act
            var result = await controller.YoyoView();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("~/Views/Yoyo/NoTestFoundView.cshtml", viewResult.ViewName);
        }
        [Fact]
        public async Task Controller_GetBeepTestData_ReturnsNotFound_WhenNoBeeptestFileFound()
        {
            //Arrange
            var mockRepo = new Mock<IYoyoRepository>();
            mockRepo.Setup(repo => repo.GetBeepTestData()).ReturnsAsync((IEnumerable<BeepTestData>)null);
            var controller = new YoyoController(mockRepo.Object);

            // Act
            var result = await controller.GetBeepTestData();

            // Assert
            var objectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Beep test file might be empty or not found", objectResult.Value);
            Assert.Equal((int)HttpStatusCode.NotFound, objectResult.StatusCode);
        }
        [Fact]
        public void Controller_UpdateAthleteStatus_ReturnsNotFound_WhenPassInvalidAthleteId()
        {
            //Arrange
            var mockRepo = new Mock<IYoyoRepository>();
            var levelShuttleDetail = new LevelShuttleDetail()
            {
                Level = "1",
                Shuttle = "1"
            };
            mockRepo.Setup(repo => repo.UpdateAthleteStatus(1, ActionType.Completed, levelShuttleDetail)).Returns((AthleteInfo)null);
            var controller = new YoyoController(mockRepo.Object);

            // Act
            var result = controller.UpdateAthleteStatus(1, ActionType.Completed, levelShuttleDetail);

            // Assert
            var objectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Athlete id 1 is not found", objectResult.Value);
            Assert.Equal((int)HttpStatusCode.NotFound, objectResult.StatusCode);
        }
        [Fact]
        public void Controller_OnTestCompleted_ReturnsNotFound_WhenLevelShuttleDetailsContainsInvalidAthleteId()
        {
            //Arrange
            var mockRepo = new Mock<IYoyoRepository>();
            var levelShuttleDetails = new List<LevelShuttleDetails>()
            {
                new LevelShuttleDetails()
                {
                    LevelShuttleDetail = new LevelShuttleDetail(){ Level ="1", Shuttle ="1"},
                    Id = 1
                },
                new LevelShuttleDetails()
                {
                    LevelShuttleDetail = new LevelShuttleDetail(){ Level ="2", Shuttle ="2"},
                    Id = 2
                }
            };
            var testCompletedResponse = new TestCompletedResponse()
            {
                IsValid = false
            };
            mockRepo.Setup(repo => repo.OnTestCompleted(levelShuttleDetails))
                .Returns(testCompletedResponse);
            var controller = new YoyoController(mockRepo.Object);

            // Act
            var result = controller.OnTestCompleted(levelShuttleDetails);

            // Assert
            var objectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("param - levelShuttleDetails might be null or  one or more athlete Id is invalid", objectResult.Value);
            Assert.Equal((int)HttpStatusCode.NotFound, objectResult.StatusCode);
        }
        [Fact]
        public void Controller_OnTestCompleted_ReturnsBadRequest_WhenLevelShuttleDetailsIsNull()
        {
            //Arrange
            var controller = new YoyoController(null);

            // Act
            var result = controller.OnTestCompleted(null);

            // Assert
            var viewResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid object value - levelShuttleDetails", viewResult.Value);
            Assert.Equal((int)HttpStatusCode.BadRequest, viewResult.StatusCode);
        }
        [Fact]
        public async Task Repo_OnTestCompleted_ReturnsTestCompletedResponseResult_WithIsValidFalse_WhenLevelShuttleDetailsContainsInvalidAsync()
        {
            //Arrange
            var mockConfig = YoyoTestMockModels.GetAppConfig();

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperProfile());
            });
            var mapper = mockMapper.CreateMapper();

            var repository = new YoyoRepository(mockConfig, mapper);
            var yoyoViewModel = await repository.GetViewModelAsync();

            //Act
            var result = repository.OnTestCompleted(YoyoTestMockModels.GetListOfInvalidLevelShuttleDetails());

            // Assert
            var obj = Assert.IsAssignableFrom<TestCompletedResponse>(result);
            Assert.False(obj.IsValid);
        }
    }
}
