using Bongo.Models.Model.VM;
using System.ComponentModel.DataAnnotations;

namespace Bongo.Models.Model
{
    public class StudyRoomBooking : StudyRoomBookingBase
    {
        [Key]
        public int BookingId { get; set; }
        public int StudyRoomId { get; set; }
    }
}
