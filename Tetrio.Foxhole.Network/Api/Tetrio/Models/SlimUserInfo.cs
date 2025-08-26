namespace Tetrio.Foxhole.Network.Api.Tetrio.Models;

public class SlimUserInfo
{
    public string UserId { get; set; }
    public string Username { get; set; }
    public string Avatar { get; set; }
    public string Banner { get; set; }
    public double? AvatarRevision { get; set; }
    public double? BannerRevision { get; set; }
}