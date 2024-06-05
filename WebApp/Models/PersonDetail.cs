using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace WebApp.Models
{
    public class PersonDetail
    {
        public int Id { get; set; }
        public DateOnly BirthDay { get; set; }
        public string PersonCity { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
    }
}
