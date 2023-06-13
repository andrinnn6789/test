using System.ComponentModel.DataAnnotations;

namespace IAG.Infrastructure.Crud;

public interface IEntityKey<out T>
{
    [Key] 
    T Id { get; }
}