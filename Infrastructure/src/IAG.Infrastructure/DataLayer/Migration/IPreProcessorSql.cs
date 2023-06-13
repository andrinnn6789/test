namespace IAG.Infrastructure.DataLayer.Migration;

public interface IPreProcessorSql : IProcessor
{
    string Process(string command);
}