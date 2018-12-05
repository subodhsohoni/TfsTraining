using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace TFS_REST_API_Client
{
    class Program
    {
        const String c_collectionUri = "https://dev.azure.com/subodhsohoni";
        const String c_projectName = "SLB Migration";
        const String c_repoName = "SLB Migration";
        static void Main(string[] args)
        {
            VssCredentials creds = new VssClientCredentials();
            creds.Storage = new VssClientCredentialStorage();

            // Connect to Azure DevOps Services
            VssConnection connection = new VssConnection(new Uri(c_collectionUri), creds);
            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();
            Console.Write("Enter work item id to fetch:");
            WorkItem wi = witClient.GetWorkItemAsync(int.Parse(Console.ReadLine())).Result;
            Console.WriteLine(wi.Fields["System.Title"]);


            Console.WriteLine("Creating new work item....");
            JsonPatchDocument patchDocument = new JsonPatchDocument();
            patchDocument.Add(
                new JsonPatchOperation()
                {

                    Operation = Operation.Add,

                    Path = "/fields/System.Title",

                    Value = "Sample task 1"

                }

            );
            WorkItem newWorkItem = witClient.CreateWorkItemAsync(patchDocument, c_projectName, "Task").Result;
            Console.WriteLine("Work item with ID:" + newWorkItem.Id + " Created");
        }
    }
}
