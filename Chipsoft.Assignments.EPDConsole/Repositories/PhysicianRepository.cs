using Chipsoft.Assignments.EPDConsole.model;
using Microsoft.EntityFrameworkCore;

namespace Chipsoft.Assignments.EPDConsole.Repositories
{
    public class PhysicianRepository : IPhysicianRepository
    {

        private readonly EPDDbContext _context;

        public PhysicianRepository(EPDDbContext context)
        {
            _context = context;
            
        }
        public void Add(Physician physician)
        {
            _context.Physicians.Add(physician);
        }

        public void Delete(int id)
        {
            var physician = _context.Physicians.Find(id);
            if (physician != null)
            {
                _context.Physicians.Remove(physician);
            }
        }

        public IEnumerable<Physician> GetAll()
        {
            return _context.Physicians
                .Include(p => p.Appointments)
                .ToList();        }

        public Physician? GetById(int id)
        {
            return _context.Physicians
                .Include(p => p.Appointments)
                .FirstOrDefault(p => p.PhysicianId == id);        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}