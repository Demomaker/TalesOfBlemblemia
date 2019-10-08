using System.Collections.Generic;

namespace Game
{
    public interface Repository<T>
    {
        void Insert(T myObject);

        List<T> FindAll();

        void Update(T myObject);

        void Delete(int id);
    }
}