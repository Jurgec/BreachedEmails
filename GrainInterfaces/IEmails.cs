using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BreachedEmails
{
    public interface IEmails : Orleans.IGrainWithStringKey
    {
        Task<string> GET(string email);
        Task<string> Create(string email);
    }
}
