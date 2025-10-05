using Chipsoft.Assignments.EPDConsole.model;
using Chipsoft.Assignments.EPDConsole.Services;
using Microsoft.EntityFrameworkCore;

namespace Chipsoft.Assignments.EPDConsole.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly EPDDbContext _context;

        public AppointmentRepository(EPDDbContext context)
        {
            _context = context;
        }

        public void CreateAppointment(int patientId, int physicianId, DateTime appointmentDate, TimeSpan selectedTimeSlot)
        {
            var appointmentDateTime = appointmentDate.Date + selectedTimeSlot;

            var appointment = new Appointment
            {
                PatientId = patientId,
                PhysicianId = physicianId,
                Date = appointmentDateTime
            };

            _context.Appointments.Add(appointment);
            _context.SaveChanges();
        }

        public List<TimeSpan> GetAvailableTimeSlots(int physicianId, DateTime date)
        {
            var possibleSlots = new List<TimeSpan>
            {
                new(9, 0, 0),
                new(9, 20, 0),
                new(9, 40, 0),
                new(10, 0, 0),
                new(10, 20, 0),
                new(10, 40, 0),
                new(11, 0, 0)
            };

            var bookedSlots = _context.Appointments
                .Where(a => a.PhysicianId == physicianId && a.Date.Date == date.Date)
                .Select(a => a.Date.TimeOfDay)
                .ToList();

            var freeSlots = possibleSlots.Except(bookedSlots).ToList();
            return freeSlots;
        }

        public IEnumerable<Appointment> GetUpcomingAppointments()
        {
            return _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Physician)
                .Where(a => a.Date >= DateTime.Now)
                .OrderBy(a => a.Date)
                .ToList();
        }
    }
}
