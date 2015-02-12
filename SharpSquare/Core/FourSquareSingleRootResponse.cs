using FourSquare.SharpSquare.Entities;

namespace FourSquare.SharpSquare.Core
{
    public class FourSquareSingleRootResponse<T> : FourSquareResponse where T : FourSquareEntity
    {
        public T response { get; set; }
    }
}