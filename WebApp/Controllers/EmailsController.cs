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
            try
            {
                if (IsValidEmail(name + "@" + domain))
                {
                    var grain = this.orleansClient.GetGrain<IEmails>(domain);
                    var response = grain.GET(name + "@" + domain);
                    if (response.Result)
                    {
                        return Ok();
                    }
                    else if (!response.Result)
                    {
                        return NotFound();
                    }                   
                }
                return BadRequest(string.Format("Not valid email."));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [AcceptVerbs("Create")]
        public IActionResult Create(string name, string domain)
        {
            try
            {
                if (IsValidEmail(name + "@" + domain))
                {
                    var grain = this.orleansClient.GetGrain<IEmails>(domain);
                    var response = grain.Create(name + "@" + domain);
                    if (response.Result)
                    {
                        return Created("", name + "@" + domain);
                    }
                    else if (!response.Result)
                    {
                        return Conflict();
                    }
                }
                return BadRequest(string.Format("Not valid email"));
            }
            catch(Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }

        }
   
        public bool IsValidEmail(string source)
        {
            return new EmailAddressAttribute().IsValid(source);
        }
    }
}
