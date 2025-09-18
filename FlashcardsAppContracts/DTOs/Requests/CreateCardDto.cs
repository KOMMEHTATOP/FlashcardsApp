using System.ComponentModel.DataAnnotations;

namespace FlashcardsAppContracts.DTOs.Requests;

public class CreateCardDto
{
    [Required(ErrorMessage = "Вопрос обязателен")]
    [StringLength(300, ErrorMessage = "Вопрос не может быть длиннее 300 символов")]
    public required string Question { get; set; }
    
    [Required(ErrorMessage = "Ответ обязателен")]
    [StringLength(2000, ErrorMessage = "Ответ не может быть длиннее 2000 символов")]
    public required string Answer { get; set; }
}
