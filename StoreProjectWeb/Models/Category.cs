using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StoreProjectWeb.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        [MaxLength(30)]
        [DisplayName("Category Name")]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1,100, ErrorMessage = "Display Order should be a value between 1 and 100")]
        public int DisplayOrder { get; set; }
    }
}
