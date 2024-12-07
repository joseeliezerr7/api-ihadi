using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace api_ihadi.Models
{
    [Table("TimeEntries")]
    public class TimeEntry
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        [Required]
        public string SupportedPerson { get; set; }

        [Required]
        public string SupportedPersonCountry { get; set; }

        [Required]
        public string WorkingLanguage { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string StartTime { get; set; }

        [Required]
        public string EndTime { get; set; }

        [Required]
        public WorkType WorkType { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
