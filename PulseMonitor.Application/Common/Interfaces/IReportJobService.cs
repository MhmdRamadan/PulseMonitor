namespace PulseMonitor.Application.Common.Interfaces;

public interface IReportJobService
{
    string EnqueueReportGeneration(Guid reportId);
}
