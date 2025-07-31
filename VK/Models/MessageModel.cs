namespace VK.Models
{
    public class MessageModel(string User, string Text, bool IsMe)
    {
        public string User { get; set; } = User;
        public string Text { get; set; } = Text;
        public bool IsMe { get; set; } = IsMe;
    }
}
