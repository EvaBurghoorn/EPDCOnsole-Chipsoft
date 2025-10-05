using Chipsoft.Assignments.EPDConsole;
using Chipsoft.Assignments.EPDConsole.model;

public class PatientRepository : IPatientRepository
{
    private readonly EPDDbContext _context;

    public PatientRepository(EPDDbContext context)
    {
        _context = context;
    }

    public void Add(Patient patient)
    {
        _context.Patients.Add(patient);
    }

    public void Delete(int id)
    {
        var patient = _context.Patients.Find(id);
        if (patient != null)
            _context.Patients.Remove(patient);
    }

    public IEnumerable<Patient> GetAll()
    {
       return _context.Patients.ToList();
    }

    public Patient? GetById(int id)
    {
        return _context.Patients.Find(id);
    }

    public void Save()
    {
        _context.SaveChanges();
    }
}
