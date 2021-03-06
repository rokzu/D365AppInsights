using JLattimer.D365AppInsights;
using System;

namespace D365AppInsights.Action
{
    public class LogDependencyAsync : PluginBase
    {
        #region Constructor/Configuration
        private readonly string _unsecureConfig;
        private readonly string _secureConfig;

        public LogDependencyAsync(string unsecure, string secure)
            : base(typeof(LogDependency))
        {
            _unsecureConfig = unsecure;
            _secureConfig = secure;
        }
        #endregion

        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            if (localContext == null)
                throw new ArgumentNullException(nameof(localContext));

            try
            {
                AiLogger aiLogger = new AiLogger(_unsecureConfig, localContext.OrganizationService, localContext.TracingService,
                    localContext.PluginExecutionContext, localContext.PluginExecutionContext.Stage, null);

                string name = ActionHelpers.GetInputValue<string>("name", localContext.PluginExecutionContext, localContext.TracingService);
                string method = ActionHelpers.GetInputValue<string>("method", localContext.PluginExecutionContext, localContext.TracingService);
                string typeInput = ActionHelpers.GetInputValue<string>("type", localContext.PluginExecutionContext, localContext.TracingService);
                int? duration = ActionHelpers.GetInputValue<int?>("duration", localContext.PluginExecutionContext, localContext.TracingService);
                int? resultcode = ActionHelpers.GetInputValue<int?>("resultcode", localContext.PluginExecutionContext, localContext.TracingService);
                bool? success = ActionHelpers.GetInputValue<bool?>("success", localContext.PluginExecutionContext, localContext.TracingService);
                string data = ActionHelpers.GetInputValue<string>("data", localContext.PluginExecutionContext, localContext.TracingService);

                if (string.IsNullOrEmpty(name) || duration == null || string.IsNullOrEmpty(typeInput) || success == null)
                {
                    string errorMessage;
                    if (string.IsNullOrEmpty(name))
                        errorMessage = "Name must be populated";
                    else if (duration == null)
                        errorMessage = "Duration must be populated";
                    else if (string.IsNullOrEmpty(typeInput))
                        errorMessage = "Type must be populated";
                    else
                        errorMessage = "Success must be populated";

                    localContext.TracingService.Trace(errorMessage);
                    return;
                }

                aiLogger.WriteDependency(name, method, typeInput, (int)duration, resultcode, (bool)success, data);
            }
            catch (Exception e)
            {
                localContext.TracingService.Trace($"Unhandled Exception: {e.Message}");
            }
        }
    }
}