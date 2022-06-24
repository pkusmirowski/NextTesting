using Bongo.Models.Model;
using System.Collections.Generic;

namespace Bongo.Core.Services.IServices
{
    public interface IStudyRoomService
    {
        IEnumerable<StudyRoom> GetAll();
    }
}
