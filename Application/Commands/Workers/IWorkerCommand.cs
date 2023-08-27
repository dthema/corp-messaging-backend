using Domain.Models.Workers;

namespace Application.Commands.Workers;

public interface IWorkerCommand
{
    Task<int?> Execute(Employee employee);
    Task<int?> Execute(Manager manager);
    Task<int?> Execute(Chief chief);
}