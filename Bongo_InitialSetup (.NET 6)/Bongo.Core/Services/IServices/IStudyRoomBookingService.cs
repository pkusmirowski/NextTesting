using Bongo.Models.Model;
using Bongo.Models.Model.VM;
using System.Collections.Generic;

namespace Bongo.Core.Services.IServices
{
    public interface IStudyRoomBookingService
    {
        StudyRoomBookingResult BookStudyRoom(StudyRoomBooking request);
        IEnumerable<StudyRoomBooking> GetAllBooking();
    }
}
