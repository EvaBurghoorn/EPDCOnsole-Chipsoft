using Chipsoft.Assignments.EPDConsole.model;

namespace Chipsoft.Assignments.EPDConsole.Services
{
    public interface IAppointmentRepository
    {
        IEnumerable<Appointment> GetUpcomingAppointments();
        void CreateAppointment(int patientId, int physicianId, DateTime appointmentDate, TimeSpan selectedTimeSlot);
        List<TimeSpan> GetAvailableTimeSlots(int physicianId, DateTime date);
    }
}
