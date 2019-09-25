using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BreachedEmails
{
    public interface IEmails : Orleans.IGrainWithStringKey
    {
        Task<bool> GET(string email);
        Task<bool> Create(string email);
    }
}
