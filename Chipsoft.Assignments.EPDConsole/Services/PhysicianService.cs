using Chipsoft.Assignments.EPDConsole.model;
using Chipsoft.Assignments.EPDConsole.Repositories;

namespace Chipsoft.Assignments.EPDConsole.Services
{
    public class PhysicianService
    {
        private readonly PhysicianRepository _physicianRepository;

        public PhysicianService(PhysicianRepository physicianRepository)
        {
            _physicianRepository = physicianRepository;
        }

        public void AddPhysician(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Naam mag niet leeg zijn.");

            var physician = new Physician
            {
                Name = name
            };

            _physicianRepository.Add(physician);
            _physicianRepository.Save();
        }

        public IEnumerable<Physician> GetAll()
        {
            return _physicianRepository.GetAll();
        }

        public Physician? GetById(int id)
        {
            return _physicianRepository.GetById(id);
        }

        public void DeletePhysician(int id)
        {
            var physician = _physicianRepository.GetById(id);
            if (physician == null)
                throw new ArgumentException("Arts niet gevonden.");

            _physicianRepository.Delete(id);
            _physicianRepository.Save();
        }
    }
}
