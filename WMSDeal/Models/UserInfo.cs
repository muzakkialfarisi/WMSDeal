using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSDeal.Models
{
    public class UserInfo
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string JobPosName { get; set; } = string.Empty;
        public int ProfileId { get; set; } 
        public string Profile { get; set; } = string.Empty;
        public string HouseCode { get; set; } = string.Empty;
        public string Warehouse { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime ExpireDate { get; set; } = DateTime.Now;
    }
}
