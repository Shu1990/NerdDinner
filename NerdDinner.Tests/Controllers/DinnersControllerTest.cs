using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NerdDinner.Controllers;
using NerdDinner.Models;
using NerdDinner.Tests.Fakes;

namespace NerdDinner.Tests.Controllers
{
    [TestClass]
    public class DinnersControllerTest
    {
        List<Dinner> CreateTestDinners()
        {
            List<Dinner> dinners = new List<Dinner>();
            for (int i = 0; i < 101; i++)
            {
                Dinner sampleDinner = new Dinner()
                {
                    DinnerID = i,
                    Title = "Sample Dinner",
                    HostedBy = "SomeUser",
                    Address = "Some Address",
                    Country = "USA",
                    ContactPhone = "425-555-1212",
                    Description = "Some description",
                    EventDate = DateTime.Now.AddDays(i),
                    Latitude = 99,
                    Longitude = -99
                };
                dinners.Add(sampleDinner);
            }
            return dinners;
        }
        DinnersController CreateDinnersController()
        {
            var repository = new FakeDinnerRepository(CreateTestDinners());
            return new DinnersController(repository);
        }
        [TestMethod]
        public void DetailsAction_Should_Return_View_For_Dinner()
        {
            // Arrange
            var controller = CreateDinnersController();
            // Act
            var result = controller.Details(1);
            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
        [TestMethod]
        public void DetailsAction_Should_Return_NotFoundView_For_BogusDinner()
        {
            // Arrange
            var controller = CreateDinnersController();
            // Act
            var result = controller.Details(999) as ViewResult;
            // Assert
            Assert.AreEqual("NotFound", result.ViewName);
        }

        [TestMethod]
        public void EditAction_Should_Return_View_For_ValidDinner()
        {
            // Arrange
            var controller = CreateDinnersControllerAs("SomeUser");
            // Act
            var result = controller.Edit(1) as ViewResult;
            // Assert
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(DinnerFormViewModel));
        }

        [TestMethod]
        public void EditAction_Should_Return_EditView_When_ValidOwner()
        {
            // Arrange
            var controller = CreateDinnersControllerAs("SomeUser");
            // Act
            var result = controller.Edit(1) as ViewResult;
            // Assert
            Assert.IsInstanceOfType(result.ViewData.Model,
            typeof(DinnerFormViewModel));
        }
        [TestMethod]
        public void EditAction_Should_Return_InvalidOwnerView_When_InvalidOwner()
        {
            // Arrange
            var controller = CreateDinnersControllerAs("NotOwnerUser");
            // Act
            var result = controller.Edit(1) as ViewResult;
            // Assert
            Assert.AreEqual(result.ViewName, "InvalidOwner");
        }

        [TestMethod]
        public void EditAction_Should_Redirect_When_Update_Successful()
        {
            // Arrange
            var controller = CreateDinnersControllerAs("SomeUser");

            var formValues = new FormCollection() {
                                        { "Title", "Another value" },
                                        { "Description", "Another description" }
                                        };

            controller.ValueProvider = formValues.ToValueProvider();

            // Act
            var result = controller.Edit(1, formValues) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Details", result.RouteValues["Action"]);
        }

        [TestMethod]
        public void EditAction_Should_Redisplay_With_Errors_When_Update_Fails()
        {
            // Arrange
            var controller = CreateDinnersControllerAs("SomeUser");

            var formValues = new FormCollection() {{ "EventDate", "Bogus date value!!!"}};

            controller.ValueProvider = formValues.ToValueProvider();

            // Act
            var result = controller.Edit(1, formValues) as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Expected redisplay of view");
            Assert.IsTrue(result.ViewData.ModelState.Count > 0, "Expected errors");
        }


        DinnersController CreateDinnersControllerAs(string userName)
        {
            var mock = new Mock<ControllerContext>();
            mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns(userName);
            mock.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            var controller = CreateDinnersController();
            controller.ControllerContext = mock.Object;
            return controller;
        }

    }
}
