using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MessageScheduler.Data;
using MessageScheduler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using AutoMapper;

namespace MessageScheduler.Controllers
{
    [Route("api/v1/ScheduledTexts")]
    [ApiController]
    public class ScheduledTextsController : ControllerBase
    {
        private IScheduledTextRepository repo;
        private IMapper mapper;

        public ScheduledTextsController(IScheduledTextRepository repo, IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ScheduledTextModel>> Get()
        {
            IEnumerable<ScheduledText> scheduledTexts = repo.GetScheduledTexts();
            return Ok(mapper.Map<IEnumerable<ScheduledText>, IEnumerable<ScheduledTextModel>>(scheduledTexts));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ScheduledTextModel> Get(int id)
        {
            ScheduledText text = repo.GetScheduledTextById(id);
            if  (text is null)
            {
                return NotFound();
            }
            return mapper.Map<ScheduledTextModel>(text);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<ScheduledTextModel> Post([FromBody] ScheduledTextModel scheduledText)
        {
            ScheduledText text = mapper.Map<ScheduledText>(scheduledText);
            ScheduledText existing =
                (from txt in repo.GetScheduledTexts()
                 where txt.PhoneNumber == text.PhoneNumber
                 select txt).FirstOrDefault();
            if (existing is null)
            {
                repo.CreateScheduledText(text);
                return Created($"{HttpContext.Request.GetEncodedUrl()}/{text.Id}", text);
            }
            return Ok(mapper.Map<ScheduledTextModel>(existing));
        }

        // DELETE api/<ScheduleTextController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Delete(int id)
        {
            ScheduledText text = repo.GetScheduledTextById(id);
            if (text is null)
            {
                return NotFound();
            }
            repo.DeleteScheduledText(text.Id);
            return Ok();
        }
    }
}
