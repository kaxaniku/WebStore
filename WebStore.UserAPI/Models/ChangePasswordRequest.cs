namespace WebStore.UserAPI.Models;

public record ChangePasswordRequest(string OldPassword, string NewPassword);