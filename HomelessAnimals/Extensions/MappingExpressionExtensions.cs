using AutoMapper;

namespace HomelessAnimals.Extensions
{
    public static class MappingExpressionExtensions
    {
        public static IMappingExpression<T, K> Trim<T, K>(this IMappingExpression<T, K> expression)
        {
            return expression.AddTransform<string>(s =>
                string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s) ? null : s.Trim());
        }
    }
}
