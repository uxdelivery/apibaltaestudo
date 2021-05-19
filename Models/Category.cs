using System.ComponentModel.DataAnnotations;

namespace APICrudBasica.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MaxLength(60, ErrorMessage = "Esse campo de conter entre 3 e 60 caracteres")]
        [MinLength(3, ErrorMessage = "Esse campo de conter entre 3 e 60 caracteres")]
        public string Title { get; set; }
    }
}