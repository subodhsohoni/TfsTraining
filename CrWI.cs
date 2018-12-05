using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace CreateWorkItem
{
    public class CrWI : ISubscriber
    {
        
        public string Name
        {
            get { return "CrWI"; }
        }

        public SubscriberPriority Priority
        {
            get { return SubscriberPriority.High; }
        }
        public EventNotificationStatus ProcessEvent(IVssRequestContext requestContext, NotificationType notificationType, object notificationEventArgs, out int statusCode, out string statusMessage, out ExceptionPropertyCollection properties)
        {
            Logger.CreateLogEntry("in main event", "eventHanlder");
            WorkItemChangedEvent wiChanged = notificationEventArgs as WorkItemChangedEvent;
            //Compulsory out parameters
            statusCode = 0;
            statusMessage = string.Empty;
            properties = null;
            //get uri for TFS
            string MachineName =  System.Environment.MachineName;
            string coll =  requestContext.ServiceHost.Name.ToUpper();
            var tfsLocation = requestContext.GetService<Microsoft.VisualStudio.Services.Location.Server.ILocationService>();
            Uri collectionUri = new Uri("http://" + MachineName + ":8080/tfs/" + coll);
            ICredentials credential = CredentialCache.DefaultCredentials;
            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(collectionUri, credential);
            WorkItemStore store = new WorkItemStore(tpc);
            string projName = store.Projects[wiChanged.PortfolioProject].Name.ToUpper();
            Project project = store.Projects[projName];

            //find the work item which is being changed
            
            if (notificationType == NotificationType.Notification && notificationEventArgs is WorkItemChangedEvent)
            {
                Logger.CreateLogEntry("entered event handler","eventHanlder");
                if (wiChanged.ChangeType == ChangeTypes.New)
                {
                    //get the current work item
                    Logger.CreateLogEntry("in new WI", "eventHanlder");
                    Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItem wi = store.GetWorkItem(wiChanged.CoreFields.IntegerFields[0].NewValue);
                    string workItemTpe = wi.Type.Name;
                    wi.Title += " demo event";
                    wi.Save();
                }
            }
            return EventNotificationStatus.ActionApproved;
        }

        public Type[] SubscribedTypes()
        {
            return new Type[] { typeof(WorkItemChangedEvent) };
        }
    }
    public class Logger
    {
        public static void CreateLogEntry(string Message, string FileName)
        {
            string LogPath = @"C:\Users\Gouri\Documents\log";
            //String LogPath = Path.GetPathRoot(System.Environment.SystemDirectory) + @"\Program Files (x86)\SSGS\TicketM\Logs";
            if (!Directory.Exists(LogPath))
                Directory.CreateDirectory(LogPath);
            FileName = LogPath + "\\" + FileName + ".log";
            StreamWriter log = new StreamWriter(FileName, true);
            log.WriteLine(System.DateTime.Now + ":" + Message);
            log.Flush();
            log.Close();
        }
    }
}
