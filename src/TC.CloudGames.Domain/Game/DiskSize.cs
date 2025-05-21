namespace TC.CloudGames.Domain.Game
{
    public sealed record DiskSize
    {
        public decimal SizeInGb { get; }

        private DiskSize(decimal sizeInGb)
        {
            SizeInGb = sizeInGb;
        }

        /// <summary>
        /// Builder pattern for creating and validating DiskSize objects.
        /// </summary>
        public static Result<DiskSize> Create(Action<DiskSizeBuilder> configure)
        {
            var builder = new DiskSizeBuilder();
            configure(builder);
            return builder.Build();
        }

        public class DiskSizeBuilder
        {
            public decimal SizeInGb { get; set; }

            public Result<DiskSize> Build()
            {
                var diskSize = new DiskSize(SizeInGb);
                var validator = new DiskSizeValidator().ValidationResult(diskSize);

                if (!validator.IsValid)
                {
                    return Result.Invalid(validator.AsErrors());
                }

                return diskSize;
            }
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
                        .WithMessage("Disk size value is required.")
                        .WithErrorCode($"{nameof(DiskSize)}.Required")
                        .OverridePropertyName(nameof(DiskSize))
                    .GreaterThan(0)
                        .WithMessage("Disk size value must be greater than 0.")
                        .WithErrorCode($"{nameof(DiskSize)}.GreaterThanZero")
                        .OverridePropertyName(nameof(DiskSize));
            }
        }
    }
}
