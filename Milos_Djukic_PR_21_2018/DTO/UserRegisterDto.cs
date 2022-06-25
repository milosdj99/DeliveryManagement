using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Milos_Djukic_PR_21_2018.DTO
{
    public class UserRegisterDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string ImageUrl { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
    }
}
