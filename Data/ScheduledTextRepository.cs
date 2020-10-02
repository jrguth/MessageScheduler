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
        private DbContextOptions<MessageSchedulerContext> options;

        public ScheduledTextRepository(DbContextOptions<MessageSchedulerContext> options)
        {
            this.options = options;
        }

        public void CreateScheduledText(ScheduledText scheduledText)
        {
            using (var context = new MessageSchedulerContext(options))
            {
                context.ScheduledTexts.Add(scheduledText);
                context.SaveChanges();
            }
        }

        public void DeleteScheduledText(int id)
        {
            using (var context = new MessageSchedulerContext(options))
            {
                ScheduledText text = context.ScheduledTexts.Find(id);
                context.ScheduledTexts.Remove(text);
                context.SaveChanges();
            }           
        }

        public ScheduledText GetScheduledTextById(int id)
        {
            using (var context = new MessageSchedulerContext(options))
            {
                return context.ScheduledTexts.Find(id);
            }                
        }

        public IEnumerable<ScheduledText> GetScheduledTexts()
        {
            using (var context = new MessageSchedulerContext(options))
            {
                return context.ScheduledTexts.ToList();
            }             
        }

        public void UpdateScheduledText(ScheduledText scheduledText)
        {
            using (var context = new MessageSchedulerContext(options))
            {
                context.Entry(scheduledText).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}
