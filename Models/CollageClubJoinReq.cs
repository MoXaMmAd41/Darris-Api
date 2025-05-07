using Darris_Api.Roles;

namespace Darris_Api.Models
{
    public class CollageClubJoinReq
    {

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double GPA { get; set; }
        public JoinCollageClubReqStatus Status { get; set; } = JoinCollageClubReqStatus.Pending;
        public int StudentId { get; set; }
        public UserInformation Student { get; set; }
    }
}
