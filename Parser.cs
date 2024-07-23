namespace TiskStitku
{
    class Parser
    {
        public void Process(ref EplPrikaz eplPrikaz)
        {
            eplPrikaz.Telo = FillOutTemplate(eplPrikaz.Sablona);
        }
        private string FillOutTemplate(string template)
        {
            string eplString=string.Empty;
            
            return eplString;
            // SouborUloh souborUloh = new SouborUloh();
            // List<Uloha> ListUloh = souborUloh.GenerujListUloh(template);
            // int pocetStitku = 1;
            // //list obsahujicí jeden člen pokud se dotaz.Otazka najde v listu Dotazy.Data
            // List<Uloha> dataVyhledana = new List<Uloha>();
            // foreach (Uloha uloha in ListUloh) //ziskani odpovedi
            // {
            //     //nejprve se hleda odpoved v souboru s daty (Dotazy.Data)
            //     if (SouborUloh.Data != null)
            //         dataVyhledana = SouborUloh.Data.Where(x => x.Zadani.Equals(uloha.Zadani.ToLower())).ToList();
            //     if (dataVyhledana.Count != 0)
            //     {
            //         uloha.Vysledek = dataVyhledana[0].Vysledek;
            //     }
            //     //sablona s P na konci
            //     else if (uloha.Zadani == "počet štítků")
            //     {
            //         pocetStitku = UzivRozhrani.VratCislo(" Tisk štítků na EPL tiskárně", " Tisk šablony " + Path.GetFileName(AdresaSouboru), " Zadej počet štítků od 1 do 20 (přednastaveno 1): ", 0, 20, 1);
            //         uloha.Vysledek = pocetStitku.ToString();
            //     }
            //     //otazka typu <pocet|12> prednastaveny pocet 12
            //     else if (uloha.Zadani.Contains("pocet|"))
            //     {
            //         string[] pole = uloha.Zadani.Split('|');
            //         int prednastPocet = 1;
            //         int.TryParse(pole[1], out prednastPocet);
            //         pocetStitku = UzivRozhrani.VratCislo(" Tisk štítků na EPL tiskárně", " Tisk šablony " + Path.GetFileName(AdresaSouboru), " Zadej počet štítků od 1 do 30 (přednastaveno " + prednastPocet + "): ", 0, 30, prednastPocet);
            //         uloha.Vysledek = pocetStitku.ToString();
            //     }
            //     //uzivatel
            //     else if (Configuration.Prihlasit && uloha.Zadani == "uzivatel")
            //     {
            //         uloha.Vysledek = Configuration.Uzivatel;
            //     }
            //     //dotaz na datum nebo cas
            //     else if (DatumNCas(uloha.Zadani))
            //     {
            //         string otazka = uloha.Zadani;
            //         if (otazka.Contains('|'))//otazka hlidajici prekroceni expirace sarze
            //         {
            //             string dotazNaExpiraci = otazka.Split('|')[1];
            //             DateTime expirace;
            //             string zjistenaExpirace = "";
            //             if (!DateTime.TryParse(dotazNaExpiraci, out expirace))
            //             /*{
			// 				zjistenaExpirace = expirace.ToString("dd.MM.yyyy");
			// 			}
			// 			else*/
            //             {
            //                 if (SouborUloh.Data != null)
            //                     dataVyhledana = SouborUloh.Data.Where(x => x.Zadani.Equals(dotazNaExpiraci.ToLower())).ToList();
            //                 if (dataVyhledana.Count != 0)
            //                 {
            //                     zjistenaExpirace = dataVyhledana[0].Vysledek;
            //                     otazka = otazka.Replace(dotazNaExpiraci, zjistenaExpirace);
            //                 }
            //                 else
            //                 {
            //                     zjistenaExpirace = DateTime.MaxValue.ToString();
            //                     otazka = otazka.Replace(dotazNaExpiraci, zjistenaExpirace);
            //                 }
            //             }
            //         }
            //         uloha.Vysledek = VratDatumNCas(otazka);
            //         if (uloha.Vysledek == "netisknout") return 1;
            //     }
            //     else if (uloha.Zadani == "GS1")
            //     {
            //         uloha.Vysledek = "\u001D";
            //     }
            //     //pokud se nenajde prednastavena odpoved polozi se otazka uzivateli
            //     else
            //     {
            //         uloha.Vysledek = UzivRozhrani.VratText(" Tisk štítků na EPL tiskárně", " Tisk šablony " + Path.GetFileName(AdresaSouboru), " Zadej " + uloha.Zadani + ": ", "");
            //     }
            // }
            // foreach (Uloha dotaz in ListUloh) //vepsani odpovedi do sablony
            // {
            //     if (dotaz.Zadani == "počet štítků")
            //         template = template.TrimEnd(new char[] { '\r', '\n' }) + dotaz.Vysledek + Environment.NewLine;
            //     else template = template.Replace("<" + dotaz.Zadani + ">", dotaz.Vysledek);
            // }
            // if (pocetStitku != 0) return 0; //pokud je pocet stitku nenulovy
            // else return 1; //jinak vraci 1
        }
        //Pomocna funkce, ktera vyhodnoti zda se otazka pta na cas|datum 
        private bool DatumNCas(string otazka)
        {
            bool nh = false;
            otazka = otazka.Split('+')[0];
            otazka = otazka.ToLower();
            if (otazka.Equals("date") || otazka.Equals("time"))
                nh = true;
            return nh;
        }
        //Pomocna funkce, ktera vraci odpoved na casovou otazku
        private string VratDatumNCas(string otazka)
        {
            string odpoved;
            string casTyp;
            double Posun = 0;
            DateTime expiraceSarze = DateTime.MaxValue;
            string[] castiOtazky = otazka.Split(new char[] { '+', '|' });
            casTyp = castiOtazky[0];
            if (castiOtazky.Length > 1) //Cas s posunem
            {
                if (!double.TryParse(castiOtazky[1], out Posun))
                    Posun = UzivRozhrani.VratCislo(" Tisk štítků na EPL tiskárně", " Tisk šablony " + Path.GetFileName(AdresaSouboru), " Zadej " + castiOtazky[1] + ": ", 0, int.MaxValue, 0);
            }
            if (castiOtazky.Length > 2) //Cas s posunem a expiraci
                if (!DateTime.TryParse(castiOtazky[2], out expiraceSarze)) expiraceSarze = DateTime.MaxValue;
            DateTime expiraceVypoctena = DateTime.Now;
            if (casTyp.Equals("date")) //vyhodnoceni dotazu na datum
            {
                expiraceVypoctena = expiraceVypoctena.AddDays(Posun);

                if (expiraceVypoctena < expiraceSarze) //materiál před expirací
                {
                    odpoved = expiraceVypoctena.ToString("d.M.yyyy");
                }
                else if (DateTime.Now > expiraceSarze) //proexpirovany material
                {
                    odpoved = "netisknout";
                    UzivRozhrani.OznameniChyby(
                        " Tisk štítků na EPL tiskárně",
                        string.Format(" Tisk šablony {0} " + Environment.NewLine + " Materiál je proexpirovaný, expirace {1}," + Environment.NewLine + " štítek se nevytiskne!", Path.GetFileName(AdresaSouboru), expiraceSarze.ToString("d.M.yyyy")),
                        " Pokračuj stisknutím libovolné klávesy...");
                }
                else //material ktery expiruje drive nez je trvanlivost po otevreni
                {
                    odpoved = expiraceSarze.ToString("d.M.yyyy");
                    UzivRozhrani.OznameniChyby(
                        " Tisk štítků na EPL tiskárně",
                        string.Format(" Tisk šablony {0}" + Environment.NewLine + " Materiál expiruje již za {1} dní ({2})!", Path.GetFileName(AdresaSouboru), (expiraceSarze - DateTime.Today).Days, expiraceSarze.ToString("d.M.yyyy")),
                        " Pokračuj stisknutím libovolné klávesy...");
                }
            }
            else odpoved = expiraceVypoctena.AddMinutes(Posun).ToString("H:mm"); //dotaz na cas
            return odpoved;
        }
    }
}