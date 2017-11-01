using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParseNFLsavant
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] targetType = new string[] { "redzone", "all", "redzone_rush" };
            List<Player> players = new List<Player>();
            HtmlDocument doc;
            string html;
            string url;

            int currentWeek = getNFLWeeknumber();


            if (currentWeek > 0)
            {
                for (int week = 1; week <= currentWeek; week++)
                {
                    foreach(string type in targetType)
                    {

                        url = String.Format("http://nflsavant.com/targets.php?rz={0}&ddlYear=2017&week={1}", type, week);
                        using (var client = new WebClient())
                        {
                            html = client.DownloadString(url);
                        }

                        doc = new HtmlDocument();
                        doc.LoadHtml(html);
                        List<HtmlNode> x = doc.DocumentNode.SelectNodes("//tr").ToList();
                        foreach(HtmlNode node in x)
                        {
                            if(node.Id != "")
                            {
                                List<HtmlNode> stats = node.ChildNodes.Where(n => n.Name == "td").ToList();
                                string nam = "";
                                string tea = "";
                                string pos = "";
                                string com = "";
                                string tar = "";

                                string tds = "";
                                string rus = "";

                                nam = stats[1].InnerText;
                                tea = stats[2].InnerText;
                                pos = stats[3].InnerText;
                                if (type != "redzone_rush")
                                {
                                    com = stats[4].InnerText;
                                    tar = stats[5].InnerText;
                                    tds = stats[7].InnerText;
                                }

                                else
                                {
                                    rus = stats[4].InnerText;
                                    tds = stats[5].InnerText;
                                }



                                Player p = players.Where(plyr => plyr.name == nam).FirstOrDefault();
                                if(p == null)
                                {
                                    p = new Player(nam, tea, pos);
                                    players.Add(p);
                                }
                                if (type != "redzone_rush")
                                    p.addPassStats(type, week, com, tar, tds);
                                
                                else
                                    p.addRushStats(type, week, rus, tds);
                            }
                        }
                    }
                }
            }
            using(StreamWriter sw = new StreamWriter("test.txt"))
            {
                string[] goodPositions = new string[] {"RB", "FB", "WR", "TE" };
                sw.Write("Name\tPosition\tTeam\tRZ Rush\tRZ Rush TDs\tRZ Targets\tRZ Comp\tRZ %\tRZ TDs\tTargets\tComp\t%\tTDs");
                for (int week = 1; week <= currentWeek; week++)
                {
                    sw.Write(String.Format("\tRZ Rushes({0})\tRZ Rush TDs({0})\tRZ Targets({0})\tRZ Completions({0})\tRZ TDs({0})\tTargets({0})\tCompletions({0})\tTDs({0})", week));
                }
                sw.WriteLine();
                foreach (Player playa in players)
                {
                    if (goodPositions.Contains(playa.position))
                    {
                        int rzt = playa.rzTargets.Sum(x => { int result = 0; Int32.TryParse(x.Value, out result); return result; });
                        int rzc = playa.rzTargetCompletions.Sum(x => { int result = 0; Int32.TryParse(x.Value, out result); return result; });
                        double rzp = rzt == 0 ? 0 : (double)rzc / rzt;
                        int t = playa.targets.Sum(x => { int result = 0; Int32.TryParse(x.Value, out result); return result; });
                        int c = playa.targetCompletions.Sum(x => { int result = 0; Int32.TryParse(x.Value, out result); return result; });
                        double p = t == 0 ? 0 : (double)c / t;

                        sw.Write(playa.name);
                        sw.Write("\t");
                        sw.Write(playa.position);
                        sw.Write("\t");
                        sw.Write(playa.team);
                        sw.Write("\t");
                        sw.Write(playa.rzRushes.Sum(x => { int result = 0; Int32.TryParse(x.Value, out result); return result; }));
                        sw.Write("\t");
                        sw.Write(playa.rzRushTDs.Sum(x => { int result = 0; Int32.TryParse(x.Value, out result); return result; }));
                        sw.Write("\t");
                        sw.Write(rzt);
                        sw.Write("\t");
                        sw.Write(rzc);
                        sw.Write("\t");
                        sw.Write(rzp.ToString("0.##"));
                        sw.Write("\t");
                        sw.Write(playa.rzTDs.Sum(x => { int result = 0; Int32.TryParse(x.Value, out result); return result; }));
                        sw.Write("\t");
                        sw.Write(t);
                        sw.Write("\t");
                        sw.Write(c);
                        sw.Write("\t");
                        sw.Write(p.ToString("0.##"));
                        sw.Write("\t");
                        sw.Write(playa.TDs.Sum(x => { int result = 0; Int32.TryParse(x.Value, out result); return result; }));
                        sw.Write("\t");
                        for (int week = 1; week <= currentWeek; week++)
                        {
                            sw.Write(playa.rzRushes.ContainsKey(week) ? playa.rzRushes[week] : "");
                            sw.Write("\t");
                            sw.Write(playa.rzRushTDs.ContainsKey(week) ? playa.rzRushTDs[week] : "");
                            sw.Write("\t");
                            sw.Write(playa.rzTargets.ContainsKey(week) ? playa.rzTargets[week] : "");
                            sw.Write("\t");
                            sw.Write(playa.rzTargetCompletions.ContainsKey(week) ? playa.rzTargetCompletions[week] : "");
                            sw.Write("\t");
                            sw.Write(playa.rzTDs.ContainsKey(week) ? playa.rzTDs[week] : "");
                            sw.Write("\t");
                            sw.Write(playa.targets.ContainsKey(week) ? playa.targets[week] : "");
                            sw.Write("\t");
                            sw.Write(playa.targetCompletions.ContainsKey(week) ? playa.targetCompletions[week] : "");
                            sw.Write("\t");
                            sw.Write(playa.TDs.ContainsKey(week) ? playa.TDs[week] : "");
                            sw.Write("\t");
                        }
                        sw.WriteLine();
                    }
                }
            }
        }

        private static int getNFLWeeknumber()
        {
            int weeknumber = -1;
            DateTime today = DateTime.Now;
            DateTime week1 = new DateTime(2017, 9, 7);
            DateTime week2 = week1.AddDays(7);
            DateTime week3 = week2.AddDays(7);
            DateTime week4 = week3.AddDays(7);
            DateTime week5 = week4.AddDays(7);
            DateTime week6 = week5.AddDays(7);
            DateTime week7 = week6.AddDays(7);
            DateTime week8 = week7.AddDays(7);
            DateTime week9 = week8.AddDays(7);
            DateTime week10 = week9.AddDays(7);
            DateTime week11 = week10.AddDays(7);
            DateTime week12 = week11.AddDays(7);
            DateTime week13 = week12.AddDays(7);
            DateTime week14 = week13.AddDays(7);
            DateTime week15 = week14.AddDays(7);
            DateTime week16 = week15.AddDays(7);
            DateTime week17 = week16.AddDays(7);



            if (today.Date >= week17)
            {
                weeknumber = 17;
            }
            else if (today.Date >= week16)
            {
                weeknumber = 16;
            }
            else if (today.Date >= week15)
            {
                weeknumber = 15;
            }
            else if (today.Date >= week14)
            {
                weeknumber = 14;
            }
            else if (today.Date >= week13)
            {
                weeknumber = 13;
            }
            else if (today.Date >= week12)
            {
                weeknumber = 12;
            }
            else if (today.Date >= week11)
            {
                weeknumber = 11;
            }
            else if (today.Date >= week10)
            {
                weeknumber = 10;
            }
            else if (today.Date >= week9)
            {
                weeknumber = 9;
            }
            else if (today.Date >= week8)
            {
                weeknumber = 8;
            }
            else if (today.Date >= week7)
            {
                weeknumber = 7;
            }
            else if (today.Date >= week6)
            {
                weeknumber = 6;
            }
            else if (today.Date >= week5)
            {
                weeknumber = 5;
            }
            else if (today.Date >= week4)
            {
                weeknumber = 4;
            }
            else if (today.Date >= week3)
            {
                weeknumber = 3;
            }
            else if (today.Date >= week2)
            {
                weeknumber = 2;
            }
            else if (today.Date >= week1)
            {
                weeknumber = 1;
            }
            return weeknumber;
        }
    }
}
