using System;
using System.Collections.Generic;
using System.Text;

namespace AnalyticsJobsService.Models
{
    public class StudySubjectEntity
    {
        public Guid Id { get; set; }
        public long StudyId { get; set; }
        public long SubjectId { get; set; }
    }
}
