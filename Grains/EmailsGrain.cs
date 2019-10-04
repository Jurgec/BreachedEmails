using Microsoft.Extensions.Logging;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Orleans;

namespace BreachedEmails
{
    public class StateEmail
    {
        public HashSet<string> Email { get; set; }
    }

    [StorageProvider(ProviderName = "EmailsDB")]
    public class EmailsGrain : Orleans.Grain<StateEmail>, IEmails
    {
        private readonly ILogger logger;
        private bool syncDatabaseStatus;

        public override Task OnActivateAsync()
        {
            if (State.Email == null)
            {
                State.Email = new HashSet<string>();
                syncDatabaseStatus = false;
            }
            this.RegisterTimer(SaveToDatabaseAsync, null, TimeSpan.FromTicks(1), TimeSpan.FromMinutes(5));
            return base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            await SaveToDatabaseAsync(null);
            //return base.OnDeactivateAsync();
        }

        public EmailsGrain(ILogger<EmailsGrain> logger)
        {
            this.logger = logger;
        }
        Task<bool> IEmails.GET(string email)
        {
            if(State.Email.Contains(email))
            {
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }


        Task<bool> IEmails.Create(string email)
        {
            if (State.Email.Contains(email))
            {
                return Task.FromResult(false);
            }
            else
            {
                State.Email.Add(email);
                syncDatabaseStatus = true;
                return Task.FromResult(true);
            }
        }

        private async Task SaveToDatabaseAsync(object arg)
        {
            if(syncDatabaseStatus)
            {
                try
                {
                    await base.WriteStateAsync();
                    syncDatabaseStatus = false;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }    
            }
            //return Task.CompletedTask;
        }

    }
}
