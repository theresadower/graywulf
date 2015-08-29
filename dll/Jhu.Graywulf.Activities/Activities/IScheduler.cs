﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Activities
{
    public interface IScheduler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="workflowInstanceId"></param>
        /// <param name="userGuid"></param>
        /// <param name="userName"></param>
        /// <param name="jobGuid"></param>
        /// <param name="jobID"></param>
        void GetContextInfo(Guid workflowInstanceId, out Guid userGuid, out string userName, out Guid jobGuid, out string jobID);

        /// <summary>
        /// Returns a server instance containing all necessary database instances
        /// </summary>
        /// <param name="databaseDefinitions"></param>
        /// <param name="databaseVersion"></param>
        /// <param name="databaseInstances"></param>
        /// <returns></returns>
        Guid GetNextServerInstance(Guid[] databaseDefinitions, string databaseVersion, Guid[] databaseInstances);
        
        /// <summary>
        /// Returns all servers containing all necessary database instances
        /// </summary>
        /// <param name="databaseDefinitions"></param>
        /// <param name="databaseVersion"></param>
        /// <param name="databaseInstances"></param>
        /// <returns></returns>
        Guid[] GetServerInstances(Guid[] databaseDefinitions, string databaseVersion, Guid[] databaseInstances);

        Guid GetNextDatabaseInstance(Guid databaseDefinition, string databaseVersion);

        Guid[] GetDatabaseInstances(Guid databaseDefinition, string databaseVersion);

        Guid[] GetDatabaseInstances(Guid serverInstance, Guid databaseDefinition, string databaseVersion);
    }
}
