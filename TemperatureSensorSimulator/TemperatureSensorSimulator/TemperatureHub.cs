using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace TemperatureSensorSimulator
{
    public class TemperatureHub: Hub
    {
        private readonly TemperatureTicker _tempTicker;
        public TemperatureHub() : this(TemperatureTicker.Instance) { }

        public TemperatureHub(TemperatureTicker tempticker)
        {
            _tempTicker = tempticker;
        }

        #region Getting tempeature
        /// <summary>
        /// it will return the temperature
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Temperature> GetTemperature()
        {
            return _tempTicker.GetTemperatures();
        }
        #endregion
        #region Overridding signalR methods
        /// <summary>
        /// This method will fire ofter connection 
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
           return base.OnDisconnected(stopCalled);
        }
        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
        #endregion
    }
    #region Error and Info logging
    /// <summary>
    /// This class inherited from HubPipelineModule
    /// </summary>
    public class CustomHandlingHubPipeLineModule : HubPipelineModule
    {
        #region OnIncomingError
        /// <summary>
        /// This method will log when ever the incoming message having any  error
        /// </summary>
        /// <param name="exceptionContext"></param>
        /// <param name="invokerContext"></param>
        protected override void OnIncomingError(ExceptionContext exceptionContext,IHubIncomingInvokerContext invokerContext)
        {
            try
            {
                Debug.WriteLine("=> Exception" + exceptionContext.Error.Message);
                if (exceptionContext.Error.InnerException != null)
                {
                    Debug.WriteLine("=> InnerException " + exceptionContext.Error.InnerException.Message);
                }
                base.OnIncomingError(exceptionContext, invokerContext);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("=> Exception" + ex.Message);
            }
        }
        #endregion
        #region OnBeforeIncoming
        /// <summary>
        /// This method will log the invoking method information 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool OnBeforeIncoming(IHubIncomingInvokerContext context)
        {
            try
            {
                Debug.WriteLine("=> Invoking " + context.MethodDescriptor.Name + " on hub" + context.MethodDescriptor.Hub.Name);
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("=> Exception " + ex.Message);
                if (ex.InnerException.Message != null)
                {
                    Debug.WriteLine("=> Inner Exception" + ex.InnerException.Message);
                }
                
            }
            return base.OnBeforeIncoming(context);
        }
        #endregion
        #region OnBeforeOutgoing
        /// <summary>
        /// This method will log the information before any message going out
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool OnBeforeOutgoing(IHubOutgoingInvokerContext context)
        {
            try
            {
                Debug.WriteLine("=> Invoking " + context.Invocation.Method + "on client hub" + context.Invocation.Hub);

            }
            catch(Exception ex)
            {
                Debug.WriteLine("=> Exception " + ex.Message);
                if (ex.InnerException.Message != null)
                {
                    Debug.WriteLine("=> Inner Exception" + ex.InnerException.Message);
                }
            }
            return base.OnBeforeOutgoing(context);
        }
        #endregion
    }
    #endregion
}
