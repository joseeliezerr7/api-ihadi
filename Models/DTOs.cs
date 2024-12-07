using System.ComponentModel.DataAnnotations;

namespace api_ihadi.Models
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
        public string Name { get; set; }
    }

    public class LoginDto
    {
        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; }
    }

    public class TimeEntryDto
    {
        [Required(ErrorMessage = "La persona soportada es requerida")]
        public string SupportedPerson { get; set; }

        [Required(ErrorMessage = "El país de la persona soportada es requerido")]
        public string SupportedPersonCountry { get; set; }

        [Required(ErrorMessage = "El idioma de trabajo es requerido")]
        public string WorkingLanguage { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "La fecha de fin es requerida")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "La hora de inicio es requerida")]
        [RegularExpression(@"^(0?[1-9]|1[0-2]):[0-5][0-9] (AM|PM)$",
            ErrorMessage = "El formato de hora debe ser 'HH:MM AM/PM'")]
        public string StartTime { get; set; }

        [Required(ErrorMessage = "La hora de fin es requerida")]
        [RegularExpression(@"^(0?[1-9]|1[0-2]):[0-5][0-9] (AM|PM)$",
            ErrorMessage = "El formato de hora debe ser 'HH:MM AM/PM'")]
        public string EndTime { get; set; }

        [Required(ErrorMessage = "El tipo de trabajo es requerido")]
        public WorkType WorkType { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string Description { get; set; }
    }

    public class UpdateProfileDto
    {
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        public string? Email { get; set; }

        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string? Password { get; set; }

        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
        public string? Name { get; set; }
    }
}