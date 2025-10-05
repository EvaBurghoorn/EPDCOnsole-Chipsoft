using Chipsoft.Assignments.EPDConsole.model;
using Chipsoft.Assignments.EPDConsole.Repositories;
using Chipsoft.Assignments.EPDConsole.Services;
using Microsoft.EntityFrameworkCore;

namespace Chipsoft.Assignments.EPDConsole
{
    public class Program
    {
        private static readonly EPDDbContext _db = new() ;
        private static readonly PatientService _patientService = new(new PatientRepository(_db));
        private static readonly PhysicianService _physicianService = new(new PhysicianRepository(_db));
        private static readonly AppointmentService _appointmentService = new(
            new AppointmentRepository(_db),
            new PatientRepository(_db),
            new PhysicianRepository(_db));

        static void Main(string[] args)
        {

            while (ShowMenu()) { }

            _db.Dispose();
        }


        private static string? ReadInput(string prompt, bool optional = false)
        {
            while (true)
            {
                Console.Write(prompt);
                var key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("\nActie geannuleerd. Terug naar menu.");
                    Console.ReadKey();
                    return null;
                }

                Console.Write(key.KeyChar);
                string input = key.KeyChar + Console.ReadLine()!;

                if (!string.IsNullOrWhiteSpace(input) || optional)
                    return input;

                Console.WriteLine("Ongeldige invoer. Probeer opnieuw.");
            }
        }

        private static void AddPatient()
        {
            Console.WriteLine("Patient creëren (druk op ESC om te annuleren)");

            var firstName = ReadInput("Voornaam: ");
            if (firstName == null) return;

            var lastName = ReadInput("Achternaam: ");
            if (lastName == null) return;

            DateTime birthDay;
            while (true)
            {
                var input = ReadInput("Geboortedatum (dd-mm-jjjj): ");
                if (input == null) return;

                if (DateTime.TryParseExact(input, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out birthDay))
                {
                    if (birthDay.Date > DateTime.Today)
                    {
                        Console.WriteLine("De geboortedatum kan niet in de toekomst liggen. Probeer opnieuw.");
                        continue;
                    }
                    break;
                }
                Console.WriteLine("Ongeldige datum. Gebruik dd-mm-jjjj. Probeer opnieuw.");
            }

            string rijksregisterNumber;
            while (true)
            {
                var input = ReadInput("Rijksregisternummer (11 cijfers, bv. 85010112345): ");
                if (input == null) return;

                rijksregisterNumber = input.Replace("-", "").Replace(".", "");
                if (rijksregisterNumber.Length == 11 && rijksregisterNumber.All(char.IsDigit))
                    break;

                Console.WriteLine("Ongeldig rijksregisternummer. Probeer opnieuw.");
            }

            var address = ReadInput("Adres: ");
            if (address == null) return;

            string email;
            while (true)
            {
                var input = ReadInput("Email: ");
                if (input == null) return; 
                try
                {
                    var addr = new System.Net.Mail.MailAddress(input);
                    if (addr.Address == input)
                    {
                        email = input;
                        break;
                    }
                }
                catch
                {
                    Console.WriteLine("Ongeldige e-mail. Probeer opnieuw.");
                }
            }

            var phone = ReadInput("Telefoon (optioneel): ", optional: true);

            _patientService.AddPatient(firstName, lastName, rijksregisterNumber, birthDay, address, email, phone);

            Console.WriteLine("Patient toegevoegd! Druk op een toets om naar het menu te gaan");
            Console.ReadKey();
        }

        private static void DeletePatient()
        {
            var patients = _patientService.GetAll().ToList();
            if (patients.Count == 0)
            {
                Console.WriteLine("Geen patiënten gevonden.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Patiënt verwijderen (of ESC om te annuleren):");
            Console.WriteLine("{0,-10} {1,-15} {2,-15} {3,-15}", "ID", "Voornaam", "Achternaam", "Rijksregisternummer");
            foreach (var pt in patients)
                Console.WriteLine("{0,-10} {1,-15} {2,-15} {3,-15}", pt.PatientId, pt.FirstName, pt.LastName, pt.RijksregisterNumber);

            var inputId = ReadInput("Kies een patiënt (ID) of ESC om te annuleren: ");
            if (inputId == null) return;

            if (int.TryParse(inputId, out int id))
            {
                _patientService.DeletePatient(id);
                Console.WriteLine("Patiënt verwijderd!");
                Console.ReadKey();
            }
        }

        private static void AddPhysician()
        {
            var name = ReadInput("Naam: ");
            if (name == null) return;

            _physicianService.AddPhysician(name);
            Console.WriteLine("Arts toegevoegd! Druk op een toets om naar het menu te gaan");
            Console.ReadKey();
        }

        private static void DeletePhysician()
        {
            var physicians = _physicianService.GetAll().ToList();
            if (physicians.Count == 0)
            {
                Console.WriteLine("Geen artsen gevonden.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Arts verwijderen (of ESC om te annuleren):");
            foreach (var p in physicians)
                Console.WriteLine($"{p.PhysicianId} - {p.Name}");

            var inputId = ReadInput("Kies een arts (ID) of ESC om te annuleren: ");
            if (inputId == null) return;

            if (int.TryParse(inputId, out int id))
            {
                _physicianService.DeletePhysician(id);
                Console.WriteLine("Arts verwijderd!");
                Console.ReadKey();
            }
        }

        private static void AddAppointment()
        {
            var patients = _patientService.GetAll().ToList();
            var physicians = _physicianService.GetAll().ToList();

            if (patients.Count == 0 || physicians.Count == 0)
            {
                Console.WriteLine("Geen patiënten of artsen beschikbaar.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Patiënten:");
            foreach (var p in patients)
                Console.WriteLine($"{p.PatientId} - {p.FirstName} {p.LastName}");

            var patientInput = ReadInput("Kies patiënt ID: ");
            if (patientInput == null) return;
            int patientId = int.Parse(patientInput);

            Console.WriteLine("Artsen:");
            foreach (var ph in physicians)
                Console.WriteLine($"{ph.PhysicianId} - {ph.Name}");

            var physicianInput = ReadInput("Kies arts ID: ");
            if (physicianInput == null) return;
            int physicianId = int.Parse(physicianInput);

            DateTime date;
            while (true)
            {
                var dateInput = ReadInput("Datum (dd-mm-jjjj): ");
                if (dateInput == null) return;

                if (DateTime.TryParseExact(dateInput, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out date))
                {
                    if (date.Date < DateTime.Today)
                    {
                        Console.WriteLine("Datum kan niet in het verleden liggen. Probeer opnieuw.");
                        continue;
                    }
                    break;
                }
                Console.WriteLine("Ongeldige datum. Gebruik dd-mm-jjjj.");
            }


            var freeSlots = _appointmentService.GetAvailableTimeSlots(physicianId, date);
            if (freeSlots.Count == 0)
            {
                Console.WriteLine("Geen vrije tijdsloten beschikbaar.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Vrije tijdsloten:");
            for (int i = 0; i < freeSlots.Count; i++)
                Console.WriteLine($"{i + 1}: {freeSlots[i]:hh\\:mm}");

            var slotInput = ReadInput("Kies tijdslot (nummer): ");
            if (slotInput == null) return;
            int slotIndex = int.Parse(slotInput) - 1;

            _appointmentService.CreateAppointment(patientId, physicianId, date, freeSlots[slotIndex]);
            Console.WriteLine("Afspraak toegevoegd!");
            Console.ReadKey();
        }

        private static void ShowAppointment()
        {
            var appointments = _appointmentService.GetUpcomingAppointments().ToList();
            if (appointments.Count == 0)
            {
                Console.WriteLine("Geen afspraken gevonden.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("{0,-20} {1,-20} {2,-20} {3,-20}", "Datum/Tijd", "Arts", "Patiënt", "Rijksregisternummer");
            foreach (var ap in appointments)
            {
                Console.WriteLine("{0,-20} {1,-20} {2,-20} {3,-20}",
                    ap.Date.ToString("dd-MM-yyyy HH:mm"),
                    ap.Physician?.Name,
                    $"{ap.Patient?.FirstName} {ap.Patient?.LastName}",
                    ap.Patient?.RijksregisterNumber);
            }

            Console.ReadKey();
        }

        public static bool ShowMenu()
        {
            Console.Clear();
            foreach (var line in File.ReadAllLines("logo.txt"))
                Console.WriteLine(line);

            Console.WriteLine();
            Console.WriteLine("1 - Patient toevoegen");
            Console.WriteLine("2 - Patienten verwijderen");
            Console.WriteLine("3 - Arts toevoegen");
            Console.WriteLine("4 - Arts verwijderen");
            Console.WriteLine("5 - Afspraak toevoegen");
            Console.WriteLine("6 - Afspraken inzien");
            Console.WriteLine("7 - Sluiten");
            Console.WriteLine("8 - Reset db");

            if (int.TryParse(Console.ReadLine(), out int option))
            {
                switch (option)
                {
                    case 1:
                        AddPatient();
                        return true;
                    case 2:
                        DeletePatient();
                        return true;
                    case 3:
                        AddPhysician();
                        return true;
                    case 4:
                        DeletePhysician();
                        return true;
                    case 5:
                        AddAppointment();
                        return true;
                    case 6:
                        ShowAppointment();
                        return true;
                    case 7: return false;
                    case 8:
                        EPDDbContext dbContext = new EPDDbContext();
                        dbContext.Database.EnsureDeleted();
                        dbContext.Database.EnsureCreated();
                        return true;
                    default:
                        return true;
                }
            }
            return true;
        }
    }
}
