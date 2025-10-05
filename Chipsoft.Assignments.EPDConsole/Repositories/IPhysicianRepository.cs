using Chipsoft.Assignments.EPDConsole.model;

namespace Chipsoft.Assignments.EPDConsole.Repositories
{
    public interface IPhysicianRepository
    {
    
        void Add(Physician physician);
        void Delete(int id);
        IEnumerable<Physician> GetAll();
        Physician? GetById(int id);
        void Save();
    }
}