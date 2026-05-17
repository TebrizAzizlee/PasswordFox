namespace NetWorkPassServer.Domain.Devices;

    public sealed record DeviceName
    {
        public string Value { get; }

        public DeviceName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Device adı boş ola bilməz");

            Value = value.Trim();
        }
    }

