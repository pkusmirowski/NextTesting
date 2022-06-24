using Bongo.Models.Model;
using System.Collections.Generic;

namespace Bongo.DataAccess.Repository.IRepository
{
    public interface IStudyRoomRepository
    {
        IEnumerable<StudyRoom> GetAll();
    }
}
