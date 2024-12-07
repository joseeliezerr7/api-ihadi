using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace api_ihadi.Models
{
    public enum WorkType
    {
        MAST,
        BTTSupport,
        Training,
        TechnicalSupport,
        VMast,
        Transcribe,
        QualityAssurance,
        IhadiSoftwareDevelopment,
        SpecialAssignment,
        Other
    }
}
