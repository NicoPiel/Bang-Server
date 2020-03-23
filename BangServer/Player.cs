namespace BangServer
{
    public class Player
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public bool ReadyCheck { get; set; }
        
        public Player(int id)
        {
            Id = id;
            Username = null;
        }

        public Player(int id, string username)
        {
            Id = id;
            Username = username;
        }

        public void SetUsername(string username)
        {
            Username = username;
        }

        public void Ready()
        {
            ReadyCheck = true;
        }

        public void Unready()
        {
            ReadyCheck = false;
        }

        public override bool Equals(object obj)
        {
            if (obj is Player)
            {
                Player other = (Player) obj;
                return (other.Id == Id);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}