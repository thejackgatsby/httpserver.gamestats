﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.GameStats.Server.Models
{
    public class PlayerScore
    {
        public PlayerScore()
        {
            
        }

        public string Name { get; set; }

        public int Frags { get; set; }

        public int Kills { get; set; }

        public int Deaths { get; set; }
    }
}
