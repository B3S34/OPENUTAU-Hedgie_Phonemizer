using OpenUtau.Api;
using OpenUtau.Core.Ustx;
using System.Linq;
using System.Collections.Generic;
using System;
using Acornima;
using System.ComponentModel.Design;

namespace ThaiHedgieVCCV {
    /// <summary>
    /// The simplest Phonemizer possible. Simply pass the lyric as phoneme.
    /// </summary>
    [Phonemizer("Thai Hedgies VCCV Phonemizer", "TH Hedgie", "Hedgie Foxity", language: "TH")]
    public class ThaiCustomCVVCPhonemizer : Phonemizer {
        static Dictionary<string, string> CodaList = new Dictionary<string, string>
        {
            {"g","ก"},{"d","ด"},{"b","บ"},{"n","น"},{"m","ม"},{"y","ย"},{"w","ว"},{"ng","ง"},
            {"ch","ฉ"},{"f","ฟ"},{"h","ห"},{"j","จ"},{"k","ค"},{"l","ล"},{"p","ป"},{"ph","พ"},{"r","ร"},{"rr","รร"},{"s","ส"},
            {"sh","ช"},{"t","ต"},{"th","ท"},{"ts","ตส"},{"v","วฟ"},{"z","ซ"}
        };
        static Dictionary<string, string[]> ClusterList = new Dictionary<string, string[]>
        {
            {"gr",["g","r"]},{"kr",["k","r"]},{"tr",["t","r"]},{"thr",["th","r"]},{"pr",["p","r"]},{"phr",["ph","r"]},{"gl",["g","l"]},{"kl",["k","l"]},{"pl",["p","l"]},{"phl",["ph","l"]},{"gw",["g","w"]},{"kw",["k","w"]},
            {"prr",["p","rr"]},{"phrr",["ph","rr"]},{"brr",["b","rr"]},{"frr",["f","rr"]},{"thrr",["th","rr"]},{"trr",["t","rr"]},{"drr",["d","rr"]},{"shrr",["sh","rr"]},{"krr",["k","rr"]},{"grr",["g","rr"]},{"vrr",["v","rr"]},
            {"br",["b","r"]},{"fr",["f","r"]},{"dr",["d","r"]},{"shr",["sh","r"]},{"vr",["v","r"]},
            {"bl",["b","l"]},{"fl",["f","l"]},{"sl",["s","l"]},
            {"thw",["th","w"]},{"dw",["d","w"]},{"sw",["s","w"]},{"tw",["t","w"]},{"vw",["v","w"]},
            {"by",["b","y"]},{"dy",["d","y"]},{"fy",["f","y"]},{"gy",["g","y"]},{"ky",["k","y"]},{"phy",["ph","y"]},{"py",["p","y"]},{"my",["m","y"]},{"ny",["n","y"]},{"vy",["v","y"]},{"zy",["z","y"]},
            {"sp",["s","p"]},{"sph",["s","ph"]},{"st",["s","t"]},{"sth",["s","th"]},{"sm",["s","m"]},{"sn",["s","n"]},{"sf",["s","f"]},
            {"spr",["s","r"]},{"sphr",["s","r"]},{"str",["s","r"]},{"sthr",["s","r"]},{"skr",["s","r"]},
            {"sprr",["s","rr"]},{"sphrr",["s","rr"]},{"strr",["s","rr"]},{"sthrr",["s","rr"]},{"skrr",["s","rr"]},
            {"spl",["s","l"]},{"sphl",["s","l"]},{"skl",["s","l"]},
            {"stw",["s","w"]},{"sthw",["s","w"]},{"skw",["s","w"]},
            {"sky",["s","y"]},{"spy",["s","y"]},{"sphy",["s","y"]},
        };
        static readonly string VowelList = "aiueoAEOU";
        static string[] PhonemeCuts(string? lyric) {
            // Cutting String to each phoneme (Include Cluster)
            if (string.IsNullOrEmpty(lyric))
                return ["-"];
            List<string> phonemes = [];
            string current_input = "";
            int cur_mode = 0;
            foreach (char nowchar in lyric) {
                if (cur_mode % 2 == 0) // Current is Consonant Mode
                {
                    if (VowelList.IndexOf(nowchar) != -1) // Its actually Vowel
                    {
                        cur_mode++;
                        if (current_input != "") {
                            phonemes.Add(current_input);
                            current_input = "";
                        }
                        phonemes.Add(nowchar.ToString());
                    } else // Its actually Consonant
                      {
                        current_input = $"{current_input}{nowchar}";
                    }
                } else // Current Is Vowel Mode
                  {
                    if (VowelList.IndexOf(nowchar) != -1) // Its actually Vowel
                    {
                        phonemes.Add(nowchar.ToString());
                    } else // Its actually Consonant
                      {
                        current_input = nowchar.ToString();
                        cur_mode++;
                    }
                }
            }
            if (cur_mode % 2 == 0) {
                phonemes.Add(current_input);
            }
            return phonemes.ToArray();
        }
        private USinger singer;
        public override void SetSinger(USinger singer) => this.singer = singer;
        public override Result Process(Note[] notes, Note? prev, Note? next, Note? prevNeighbour, Note? nextNeighbour, Note[] prevNeighbours) {
            // Note that even when input has multiple notes, only the leading note is used to produce phoneme.
            // This is because the 2nd+ notes will always be extender notes, i.e., with lyric "+" or "+<number>".
            // For this simple phonemizer, all these notes maps to a single phoneme.
            string alias;
            var attr0 = notes[0].phonemeAttributes?.FirstOrDefault(attr => attr.index == 0) ?? default;
            string color = attr0.voiceColor ?? GetParentVoiceColor();
            int shift = attr0.toneShift ?? GetParentToneShift();
            int? alt = attr0.alternate ?? GetParentAlternate();
            List<Tuple<string, int>> Lyrics = [];
            // Maybe Some actual work here
            // 1. Part Cropping (Consonant)(Vowel)(Coda)
            string[] NowPhonemes = PhonemeCuts(notes[0].lyric);
            string[] PrevPhonemes = ["-"];
            string[] NextPhonemes = ["-"];
            if (prevNeighbour != null)
                PrevPhonemes = PhonemeCuts(prevNeighbour.Value.lyric);
            if (nextNeighbour != null)
                NextPhonemes = PhonemeCuts(nextNeighbour.Value.lyric);
            // 2.1 Last Prev-Now (Prev C to Now C, CC else C.V)
            bool first_have_solo = false;
            string solo = "";
            int CurrentTick = 0;
            for (int phoID = 0; phoID < NowPhonemes.Length; phoID++) {
                if (phoID == 0) // Is first of phonemes
                {
                    if (PrevPhonemes[PrevPhonemes.Length - 1] == "-") // No Previous Neighbor
                    {
                        if (VowelList.IndexOf(NowPhonemes[0]) != -1) // First is Vowel (-.V・)
                        {
                            Lyrics.Add(($"-.{NowPhonemes[0]}・", 0).ToTuple());
                            CurrentTick += 120;
                        } else if (singer.TryGetMappedOto($"-.{NowPhonemes[0]}{NowPhonemes[1]}" + alt, notes[0].tone + shift, color, out var oto)) // (- CV) Exist
                          {
                            solo = $"-.{NowPhonemes[0]}{NowPhonemes[1]}";
                            first_have_solo = true;
                            continue;
                        } else // First is C/CC 
                          {
                            if (ClusterList.ContainsKey(NowPhonemes[0])) // is CC 
                                Lyrics.Add(($"-.{NowPhonemes[0]}", -60).ToTuple());
                            else // Is just C
                                Lyrics.Add(($"- {NowPhonemes[0]}", -60).ToTuple());
                        }
                    } else // Have Previous Neighbor
                      {
                        if (VowelList.IndexOf(NowPhonemes[0]) != -1) // First is Vowel (V.V・) / (X.V)
                        {
                            if (VowelList.IndexOf(PrevPhonemes[PrevPhonemes.Length - 1]) == -1)
                                Lyrics.Add(($"{PrevPhonemes[PrevPhonemes.Length - 1]}.{NowPhonemes[0]}", 0).ToTuple());
                            else
                                Lyrics.Add(($"{PrevPhonemes[PrevPhonemes.Length - 1]}.{NowPhonemes[0]}・", 0).ToTuple());
                            CurrentTick += 120;
                        } else // First is C/CC s
                        {
                            if (ClusterList.ContainsKey(NowPhonemes[0])) // is CC 
                            {
                                Lyrics.Add(($"{PrevPhonemes[PrevPhonemes.Length - 1]} {ClusterList.GetValueOrDefault(NowPhonemes[0])[0]}", -90).ToTuple());
                                Lyrics.Add(($"{NowPhonemes[0]}", -30).ToTuple());
                            } else // is just C
                              {
                                if (CodaList.ContainsKey(NowPhonemes[0]))
                                    Lyrics.Add(($"{PrevPhonemes[PrevPhonemes.Length - 1]} {NowPhonemes[0]}", -60).ToTuple());
                                else
                                    Lyrics.Add(($"{PrevPhonemes[PrevPhonemes.Length - 1]}.{NowPhonemes[0]}", 0).ToTuple());
                            }
                        }
                    }
                } else {
                    if (phoID == 1 && first_have_solo) // No Previous Neighbor & Have (- CV)
                    {
                        Lyrics.Add((solo, 0).ToTuple());
                        CurrentTick += 240;
                        continue;
                    } else // Basic CV,VV,VC,CC
                      {
                        if (VowelList.IndexOf(NowPhonemes[phoID]) != -1) // Is Vowel (CV / VV)
                        {
                            if (VowelList.IndexOf(NowPhonemes[phoID - 1]) != -1) // It's VV
                            {
                                Lyrics.Add(($"{NowPhonemes[phoID - 1]}.{NowPhonemes[phoID]}", CurrentTick).ToTuple());
                            } else // It's CV / CCV
                              {
                                if (!ClusterList.ContainsKey(NowPhonemes[phoID - 1])) // It's CV
                                    Lyrics.Add(($"{NowPhonemes[phoID - 1]}{NowPhonemes[phoID]}", CurrentTick).ToTuple());
                                else // It's CCV -> CV
                                    Lyrics.Add(($"{ClusterList.GetValueOrDefault(NowPhonemes[phoID - 1], ["r", "r"])[1]}{NowPhonemes[phoID]}", CurrentTick).ToTuple());
                            }
                        } else // Current is not Vowel (VC / CC)
                          {
                            if (VowelList.IndexOf(NowPhonemes[phoID - 1]) != -1) // Is VC
                            {
                                // Coda Timing Calculation
                                int CodaTime = 60;
                                if (nextNeighbour != null)
                                    if (ClusterList.ContainsKey(NextPhonemes[0]))
                                        CodaTime = 90;
                                // Alias Placing
                                // Math.Min(Math.Max(C,A),B) // A < C < B // V-C < 
                                if (phoID == NowPhonemes.Length - 1 && nextNeighbour == null)
                                    Lyrics.Add(($"{NowPhonemes[phoID - 1]}.{CodaList.GetValueOrDefault(NowPhonemes[phoID])}-", Math.Min(Math.Max(notes[0].duration - CodaTime, CurrentTick + 90), notes[0].duration - CodaTime)).ToTuple());
                                else
                                    Lyrics.Add(($"{NowPhonemes[phoID - 1]}.{CodaList.GetValueOrDefault(NowPhonemes[phoID])}", Math.Min(Math.Max(notes[0].duration - CodaTime, CurrentTick + 90), notes[0].duration - CodaTime)).ToTuple());
                                CurrentTick -= 120;
                            } else {
                                if (phoID == NowPhonemes.Length - 1 && nextNeighbour == null)
                                    Lyrics.Add(($"{NowPhonemes[phoID - 1]} {NowPhonemes[phoID]}-", CurrentTick).ToTuple());
                                else
                                    Lyrics.Add(($"{NowPhonemes[phoID - 1]} {NowPhonemes[phoID]}", CurrentTick).ToTuple());

                            }
                        }
                        CurrentTick += 120;
                    }
                }
            }
            // 2.3 If there is no Next Neighbor
            //if(nextNeighbour == null) {
            //    Lyrics[Lyrics.Count() - 1].Item1 = $"{Lyrics[Lyrics.Count() - 1].Item1}";
            //}
            // 2.4 Creating List of Result
            Result Otolist = new Result {
                phonemes = new Phoneme[Lyrics.Count()]
            };
            for (int phoID = 0; phoID < Lyrics.Count(); phoID++) {
                if (singer.TryGetMappedOto(Lyrics[phoID].Item1 + alt, notes[0].tone + shift, color, out var oto)) {
                    alias = oto.Alias;
                } else {
                    alias = Lyrics[phoID].Item1;
                }
                Otolist.phonemes[phoID] = new Phoneme { phoneme = alias, position = Lyrics[phoID].Item2 };
            }
            return Otolist;
        }
    }
}
