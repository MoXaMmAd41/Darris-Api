using System.ComponentModel.DataAnnotations;

namespace Darris_Api.Models.Dto
{
    public class CollageClubJoinReqDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public double GPA { get; set; }
        public int StudentId { get; set; }
    }
}
