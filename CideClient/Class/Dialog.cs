using System;
using System.Collections.Generic;
using CideClient.Structures;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace CideClient.Class
{
    public class Dialog
    {
        private FleetCarrier FC;
        public Dialog(FleetCarrier FleetCarrierInstance)
        {
            FC = FleetCarrierInstance;
        }

        public void initForUser()
        {
            string init = @"Vítejte v aplikaci CideClient, která je namíru vyvíjena pro účely Civitas Dei. Autor programu si vyhrazuje veškerá práva na tento software. Discord Tag Autora: arwwarr#8907.
            
            Tento software zaznamenává a odesílá veškeré události okolo vašeho Fleet Carrieru (FC).
            
            Než budete pokračovat, silně doporučuji abyste si zálohovali a smazali Logy ze hry (poté restartujte ED).
            Pokud tak neučiníte, mužete čekat až několik desítek minut, než se program inicializuje.
            
            Poté co se celý program inicializuje, musíte zadockovat do vašeho FC 
            a kliknout na Management FC. Toto je potřeba pro prvotní načtení a uložení FC do databáze.
            ";
            Console.WriteLine(init);
            Console.WriteLine("Stiskni libovolné tlačítko pro pokračovaí.");
            Console.ReadLine();
        }

        public int ChoiceMenu()
        {
            Console.WriteLine("Vyberte operační status: ");
            Console.WriteLine("1) Konstatní hlašení o stavu. Bez jasné destinace.");
            Console.WriteLine("2) Příprava na cestu. Stání na jednom místě účelně pro zadockovaní lidí a svoz paliva na cestu.");
            Console.WriteLine("3) Naplánovat si cestu z bodu A do bodu B. AUTOPILOT.");
            Console.Write("\r\nNapište číslo volby: ");
            switch (Console.ReadLine())
            {
                case "1":
                    ChoiceOne();
                    return 1;
                case "2":
                    ChoiceTwo();
                    return 2;
                case "3":
                    ChoiceThree();
                    return 3;
                default:
                    return ChoiceMenu();
            }
        }

        public void ChoiceOne()
        {
            FC.StatusMode = 1;
            Analytics.TrackEvent("ChoiceMenu", new Dictionary<string, string> 
            {
                    { "Mode", "1" }
            });
        }

        public void ChoiceTwo()
        {
            // Name for Destination >> for discord
            Console.Write("\r\nNapište název systému, kam plánujete letět: ");
            string NameSystem = Console.ReadLine();
            // jak dlouho bude carrier na místě >>doba odletu
            Console.WriteLine("Ve formátu hh:mm-dd-mm-yyyy >> hodina:minuta-den-mesic-rok.");
            Console.WriteLine("Příklad: 20:00-24-12-2020 - tedy 20 hodin 0 minut 24. den v měsíci, 12. měsíc, rok 2020");
            Console.Write("\r\nNapiš, kdy plánuješ letět do cílového systému: ");
            string DatetoStart = Console.ReadLine();
            
            FC.NextJump = NameSystem;
            FC.Destination = NameSystem;
            FC.NextJumpDateTime = DatetoStart;
            FC.StatusMode = 2;
            // it doesn metter how many
            FC.FuelInMarket = 999999;

            Analytics.TrackEvent("ChoiceMenu", new Dictionary<string, string> 
            {
                    { "Mode", "2" },
                    { "NameSystem", NameSystem },
                    { "DatetoStart", DatetoStart }
            });
        }

        public void ChoiceThree()
        {
            Console.WriteLine("jak mile se program inicializuje, béžte do fc managmentu");
            Console.WriteLine("Stiskni libovolné tlačítko pro pokračovaí.");
            Console.ReadLine();
            FC.StatusMode = 3;
            Analytics.TrackEvent("ChoiceMenu", new Dictionary<string, string> 
            {
                    { "Mode", "3" }
            });
        }

        public void ChoicePostThree()
        {
            Console.Write("\r\nNapište název systému, kam chcete letět: ");
            string NameSystem = Console.ReadLine();
            FC.Destination = NameSystem;
            QuestionFuelInMarket();

            Analytics.TrackEvent("ChoicePostThree", new Dictionary<string, string> 
            {
                    { "Destination", NameSystem }
            });

            Console.WriteLine("Stiskni libovolné tlačítko pro pokračovaí > vypočitaní trasy.");
            Console.ReadLine();
        }

        public void QuestionFuelInMarket()
        {
            Console.Write("\r\nNapiš, kolik paliva je právě k dispozici uvnitř FC marketu: ");
            long MarketFuel = (long)Convert.ToDouble(Console.ReadLine());
            FC.FuelInMarket = MarketFuel;

            Analytics.TrackEvent("QuestionFuelInMarket", new Dictionary<string, string> 
            {
                    { "Destination", MarketFuel.ToString() }
            });
        }

        public int ChoiceMenuFirstInitAutoPilotError()
        {
            Console.WriteLine("Nemáš potřebny počet paliva pro route skoku");
            Console.WriteLine("Chceš i přesto pokračovat?");
            Console.WriteLine("1. Ano");
            Console.WriteLine("2. Ne");
            Console.Write("\r\nNapište číslo volby: ");
            switch (Console.ReadLine())
            {
                case "1":
                    return FirstInitAutoPilotErrorDialogYes();
                case "2":
                    return FirstInitAutoPilotErrorDialogNo();
                default:
                    return ChoiceMenuFirstInitAutoPilotError();
            }
        }

        private int FirstInitAutoPilotErrorDialogNo()
        {
            Analytics.TrackEvent("ChoiceMenuFirstInitAutoPilotError", new Dictionary<string, string> 
            {
                    { "Answer", "No" }
            });
            return 2;
        }

        private int FirstInitAutoPilotErrorDialogYes()
        {
            Analytics.TrackEvent("ChoiceMenuFirstInitAutoPilotError", new Dictionary<string, string> 
            {
                    { "Answer", "Yes" }
            });
            return 1;
        }

        public int ChoiceMenuFirstInitAutoPilotRefuellingMode()
        {
            Console.WriteLine("vyber si režim dolplnovaní paliva");
            Console.WriteLine("1. ze transfer cargo -- musi tam byt jen samotné trittium");
            Console.WriteLine("2. ze fc marketu -- musi tam byt jen samotné tritium");
            Console.WriteLine("3. sám v 3.5 minutém intervalu, fc si to palivo jen jdá do sebe když bude potřebovat");
            Console.WriteLine("4. jak mile nebude palivo, fc odejde ze managmentu a uzivatel sám musi doplnit palivo a jit zovu do fc managmentu");
            Console.Write("\r\nNapište číslo volby: ");
            switch (Console.ReadLine())
            {
                case "1":
                    return ChoiceMenuFirstInitAutoPilotRefuellingModeOne();
                case "2":
                    return ChoiceMenuFirstInitAutoPilotRefuellingModeTwo();
                case "3":
                    return ChoiceMenuFirstInitAutoPilotRefuellingModeThree();
                case "4":
                    return ChoiceMenuFirstInitAutoPilotRefuellingModeFour();
                default:
                    return ChoiceMenuFirstInitAutoPilotRefuellingMode();
            }
        }

        
        private int ChoiceMenuFirstInitAutoPilotRefuellingModeOne()
        {
            Analytics.TrackEvent("ChoiceMenuFirstInitAutoPilotRefuellingMode", new Dictionary<string, string> 
            {
                    { "Mode", "CargoTransfer" }
            });
            FC.AutoPilotRefuelingMode = 1;
            return 1;
        }

        private int ChoiceMenuFirstInitAutoPilotRefuellingModeTwo()
        {
            Analytics.TrackEvent("ChoiceMenuFirstInitAutoPilotRefuellingMode", new Dictionary<string, string> 
            {
                    { "Mode", "MarketTransfer" }
            });
            FC.AutoPilotRefuelingMode = 2;
            return 2;
        }

        private int ChoiceMenuFirstInitAutoPilotRefuellingModeThree()
        {
            Analytics.TrackEvent("ChoiceMenuFirstInitAutoPilotRefuellingMode", new Dictionary<string, string> 
            {
                    { "Mode", "UserSelfAndAutoAddFuel" }
            });
            FC.AutoPilotRefuelingMode = 3;
            return 3;
        }
        private int ChoiceMenuFirstInitAutoPilotRefuellingModeFour()
        {
            Analytics.TrackEvent("ChoiceMenuFirstInitAutoPilotRefuellingMode", new Dictionary<string, string> 
            {
                    { "Mode", "WaitForUser" }
            });
            FC.AutoPilotRefuelingMode = 4;
            return 4;
        }
    }
}
