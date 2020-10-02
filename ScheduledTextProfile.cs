using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MessageScheduler.Models;
using MessageScheduler.Data;

namespace MessageScheduler
{
    public class ScheduledTextProfile : Profile
    {
        public ScheduledTextProfile()
        {
            CreateMap<ScheduledText, ScheduledTextModel>().ReverseMap();
        }
    }
}
