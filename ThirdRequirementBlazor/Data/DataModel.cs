using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ThirdRequirementBlazor.Data
{
    public class DataModel
    {
        [Required]
        [StringLength(10, ErrorMessage = "Name is too long.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First Name - use letters only please")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(10, ErrorMessage = "Name is too long.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last Name - use letters only please")]
        public string LastName { get; set; }
    }
}
