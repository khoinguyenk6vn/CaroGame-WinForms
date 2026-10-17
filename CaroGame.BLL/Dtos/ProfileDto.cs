namespace CaroGame.BLL.Dtos
{
    // Thông tin hiện ở màn Profile
    public class ProfileDto
    {
        public string Username { get; set; } = "";
        public int Score { get; set; }
        public string RankName { get; set; } = "";
        public int DefaultAvatarIndex { get; set; }

        // Rỗng nếu chưa upload avatar
        public string AvatarPath { get; set; } = "";
    }
}
