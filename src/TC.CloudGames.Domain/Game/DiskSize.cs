namespace TC.CloudGames.Domain.Game
{
    public sealed record DiskSize
    {
        public decimal SizeInGb { get; }

        private DiskSize(decimal sizeInGb)
        {
            SizeInGb = sizeInGb;
        }

        public static Result<DiskSize> Create(decimal sizeInGb)
        {
            var diskSize = new DiskSize(sizeInGb);
            var validator = new DiskSizeValidator().ValidationResult(diskSize);

            if (!validator.IsValid)
            {
                return Result.Invalid(validator.AsErrors());
            }

            return diskSize;
        }

        public class DiskSizeValidator : BaseValidator<DiskSize>
        {
            public DiskSizeValidator()
            {
                ValidateDiskSize();
            }

            protected void ValidateDiskSize()
            {
                RuleFor(x => x.SizeInGb)
                    .NotEmpty()
                        .WithMessage("Disk size is required.")
                        .WithErrorCode($"{nameof(DiskSize)}.Required")
                        .OverridePropertyName(nameof(DiskSize))
                    .GreaterThan(0)
                        .WithMessage("Disk size must be greater than 0.")
                        .WithErrorCode($"{nameof(DiskSize)}.GreaterThanZero")
                        .OverridePropertyName(nameof(DiskSize));
            }
        }
    }
}
