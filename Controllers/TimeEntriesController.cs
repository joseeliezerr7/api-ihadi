using api_ihadi.Models;
using api_ihadi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace api_ihadi.Controllers
{
    public enum WorkType
    {
        MAST,
        BTT_Support,
        Training,
        Technical_Support,
        V_MAST,
        Transcribe,
        Quality_Assurance_Processes,
        Ihadi_Software_Development,
        Special_Assignment,
        Other
    }

    [ApiController]
    [Route("api/[controller]")]
    public class TimeEntriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TimeEntriesController> _logger;

        public TimeEntriesController(AppDbContext context, ILogger<TimeEntriesController> logger)
        {
            _context = context;
            _logger = logger;
        }


     
        [HttpPost]
        public async Task<ActionResult<TimeEntry>> CreateTimeEntry([FromBody] TimeEntryDto timeEntryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { mensaje = "Datos inválidos", errores = ModelState.Values.SelectMany(v => v.Errors) });
                }

                var timeEntry = new TimeEntry
                {
                    SupportedPerson = timeEntryDto.SupportedPerson,
                    SupportedPersonCountry = timeEntryDto.SupportedPersonCountry,
                    WorkingLanguage = timeEntryDto.WorkingLanguage,
                    StartDate = timeEntryDto.StartDate,
                    EndDate = timeEntryDto.EndDate,
                    StartTime = timeEntryDto.StartTime,
                    EndTime = timeEntryDto.EndTime,
                    WorkType = timeEntryDto.WorkType,
                    Description = timeEntryDto.Description,
                    CreatedAt = DateTime.UtcNow
                };

                _context.TimeEntries.Add(timeEntry);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Registro creado exitosamente",
                    datos = new
                    {
                        id = timeEntry.Id,
                        supportedPerson = timeEntry.SupportedPerson,
                        supportedPersonCountry = timeEntry.SupportedPersonCountry,
                        workingLanguage = timeEntry.WorkingLanguage,
                        startDate = timeEntry.StartDate,
                        endDate = timeEntry.EndDate,
                        startTime = timeEntry.StartTime,
                        endTime = timeEntry.EndTime,
                        workType = timeEntry.WorkType,
                        description = timeEntry.Description,
                        createdAt = timeEntry.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el registro: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TimeEntry>>> GetTimeEntries()
        {
            try
            {
                var entries = await _context.TimeEntries
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();

                var timeEntries = entries.Select(t => new
                {
                    id = t.Id,
                    supportedPerson = t.SupportedPerson,
                    supportedPersonCountry = t.SupportedPersonCountry,
                    workingLanguage = t.WorkingLanguage,
                    startDate = t.StartDate,
                    endDate = t.EndDate,
                    startTime = t.StartTime,
                    endTime = t.EndTime,
                    workType = t.WorkType,
                    description = t.Description,
                    createdAt = t.CreatedAt
                }).ToList();

                return Ok(new { mensaje = "Registros obtenidos exitosamente", datos = timeEntries });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener los registros: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

       
        [HttpGet("{id}")]
        public async Task<ActionResult<TimeEntry>> GetTimeEntry(int id)
        {
            try
            {
                var timeEntry = await _context.TimeEntries.FindAsync(id);

                if (timeEntry == null)
                {
                    return NotFound(new { mensaje = "Registro no encontrado" });
                }

                return Ok(new
                {
                    mensaje = "Registro obtenido exitosamente",
                    datos = new
                    {
                        id = timeEntry.Id,
                        supportedPerson = timeEntry.SupportedPerson,
                        supportedPersonCountry = timeEntry.SupportedPersonCountry,
                        workingLanguage = timeEntry.WorkingLanguage,
                        startDate = timeEntry.StartDate,
                        endDate = timeEntry.EndDate,
                        startTime = timeEntry.StartTime,
                        endTime = timeEntry.EndTime,
                        workType = timeEntry.WorkType,
                        description = timeEntry.Description,
                        createdAt = timeEntry.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener el registro: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

      
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTimeEntry(int id, TimeEntryDto timeEntryDto)
        {
            try
            {
                var timeEntry = await _context.TimeEntries.FindAsync(id);

                if (timeEntry == null)
                {
                    return NotFound(new { mensaje = "Registro no encontrado" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { mensaje = "Datos inválidos", errores = ModelState.Values.SelectMany(v => v.Errors) });
                }

                timeEntry.SupportedPerson = timeEntryDto.SupportedPerson;
                timeEntry.SupportedPersonCountry = timeEntryDto.SupportedPersonCountry;
                timeEntry.WorkingLanguage = timeEntryDto.WorkingLanguage;
                timeEntry.StartDate = timeEntryDto.StartDate;
                timeEntry.EndDate = timeEntryDto.EndDate;
                timeEntry.StartTime = timeEntryDto.StartTime;
                timeEntry.EndTime = timeEntryDto.EndTime;
                timeEntry.WorkType = timeEntryDto.WorkType;
                timeEntry.Description = timeEntryDto.Description;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Registro actualizado exitosamente",
                    datos = new
                    {
                        id = timeEntry.Id,
                        supportedPerson = timeEntry.SupportedPerson,
                        supportedPersonCountry = timeEntry.SupportedPersonCountry,
                        workingLanguage = timeEntry.WorkingLanguage,
                        startDate = timeEntry.StartDate,
                        endDate = timeEntry.EndDate,
                        startTime = timeEntry.StartTime,
                        endTime = timeEntry.EndTime,
                        workType = timeEntry.WorkType,
                        description = timeEntry.Description,
                        createdAt = timeEntry.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el registro: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeEntry(int id)
        {
            try
            {
                var timeEntry = await _context.TimeEntries.FindAsync(id);
                if (timeEntry == null)
                {
                    return NotFound(new { mensaje = "Registro no encontrado" });
                }

                _context.TimeEntries.Remove(timeEntry);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Registro eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el registro: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<TimeEntry>>> FilterTimeEntries(
   [FromQuery] DateTime? startDate,
   [FromQuery] DateTime? endDate,
   [FromQuery] string workType,
   [FromQuery] string? technicianId,
   [FromQuery] string? supportedPerson,
   [FromQuery] string? country,
   [FromQuery] string? language)
        {
            try
            {
                var query = _context.TimeEntries
                    .Include(t => t.User)
                    .AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(t => t.StartDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(t => t.EndDate <= endDate.Value);

                if (!string.IsNullOrEmpty(workType))
                    query = query.Where(t => t.WorkType.ToString() == workType);

                if (!string.IsNullOrEmpty(technicianId) && int.TryParse(technicianId, out int techId))
                    query = query.Where(t => t.UserId == techId);

                if (!string.IsNullOrEmpty(supportedPerson))
                    query = query.Where(t => t.SupportedPerson.Contains(supportedPerson));

                if (!string.IsNullOrEmpty(country))
                    query = query.Where(t => t.SupportedPersonCountry.Contains(country));

                if (!string.IsNullOrEmpty(language))
                    query = query.Where(t => t.WorkingLanguage.Contains(language));

                var entries = await query
                    .OrderByDescending(t => t.CreatedAt)
                    .Select(t => new
                    {
                        id = t.Id,
                        userId = t.UserId,
                        techFieldName = t.User.Name,
                        supportedPerson = t.SupportedPerson,
                        supportedPersonCountry = t.SupportedPersonCountry,
                        workingLanguage = t.WorkingLanguage,
                        startDate = t.StartDate.ToString("yyyy-MM-dd"),
                        endDate = t.EndDate.ToString("yyyy-MM-dd"),
                        startTime = t.StartTime,
                        endTime = t.EndTime,
                        workType = t.WorkType,
                        description = t.Description,
                        createdAt = t.CreatedAt,
                        horasTrabajadas = CalculateHours(t)
                    })
                    .ToListAsync();

                return Ok(new
                {
                    mensaje = "Registros filtrados exitosamente",
                    datos = entries
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al filtrar los registros: {ex.Message}");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        private double CalculateHours(TimeEntry entry)
        {
            var startTimeSpan = TimeSpan.Parse(entry.StartTime);
            var endTimeSpan = TimeSpan.Parse(entry.EndTime);

            var days = (entry.EndDate.Date - entry.StartDate.Date).Days;

            if (days == 0)
            {
                return (endTimeSpan - startTimeSpan).TotalHours;
            }

            var totalHours = 0.0;
            totalHours += 24 - startTimeSpan.TotalHours; 
            totalHours += (days - 1) * 24;
            totalHours += endTimeSpan.TotalHours; 

            return Math.Round(totalHours, 2);
        }

        private string ConvertTo24HourFormat(string time12Hour)
        {
            try
            {
               
                DateTime dateTime = DateTime.ParseExact(time12Hour, "h:mm tt", CultureInfo.InvariantCulture);
             
                return dateTime.ToString("HH:mm");
            }
            catch
            {
                throw new FormatException("Formato de hora inválido. Use el formato 'HH:MM AM/PM'");
            }
        }

        private string ConvertTo12HourFormat(string time24Hour)
        {
            try
            {
               
                DateTime dateTime = DateTime.ParseExact(time24Hour, "HH:mm", CultureInfo.InvariantCulture);
                
                return dateTime.ToString("h:mm tt");
            }
            catch
            {
                throw new FormatException("Formato de hora inválido. Use el formato 'HH:mm'");
            }
        }

       
    }
}