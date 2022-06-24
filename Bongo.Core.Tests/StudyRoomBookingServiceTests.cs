using Bongo.Core.Services;
using Bongo.DataAccess.Repository.IRepository;
using Bongo.Models.Model;
using Bongo.Models.Model.VM;
using Moq;
using NUnit.Framework;

namespace Bongo.Core
{
    [TestFixture]
    public class StudyRoomBookingServiceTests
    {
        private StudyRoomBooking _request = null!;
        private List<StudyRoom> _availableStudyRoom = null!;
        private Mock<IStudyRoomBookingRepository> _studyRoomBookingRepoMock = null!;
        private Mock<IStudyRoomRepository> _studyRoomRepoMock = null!;
        private StudyRoomBookingService _bookingService = null!;

        [SetUp]
        public void SetUp()
        {
            _request = new StudyRoomBooking
            {
                FirstName = "Ben",
                LastName = "Spark",
                Email = "john@gmail.com",
                Date = new DateTime(2022, 1, 3)
            };

            _availableStudyRoom = new List<StudyRoom>()
            {
                new StudyRoom
                {
                    Id = 10,
                    RoomName = "Michigan",
                    RoomNumber = "A202"
                }
            };

            _studyRoomBookingRepoMock = new Mock<IStudyRoomBookingRepository>();
            _studyRoomRepoMock = new Mock<IStudyRoomRepository>();
            _studyRoomRepoMock.Setup(r => r.GetAll()).Returns(_availableStudyRoom);
            _bookingService = new StudyRoomBookingService(_studyRoomBookingRepoMock.Object, _studyRoomRepoMock.Object);
        }

        [TestCase]
        public void GetAllBooking_InvokeMethod_CheckIfRepoIsCalled()
        {
            _bookingService.GetAllBooking();
            _studyRoomBookingRepoMock.Verify(x => x.GetAll(null), Times.Once);
        }

        [TestCase]
        public void BookingException_NullRequest_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => _bookingService.BookStudyRoom(null));

            if (exception != null)
            {
                Assert.AreEqual("Value cannot be null. (Parameter 'request')", exception.Message);
            }
        }

        [Test]
        public void StudyRoomBooking_SaveBookingWithAvailableRoom_RetrunsResultWithAllValues()
        {
            StudyRoomBooking? savedStudyRoomBooking = null;
            _studyRoomBookingRepoMock.Setup(x => x.Book(It.IsAny<StudyRoomBooking>())).Callback<StudyRoomBooking>(booking => savedStudyRoomBooking = booking);

            //act
            _bookingService.BookStudyRoom(_request);

            //assert
            _studyRoomBookingRepoMock.Verify(x => x.Book(It.IsAny<StudyRoomBooking>()), Times.Once);
            Assert.IsNotNull(savedStudyRoomBooking);
            if (savedStudyRoomBooking != null)
            {
                Assert.AreEqual(_request.FirstName, savedStudyRoomBooking.FirstName);
                Assert.AreEqual(_request.LastName, savedStudyRoomBooking.LastName);
                Assert.AreEqual(_request.Email, savedStudyRoomBooking.Email);
                Assert.AreEqual(_request.Date, savedStudyRoomBooking.Date);
                Assert.AreEqual(_availableStudyRoom[0].Id, savedStudyRoomBooking.StudyRoomId);
            }
        }

        [Test]
        public void StudyRoomBookingResultCheck_InputRequest_ValuesMatchInResult()
        {
            StudyRoomBookingResult result = _bookingService.BookStudyRoom(_request);
            Assert.NotNull(result);
            Assert.AreEqual(_request.FirstName, result.FirstName);
            Assert.AreEqual(_request.LastName, result.LastName);
            Assert.AreEqual(_request.Email, result.Email);
            Assert.AreEqual(_request.Date, result.Date);
        }

        [TestCase(true, ExpectedResult = StudyRoomBookingCode.Success)]
        [TestCase(false, ExpectedResult = StudyRoomBookingCode.NoRoomAvailable)]
        public StudyRoomBookingCode ResultCodeSuccess_RoomAvability_ReturnSuccessResultCode(bool roomAvailibility)
        {
            if (!roomAvailibility)
            {
                _availableStudyRoom.Clear();
            }
            return _bookingService.BookStudyRoom(_request).Code;
        }

        [TestCase(0, false)]
        [TestCase(50, true)]
        public void StudyRoomBooking_BookRoomWithAvailability_RetrunsBookingId(int expected, bool roomAvaibility)
        {
            if (!roomAvaibility)
            {
                _availableStudyRoom.Clear();
            }

            _studyRoomBookingRepoMock.Setup(x => x.Book(It.IsAny<StudyRoomBooking>())).Callback<StudyRoomBooking>(booking => booking.BookingId = 50);
            var result = _bookingService.BookStudyRoom(_request);
            Assert.AreEqual(expected, result.BookingId);
        }

        [Test]
        public void BookNotInvoked()
        {
            _availableStudyRoom.Clear();
            var result = _bookingService.BookStudyRoom(_request);
            _studyRoomBookingRepoMock.Verify(x => x.Book(It.IsAny<StudyRoomBooking>()), Times.Never);
        }
    }
}
