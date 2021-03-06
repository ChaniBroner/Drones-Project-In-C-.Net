using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;

namespace Dal
{
    /// <summary>
    /// A class DataSource that contains:
    /// Rand, DroneList, BaseStationList, CustomerList, ParceList, 
    /// ChargingDroneList, class Config and functions-Initialize
    /// </summary>
    internal class DataSource
    {
        /// <summary>
        /// const variables.
        /// </summary>
        const int INITIALIZE_DRONE = 5;
        const int INITIALIZE_CUSTOMER = 10;
        const int INITIALIZE_BASE_STATION = 2;
        const int INITIALIZE_PARCEL = 5;

        //  INITIALIZE_PARCEL always < INITIALIZE_DRONE ( for droneId in parcel initializing )
        //  INITIALIZE_CUSTOMER always >= 2 ( for two difference customers id in parcel )

        /// <summary>
        /// A class Config that contains :
        /// IndexParcel,chargingRate,heavyWeight,mediumWeight,lightWeight,available and method :RandBetweenRange
        /// </summary>
        internal class Config
        {
            static public double Available = RandBetweenRange(0, 0.0001);
            static public double LightWeight = RandBetweenRange(Available, 0.0002);
            static public double MediumWeight = RandBetweenRange(LightWeight, 0.0003);
            static public double HeavyWeight = RandBetweenRange(MediumWeight, 0.0004);
            static public double ChargingRate = 20;//TODO לאתחל
            internal static int IndexParcel = 1000;

            private static double RandBetweenRange(double min, double max)
            {
                return (Rand.NextDouble() * (max - min)) + min;
            }
        }

        /// <summary>
        /// an internal object of Random .
        /// </summary>
        internal static Random Rand;

        /// <summary>
        /// A list of Drones.
        /// </summary>
        static internal List<Drone> DroneList;

        /// <summary>
        /// A list of Base Stations.
        /// </summary>
        static internal List<BaseStation> BaseStationList;

        /// <summary>
        /// A list of Customers.
        /// </summary>
        static internal List<Customer> CustomerList;

        /// <summary>
        /// A list of Parcels.
        /// </summary>
        static internal List<Parcel> ParceList;

        /// <summary>
        /// A list of Charging Drones.
        /// </summary>
        static internal List<ChargingDrone> ChargingDroneList;

        /// <summary>
        /// match list by type
        /// </summary>
        static internal Dictionary<Type, IList> Data;

        /// <summary>
        /// A static constructor of DataSource.
        /// </summary>
        static DataSource()
        {
            Rand = new Random();
            DroneList = new List<Drone>();
            BaseStationList = new List<BaseStation>();
            CustomerList = new List<Customer>();
            ParceList = new List<Parcel>();
            ChargingDroneList = new List<ChargingDrone>();
            Data = new()
            {
                [typeof(Drone)] = DroneList,
                [typeof(Customer)] = CustomerList,
                [typeof(Parcel)] = ParceList,
                [typeof(BaseStation)] = BaseStationList,
                [typeof(ChargingDrone)] = ChargingDroneList,
            };
        }

        /// <summary>
        /// innitialize: Drones, Customers,Base Stations, Parcels,ChargingDrone.
        /// </summary>
        internal static void Initialize()
        {
            InitializeBaseStations();
            InitializeCustomers();
            InitializeDrones();
            InitializeParcels();
            InitializeChargingDrone();
        }

        /// <summary>
        /// A function that initalize Drones:
        /// </summary>
        private static void InitializeDrones()
        {
            for (int i = 0; i < INITIALIZE_DRONE; ++i)
            {
                DroneList.Add(new Drone()
                {
                    Id = Rand.Next(1000, 10000),
                    Model = Rand.Next(1000, 10000).ToString(),
                    MaxWeight = (WeightCategories)Rand.Next(0, Enum.GetNames(typeof(WeightCategories)).Length),
                });
            }
        }

        /// <summary>
        /// A function that initalize Customers:
        /// </summary>
        private static void InitializeCustomers()
        {
            string[] initNames = { "Uria", "Aviad", "Odel", "Natan", "Or", "Keren" };
            string[] initDigitsPhone = { "0556", "0548", "0583", "0533", "0527", "0522", "0505", "0584" };
            for (int i = 0; i < INITIALIZE_CUSTOMER; i++)
            {
                var customer=new Customer()
                {
                    Id = Rand.Next(100000000, 1000000000),
                    Name = initNames[Rand.Next(0, initNames.Length)],
                    Phone = initDigitsPhone[Rand.Next(0, initDigitsPhone.Length)],
                    Longitude = Rand.Next(0, 90) + Rand.NextDouble(),
                    Latitude = Rand.Next(-90, 90) + Rand.NextDouble()
                };
                customer.Phone += (Rand.Next(100000, 1000000)).ToString();
            CustomerList.Add(customer);
            }
        }

        /// <summary>
        /// A function that initalize BaseStation:
        /// </summary>
        private static void InitializeBaseStations()
        {
            string[] initNameStation = { "TelTzion", "TelAviv", "Rahanana", "Eilat", "Jerusalem" };
            for (int i = 0; i < INITIALIZE_BASE_STATION; ++i)
            {
                BaseStationList.Add(new BaseStation()
                {
                    Id = Rand.Next(100000000, 1000000000),
                    NameStation = initNameStation[Rand.Next(0, initNameStation.Length)],
                    NumberOfChargingPositions = Rand.Next(0, 50),
                    Longitude = Rand.Next(0, 90) + Rand.NextDouble(),
                    Latitude = Rand.Next(-90, 90) + Rand.NextDouble()
                });
            }
        }

        /// <summary>
        /// A function that initalize Parcels:
        /// </summary>
        private static void InitializeParcels()
        {
            Parcel newParcel;
            for (int i = 0; i < INITIALIZE_PARCEL; ++i)
            {
                newParcel = new Parcel() { Id = ++Config.IndexParcel };
                newParcel.SenderId = CustomerList[Rand.Next(CustomerList.Count)].Id;
                do
                {
                    newParcel.GetterId = CustomerList[Rand.Next(CustomerList.Count)].Id;
                } while (newParcel.GetterId == newParcel.SenderId);
                newParcel.Weight = (WeightCategories)Rand.Next(Enum.GetNames(typeof(WeightCategories)).Length);
                newParcel.MPriority = (UrgencyStatuses)Rand.Next(Enum.GetNames(typeof(UrgencyStatuses)).Length);
                newParcel.CreatedTime = DateTime.Now;
                if (i % 2 == 0)
                {
                    var availableDrone = DroneList.FirstOrDefault(d => d.MaxWeight >= newParcel.Weight
                                                                        && !ParceList.Any(p => p.DroneId == d.Id));
                    if (!availableDrone.Equals(default(Drone)))
                    {
                        newParcel.DroneId = availableDrone.Id;
                        newParcel.BelongParcel = DateTime.Now;
                        //fillParcel.PickingUp = fillParcel.BelongParcel.Value.AddDays(Rand.Next(0, 11));
                        //fillParcel.Arrival = fillParcel.PickingUp.Value.AddDays(Rand.Next(0, 11));
                    }
                }
                ParceList.Add(newParcel);
            }
        }

        /// <summary>
        /// A function that initalize Charging Drones:
        /// </summary>
        private static void InitializeChargingDrone()
        {
            //get all stations with numberofpositions >0
            var BaseStationsWithChargingPosition = BaseStationList.Where(baseStation => baseStation.NumberOfChargingPositions > 0);

            //if there are ChargingPositions so we can charge
            if (BaseStationsWithChargingPosition.Any())
            {
                for (int i = 0; i < DroneList.Count; i++)
                {
                    //if the drone doesnt take a parcel
                    if (ParceList.FirstOrDefault(p => p.DroneId == DroneList.ElementAt(i).Id).Equals(default))
                    {
                        var index = Rand.Next(BaseStationsWithChargingPosition.Count());
                        if (ChargingDroneList.Where(c => c.StationId == index).Count() < BaseStationList.ElementAt(index).NumberOfChargingPositions)
                        {
                            ChargingDroneList.Add(new(){ DroneId=DroneList.ElementAt(i).Id, StationId= BaseStationList.ElementAt(index).Id, EnteranceTime= DateTime.Now});
                        }
                    }
                }
            }
        }

    }
}

