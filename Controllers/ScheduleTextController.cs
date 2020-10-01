using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MessageScheduler.Data;
using MessageScheduler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace MessageScheduler.Controllers
{
    [Route("api/v1/ScheduledTexts")]
    [ApiController]
    public class ScheduleTextController : ControllerBase
    {
        private IScheduledTextRepository repo;
        public ScheduleTextController(IScheduledTextRepository repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ScheduledTextModel>> Get()
        {
            return Ok(repo.GetScheduledTexts());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ScheduledTextModel> Get(int id)
        {
            ScheduledTextModel text = repo.GetScheduledTextById(id);
            if  (text is null)
            {
                return NotFound();
            }
            return text;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<ScheduledTextModel> Post([FromBody] ScheduledTextModel scheduledText)
        {
            repo.CreateScheduledText(scheduledText);
            return Created($"{HttpContext.Request.GetEncodedUrl()}/{scheduledText.Id}", scheduledText);
        }

        // DELETE api/<ScheduleTextController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Delete(int id)
        {
            ScheduledTextModel text = repo.GetScheduledTextById(id);
            if (text is null)
            {
                return NotFound();
            }
            repo.DeleteScheduledText(text.Id);
            return Ok();
        }
    }
}
