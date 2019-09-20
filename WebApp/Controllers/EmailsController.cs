using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Net.Mail;

namespace BreachedEmails
{
    [Route("{name}@{domain}")]
    [ApiController]
    public class EmailsController : ControllerBase
    {
        private IClusterClient orleansClient;

        public EmailsController(IClusterClient orleansClient)
        {
            this.orleansClient = orleansClient;
        }

        [HttpGet]
        public IActionResult Get(string name, string domain)
        {
            if (IsValidEmail(name + "@" + domain))
            {
                var grain = this.orleansClient.GetGrain<IEmails>(domain);
                if (grain == null)
                {
                    return NotFound(string.Format("No grain found with id = {0}", domain));
                }
                var response = grain.GET(name + "@" + domain);
                if (response.Result.ToString() == "OK")
                {
                    return Ok();
                }
                else if (response.Result.ToString() == "NotFound")
                {
                    return NotFound();
                }
            }
            return BadRequest(string.Format("Not valid email"));
        }

        [AcceptVerbs("Create")]
        public IActionResult Create(string name, string domain)
        {
            if (IsValidEmail(name + "@" + domain))
            {
                var grain = this.orleansClient.GetGrain<IEmails>(domain);
                if (grain == null)
                {
                    return NotFound(string.Format("No grain found with id = {0}", domain));
                }
                var response = grain.Create(name + "@" + domain);
                if (response.Result.ToString() == "Created")
                {
                    return Created("",name+"@"+domain);
                }
                else if (response.Result.ToString() == "Conflict")
                {
                    return Conflict();
                }
            }
            return BadRequest(string.Format("Not valid email"));
        }

        
        public bool IsValidEmail(string source)
        {
            return new EmailAddressAttribute().IsValid(source);
        }
}
}
