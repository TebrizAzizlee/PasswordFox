using NetWorkPassServer.Domain.Branches.ValueObjects;
using SharedLibrary.Abstractions.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NetWorkPassServer.Domain.Branches;
public sealed class Branch : Entity
{
    private Branch()
    {

    }
    public Branch(Name name, Address address)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(address);
        Name = name;
        Address = address;

    }
    public Name Name { get; private set; } = default!;
    public Address Address { get; private set; } = default!;
    public void SetName(Name name)
    {
        if (name is null) throw new ArgumentNullException(nameof(name));
        Name = name; // VO daxilində validasiya olmalıdır
    }
    public void SetAddress(Address address)
    {
        if (address is null) throw new ArgumentNullException(nameof(address));
        Address = address;
    }
    public void Update(Name name, Address address)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(address);
        Name=name;
        Address=address;
    }
    public void Deactivate()
    {
        if (!IsActive) return;

        SetStatus(false);
        // gələcəkdə audit əlavə edə bilərsən
    }
}
