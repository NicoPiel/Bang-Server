using System.Collections.Generic;

namespace BangServer.game
{
    public class Player
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public bool ReadyCheck { get; set; }
        
        public int Range { get; set; }
        public int Distance{ get; set; }
        public int Health{ get; set; }
        public int MaxHealth{ get; set; }

        public bool TheirTurn { get; set; }
        
        public string Role { get; set; }
        public string Character { get; set; }
        
        public List<string> Hand { get; }
        
        public Player(int id)
        {
            Id = id;
            Username = null;
            ReadyCheck = false;

            Range = 1;
            Distance = 1;
            MaxHealth = 0;
            Health = MaxHealth;

            Role = string.Empty;
            Character = string.Empty;
            
            Hand = new List<string>();
        }

        public void SetUsername(string username)
        {
            Username = username;
        }

        public void AssignRole(string role)
        {
            Role = role;
        }

        public void AssignCharacter(string character)
        {
            Character = character;
        }

        public void Ready()
        {
            ReadyCheck = true;
        }

        public void Unready()
        {
            ReadyCheck = false;
        }

        public int CardsInHand()
        {
            return Hand.Count;
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

        public override string ToString()
        {
            return Username;
        }
    }
}