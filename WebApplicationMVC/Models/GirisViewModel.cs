using System.ComponentModel.DataAnnotations;

namespace WebApplicationMVC.Models
{
    public class GirisViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı Adı Gereklidir")]
        [StringLength(30, ErrorMessage = "Kullanıcı Adı En fazla 30 karakter")]
        [MinLength(5, ErrorMessage = "Kullanıcı Adı En az 5 karakter")]
        [Display(Name = "Kullanıcı Adı", Prompt = "Bir İsim")]
        public string Ad { get; set; }

        [Required(ErrorMessage = "Parola Gereklidir")]
        [MinLength(5, ErrorMessage = "Parola En az 5 karakter")]
        [MaxLength(30, ErrorMessage = "Parola En fazla 30 karakter")]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string Parola { get; set; }
    }
}
