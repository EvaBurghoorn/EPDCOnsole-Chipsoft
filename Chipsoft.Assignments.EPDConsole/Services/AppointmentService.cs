using Chipsoft.Assignments.EPDConsole.model;
using Chipsoft.Assignments.EPDConsole.Repositories;

namespace Chipsoft.Assignments.EPDConsole.Services
{
    public class AppointmentService
    {
        private readonly AppointmentRepository _appointmentRepository;
        private readonly PatientRepository _patientRepository;
        private readonly PhysicianRepository _physicianRepository;

        public AppointmentService(
            AppointmentRepository appointmentRepository,
            PatientRepository patientRepository,
            PhysicianRepository physicianRepository)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _physicianRepository = physicianRepository;
        }

        public void CreateAppointment(int patientId, int physicianId, DateTime date, TimeSpan timeSlot)
        {
            var patient = _patientRepository.GetById(patientId);
            var physician = _physicianRepository.GetById(physicianId);

            if (patient == null)
                throw new ArgumentException("Patiënt niet gevonden.");

            if (physician == null)
                throw new ArgumentException("Arts niet gevonden.");

            if (date.Date <= DateTime.Today)
                throw new ArgumentException("De afspraakdatum mag niet in het verleden liggen.");

            var availableSlots = _appointmentRepository.GetAvailableTimeSlots(physicianId, date);
            if (!availableSlots.Contains(timeSlot))
                throw new ArgumentException("Het gekozen tijdslot is niet beschikbaar.");

            _appointmentRepository.CreateAppointment(patientId, physicianId, date, timeSlot);
        }

        public IEnumerable<Appointment> GetUpcomingAppointments()
        {
            return _appointmentRepository.GetUpcomingAppointments();
        }

        public List<TimeSpan> GetAvailableTimeSlots(int physicianId, DateTime date)
        {
            return _appointmentRepository.GetAvailableTimeSlots(physicianId, date);
        }
    }
}
