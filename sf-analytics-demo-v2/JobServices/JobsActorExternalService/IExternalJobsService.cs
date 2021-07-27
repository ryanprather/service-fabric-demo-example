using JobModels;
using System.Threading.Tasks;

namespace JobsActorExternalService
{
    public interface IExternalJobsService
    {
        Task InitNewJob(IJobDto jobDto);
    }
}
