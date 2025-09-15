using System.ComponentModel.DataAnnotations;

namespace FlashcardsBlazorUI.Models
{
    public class CreateCardDto
    {
        [Required(ErrorMessage = "Вопрос обязателен")]
        public string Question { get; set; } = "";

        [Required(ErrorMessage = "Ответ обязателен")]
        public string Answer { get; set; } = "";
    }
}
