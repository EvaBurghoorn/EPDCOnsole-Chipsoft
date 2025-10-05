using Chipsoft.Assignments.EPDConsole.model;

public interface IPatientRepository
{
    void Add(Patient patient);
    void Delete(int id);
    IEnumerable<Patient> GetAll();
    Patient? GetById(int id);
    void Save();
}
