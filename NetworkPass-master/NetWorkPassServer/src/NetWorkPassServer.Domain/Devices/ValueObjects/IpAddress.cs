namespace NetWorkPassServer.Domain.Devices;

    public sealed record IpAddress
    {
        public string Value { get; }

        public IpAddress(string value)
        {
            if (!System.Net.IPAddress.TryParse(value, out _))
                throw new ArgumentException("IP düzgün deyil");

            Value = value;
        }

        public override string ToString() => Value;
    }

