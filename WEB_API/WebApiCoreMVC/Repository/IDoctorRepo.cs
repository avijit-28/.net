using WebApiCoreMVC.Models;

namespace WebApiCoreMVC.Repository
{
    public interface IDoctorRepo
    {
        Task<(IEnumerable<Doctor> Doctors, string Message)> GetAllDoctorsAsync();
        Task<(Doctor? Doctor, string Message)> GetDoctorByIdAsync(int id);
        Task<string> AddDoctorAsync(Doctor doctor);
        Task<string> UpdateDoctorAsync(Doctor doctor);
        Task<string> DeleteDoctorAsync(int id);
    }
}
