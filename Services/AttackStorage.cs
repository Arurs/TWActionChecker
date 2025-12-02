using System.Collections.Concurrent;
using TribalWarsCheckAPI.Interfaces;
using TribalWarsCheckAPI.Models;

namespace TribalWarsCheckAPI.Services;

public class AttackStorage : IAttackStorage
{
    public ConcurrentBag<Attack> Attacks { get; set; } = new ConcurrentBag<Attack>();
}
