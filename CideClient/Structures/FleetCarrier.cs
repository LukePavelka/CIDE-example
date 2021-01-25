namespace CideClient.Structures
{
    using System;
    using System.Collections.Generic;
    using CideClient.Api;
    using CideClient.Class;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using J = Newtonsoft.Json.JsonPropertyAttribute;
    using R = Newtonsoft.Json.Required;
    using N = Newtonsoft.Json.NullValueHandling;
    using System.Threading.Tasks;
    using Microsoft.AppCenter.Analytics;

    public partial class FleetCarrier
    {
        [J("id", NullValueHandling = N.Ignore)]               public string Id { get; set; }              
        [J("callsign", NullValueHandling = N.Ignore)]         public string Callsign { get; set; }        
        [J("name", NullValueHandling = N.Ignore)]             public string Name { get; set; }            
        [J("ownerCmdr", NullValueHandling = N.Ignore)]        public string OwnerCmdr { get; set; }       
        [J("location", NullValueHandling = N.Ignore)]         public string Location { get; set; }        
        [J("nextJump", NullValueHandling = N.Ignore)]         public string NextJump { get; set; }        
        [J("nextJumpDateTime", NullValueHandling = N.Ignore)] public string NextJumpDateTime { get; set; }
        [J("destination", NullValueHandling = N.Ignore)]      public string Destination { get; set; }     
        [J("dockingAccess", NullValueHandling = N.Ignore)]    public string DockingAccess { get; set; }   
        [J("allowNotorious", NullValueHandling = N.Ignore)]   public bool? AllowNotorious { get; set; }   
        [J("fuelLevel", NullValueHandling = N.Ignore)]        public long? FuelLevel { get; set; }        
        [J("jumpRangeCurr", NullValueHandling = N.Ignore)]    public double? JumpRangeCurr { get; set; }    
        [J("jumpRangeMax", NullValueHandling = N.Ignore)]     public double? JumpRangeMax { get; set; }     
        [J("jumpLeft", NullValueHandling = N.Ignore)]         public long? JumpLeft { get; set; }         
        [J("jumpMax", NullValueHandling = N.Ignore)]          public long? JumpMax { get; set; }          
        [J("statusMode", NullValueHandling = N.Ignore)]       public long? StatusMode { get; set; }       
        [J("jumpDateTime", NullValueHandling = N.Ignore)]     public string JumpDateTime { get; set; }    
        [J("fuelInMarket", NullValueHandling = N.Ignore)]     public long? FuelInMarket { get; set; }     
        [J("fuelNeeded", NullValueHandling = N.Ignore)]       public long? FuelNeeded { get; set; }  
        public string CapacityUsed { get; set; }
        public FleetCarrierRoutes JumpRoutes { get; set; }
        public string NextJumpEM;
        public bool AutoPilotEnabled;
        public int AutoPilotEnabledFirst;
        public int AutoPilotRefuelingMode;
        public int AutoPilotRefuelingIntervalPause;

        public void send()
        {
            Analytics.TrackEvent("FleetCarrierObject", new Dictionary<string, string> 
            {
                    { "FCName", Name },
                    { "Location", Location },
                    { "NextJump", NextJump },
                    { "NextJumpDateTime", NextJumpDateTime },
                    { "Destination", Destination },
                    { "FuelNeeded", FuelNeeded.ToString() },
                    { "FuelLevel", FuelLevel.ToString() },
                    { "FuelInMarket", FuelInMarket.ToString() },
                    { "JumpLeft", JumpLeft.ToString() },
                    { "JumpMax", JumpMax.ToString() },
                    { "StatusMode", StatusMode.ToString() },
                    { "JumpDateTime", JumpDateTime },
                    { "AutoPilotEnabled", AutoPilotEnabled.ToString() },
                    { "AutoPilotEnabledFirst", AutoPilotEnabledFirst.ToString() },
                    { "AutoPilotRefuelingMode", AutoPilotRefuelingMode.ToString() },
                    { "AutoPilotRefuelingIntervalPause", AutoPilotRefuelingIntervalPause.ToString() }
            });
            JumpLeftUpdate();
            //1. zjistit zdá je to v db ano či ne
            if (Id != null)
            {
                var json = JsonConvert.SerializeObject(this);
                string resultGet = CideApi.get(Id);
                if (resultGet == "error")
                {
                    string resultPost = CideApi.post(json);
                    Console.WriteLine("Carrier nenalezen, vytvařim v db novou položku s aktualnima informacema.");
                }
                else
                {
                    string resultPut = CideApi.put(json,Id);
                    Console.WriteLine("Aktualizuju informace o db.");
                }
            }
        } 

        public void CalculateJumpRoute()
        {
            if (this.StatusMode == 2)
            {
                CalculateRoute.MakeRoute(this, this.Location, this.Destination, "5200", "1000","5200");
            }
            if (this.StatusMode == 3)
            {
                CalculateRoute.MakeRoute(this, this.Location, this.Destination, this.CapacityUsed, this.FuelLevel.ToString(), this.FuelInMarket.ToString());
            }
        
        }  

        public void AutoPilotInit()
        {
            // je uživatel upozornen na to že musi dat hru do potřebného stavu?

            // mame dostatek paliva pro skok?
            //máme
            if (fuelForJump())
            {
                //skočime  >> skočit do nasledovniho systemu // zjistit kam skakame >>double check
                //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
                this.AutoPilotEnabledFirst = 1;
                Task.Run(async delegate 
                {
                    Console.WriteLine("odpočitavam minutu");
                    await Task.Delay(TimeSpan.FromMinutes(1));
                    AutoPilot.SetJump(CalculateRoute.NextSystemFromFCR(this.JumpRoutes, this));
                    this.AutoPilotEnabledFirst = 2;
                });
                this.AutoPilotEnabled = true;
            }
            // nemame
            else
            {
                Console.WriteLine("Není dostatek paliva pro cestu!");
                var D = new Dialog(this);
                int choice = D.ChoiceMenuFirstInitAutoPilotError();
                if (choice == 1)
                {
                    this.AutoPilotEnabledFirst = 1;
                    Task.Run(async delegate 
                    {
                        Console.WriteLine("odpočitavam minutu");
                        await Task.Delay(TimeSpan.FromMinutes(1));
                        AutoPilot.SetJump(CalculateRoute.NextSystemFromFCR(this.JumpRoutes, this));
                        this.AutoPilotEnabledFirst = 2;
                    });
                    this.AutoPilotEnabled = true;
                }
                else if(choice == 2)
                {
                    this.AutoPilotEnabled = false;
                }
            }
        }  

        private bool fuelForJump()
        {
            long? fuelForJump = CalculateRoute.FuelNeededRoute(this.JumpRoutes.Result.Jumps,this);
            if (this.FuelLevel + this.FuelInMarket >= fuelForJump)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AutoPilotCheckFuel()
        {
            Task.Run(async delegate 
                {
                    Console.WriteLine("Kontroluji hladinu paliva ve fc managmentu");
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    AutoPilot.OpenFcManagment();
                });
        }

        public void AutoPilotNormalMode()
        {
            string nextSys = CalculateRoute.NextSystemFromFCR(this.JumpRoutes, this);
            if (nextSys != null && fuelForJumpOne())
            {
                this.AutoPilotRefuelingIntervalPause = 1;
                Task.Run(async delegate 
                {
                    Console.WriteLine("odpočitavam 3.5 minuty");
                    await Task.Delay(TimeSpan.FromMinutes(3.5));
                    // upravit funkci aby byla pro novou verzi
                    AutoPilot.SetJumpFromManegment(nextSys); 
                    this.AutoPilotRefuelingIntervalPause = 0;
                });
                this.AutoPilotEnabled = true; 
            }
            else if (nextSys != null && fuelForJumpOne() == false)
            {
                if (this.AutoPilotRefuelingMode == 1)
                {
                    Task.Run(async delegate 
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        AutoPilot.ExitFcManagment();
                        Console.WriteLine("Spouštím kod pro palivo");
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        AutoPilot.TransferFuel();
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        AutoPilotCheckFuel();
                    });    
                }
                else if (this.AutoPilotRefuelingMode == 2)
                {
                    Task.Run(async delegate 
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        AutoPilot.ExitFcManagment();
                        Console.WriteLine("Spouštím kod pro palivo");
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        AutoPilot.BuyFromMarketAndAdd();
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        AutoPilotCheckFuel();
                    });
                }
                else if (this.AutoPilotRefuelingMode == 3)
                {
                    Task.Run(async delegate 
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        AutoPilot.ExitFcManagment();
                        Console.WriteLine("Spouštím kod pro palivo");
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        AutoPilot.AddFuelToCarrier();
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        AutoPilotCheckFuel();
                    });                 
                }
                else if (this.AutoPilotRefuelingMode == 4)
                {
                    Task.Run(async delegate 
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        AutoPilot.ExitFcManagment();
                    });                 
                }
            }
            else if (nextSys == null)
            {
                AutoPilot.ExitFcManagment();
                Console.WriteLine("vypinam autopilota, jsme v cili");
                this.AutoPilotEnabled = false;
                this.StatusMode = 4;
            }
        }

        private bool fuelForJumpOne()
        {
            long? fuelForJump = CalculateRoute.FuelNeededRouteOneJump(this.JumpRoutes.Result.Jumps,this);
            if (this.FuelLevel >= fuelForJump + 40)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void JumpLeftUpdate()
        {
            if (this.StatusMode == 3 && this.JumpRoutes != null && this.AutoPilotEnabled == true)
            {
                this.JumpLeft = CalculateRoute.CalculateJumpLeft(this);
            }
        }

        public string jumpDestCheck()
        {
            return CalculateRoute.NextSystemFromFCR(this.JumpRoutes, this);
        }
    }
}
