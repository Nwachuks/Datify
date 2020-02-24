namespace Datify.API.Models
{
    public class Like
    {
        // Id of user that likes
        public int LikerId { get; set; }

        // Id of user that is liked
        public int LikeeId { get; set; }
        public User Liker { get; set; }
        public User Likee { get; set; }
    }
}