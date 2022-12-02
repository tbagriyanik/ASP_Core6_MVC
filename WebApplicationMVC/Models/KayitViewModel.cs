using System.ComponentModel.DataAnnotations;

namespace WebApplicationMVC.Models
{
    public class KayitViewModel
    {
        [Required(ErrorMessage = "Kullanıcı Adı Gereklidir")]
        [StringLength(30, ErrorMessage = "Ad En fazla 30 karakter")]
        [MinLength(5, ErrorMessage = "Ad En az 5 karakter")]
        [Display(Name = "Kullanıcı Adı", Prompt = "Bir İsim")]
        public string Ad { get; set; }

        [Required(ErrorMessage = "Parola 1 Gereklidir")]
        [MinLength(5, ErrorMessage = "Parola 1 En az 5 karakter")]
        [MaxLength(30, ErrorMessage = "Parola 1 En fazla 30 karakter")]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string Parola { get; set; }

        [Required(ErrorMessage = "Parola 2 Gereklidir")]
        [MinLength(5, ErrorMessage = "Parola 2 En az 5 karakter")]
        [MaxLength(30, ErrorMessage = "Parola 2 En fazla 30 karakter")]
        [DataType(DataType.Password)]
        [Display(Name = "Parola Tekrar")]
        [Compare(nameof(Parola), ErrorMessage = "Parolalar aynı olmalıdır")]
        public string ReParola { get; set; }
    }
}
