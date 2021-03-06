﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
#if Otter
using Inedo.Otter.Data;
using Inedo.Otter.Extensibility;
using Inedo.Otter.Extensibility.VariableFunctions;
using ContextType = Inedo.Otter.IOtterContext;
#elif BuildMaster
using Inedo.BuildMaster.Data;
using Inedo.BuildMaster.Extensibility;
using Inedo.BuildMaster.Extensibility.VariableFunctions;
using ContextType = Inedo.BuildMaster.Extensibility.IGenericBuildMasterContext;
#endif

namespace Inedo.Extensions.VariableFunctions.Server
{
    [ScriptAlias("ServersInEnvironment")]
    [Description("Returns a list of all the servers in the specified environment name.")]
    public sealed class ServersInEnvironmentVariableFunction : VectorVariableFunction
    {
        [DisplayName("environmentName")]
        [VariableFunctionParameter(0, Optional = true)]
        [Description("The name of the evironment. If not supplied, the current environment in context will be used.")]
        public string EnvironmentName { get; set; }

        protected override IEnumerable EvaluateVector(ContextType context)
        {
            int? environmentId = FindEnvironment(this.EnvironmentName, context);
            if (environmentId == null)
                return null;

            return DB.Servers_SearchServers(Has_ServerRole_Id: null, In_Environment_Id: environmentId)
#if Otter
                .Servers_Extended
#elif BuildMaster
                .Servers
#endif
                .Select(s => s.Server_Name);
        }

        private int? FindEnvironment(string environmentName, ContextType context)
        {
            var allEnvironments = DB.Environments_GetEnvironments();

            if (!string.IsNullOrEmpty(environmentName))
            {
                return allEnvironments
                    .FirstOrDefault(e => e.Environment_Name.Equals(environmentName, StringComparison.OrdinalIgnoreCase))
                    ?.Environment_Id;
            }
            else
            {
                return allEnvironments
                    .FirstOrDefault(e => e.Environment_Id == context.EnvironmentId)
                    ?.Environment_Id;
            }
        }
    }
}
