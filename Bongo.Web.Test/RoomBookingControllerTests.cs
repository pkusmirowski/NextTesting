using Bongo.Core.Services.IServices;
using Bongo.Models.Model;
using Bongo.Models.Model.VM;
using Bongo.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Bongo.Web
{
    [TestFixture]
    public class RoomBookingControllerTests
    {
        private Mock<IStudyRoomBookingService> _studyRoomBookingService = null!;
        private RoomBookingController _bookingController = null!;

        [SetUp]
        public void SetUp()
        {
            _studyRoomBookingService = new Mock<IStudyRoomBookingService>();
            _bookingController = new RoomBookingController(_studyRoomBookingService.Object);
        }

        [Test]
        public void IndexPage_CallRequest_VerifyGetAllInvoked()
        {
            _bookingController.Index();
            _studyRoomBookingService.Verify(x => x.GetAllBooking(), Times.Once());
        }

        [Test]
        public void BookRoomCheck_ModelStateInvalid_ReturnView()
        {
            _bookingController.ModelState.AddModelError("test", "test");

            var result = _bookingController.Book(new StudyRoomBooking());
            ViewResult viewResult = (ViewResult)result;
            Assert.AreEqual("Book", viewResult.ViewName);
        }

        [Test]
        public void BookRoomCheck_NotSuccessful_NoRoomCode()
        {
            _studyRoomBookingService.Setup(x => x.BookStudyRoom(It.IsAny<StudyRoomBooking>())).Returns(new StudyRoomBookingResult()
            {
                Code = StudyRoomBookingCode.NoRoomAvailable
            });

            var result = _bookingController.Book(new StudyRoomBooking());
            Assert.IsInstanceOf<ViewResult>(result);
            ViewResult viewResult = (ViewResult)result;
            Assert.AreEqual("No Study Room available for selected date", viewResult.ViewData["Error"]);
        }

        [Test]
        public void BookRoomCheck_Successful_SuccessCodeAndRedirect()
        {
            //arrange
            _studyRoomBookingService.Setup(x => x.BookStudyRoom(It.IsAny<StudyRoomBooking>())).Returns((StudyRoomBooking booking) => new StudyRoomBookingResult
            {
                Code = StudyRoomBookingCode.Success,
                FirstName = booking.FirstName,
                LastName = booking.LastName,
                Date = booking.Date,
                Email = booking.Email
            });

            //act
            var result = _bookingController.Book(new StudyRoomBooking()
            {
                Date = DateTime.Now,
                Email = "test@dotnet.pl",
                FirstName = "Hello",
                StudyRoomId = 1
            });

            //assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            RedirectToActionResult actionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Hello", actionResult.RouteValues["FirstName"]);
            Assert.AreEqual(StudyRoomBookingCode.Success, actionResult.RouteValues["Code"]);
        }
    }
}
