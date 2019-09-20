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
        private long syncDatabaseStatus;

        public override Task OnActivateAsync()
        {
            if (State.Email == null)
            {
                State.Email = new HashSet<string>();
                syncDatabaseStatus = 0;
            }
            this.RegisterTimer(SaveToDatabase, null, TimeSpan.FromTicks(1), TimeSpan.FromMinutes(5));
            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            SaveToDatabase(this);
            return base.OnDeactivateAsync();
        }

        public EmailsGrain(ILogger<EmailsGrain> logger)
        {
            this.logger = logger;
        }
        Task<string> IEmails.GET(string email)
        {
            if(State.Email.Contains(email))
            {
                return Task.FromResult("OK");
            }
            else
            {
                return Task.FromResult("NotFound");
            }
        }


        Task<string> IEmails.Create(string email)
        {
            if (State.Email.Contains(email))
            {
                return Task.FromResult("Conflict");
            }
            else
            {
                State.Email.Add(email);
                syncDatabaseStatus = 1;
                return Task.FromResult("Created");
            }
        }

        private Task SaveToDatabase(object arg)
        {
            if(syncDatabaseStatus==1)
            {
                syncDatabaseStatus = 0;
                return base.WriteStateAsync();
            }
            return Task.CompletedTask;
        }

    }
}
