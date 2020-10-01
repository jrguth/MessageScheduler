using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageScheduler.Data
{
    public interface IScheduledTextRepository
    {
        IEnumerable<ScheduledText> GetScheduledTexts();
        ScheduledText GetScheduledTextById(int id);
        void CreateScheduledText(ScheduledText scheduledText);
        void DeleteScheduledText(int id);
        void UpdateScheduledText(ScheduledText scheduledText);

    }
}
