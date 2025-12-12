using FlashcardsApp.Models.DTOs.Admin;

namespace FlashcardsApp.BLL.Interfaces;

public interface IAdminBL
{
    Task<List<AdminUserDto>> GetAllUsersAsync();
}
