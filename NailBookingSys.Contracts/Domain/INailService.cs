using System;
using System.Collections.Generic;
using System.Text;

namespace NailBookingSys.Contracts.Domain;

public interface INailService
{
    ServiceType ServiceType { get; }
    string Name { get; }
    int DurationInMinutes { get; }
    decimal Price { get; }
}
