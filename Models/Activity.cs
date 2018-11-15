using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam.Models
{

    public class ValidDateAttribute : ValidationAttribute ///Date Validation
    {
        public override bool IsValid(object value)
        {
            DateTime d = Convert.ToDateTime(value);
            return d >= DateTime.Now;
        }
    }
    public class Activity
    {
        [Key]
        public int ActivityId { get; set; }

        
        [Required]
        [MinLength(2, ErrorMessage="Title must be more than 2 characters!")]
        public string ActivityTitle {get; set;}

        [Required]
        public DateTime ActivityDate {get; set;}

        [Required]
        public int Duration {get; set;}

        [Required]
        public string DurationType {get; set;}

        [Required]
        [MinLength(10, ErrorMessage="Description must be more than 10 characters!")]
        public string Description {get; set;}

        public int UserId {get; set;}

        public User User {get; set;}

        public List<RSVP> RSVPs { get; set; }
    

        }
}