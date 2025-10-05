using Chipsoft.Assignments.EPDConsole.model;

namespace Chipsoft.Assignments.EPDConsole.Services
{
    public class PatientService
    {
        private readonly PatientRepository _patientRepository;

        public PatientService(PatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public void AddPatient(string firstName, string lastName, string rijksregisterNumber, DateTime birthDay, string address, string email, string? phone)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Voornaam en achternaam mogen niet leeg zijn.");

            if (birthDay > DateTime.Today)
                throw new ArgumentException("Geboortedatum kan niet in de toekomst liggen.");

            if (rijksregisterNumber.Length != 11 || !rijksregisterNumber.All(char.IsDigit))
                throw new ArgumentException("Rijksregisternummer moet 11 cijfers bevatten.");

            var patient = new Patient
            {
                FirstName = firstName,
                LastName = lastName,
                RijksregisterNumber = rijksregisterNumber,
                BirthDay = birthDay,
                Address = address,
                Email = email,
                Phone = string.IsNullOrWhiteSpace(phone) ? null : phone
            };

            _patientRepository.Add(patient);
            _patientRepository.Save();
        }

        public IEnumerable<Patient> GetAll()
        {
            return _patientRepository.GetAll();
        }

        public Patient? GetById(int id)
        {
            return _patientRepository.GetById(id);
        }

        public void DeletePatient(int id)
        {
            var patient = _patientRepository.GetById(id);
            if (patient == null)
                throw new ArgumentException("Patiënt niet gevonden.");

            _patientRepository.Delete(id);
            _patientRepository.Save();
        }
    }
}
