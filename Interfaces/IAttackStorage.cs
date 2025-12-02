using System.Collections.Concurrent;
using TribalWarsCheckAPI.Models;

namespace TribalWarsCheckAPI.Interfaces;

public interface IAttackStorage
{
    ConcurrentBag<Attack> Attacks { get; set; }
}
