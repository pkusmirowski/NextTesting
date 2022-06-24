using Bongo.Models.Model;
using System;
using System.Collections.Generic;

namespace Bongo.DataAccess.Repository.IRepository
{
    public interface IStudyRoomBookingRepository
    {
        void Book(StudyRoomBooking booking);
        IEnumerable<StudyRoomBooking> GetAll(DateTime? dateTime);
    }
}
