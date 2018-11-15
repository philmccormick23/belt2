using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-z]+$", ErrorMessage="First Name can only include letters")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-z]+$", ErrorMessage="Last Name can only include letters")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^a-zA-Z0-9])(?!.*\s).{8,20}$", ErrorMessage = "The password needs a number, a letter, and a special character")]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public List<RSVP> RSVPs { get; set; }

        public List<Activity> Activities { get; set; }

        [NotMapped]
        [Compare("Password", ErrorMessage = "Password and password confirmation fields must match.")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}

        }
}