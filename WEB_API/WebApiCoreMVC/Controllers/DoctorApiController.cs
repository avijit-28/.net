using Microsoft.AspNetCore.Mvc;
using WebApiCoreMVC.Models;
using WebApiCoreMVC.Repository;

namespace WebApiCoreMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorApiController : ControllerBase
    {
        private readonly IDoctorRepo _doctorRepo;

        public DoctorApiController(IDoctorRepo doctorRepo)
        {
            _doctorRepo = doctorRepo;
        }

        // GET: api/DoctorApi
        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            try
            {
                var (doctors, message) = await _doctorRepo.GetAllDoctorsAsync();
                return Ok(new { success = true, message, data = doctors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/DoctorApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            try
            {
                var (doctor, message) = await _doctorRepo.GetDoctorByIdAsync(id);
                if (doctor == null)
                {
                    return NotFound(new { success = false, message = $"Doctor with ID {id} not found." });
                }
                return Ok(new { success = true, message, data = doctor });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST: api/DoctorApi
        [HttpPost]
        public async Task<IActionResult> AddDoctor([FromBody] Doctor doctor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Validation failed", errors = ModelState });
            }

            try
            {
                var message = await _doctorRepo.AddDoctorAsync(doctor);
                return Ok(new { success = true, message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // PUT: api/DoctorApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] Doctor doctor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Validation failed", errors = ModelState });
            }

            try
            {
                doctor.DoctorId = id;
                var message = await _doctorRepo.UpdateDoctorAsync(doctor);
                return Ok(new { success = true, message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // DELETE: api/DoctorApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            try
            {
                var message = await _doctorRepo.DeleteDoctorAsync(id);
                return Ok(new { success = true, message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
