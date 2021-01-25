using System;
using System.Threading.Tasks;
using EliteAPI.Abstractions;
using Microsoft.Extensions.Logging;
using CideClient.Structures;
using CideClient.Api;
using CideClient.Class;
using Newtonsoft.Json;

namespace CideClient
{
    // Core class of our application
    public class Core
    {
        private readonly IEliteDangerousApi _api;
        private readonly ILogger<Core> _log;
        private FleetCarrier FC;
        private Dialog ConsoleDialog;
        

        public Core(ILogger<Core> log, IEliteDangerousApi api)
        {
            // Get our dependencies through dependency injection
            _log = log;
            _api = api;
            FC = new FleetCarrier();
            ConsoleDialog = new Dialog(FC);
            ConsoleDialog.initForUser();
            ConsoleDialog.ChoiceMenu();
        }

        public async Task Run()
        {

            _api.Events.CommanderEvent += (sender, e) =>
            {
                // Return if not this session
                //if (!_api.HasCatchedUp) { return; }
        
                _log.LogInformation("Jméno hráče zaznamenáno.");
                FC.OwnerCmdr = e.Name;
            };

            // if player docked on carrier
            _api.Events.DockedEvent += (sender, e) =>
            {
                if (e.StationType == "FleetCarrier")
                {
                    _log.LogInformation("Dockování na Carrieru zaznamenáno.");
                    FC.Location = e.StarSystem;
                    FC.send();
                }
            };

            // if player open carrier managment menu
            _api.Events.CarrierStatsEvent += (sender, e) =>
            {
                // Return if not this session
                if (!_api.HasCatchedUp) { return; }
                bool FirstState = false;
                bool FirstStateStatusThree = false;
                bool FirstRunAutoPilot = false;

                if (FC.Id == null)
                {
                    _log.LogInformation($"Nastavuji výchozí Carrier na jméno {e.Name} a uzamykám.");
                    FC.Id = e.CarrierId.ToString(); 
                    if (FC.StatusMode == 2)
                    {
                        FirstState = true;
                    }
                    if (FC.StatusMode == 3)
                    {
                        ConsoleDialog.ChoiceMenuFirstInitAutoPilotRefuellingMode();
                        ConsoleDialog.ChoicePostThree();
                        FirstStateStatusThree = true;
                    }
                }
                if (e.CarrierId.ToString() == FC.Id)
                {           
                    FC.Callsign = e.Callsign;
                    FC.Name = e.Name;
                    FC.DockingAccess = e.DockingAccess;
                    FC.AllowNotorious = e.AllowNotorious;
                    FC.FuelLevel = e.FuelLevel;
                    FC.JumpRangeCurr = e.JumpRangeCurr;
                    FC.JumpRangeMax = e.JumpRangeMax;
                    FC.CapacityUsed = e.SpaceUsage.Cargo.ToString();

                    if (FirstState)
                    {
                        FC.CalculateJumpRoute();
                        FirstState = false;   
                        _log.LogInformation("Kod pro vypočitaní trasy se zapl.");
                    }

                    if (FirstStateStatusThree)
                    {
                        FC.CalculateJumpRoute();
                        FirstStateStatusThree = false;
                        // kod dialog co řekne hrači at jda hru do vychoziho stavu pro bota
                        //
                        FirstRunAutoPilot = true;
                    }

                    // kod co zapne prvotni inicializaci bota
                    if (FC.StatusMode == 3 && FirstRunAutoPilot)
                    {
                        // spustit bota co zada kurz na dalši system
                        //
                        _log.LogInformation("První inicializace autopilota.");
                        FC.AutoPilotInit();
                    }
                    if (FC.StatusMode == 3 && FC.AutoPilotEnabled && FC.AutoPilotEnabledFirst != 1 && FC.AutoPilotRefuelingIntervalPause != 1)
                    {
                        // odpočitat zhruba minutu a zapnout kod co spusti autopilota a nastavi
                        // kurz do dalšího systemu
                        FC.AutoPilotNormalMode();
                    }
                    FC.send();
                    _log.LogInformation("Podrobné údaje o FC byly zaznamenány.");
                }
            };


            _api.Events.CarrierJumpRequestEvent += (sender, e) =>
            {
                // Return if not this session
                if (!_api.HasCatchedUp) { return; }

                if (e.CarrierId.ToString() == FC.Id)
                {
                    FC.NextJump = e.SystemName;

                    System.TimeSpan cdjump = new System.TimeSpan(0, 0, 15, 0);
                    DateTime localDate = DateTime.Now;
                    System.DateTime result = localDate + cdjump;
                    FC.NextJumpDateTime = result.ToString("HH:mm-dd-MM-yyyy");  
                    FC.send(); 
                    _log.LogInformation("Žádost o skok byla zaznamenána.");
                    if (FC.StatusMode == 3 && FC.AutoPilotEnabled)
                    {
                        if (e.SystemName == FC.jumpDestCheck())
                        {
                            Console.WriteLine("požadavek o skok oveřen carrier letí správně");
                        }
                        else
                        {
                            Console.WriteLine("Carrier letí špatně vypinam autopilota");
                            FC.AutoPilotEnabled = false;
                        }
                    }

                }
            };

            _api.Events.CarrierJumpEvent += (sender, e) =>
            {
                // Return if not this session
                if (!_api.HasCatchedUp) { return; }

                if (e.StationType == "FleetCarrier" && e.MarketId.ToString() == FC.Id)
                {
                    FC.Location = e.StarSystem;
                    FC.JumpDateTime = DateTime.Now.ToString("HH:mm-dd-MM-yyyy");
                    if (FC.StatusMode != 3)
                    {
                        FC.NextJump = null;
                        FC.NextJumpDateTime = null;
                        FC.send();   
                    }
                    _log.LogInformation($"FC právě provádí skok do systému {e.StarSystem}.");
                    if (FC.StatusMode == 3 && FC.AutoPilotEnabled)
                    {
                        // odpočitat zhruba minutu a zapnout kod co spusti autopilota a nastavi
                        // kurz do dalšího systemu
                        //FC.AutoPilotNormalMode();
                        FC.AutoPilotCheckFuel();
                    }
                }
            };

            _api.Events.CarrierJumpCancelledEvent += (sender, e) =>
            {
                // Return if not this session
                if (!_api.HasCatchedUp) { return; }

                if (e.CarrierId.ToString() == FC.Id)
                {
                    FC.NextJump = null;
                    FC.NextJumpDateTime = null;
                    FC.send();
                    _log.LogInformation("FC ukončil skokovou sekvenci.");
                    if (FC.StatusMode == 3 && FC.AutoPilotEnabled)
                    {
                        // skok manualně zrušen, zrušit autopilota
                        FC.AutoPilotEnabled = false;
                    }
                }
            };
            _api.Events.CarrierDepositFuelEvent += (sender, e) =>
            {
                // Return if not this session
                if (!_api.HasCatchedUp) { return; }

                if (e.CarrierId.ToString() == FC.Id)
                {
                    FC.FuelLevel = e.Total;
                    FC.send();
                    _log.LogInformation("Převod paliva byl zaznamenán.");
                }
            };


            // Start EliteAPI
            await _api.StartAsync();
        }
    }
}