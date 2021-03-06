using System.ComponentModel.DataAnnotations;

namespace APICrudBasica.Models
{

    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MaxLength(20, ErrorMessage = "Este campo deve ter no máximo 20 caracteres")]
        [MinLength(3, ErrorMessage = "Este campo deve ter no minimo 3 caracteres")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MaxLength(20, ErrorMessage = "Este campo deve ter no máximo 20 caracteres")]
        [MinLength(3, ErrorMessage = "Este campo deve ter no minimo 3 caracteres")]
        public string Password { get; set; }

        public string Role {get;set;}
    }

}