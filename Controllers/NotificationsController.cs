using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net;
using System.Threading;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace msgraphapp.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class NotificationsController : ControllerBase
  {
    private readonly string _ngrokEndpoint;
    private readonly GraphHelper _helper;

    public NotificationsController(GraphConfig config, GraphHelper helper)
    {
      _ngrokEndpoint = config.Ngrok;
      _helper = helper;
    }

    [HttpGet]
    public async Task<ActionResult<string>> Get()
    {
      var graphServiceClient = _helper.GetGraphClient();

      var sub = new Microsoft.Graph.Subscription();
      sub.ChangeType = "created";
      sub.NotificationUrl = _ngrokEndpoint + "/api/notifications";
      sub.Resource = "/users/adelev@M365x76957727.onmicrosoft.com/mailFolders/AAMkAGM1MTAxZTZmLTY4NTktNGUxZC04NWU5LTM1ODE2YzE5OTQ0ZQAuAAAAAABZp-DdLAUtRqcqzEajRlCdAQCusxagU9PnS6NzcHGBz1LMAAAAAAEMAAA=/messages";
      sub.ExpirationDateTime = DateTime.UtcNow.AddMinutes(5);
      sub.ClientState = "SecretClientState";

      var newSubscription = await graphServiceClient
        .Subscriptions
        .Request()
        .AddAsync(sub);

      return $"Subscribed. Id: {newSubscription.Id}, Expiration: {newSubscription.ExpirationDateTime}";
    }

    [HttpPost]
    public async Task<ActionResult<string>> Post([FromQuery]string? validationToken = null)
    {
      // handle validation
      if(!string.IsNullOrEmpty(validationToken))
      {
        Console.WriteLine($"Received Token: '{validationToken}'");
        return Ok(validationToken);
      }

      // handle notifications
      using (StreamReader reader = new StreamReader(Request.Body))
      {
        string content = await reader.ReadToEndAsync();

        Console.WriteLine(content);

        var notifications = JsonSerializer.Deserialize<ChangeNotificationCollection>(content);

        if (notifications != null) {
          foreach(var notification in notifications.Value)
          {
            Console.WriteLine($"Received notification: '{notification.Resource}', {notification.ResourceData.AdditionalData["id"]}");
          }
        }
      }

      return Ok();
    }

    
   

  }
}