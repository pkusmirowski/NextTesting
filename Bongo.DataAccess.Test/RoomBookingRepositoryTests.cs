using Bongo.DataAccess.Repository;
using Bongo.Models.Model;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections;

namespace Bongo.DataAccess
{
    [TestFixture]
    public class RoomBookingRepositoryTests
    {
        private readonly StudyRoomBooking strudyRoomBoking_One;
        private readonly StudyRoomBooking strudyRoomBoking_Two;
        private DbContextOptions<ApplicationDbContext> options;
        public RoomBookingRepositoryTests()
        {
            strudyRoomBoking_One = new StudyRoomBooking()
            {
                FirstName = "Ben1",
                LastName = "Spark1",
                Date = new DateTime(2023, 1, 1),
                Email = "ben1@gmail.com",
                BookingId = 11,
                StudyRoomId = 1
            };

            strudyRoomBoking_Two = new StudyRoomBooking()
            {
                FirstName = "Ben2",
                LastName = "Spark2",
                Date = new DateTime(2023, 2, 2),
                Email = "ben2@gmail.com",
                BookingId = 22,
                StudyRoomId = 2
            };
        }

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "temp_Bongo").Options;
        }

        [Test]
        [Order(1)]
        public void SaveBooking_Booking_One_CheckTheValuesFromDatabase()
        {
            //act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new StudyRoomBookingRepository(context);
                repository.Book(strudyRoomBoking_One);
            }

            //assert
            using (var context = new ApplicationDbContext(options))
            {
                var bookingFromDb = context.StudyRoomBookings.FirstOrDefault(u => u.BookingId == 11);
                if (bookingFromDb != null)
                {
                    Assert.AreEqual(strudyRoomBoking_One.FirstName, bookingFromDb.FirstName);
                    Assert.AreEqual(strudyRoomBoking_One.LastName, bookingFromDb.LastName);
                    Assert.AreEqual(strudyRoomBoking_One.Date, bookingFromDb.Date);
                    Assert.AreEqual(strudyRoomBoking_One.Email, bookingFromDb.Email);
                    Assert.AreEqual(strudyRoomBoking_One.BookingId, bookingFromDb.BookingId);
                    Assert.AreEqual(strudyRoomBoking_One.StudyRoomId, bookingFromDb.StudyRoomId);
                }
            }
        }

        [Test]
        [Order(2)]
        public void SaveBooking_Booking_OneAndTwo_CheckBothFromDatabase()
        {
            //arrange
            var expectedResult = new List<StudyRoomBooking> { strudyRoomBoking_One, strudyRoomBoking_Two };

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureDeleted();
                var repository = new StudyRoomBookingRepository(context);
                repository.Book(strudyRoomBoking_One);
                repository.Book(strudyRoomBoking_Two);
            }
            //act
            List<StudyRoomBooking> actList;
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new StudyRoomBookingRepository(context);
                actList = repository.GetAll(null).ToList();
            }

            //assert
            CollectionAssert.AreEqual(expectedResult, actList, new BookingCompare());
        }

        private class BookingCompare : IComparer
        {
            public int Compare(object x, object y)
            {
                var booking1 = (StudyRoomBooking)x;
                var booking2 = (StudyRoomBooking)y;

                if (booking1.BookingId != booking2.BookingId)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
