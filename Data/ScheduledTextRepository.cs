using MessageScheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MessageScheduler.Data
{
    public class ScheduledTextRepository : IScheduledTextRepository
    {
        private MessageSchedulerContext context;

        public ScheduledTextRepository(MessageSchedulerContext context)
        {
            this.context = context;
        }

        public void CreateScheduledText(ScheduledText scheduledText)
        {
            context.ScheduledTexts.Add(scheduledText);
            context.SaveChanges();
        }

        public void DeleteScheduledText(int id)
        {
            ScheduledText text = context.ScheduledTexts.Find(id);
            context.ScheduledTexts.Remove(text);
            context.SaveChanges();
        }

        public ScheduledText GetScheduledTextById(int id)
        {
            return context.ScheduledTexts.Find(id);
        }

        public IEnumerable<ScheduledText> GetScheduledTexts()
        {
            return context.ScheduledTexts.ToList();
        }

        public void UpdateScheduledText(ScheduledText scheduledText)
        {
            context.Entry(scheduledText).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}
