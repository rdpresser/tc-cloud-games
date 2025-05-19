namespace TC.CloudGames.Domain.Abstractions
{
    public interface IBaseValidator<in TEntity>
        where TEntity : class
    {
        ValidationResult ValidationResult(TEntity entity);
        //Task<ValidationResult> ValidationResultAsync(TEntity entity);
        //IEnumerable<ValidationError> ValidationErrors(TEntity entity);
        //IEnumerable<ValidationError> ValidationErrors(ValidationResult validationResult);
        //IDictionary<string, string[]> Errors(ValidationResult validationResult);
    }
}
