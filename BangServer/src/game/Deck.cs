using System;
using System.Collections.Generic;
using BangServer.util;

namespace BangServer.game
{
    public class Deck : Stack<string>
    {
        public List<string> CardList { get; set; }

        public Deck()
        {
            Setup();
        }

        public string Draw()
        {
            if (CardsRemaining() > 0) return Pop();
            
            Setup();
            return Pop();
        }

        public List<string> DrawMultiple(int number)
        {
            List<string> draw = new List<string>();

            for (int i = 0; i < number; i++)
            {
                if (CardsRemaining() > 0) draw.Add(Pop());
                else
                {
                    Setup();
                    draw.Add(Pop());
                }
            }

            return draw;
        }

        private void Setup()
        {
            CardList = new List<string>();
            
            AddCardsToDeck();
            CardList.Shuffle();

            foreach (string card in CardList)
            {
                Push(card);
            }
        }

        private void AddCardsToDeck()
        {
            CardList.AddRange(new []
            {
                "BANG!_K_2",
                "BANG!_K_3",
                "BANG!_K_4",
                "BANG!_K_5",
                "BANG!_K_6",
                "BANG!_K_7",
                "BANG!_K_8",
                "BANG!_K_9",
                "BANG!_C_2",
                "BANG!_C_3",
                "BANG!_C_4",
                "BANG!_C_5",
                "BANG!_C_6",
                "BANG!_C_7",
                "BANG!_C_8",
                "BANG!_C_9",
                "BANG!_C_10",
                "BANG!_C_J",
                "BANG!_C_Q",
                "BANG!_C_K",
                "BANG!_C_A",
                "BANG!_H_Q",
                "BANG!_H_K",
                "BANG!_H_A",
                "BANG!_P_A",
                "Missed!_P_10",
                "Missed!_P_J",
                "Missed!_P_Q",
                "Missed!_P_K",
                "Missed!_P_A",
                "Missed!_P_2",
                "Missed!_P_3",
                "Missed!_P_4",
                "Missed!_P_5",
                "Missed!_P_6",
                "Missed!_P_7",
                "Missed!_P_8",
                "Beer_H_6",
                "Beer_H_7",
                "Beer_H_8",
                "Beer_H_9",
                "Beer_H_10",
                "Beer_H_J",
                "Saloon_H_5",
                "Wells Fargo_H_3",
                "Diligenza_P_9",
                "Diligenza_P_9",
                "General Store_K_9",
                "General Store_P_Q",
                "Panic!_H_J",
                "Panic!_H_Q",
                "Panic!_H_A",
                "Panic!_C_8",
                "Cat Balou_C_9",
                "Cat Balou_C_10",
                "Cat Balou_C_J",
                "Cat Balou_H_K",
                "Indians!_C_K",
                "Indians!_C_A",
                "Duel_C_Q",
                "Duel_P_J",
                "Duel_K_8",
                "Gatling_H_10",
                "Mustang_H_8",
                "Mustang_H_9",
                "Appaloosa/Scope_P_A",
                "Barrel_P_Q",
                "Barrel_P_K",
                "Dynamite_H_2",
                "Jail_P_10",
                "Jail_P_J",
                "Jail_H_4",
                "Volcanic_P_10",
                "Volcanic_K_10",
                "Schofield_K_J",
                "Schofield_K_Q",
                "Schofield_P_A",
                "Remington_K_K",
                "Rev.Carbine_K_A",
                "Winchester_P_8"
            });
        }

        public int CardsRemaining()
        {
            return this.Count;
        }
    }
}