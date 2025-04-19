using Dapper;
using System.Data;

namespace TC.CloudGames.Infra.Data.Configurations.Data
{
    public sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
    {
        public override DateOnly Parse(object value)
        {
            return DateOnly.FromDateTime((DateTime)value);
        }

        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            parameter.DbType = DbType.Date;
            parameter.Value = value;
        }
    }
}
