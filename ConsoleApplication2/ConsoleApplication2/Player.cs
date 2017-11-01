using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFL_Targets
{
    class Player
    {
        public string name { get; set; }
        public string team { get; set; }
        public string position { get; set; }
        public Dictionary<int, string> targets { get; set; }
        public Dictionary<int, string> targetCompletions { get; set; }
        public Dictionary<int, string> TDs { get; set; }
        public Dictionary<int, string> rzTargets { get; set; }
        public Dictionary<int, string> rzTargetCompletions { get; set; }
        public Dictionary<int, string> rzTDs { get; set; }
        public Dictionary<int, string> rzRushes { get; set; }
        public Dictionary<int, string> rzRushTDs { get; set; }

        public Player(string _name)
        {
            name = _name;
            name = "";
            team = "";
            position = "";
            targets = new Dictionary<int, string>();
            targetCompletions = new Dictionary<int, string>();
            TDs = new Dictionary<int, string>();
            rzTargets = new Dictionary<int, string>();
            rzTargetCompletions = new Dictionary<int, string>();
            rzTDs = new Dictionary<int, string>();
            rzRushes = new Dictionary<int, string>();
            rzRushTDs = new Dictionary<int, string>();
        }

        public Player(string _name, string _team, string _position)
        {
            name = _name;
            team = _team;
            position = _position;
            targets = new Dictionary<int, string>();
            targetCompletions = new Dictionary<int, string>();
            TDs = new Dictionary<int, string>();
            rzTargets = new Dictionary<int, string>();
            rzTargetCompletions = new Dictionary<int, string>();
            rzTDs = new Dictionary<int, string>();
            rzRushes = new Dictionary<int, string>();
            rzRushTDs = new Dictionary<int, string>();
        }

        internal void addPassStats(string type, int week, string com, string tar, string tds)
        {
            if(type == "redzone")
            {
                rzTargets.Add(week, tar);
                rzTargetCompletions.Add(week, com);
                rzTDs.Add(week, tds);
            }
            else if(type == "all")
            {
                targets.Add(week, tar);
                targetCompletions.Add(week, com);
                TDs.Add(week, tds);
            }
        }

        internal void addRushStats(string type, int week, string rus, string tds)
        {
            rzRushes.Add(week, rus);
            rzRushTDs.Add(week, tds);
        }
    }
}
