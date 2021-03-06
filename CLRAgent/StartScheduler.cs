﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Threading;
using System.Timers;
using CLRAgent.Library;
using CLRAgent;
using System.Collections.Generic;


public partial class StoredProcedures
{
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void StartScheduler(string databaseName, int waitTimeMs)
    {
        //Declare scheduler timer
        System.Timers.Timer scheduleTimer = new System.Timers.Timer(waitTimeMs);
        scheduleTimer.Elapsed += new ElapsedEventHandler(scheduleTimer_Elapsed);        
        scheduleTimer.AutoReset = true;
        scheduleTimer.Start();
        GC.KeepAlive(scheduleTimer);                
    }

    //Check for pending agent jobs
    static void scheduleTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        AgentDb db = new AgentDb();
        List<Job> jobs = db.CheckSchedule();
        if (jobs.Count > 0)
        {            
            Agent agentInstance = new Agent(jobs);
            Thread t = new Thread(agentInstance.RunJobsAsync);
            t.Start();
        }
    }
};
