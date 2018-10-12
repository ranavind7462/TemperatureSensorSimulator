using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace TemperatureSensorSimulator
{
    public class TemperatureTicker
    {
        #region private variables

        //Singleton instance creating for hub
        // Lazy instance is used for a thread safe  
        private readonly static Lazy<TemperatureTicker> _instance = new Lazy<TemperatureTicker>
            (()=>new TemperatureTicker(GlobalHost.ConnectionManager.GetHubContext<TemperatureHub>().Clients));
      
        //Concurrent Dictionary used for thread         
        private readonly ConcurrentDictionary<DateTime, Temperature> _tempList = new ConcurrentDictionary<DateTime, Temperature>();
                                
        //This is used for  any other thread is updating the temperature 
        private readonly object _updateTemperatureLock = new object();

        private readonly TimeSpan _interval = TimeSpan.FromMilliseconds(5000);

        private readonly Random _random = new Random();

        private readonly Timer _timer;

        private volatile bool _updatingTemperature = false;
        private IHubConnectionContext<dynamic> Clients { get; set; }

        #endregion

        #region public methods
        public TemperatureTicker(IHubConnectionContext<dynamic> clients)
        {
            try
            {
                Clients = clients;
                _tempList.Clear();
                var templist = new List<Temperature>()
                {
                new Temperature{Date=DateTime.Now,TemperatureValue=30}
                };
                templist.ForEach(temp => _tempList.TryAdd(temp.Date, temp));

                _timer = new Timer(updateTemperature, null, _interval, _interval);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("=> Exception " + ex.Message);
            }
        }
       
        public static TemperatureTicker Instance => _instance.Value;

        public IEnumerable<Temperature> GetTemperatures()
        {
            return _tempList.Values;
        }
        #endregion

        #region privte methods
        private void updateTemperature(object state)
        {
            try
            {
                lock (_updateTemperatureLock)
                {
                    if (!_updatingTemperature)
                    {
                        _updatingTemperature = true;
                        foreach (var temp in _tempList.Values)
                        {
                            if (TryUpdateTemperature(temp))
                            {
                                BroadCostTemperature(temp);
                            }
                        }
                        _updatingTemperature = false;
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("=> Exception " + ex.Message);
            }
        }
        
        /// <summary>
        /// This method used for generating random numbers between the range of -10 to70
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        private bool TryUpdateTemperature(Temperature temp)
        {
            try
            {
                //Here we need to get the temperatue from device,As device not there just  used random number
                temp.Date = DateTime.Now;

                temp.TemperatureValue = _random.Next(-10, 70);
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("=> Exception " + ex.Message);
                return false;
            }

        }

        #region BroadCostTemperature
        /// <summary>
        /// Broadcost the temperature to the all the  clients
        /// </summary>
        /// <param name="temp"></param>
        private void BroadCostTemperature(Temperature temp)
        {
            Clients.All.loadTemperature(temp);
        }
        #endregion
        #endregion
    }
}
