using Speckle.Core.Models;

public interface IConverter<TObject, TDesObject>
{
    /// <summary>
    /// Converter to <see cref="Base"/> from <see cref="TSwObject"/>
    /// </summary>
    /// <param name="object"></param>
    /// <returns></returns>
    TDesObject Convert(TObject @object);
}
