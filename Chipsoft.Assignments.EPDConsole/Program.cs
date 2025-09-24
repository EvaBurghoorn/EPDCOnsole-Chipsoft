using Chipsoft.Assignments.EPDConsole.model;
using Microsoft.EntityFrameworkCore;

namespace Chipsoft.Assignments.EPDConsole
{
    public class Program
    {

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
            using var db = new EPDDbContext();


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
                //if (input == null) return;

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

            var patient = new Patient
            {
                RijksregisterNumber = rijksregisterNumber!,
                FirstName = firstName!,
                LastName = lastName!,
                BirthDay = birthDay,
                Address = address!,
                Email = email!,
                Phone = string.IsNullOrWhiteSpace(phone) ? null : phone
            };

            db.Patients.Add(patient);
            db.SaveChanges();

            Console.WriteLine("Patient toegevoegd! Druk op een toets om naar het menu te gaan");
            Console.ReadKey();
        }


        private static void ShowAppointment()
        {
            using var db = new EPDDbContext();

            var appointments = db.Appointments
                .Include(a => a.Physician)
                .Include(a => a.Patient)
                .Where(a => a.Date >= DateTime.Now)
                .OrderBy(a => a.Date)
                .ToList();

            if (appointments.Count == 0)
            {
                Console.WriteLine("Geen afspraken gevonden.");
                Console.WriteLine("Druk op een toets om terug te keren naar het menu.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("{0,-20} {1,-20} {2,-20} {3,-20}", "Datum/Tijd", "Arts", "Patiënt", "Rijksregisternummer");
            Console.WriteLine(new string('-', 100));

            foreach (var ap in appointments)
            {
                Console.WriteLine("{0,-20} {1,-20} {2,-20} {3, -20}",
                    ap.Date.ToString("dd-MM-yyyy HH:mm"),
                    ap.Physician?.Name,
                    $"{ap.Patient?.FirstName} {ap.Patient?.LastName}",
                    ap.Patient?.RijksregisterNumber);
            }

            Console.WriteLine(new string('-', 100));
            Console.WriteLine("Druk op een toets om terug te keren naar het menu.");
            Console.ReadKey();
        }


        private static void AddAppointment()
        {
            using var db = new EPDDbContext();
            var physicians = db.Physicians.ToList();
            var patients = db.Patients.ToList();

            if (physicians.Count == 0 || patients.Count == 0)
            {
                Console.WriteLine(physicians.Count == 0 ? "Geen artsen gevonden." : "Geen patienten gevonden.");
                Console.WriteLine("Druk op een toets om terug te keren naar het menu.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Afspraak maken:");

            Console.WriteLine("Patiënten:");
            Console.WriteLine("{0,-10} {1,-15} {2,-15} {3,-20}", "ID-Nummer", "Voornaam", "Achternaam", "Rijksregisternummer");
            Console.WriteLine(new string('-', 80));
            foreach (var pt in patients)
                Console.WriteLine("{0,-10} {1,-15} {2,-15} {3,-20}", pt.PatientId, pt.FirstName, pt.LastName, pt.RijksregisterNumber);
            Console.WriteLine(new string('-', 80));

            Patient selectedPatient;
            while (true)
            {
                var input = ReadInput("Kies een patiënt (ID) of ESC om te annuleren: ");
                if (input == null) return;
                selectedPatient = patients.FirstOrDefault(p => p.PatientId.ToString() == input);
                if (selectedPatient != null) break;
                Console.WriteLine("Ongeldige keuze, probeer opnieuw.");
            }

            Console.WriteLine("\nArtsen:");
            foreach (var ph in physicians)
                Console.WriteLine($"{ph.PhysicianId}: {ph.Name}");

            Physician selectedPhysician;
            while (true)
            {
                var input = ReadInput("Kies een arts (ID) of ESC om te annuleren: ");
                if (input == null) return;
                selectedPhysician = physicians.FirstOrDefault(p => p.PhysicianId.ToString() == input);
                if (selectedPhysician != null) break;
                Console.WriteLine("Ongeldige keuze, probeer opnieuw.");
            }

            DateTime appointmentDate;
            while (true)
            {
                var input = ReadInput("Datum (dd-mm-jjjj) of ESC om te annuleren: ");
                if (input == null) return;
                if (DateTime.TryParseExact(input, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out appointmentDate))
                {
                    if (appointmentDate.Date <= DateTime.Today)
                    {
                        Console.WriteLine("Datum kan niet in het verleden liggen. Probeer opnieuw.");
                        continue;
                    }
                    break;
                }
                Console.WriteLine("Ongeldige datum. Gebruik dd-mm-jjjj.");
            }

            var possibleSlots = new List<TimeSpan>
            {
                new(9,0,0), 
                new(9,20,0), 
                new(9,40,0),
                new(10,0,0), 
                new(10,20,0), 
                new(10,40,0),
                new(11,0,0)
            };

            var bookedSlots = db.Appointments
                .Where(a => a.PhysicianId == selectedPhysician.PhysicianId && a.Date.Date == appointmentDate.Date)
                .Select(a => a.Date.TimeOfDay)
                .ToList();
            var freeSlots = possibleSlots.Except(bookedSlots).ToList();

            if (freeSlots.Count == 0)
            {
                Console.WriteLine("Geen vrije tijdsloten beschikbaar. Kies een andere datum.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Vrije tijdsloten:");
            for (int i = 0; i < freeSlots.Count; i++)
                Console.WriteLine($"{i + 1} - {freeSlots[i]:hh\\:mm}");

            int slotIndex;
            while (true)
            {
                var input = ReadInput("Kies een tijdslot (nummer) of ESC om te annuleren: ");
                if (input == null) return;
                if (int.TryParse(input, out slotIndex) && slotIndex >= 1 && slotIndex <= freeSlots.Count)
                    break;
                Console.WriteLine("Ongeldige keuze, probeer opnieuw.");
            }

            var appointmentDateTime = appointmentDate.Date + freeSlots[slotIndex - 1];

            var appointment = new Appointment
            {
                Date = appointmentDateTime,
                PhysicianId = selectedPhysician.PhysicianId,
                PatientId = selectedPatient.PatientId
            };

            db.Appointments.Add(appointment);
            db.SaveChanges();

            Console.WriteLine($"\nAfspraak gemaakt op {appointmentDateTime:dd-MM-yyyy HH:mm} met arts {selectedPhysician.Name} en patiënt {selectedPatient.FirstName} {selectedPatient.LastName}.");
            Console.WriteLine("Druk op een toets om terug te keren naar het menu.");
            Console.ReadKey();
        }



        private static void DeletePhysician()
        {
            using var db = new EPDDbContext();

            var physicians = db.Physicians.ToList();
            if (physicians.Count == 0)
            {
                Console.WriteLine("Geen artsen gevonden.");
                Console.WriteLine("Druk op een toets om terug te keren naar het menu.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Arts verwijderen (of ESC om te annuleren):");
            Console.WriteLine("{0,-10} {1,-15}", "ID-Nummer", "Naam");
            Console.WriteLine(new string('-', 40));

            foreach (var p in physicians)
            {
                Console.WriteLine("{0,-10} {1,-15}", p.PhysicianId, p.Name);
            }
            Console.WriteLine(new string('-', 40));

            Physician selectedPhysician;
            while (true)
            {
                var input = ReadInput("Kies een arts (ID) of ESC om te annuleren: ");
                if (input == null) return;


                selectedPhysician = physicians.FirstOrDefault(p => p.PhysicianId.ToString() == input);
                if (selectedPhysician != null) break;

                Console.WriteLine("Ongeldige keuze, probeer opnieuw.");
            }

            db.Physicians.Remove(selectedPhysician);
            db.SaveChanges();

            Console.WriteLine($"Arts {selectedPhysician.Name} verwijderd!");
            Console.WriteLine("Druk op een toets om terug te keren naar het menu.");
            Console.ReadKey();
        }


        private static void AddPhysician()
        {
            using var db = new EPDDbContext();

            Console.WriteLine("Arts toevoegen (ESC om te annuleren):");

            var name = ReadInput("Naam: ");
            if (name == null) return;

            var physician = new Physician
            {
                Name = name
            };

            db.Physicians.Add(physician);
            db.SaveChanges();

            Console.WriteLine("Arts toegevoegd! Druk op een toets om naar het menu te gaan");
            Console.ReadKey();
        }


        private static void DeletePatient()
        {

            using var db = new EPDDbContext();

            var patients = db.Patients.ToList();
            if (patients.Count == 0)
            {
                Console.WriteLine("Geen patiënten gevonden.");
                Console.WriteLine("Druk op een toets om terug te keren naar het menu.");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Patiënt verwijderen (of ESC om te annuleren):");
            Console.WriteLine("{0,-10} {1,-15} {2,-15} {3, -15}", "ID-Nummer", "Voornaam", "Achternaam", "Rijksregisternummer");
            Console.WriteLine(new string('-', 40)); 

            foreach (var pt in patients)
            {
                Console.WriteLine("{0,-10} {1,-15} {2,-15} {3,-15}", pt.PatientId, pt.FirstName, pt.LastName, pt.RijksregisterNumber);
            }
            Console.WriteLine(new string('-', 40));


            Patient selectedPatient;
            while (true)
            {
                var input = ReadInput("Kies een patiënt (ID) of ESC om te annuleren: ");
                if (input == null) return;


                selectedPatient = patients.FirstOrDefault(p => p.PatientId.ToString() == input);
                if (selectedPatient != null) break;

                Console.WriteLine("Ongeldige keuze, probeer opnieuw.");
            }

            db.Patients.Remove(selectedPatient);
            db.SaveChanges();

            Console.WriteLine($"Patiënt {selectedPatient.FirstName} {selectedPatient.LastName} verwijderd!");
            Console.WriteLine("Druk op een toets om terug te keren naar het menu.");
            Console.ReadKey();

        }

        static void Main(string[] args)
        {
            while (ShowMenu())
            { }
        }

        public static bool ShowMenu()
        {
            Console.Clear();
            foreach (var line in File.ReadAllLines("logo.txt"))
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("");
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
                    case 7:
                        return false;
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